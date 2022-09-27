﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Server.Commands;

using System.Diagnostics;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

using Server.Annotations;

using Spectre.Console;

[UsedImplicitly]
internal class RunCommand : ServerCommand, IAsyncCommand<RunCommand.RunArgs>
{
   #region Constants and Fields

   private readonly CancellationTokenSource tokenSource = new();

   #endregion

   #region Constructors and Destructors

   public RunCommand(IConsole console)
      : base(console)
   {
   }

   #endregion

   #region IAsyncCommand<RunArgs> Members

   public async Task ExecuteAsync(CancellationToken cancellationToken)
   {
      var serverName = GetServerName();
      AnsiConsole.WriteLine($"Starting server with name {serverName}");

      if (Arguments.WaitForKey)
      {
         AnsiConsole.MarkupLine("[blue]Waiting for ENTER to startup the server[/]");
         Console.ReadLine();
      }

      await using var communicationServer = StartServer(serverName, c => c.AddProcessMonitoring().AddCancellationHandler(CancelExecution));

      var progressReporter = communicationServer.GetProgressReporter();
      var resultReporter = communicationServer.GetResultReporter();

      var delay = Arguments.ExecutionTime * 9;
      var stopwatch = Stopwatch.StartNew();

      await AnsiConsole.Progress().StartAsync(async progressContext =>
      {
         var progressTask = progressContext.AddTask("Setup Progress");
         for (var i = 0; i <= 100; i++)
         {
            if (tokenSource.Token.IsCancellationRequested)
            {
               resultReporter.AddData("Cancellation", "Cancellation was accepted");
               break;
            }

            progressReporter.ReportProgress(i, $"Progress {i}");
            progressTask.Value = i;
            if (i == 55)
               resultReporter.AddData("FirstError", "Bend index on number 55");

            await Task.Delay(delay, cancellationToken);
         }
      });

      stopwatch.Stop();
      AnsiConsole.MarkupLine($"[blue]execution took {stopwatch.Elapsed}[/]");

      resultReporter.AddData("MoreAdditionalData", "This is additional data");
      resultReporter.ReportSuccess();
      AnsiConsole.WriteLine("shutting down communication server");

      AnsiConsole.WriteLine("Setup has finished");
   }

   public RunArgs Arguments { get; set; } = null!;

   #endregion

   #region Methods

   private bool CancelExecution()
   {
      tokenSource.Cancel();
      return tokenSource.IsCancellationRequested;
   }

   private string GetServerName()
   {
      if (!string.IsNullOrWhiteSpace(Arguments.ServerName))
         return Arguments.ServerName;

      var process = Process.GetCurrentProcess();
      return $"{process.ProcessName}.{process.Id}";
   }

   #endregion

   [UsedImplicitly]
   internal class RunArgs
   {
      #region Public Properties

      [Argument("time", "t")]
      [HelpText("Execution time in seconds")]
      public int ExecutionTime { get; set; } = 5;

      [Argument("name", "server")]
      [HelpText("The name the server should run under")]
      public string? ServerName { get; set; } = null;

      [Option("waitForStartup", "w")]
      [HelpText("Waits for a enter to startup the inter-process server")]
      public bool WaitForKey { get; set; }

      #endregion
   }
}