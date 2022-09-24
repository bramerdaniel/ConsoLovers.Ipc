# ConsoLovers.Ipc

[![ConsoLovers.Ipc](https://github.com/bramerdaniel/ConsoLovers.Ipc/actions/workflows/ConsoLovers.Ipc.yml/badge.svg?branch=master)](https://github.com/bramerdaniel/ConsoLovers.Ipc/actions/workflows/ConsoLovers.Ipc.yml)

[![NuGet version (ConsoLovers.Ipc)](https://img.shields.io/nuget/v/ConsoLovers.Ipc.svg?style=flat-square)](https://www.nuget.org/packages/ConsoLovers.Ipc/)

## What is it ?
It is a small library for inter-process communication on a single machine,
using [gRPC](https://grpc.io/) with [Unix Domain Sockets](https://de.wikipedia.org/wiki/Unix_Domain_Socket).
Microsoft has an article called [Inter-process communication with gRPC](https://learn.microsoft.com/en-us/aspnet/core/grpc/interprocess?view=aspnetcore-6.0#configure-unix-domain-sockets),
and the [ConsoLovers.Ipc package](https://www.nuget.org/packages/ConsoLovers.Ipc) provides an easy and flexible implementation, 
without having to care about all the infratructure setup, that needs to be done.

## Usage on server side
This is how you set it up on the server side 
(The process that hosts the gRPC server)

```C#
      using var server = InterProcessCommunication.CreateServer()
         .ForCurrentProcess()
         .UseDefaults()
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

         result.Success();
      }
      catch (Exception e)
      {
         result.AddData(nameof(e.StackTrace), e.StackTrace);
         result.ReportResult(1, e.Message);
      }
```

Now your application is reporting progress and returning the outcome to it's clients.

## Usage on client side

On the client side you need to create a client factory, 
that is able to create the service clients you want to use.

```C#
    var serverProcess = Process.GetProcessesByName("Server").FirstOrDefault();
    var clientFactory = InterProcessCommunication.CreateClientFactory()
       .ForProcess(serverProcess))
       .AddDefaultClients()
       .Build();
```

Once created, the factory can create the required clients,
that will give you the information provided by the server.

```C#
      var progressClient = clientFactory.CreateProgressClient();
      progressClient.ProgressChanged += (_, e) => Console.WriteLine("{0}% {1}", e.Percentage, e.Message);

      var resultClient = clientFactory.CreateResultClient();
      var result = await resultClient.WaitForResultAsync();
      Console.WriteLine("{0} {1}", result.Message, result.ExitCode);

      foreach (var keyPair in result.Data)
         Console.WriteLine("{0} {1}", keyPair.Key, keyPair.Value);
```
