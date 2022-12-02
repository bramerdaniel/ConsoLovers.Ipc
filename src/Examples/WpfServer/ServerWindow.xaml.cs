// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerWindow.xaml.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace WpfServer
{
   using System.Threading;
   using System.Threading.Tasks;
   using System.Windows;

   using ConsoLovers.Ipc;

   /// <summary>Interaction logic for MainWindow.xaml</summary>
   public partial class ServerWindow
   {
      #region Constants and Fields

      private readonly IServerLogger logger;

      private IResultReporter? resultReporter;

      private IIpcServer? server;

      #endregion

      #region Constructors and Destructors

      public ServerWindow()
      {
         InitializeComponent();
         logger = new ServerTextBoxLogger(textBox, ServerLogLevel.Debug);
      }

      #endregion

      #region Methods

      private async void OnDisposeServer(object sender, RoutedEventArgs e)
      {
         // TODO this hangs when a progress is running...why
         //// server.Dispose();
         await Task.Run(() => server?.Dispose());
         startServer.IsEnabled = true;
         disposeServer.IsEnabled = false;
      }

      private void OnReportProgress(object sender, RoutedEventArgs e)
      {
         Task.Run(ReportProgressInternal);
      }

      private void OnReportResult(object sender, RoutedEventArgs e)
      {
         if (server == null)
            return;

         resultReporter = server.GetResultReporter();
         resultReporter.ReportResult(0, "Everything OK");
      }

      private void OnStartServer(object sender, RoutedEventArgs e)
      {
         startServer.IsEnabled = false;

         server = IpcServer.CreateServer()
            .ForCurrentProcess()
            .AddDiagnosticLogging(logger)
            .AddProcessMonitoring()
            .Start();

         disposeServer.IsEnabled = true;
         reportProgress.IsEnabled = true;
         reportResult.IsEnabled = true;
      }

      private void ReportProgressInternal()
      {
         if (server == null)
            return;

         using var reporter = server.GetProgressReporter();

         for (int i = 0; i <= 100; i++)
         {
            reporter.ReportProgress(i, string.Empty);
            Thread.Sleep(50);
         }
      }

      #endregion
   }
}