# ConsoLovers.Toolkit.ProcessMonitoring.ServerExtension ![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/ConsoLovers.Toolkit.ProcessMonitoring.ServerExtension?style=plastic)

The package contains a set of extensions, that makes the usageage of the ConsoLovers.Ipc.ProcessMonitoring.Server package easier. 

```C#
      static async Task Main()
      {
         await ConsoleApplication.WithArguments<ApplicationArgs>()
            .AddProcessMonitoringServer() // This is enought for starting an ipc server for the current process
            .RunAsync();
      }
```


Configuring the inter-process communication server for the ConsoleApplication can be done inline

```C#
      static async Task Main()
      {
         await ConsoleApplication.WithArguments<ApplicationArgs>()
            .AddProcessMonitoringServer(config => config.ForName("MyServerName"))
            .RunAsync();
      }
```
