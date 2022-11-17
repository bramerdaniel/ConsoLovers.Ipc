// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Server;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

internal class Program
{
   #region Public Methods and Operators

   public static async Task Main()
   {
      try
      {
         await DoIt();
      }
      catch (Exception e)
      {
         Console.WriteLine(e);
      }
      
      Console.ReadLine();


      await ConsoleApplication.WithArguments<ServerArgs>()
         .RunAsync();
   }

   private static async Task DoIt()
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
      reporter.ReportSuccess();
      // await server.DisposeAsync();
      await Task.Delay(TimeSpan.FromMinutes(10));
      Environment.Exit(0);
   }

   #endregion
}