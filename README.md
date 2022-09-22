# ConsoLovers.Ipc

Usage on server side

```C#

   private static IInterProcessCommunicationServer CreateCommunicationServer()
   {
      return InterProcessCommunication.CreateServer()
         .ForCurrentProcess()
         .RemoveAspNetCoreLogging()
         .UseProgressReporter()
         .UseResultReporter()
         .Start();
   }
```

```C#
using (var communicationServer = CreateCommunicationServer())
{
    var progressServer = communicationServer.GetProgressReporter();
    var resultReporter = communicationServer.GetResultReporter();

    AnsiConsole.Progress().Start(progressContext =>
    {
        var progressTask = progressContext.AddTask("Setup Progress");
        for (var i = 0; i < 100; i++)
        {
            Thread.Sleep(400);
            progressServer.ReportProgress(i, $"Progress {i}");
            progressTask.Value = i;
        }
        });

    resultReporter.ReportResult(4, "Something went wrong");
    AnsiConsole.WriteLine("shutting down communication server");
}
```