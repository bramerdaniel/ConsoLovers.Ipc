namespace WpfServer
{
   using System;
   using System.Threading;
   using System.Threading.Tasks;
   using System.Windows;

   using ConsoLovers.Ipc;

   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class ServerWindow : Window
   {
      private Task task;

      private readonly IServerLogger logger;

      private IIpcServer server;

      public ServerWindow()
      {
         InitializeComponent();
         logger = new ServerTextBoxLogger(textBox, ServerLogLevel.Debug);
      }
      
      private async Task StartTheServer()
      {
         var server = IpcServer.CreateServer()
            .ForCurrentProcess()
            .AddDiagnosticLogging(logger)
            .AddProcessMonitoring()
            .Start();
         
         await server.WaitForClientAsync(TimeSpan.FromMinutes(5));
         await DoWork(server);
      }

      private async Task DoWork(IIpcServer server)
      {
         //var progressReporter = server.GetProgressReporter();
         //for (var i = 0; i <= 100; i++)
         //   progressReporter.ReportProgress(i, $"Progress of {i,3} %");

         //progressReporter.ProgressCompleted();
         await Task.Delay(3000);
         var reporter = server.GetResultReporter();
         reporter.ReportSuccess();
      }


      private void OnStartServer(object sender, RoutedEventArgs e)
      {
         server = IpcServer.CreateServer()
            .ForCurrentProcess()
            .AddDiagnosticLogging(logger)
            .AddProcessMonitoring()
            .Start();

         disposeServer.IsEnabled = true;
         reportProgress.IsEnabled = true;
         reportResult.IsEnabled = true;
      }

      private void OnReportResult(object sender, RoutedEventArgs e)
      {
         // server.WaitForClientAsync(CancellationToken.None);

         var reporter = server.GetResultReporter();
         reporter.ReportResult(0, "Everything OK");
      }

      private void OnDisposeServer(object sender, RoutedEventArgs e)
      {
         server.Dispose();
      }

      private void OnReportProgress(object sender, RoutedEventArgs e)
      {
         Task.Run(ReportProgressInternal);
      }

      private void ReportProgressInternal()
      {
         var reporter = server.GetProgressReporter();
         for (int i = 0; i <= 100; i++)
         {
            reporter.ReportProgress(i, string.Empty);
            Thread.Sleep(30);
         }
      }
   }
}
