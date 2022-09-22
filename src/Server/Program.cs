// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Server;

using System.Diagnostics;

using ConsoLovers.Ipc;


using Spectre.Console;

internal class Program
{
   #region Public Methods and Operators

   public static void Main()
   {
      Console.Title = Process.GetCurrentProcess().GetCommunicationAddress();
      AnsiConsole.WriteLine("starting communication server");

      using (var communicationServer = CreateCommunicationServer())
      {
         var progressServer = communicationServer.GetProgressReporter();
         var resultReporter = communicationServer.GetResultReporter();

         AnsiConsole.Progress().Start(progressContext =>
         {
            var progressTask = progressContext.AddTask("Setup Progress");
            for (var i = 0; i < 100; i++)
            {
               Thread.Sleep(300);
               progressServer.ReportProgress(i, $"Progress {i}");
               progressTask.Value = i;
               if (i == 55)
                  resultReporter.AddData("FirstError", "Verbogener index auf nummer 55");
            }
         });

         resultReporter.AddData("CustomData", "This went wrong");
         resultReporter.ReportResult(4, "Something went wrong");
         AnsiConsole.WriteLine("shutting down communication server");
      }

      AnsiConsole.WriteLine("Setup has finished");
   }

   #endregion

   #region Methods

   private static IInterProcessCommunicationServer CreateCommunicationServer()
   {
      return InterProcessCommunication
         .CreateServer()
         .ForCurrentProcess()
         .RemoveAspNetCoreLogging()
         .UseProgressReporter()
         .UseResultReporter()
         .Start();
   }

   #endregion
}