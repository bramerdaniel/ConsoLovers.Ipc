using System;
using System.Linq;
using System.Windows;

namespace WpfClient
{
   using System.Diagnostics;
   using System.Threading;
   using System.Threading.Tasks;

   using ConsoLovers.Ipc;
   using ConsoLovers.Ipc.ProcessMonitoring;

   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class ClientWindow : Window
   {
      private Process? process;

      private readonly IClientLogger logger;

      private IClientFactory factory;

      private IResultClient resultClient;

      private IProgressClient progressClient;

      public ClientWindow()
      {
         InitializeComponent();
         logger = new TextBoxLogger(textBox, ClientLogLevel.Debug);
      }

      private async void ConnectToServer(object sender, RoutedEventArgs e)
      {
         process = Process.GetProcesses().FirstOrDefault(x => x.ProcessName.Contains("WpfServer"));

         factory = IpcClient.CreateClientFactory()
            .ForProcess(process)
            .WithLogger(new TextBoxLogger(textBox, ClientLogLevel.Debug))
            .AddProcessMonitoringClients()
            .Build();


      }

      private void CreateClients(object sender, RoutedEventArgs e)
      {
         resultClient = factory.CreateResultClient();
         progressClient = factory.CreateProgressClient();
      }

      private void WaitForProgress(object sender, RoutedEventArgs e)
      {
         waitForProgress.IsEnabled = false;
         Task.Run(Function).ContinueWith(_ => waitForProgress.IsEnabled = true, TaskScheduler.FromCurrentSynchronizationContext());
         
         async Task Function()
         {
            try
            {
               progressClient.ProgressChanged += OnProgressChanged;
               await progressClient.WaitForCompletedAsync();
               logger.Info("Progress completed");
            }
            catch (IpcException ex)
            {
               logger.Info(ex.Message);
            }
            catch (Exception exception)
            {
               logger.Error(exception.Message);
            }
         }
      }

      private void OnProgressChanged(object? sender, ProgressEventArgs e)
      {
         UpdateProgress(e.Percentage);


      }

      private void UpdateProgress(int percentage)
      {
         if (Dispatcher.CheckAccess())
         {
            progressLabel.Text = $"{percentage,3} %";
            progressBar.Value = percentage;
         }
         else
         {
            Dispatcher.BeginInvoke(UpdateProgress, percentage);
         }
      }

      private void WaitForResult(object sender, RoutedEventArgs e)
      {
         waitForResult.IsEnabled = false;
         Task.Run(Function).ContinueWith(_ => waitForResult.IsEnabled = true, TaskScheduler.FromCurrentSynchronizationContext());

         async Task Function()
         {
            try
            {
               var result = await resultClient.WaitForResultAsync(TimeSpan.FromMinutes(5));
               logger.Info($"Result = {result.ExitCode}, Message={result.Message}");
            }
            catch (IpcException ex)
            {
               logger.Info(ex.Message);
            }
            catch (Exception exception)
            {
               logger.Error(exception.Message);
            }
         }
      }

      private async void WaitForServer(object sender, RoutedEventArgs e)
      {
         await factory.WaitForServerAsync(CancellationToken.None);
      }

      private void OnDisposeClients(object sender, RoutedEventArgs e)
      {
         resultClient.Dispose();
         progressClient.Dispose();
      }
   }
}
