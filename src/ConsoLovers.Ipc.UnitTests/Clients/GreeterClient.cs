// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitTestClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTests.Clients;

using ConsoLovers.Ipc.UnitTests.Grpc;

internal class GreeterClient : ConfigurableClient<GreeterService.GreeterServiceClient>
{
   internal string SayHello(string name)
   {
      var response = ServiceClient.SayHello(new SayHelloRequest{ Name = name });
      return response.Message;
   }
}