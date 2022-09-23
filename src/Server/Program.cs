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

   static CancellationTokenSource tokenSource = new();

   public static void Main()
   {
      Console.Title = Process.GetCurrentProcess().GetServerName();
      AnsiConsole.WriteLine("starting communication server");

      using (var communicationServer = CreateCommunicationServer())
      {
         var progressReporter = communicationServer.GetProgressReporter();
         var resultReporter = communicationServer.GetResultReporter();

         AnsiConsole.Progress().Start(progressContext =>
         {
            var progressTask = progressContext.AddTask("Setup Progress");
            for (var i = 0; i <= 100; i++)
            {
               if (tokenSource.Token.IsCancellationRequested)
               {
                  resultReporter.AddData("Cancellation", "Cancellation was accepted");
                  break;
               }

               Task.Delay(30).Wait();

               progressReporter.ReportProgress(i, $"Progress {i}");
               progressTask.Value = i;
               if (i == 55)
               {
                  resultReporter.AddData("FirstError", "Verbogener index auf nummer 55");
               }
            }
         });

         resultReporter.AddData("MoreAdditionalData", "This is additional data");
         resultReporter.Success();
         AnsiConsole.WriteLine("shutting down communication server");
      }
      
      AnsiConsole.WriteLine("Setup has finished");
   }

   private static bool CancelExecution()
   {
      tokenSource.Cancel();
      return tokenSource.IsCancellationRequested;
   }

   #endregion

   #region Methods

   private static IInterProcessCommunicationServer CreateCommunicationServer()
   {
      return InterProcessCommunication
         .CreateServer()
         .ForCurrentProcess()
         .UseDefaults()
         .UseCancellationHandler(CancelExecution)
         .Start();
   }

   #endregion
}