// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Client;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.ConsoleToolkit.Core.Builders;
using ConsoLovers.ConsoleToolkit.Core.Exceptions;
using ConsoLovers.Ipc;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
public static class Program
{
   #region Public Methods and Operators

   private static readonly IConsole console = new ConsoleProxy();

   public static async Task Main(string[] args)
   {
      try
      {
         var factory = IpcClient.CreateClientFactory()
            .ForName("server")
            .WithLogger(ClientLogLevel.Trace, Log)
            .AddResultClient()
            .Build();

         var resultClient = factory.CreateResultClient();
         console.WriteLine("Waiting for server");
         await resultClient.WaitForServerAsync(CancellationToken.None);

         console.WriteLine("Waiting for result");
         var resultInfo = await resultClient.WaitForResultAsync();
         console.WriteLine($"Result: {resultInfo.ExitCode}, {resultInfo.Message}", ConsoleColor.Green);
      }
      catch (Exception e)
      {
         console.WriteLine(e.ToString(), ConsoleColor.Red);
      }
      
      Console.WriteLine("Finished");
      Console.ReadLine();
   }

   private static void Log(string message)
   {
      Console.WriteLine("{0} : {1}", DateTime.Now, message);
   }


   private static async Task GenericMenu(string[] args)
   {
      await ConsoleApplication.WithArguments<ClientArgs>()
         .AddService(x => x.AddSingleton(CreateClientFactory))
         .UseExceptionHandler(typeof(SpectreHandler))
         .UseMenuWithoutArguments(c =>
         {
            c.MenuOptions.CloseKeys = new[] { ConsoleKey.Escape };
            c.MenuOptions.Header = new ClientArgs();
         })
         .RunAsync(args, CancellationToken.None);

      Console.WriteLine("Finished");
      Console.ReadLine();
   }

   #endregion

   #region Methods

   private static IClientFactory CreateClientFactory(IServiceProvider services)
   {
      var args = services.GetRequiredService<IConsoleApplication<ClientArgs>>();

      var process = FindProcess(args.Arguments);

      return IpcClient.CreateClientFactory()
         .ForProcess(process)
         .AddProcessMonitoringClients()
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