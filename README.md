<h1 align="center">ConsoLovers.Ipc.Sever & Client</h1>
<h4 align="center">Easy to use inter-process communication with <a href="https://grpc.io">gRPC</a> and <a href="https://de.wikipedia.org/wiki/Unix_Domain_Socket">unix domain sockets</a></h4>

<div align="center">
  <!-- License -->
    <a href="https://github.com/bramerdaniel/ConsoLovers.Ipc/blob/15d081396399fe773bb072f5b3e6a7102549aaf0/LICENSE">
      <img alt="GitHub" src="https://img.shields.io/github/license/bramerdaniel/ConsoLovers.Ipc?style=flat">
    </a>

  <!-- Build Status -->
  <a href="[https://travis-ci.org/choojs/choo](https://github.com/bramerdaniel/ConsoLovers.Ipc/actions/workflows/ConsoLovers.Ipc.yml)">
    <img src="https://github.com/bramerdaniel/ConsoLovers.Ipc/actions/workflows/ConsoLovers.Ipc.yml/badge.svg?branch=master" alt="Build Status" />
  </a>
  
   <!-- Server nuget package -->
  <a href="https://www.nuget.org/packages/ConsoLovers.Ipc.Server">
     <img alt="Nuget (with prereleases)" src="https://img.shields.io/nuget/vpre/ConsoLovers.Ipc.Server?label=nuget%20%28Sever%29">
  </a>
  
  <!-- Client nuget package -->
   <a href="https://www.nuget.org/packages/ConsoLovers.Ipc.Client">
     <img alt="Nuget (with prereleases)" src="https://img.shields.io/nuget/vpre/ConsoLovers.Ipc.Client?label=nuget%20%28Client%29">
   </a>

  
  <img alt="platform" src="https://img.shields.io/badge/platform-win%20%7C%20linux-blue">
</div>



## Description
It is a small library for inter-process communication on a single machine,
using [gRPC](https://grpc.io/) with [Unix Domain Sockets](https://de.wikipedia.org/wiki/Unix_Domain_Socket).
Microsoft has an article called [Inter-process communication with gRPC](https://learn.microsoft.com/en-us/aspnet/core/grpc/interprocess?view=aspnetcore-6.0#configure-unix-domain-sockets),
and the [ConsoLovers.Ipc.Server](https://www.nuget.org/packages/ConsoLovers.Ipc.Server) & Client packages provide an easy and flexible implementation, 
without having to care about all the infratructure setup, that needs to be done.

## The available packages

The library is splitt into serverside and client side packages. This reduces the client side dependencies to a minimum.


Package  | Version | Description
-------- | -------- | --------
ConsoLovers.Ipc.Server   | [![NuGet version (ConsoLovers.Ipc.Server)](https://img.shields.io/nuget/v/ConsoLovers.Ipc.Server.svg?style=flat)](https://www.nuget.org/packages/ConsoLovers.Ipc.Server/) | Package for the process that hosts the gRPC server
ConsoLovers.Ipc.Client   | [![NuGet version (ConsoLovers.Ipc.Client)](https://img.shields.io/nuget/v/ConsoLovers.Ipc.Client.svg?style=flat)](https://www.nuget.org/packages/ConsoLovers.Ipc.Client/)  | Package for a client process that wants to communicate with a server 

> After adding the ConsoLovers.Ipc.Server or ConsoLovers.Ipc.Client package, you do not have any other useful gRPC service than the .

## Quick start 
 - [Setup the server side](https://github.com/bramerdaniel/ConsoLovers.Ipc/wiki/Server-setup) 
 - [Setup the client side](https://github.com/bramerdaniel/ConsoLovers.Ipc/wiki/Client-setup) 
 ---
 - [Setup server side with the ConsoLovers.Toolkit.Core](https://github.com/bramerdaniel/ConsoLovers.Ipc/wiki/Server-setup) 

## Additional packages

Package  | Version | Description
-------- | -------- | --------
ConsoLovers.Ipc.ProcessMonitoring.Server   | [![NuGet version (ConsoLovers.Ipc.ProcessMonitoring.Server)](https://img.shields.io/nuget/v/ConsoLovers.Ipc.ProcessMonitoring.Server.svg?style=flat)](https://www.nuget.org/packages/ConsoLovers.Ipc.ProcessMonitoring.Server/) | Server package for a process that should be monitored
ConsoLovers.Ipc.ProcessMonitoring.Client   | [![NuGet version (ConsoLovers.Ipc.ProcessMonitoring.Client)](https://img.shields.io/nuget/v/ConsoLovers.Ipc.ProcessMonitoring.Client.svg?style=flat)](https://www.nuget.org/packages/ConsoLovers.Ipc.ProcessMonitoring.Client/)  | Client package for applications that want to monitor processes hosting the ConsoLovers.Ipc.ProcessMonitoring.Server package services

When you are using the ConsoLovers.ConsoleToolkit you can use the following packages

Package  | Version | Description
-------- | -------- | --------
ConsoLovers.Toolkit   .Ipc.ServerExtension   | ![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/ConsoLovers.Toolkit.Ipc.ServerExtension?style=plastic) | Easy usage of the ConsoLovers.Ipc.Server combination with the ConsoLovers.ConsoleToolkit.Core
ConsoLovers.Toolkit   .ProcessMonitoring.ServerExtension  | ![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/ConsoLovers.Toolkit.ProcessMonitoring.ServerExtension?style=plastic)| Easy usage of the ConsoLovers.Ipc.ProcessMonitoring.Server in combination with the ConsoLovers.ConsoleToolkit.Core







## Usage on server side
This is how you set it up on the server side with the example of the ProcessMonitoring package

```C#
      using var server = IpcServer.CreateServer()
                            .ForCurrentProcess()
                            .AddProcessMonitoring()
                            .Start();
```

When the server is up and running, you can get your added services from the servers
dependency injection container, and use them in your application logic.

```C#
      var reporter = server.GetProgressReporter();
      var result = server.GetResultReporter();

      try
      {
         for (int i = 0; i <= 100; i++)
            reporter.ReportProgress(i, $"Hello from {i}");

         result.ReportSuccess();
      }
      catch (Exception e)
      {
         result.AddData(nameof(e.StackTrace), e.StackTrace);
         result.ReportError(1, e.Message);
      }
```

Now your application is reporting progress and returning the outcome to it's clients.

## Usage on client side

On the client side you need to create a client factory, 
that is able to create the service clients you want to use.

```C#
    var serverProcess = Process.GetProcessesByName("Server").FirstOrDefault();
    
    var clientFactory = IpcClient.CreateClientFactory()
         .ForProcess(process)
         .AddProcessMonitoringClients()
         .Build();
```

Once created, the factory can create the required clients,
that will give you the information provided by the server.

```C#
      var progressClient = clientFactory.CreateProgressClient();
      progressClient.ProgressChanged += (_, e) => Console.WriteLine("{0}% {1}", e.Percentage, e.Message);

      var resultClient = clientFactory.CreateResultClient();
      var result = await resultClient.WaitForResultAsync();
      Console.WriteLine("ExitCode={0}, Message={1}", result.Message, result.ExitCode);

      foreach (var keyPair in result.Data)
         Console.WriteLine("{0} {1}", keyPair.Key, keyPair.Value);
```

## Add a custom gRPC service
DOCUMENTATION IS COMMING SOON
