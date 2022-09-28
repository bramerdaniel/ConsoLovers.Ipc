// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GreeterClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace IpcServerExtension.Client;

using ConsoLovers.Ipc;

using IpcServerExtension.Grpc;

internal class GreeterClient : ConfigurableClient<Grpc.GreeterService.GreeterServiceClient>, IGreeterClient
{
   public async Task<string> SayHelloAsync(string name)
   {
      var helloReply = await ServiceClient.SayHelloAsync(new HelloRequest { Name = name });
      return helloReply.Message;
   }
}