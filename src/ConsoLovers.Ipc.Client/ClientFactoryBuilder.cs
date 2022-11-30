// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientFactoryBuilder.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.DependencyInjection;

internal class ClientFactoryBuilder : IClientFactoryBuilder, IClientFactoryBuilderWithoutName
{
   #region Constants and Fields

   private readonly ServiceCollection serviceCollection;
   
   private CultureInfo? clientCulture;

   private IClientLogger? loggerToUse;
   
   private Func<string> resolveSocketFile;

   #endregion

   #region Constructors and Destructors

   public ClientFactoryBuilder()
   {
      serviceCollection = new ServiceCollection();
      serviceCollection.AddSingleton<IClientFactoryBuilder>(this);
   }

   #endregion

   #region IClientFactoryBuilder Members

   public IClientFactoryBuilder AddClient<T>()
      where T : class, IConfigurableClient
   {
      return AddService(services => services.AddSingleton<T>());
   }

   /// <summary>Adds a service to the <see cref="IClientFactoryBuilder"/>.</summary>
   /// <param name="services">The services.</param>
   /// <returns>The <see cref="IClientFactoryBuilder"/> the method was called on</returns>
   /// <exception cref="System.ArgumentNullException">services</exception>
   public IClientFactoryBuilder AddService(Action<ServiceCollection> services)
   {
      if (services == null)
         throw new ArgumentNullException(nameof(services));

      services(serviceCollection);
      return this;
   }

   /// <summary>Specifies the default culture the clients will be created with.</summary>
   /// <param name="culture">
   ///    The default client culture every client will be created with, when no other culture is specified in the
   ///    <see cref="IClientFactory.CreateClient{T}()"/> method.
   /// </param>
   /// <returns>The <see cref="IClientFactoryBuilder"/> the method was called on</returns>
   /// <exception cref="System.ArgumentNullException">culture</exception>
   public IClientFactoryBuilder WithDefaultCulture(CultureInfo culture)
   {
      clientCulture = culture ?? throw new ArgumentNullException(nameof(culture));
      return this;
   }

   public IClientFactoryBuilder WithLogger(IClientLogger logger)
   {
      loggerToUse = logger;
      return this;
   }

   /// <summary>Specifies the default culture the clients will be created with.</summary>
   /// <param name="culture">
   ///    The default client culture name every client will be created with, when no other culture is specified in the
   ///    <see cref="IClientFactory.CreateClient{T}()"/> method.
   /// </param>
   /// <returns>The <see cref="IClientFactoryBuilder"/> the method was called on</returns>
   public IClientFactoryBuilder WithDefaultCulture(string culture)
   {
      if (culture == null)
         throw new ArgumentNullException(nameof(culture));

      var cultureInfo = CultureInfo.GetCultureInfo(culture);
      return WithDefaultCulture(cultureInfo);
   }

   public IClientFactory Build()
   {
      var socketFile = resolveSocketFile();
      EnsureValidFilePath(socketFile, "ResolvedSocketFile");

      if (socketFile == null)
         throw new InvalidOperationException($"The {nameof(socketFile)} was not specified yet");
      
      var logger = loggerToUse ?? new ClientDelegateLogger(_ => { });
      serviceCollection.AddSingleton(logger);

      var channelFactory = new ChannelFactory(socketFile, logger);
      serviceCollection.AddSingleton(channelFactory);

      var providerFactory = new DefaultServiceProviderFactory(new ServiceProviderOptions { ValidateOnBuild = true });
      var serviceProvider = providerFactory.CreateServiceProvider(serviceCollection);

      var clientFactory = new ClientFactory(serviceProvider, channelFactory, clientCulture);
      logger.Debug("Created client factory");
      return clientFactory;
   }

   #endregion

   #region IClientFactoryBuilderWithoutName Members

   public IClientFactoryBuilder ForName(string name)
   {
      if (name == null)
         throw new ArgumentNullException(nameof(name));

      EnsureValidFileName(name);
      return WithSocketFile(() => Path.Combine(GetSocketDirectory(), $"{name}.uds"));
   }

   private string GetSocketDirectory()
   {
      var socketDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
      loggerToUse?.Debug($"Using socket directory {socketDirectory}");
      return socketDirectory;
   }

   public IClientFactoryBuilder ForProcess(Process process)
   {
      if (process == null)
         throw new ArgumentNullException(nameof(process));

      return ForName($"{process.ProcessName}.{process.Id}");
   }

   public IClientFactoryBuilder WithSocketFile(Func<string> computeSocketFile)
   {
      resolveSocketFile = computeSocketFile ?? throw new ArgumentNullException(nameof(computeSocketFile));
      return this;
   }
   

   public IClientFactoryBuilder WithSocketFile(string file)
   {
      return WithSocketFile(() => file);
   }

   #endregion

   #region Methods

   private static void EnsureValidFileName(string fileName, [CallerArgumentExpression("fileName")] string? callerExpression = null)
   {
      if (fileName is null)
         throw new ArgumentNullException(callerExpression);

      if (string.IsNullOrWhiteSpace(fileName))
         throw new ArgumentException(callerExpression, $"{callerExpression} must not be empty.");

      if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
         throw new ArgumentNullException(callerExpression, $"{callerExpression} is not a valid file name.");
   }

   private static void EnsureValidFilePath(string fileName, [CallerArgumentExpression("fileName")] string? callerExpression = null)
   {
      if (fileName is null)
         throw new ArgumentNullException(callerExpression);

      if (string.IsNullOrWhiteSpace(fileName))
         throw new ArgumentException(callerExpression, $"{callerExpression} must not be empty.");

      if (fileName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
         throw new ArgumentNullException(callerExpression, $"{callerExpression} is not a valid file name.");
   }

   #endregion
}