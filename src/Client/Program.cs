// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Client;

using System.Diagnostics;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.ConsoleToolkit.Core.Builders;
using ConsoLovers.ConsoleToolkit.Core.Exceptions;
using ConsoLovers.Ipc;
using ConsoLovers.Ipc.Client;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console;

public static class Program
{
   #region Public Methods and Operators

   public static async Task Main()
   {
      Initialize(1000);

      await ConsoleApplication.WithArguments<ClientArgs>()
         .AddService(x => x.AddSingleton(CreateClientFactory))
         .UseExceptionHandler(typeof(SpectreHandler))
         .UseApplicationLogic(typeof(ShowProgressLogic))
         .RunAsync();
   }

   #endregion

   #region Methods

   private static IClientFactory CreateClientFactory(IServiceProvider services)
   {
      var args = services.GetRequiredService<IConsoleApplication<ClientArgs>>();

      var process = FindProcess(args.Arguments);

      return IpcClient.CreateClientFactory()
         .ForProcess(process)
         .AddProgressClient()
         .AddResultClient()
         .AddCancellationClient()
         .Build();
   }

   private static Process FindProcess(ClientArgs args)
   {
      var processes = Process.GetProcessesByName(args.ProcessName);
      if (processes.Length == 0)
         throw new CommandLineArgumentValidationException($"A process with name {args.ProcessName} could not be found");
      if (processes.Length == 1)
         return processes[0];
      return SelectProcess(processes);
   }

   private static void Initialize(int startupDelay)
   {
      Console.Title = "client";

      AnsiConsole.Progress().Start(progressContext =>
      {
         var progressTask = progressContext.AddTask("Startup delay");
         var waitTime = startupDelay / 100;

         for (var i = 0; i < 100; i++)
         {
            Thread.Sleep(waitTime);
            progressTask.Value = i;
         }
      });

      Console.Clear();
   }

   private static Process SelectProcess(Process[] processes)
   {
      var prompt = new SelectionPrompt<Process>()
         .Title("Select the progress")
         .PageSize(10)
         .MoreChoicesText("[grey](Move up and down to find more processed)[/]")
         .AddChoices(processes)
         .UseConverter(process => $"{process.ProcessName} ({process.Id})");

      var selectedProcess = AnsiConsole.Prompt(prompt);
      if (selectedProcess == null)
         throw new CommandLineArgumentValidationException($"User canceled the process selection");

      return selectedProcess;
   }

   #endregion
}