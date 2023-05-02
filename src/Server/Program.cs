// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Server;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

using Spectre.Console;

internal class Program
{
   #region Public Methods and Operators

   public static async Task Main()
   {
      try
      {
         await ReportProgress();
      }
      catch (Exception e)
      {
         AnsiConsole.WriteException(e, ExceptionFormats.ShortenEverything);
      }

      Console.ReadLine();

      await ConsoleApplication.WithArguments<ServerArgs>()
         .RunAsync();
   }

   public static async Task ReportProgress()
   {
      //await Task.Delay(1000);
      var server = IpcServer.CreateServer()
         .ForName("server")
         //.ForCurrentProcess()
         .RemoveAspNetCoreLogging()
         .AddProgressReporter()
         .AddResultReporter()
         .AddDiagnosticLogging(new ConsoleLogger(ServerLogLevel.Debug))
         .Start();

      var reporter = server.GetProgressReporter();
      await server.WaitForClientAsync(CancellationToken.None);
      Task.Run(() =>
      {
         for (int i = 0; i <= 100; i++)
         {
            reporter.ReportProgress(i, $"Finished {i,3}%");
            Task.Delay(100).GetAwaiter().GetResult();
         }
      });

      server.GetResultReporter().ReportSuccess();


      Console.ReadLine();
      await Task.Delay(TimeSpan.FromMinutes(10));
      Environment.Exit(0);
   }
   private static async Task ReportResult()
   {
      await Task.Delay(1000);
      var server = IpcServer.CreateServer()
         .ForName("server")
         .RemoveAspNetCoreLogging()
         .AddResultReporter()
         .AddDiagnosticLogging(new ConsoleLogger(ServerLogLevel.Trace))
         .Start();

      var reporter = server.GetResultReporter();
      await server.WaitForClientAsync(CancellationToken.None);
      // reporter.ReportSuccess();
      Console.ReadLine();
      await server.DisposeAsync();
      await Task.Delay(TimeSpan.FromMinutes(10));
      Environment.Exit(0);
   }

   #endregion
}