// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServerBuilder.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using Microsoft.Extensions.DependencyInjection;

public interface IServerBuilder
{
   #region Public Methods and Operators

   IServerBuilder AddGrpcService<T>()
      where T : class;

   IServerBuilder AddService(Action<IServiceCollection> serviceSetup);

   IInterProcessCommunicationServer Start();

   #endregion
}