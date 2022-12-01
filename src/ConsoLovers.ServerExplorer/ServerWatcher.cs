// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerWatcher.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.ServerExplorer;

using System;
using System.IO;
using System.Windows.Threading;

public class ServerWatcher
{
   private readonly string socketDirectory;

   private readonly Dispatcher dispatcher;

   private FileSystemWatcher fileSystemWatcher;

   public ServerWatcher(string socketDirectory, Dispatcher dispatcher)
   {
      this.socketDirectory = socketDirectory ?? throw new ArgumentNullException(nameof(socketDirectory));
      this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
      Initialize();
   }

   private void Initialize()
   {
      if (!Directory.Exists(socketDirectory))
         return;

      fileSystemWatcher = new FileSystemWatcher(socketDirectory);
      fileSystemWatcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName;

      fileSystemWatcher.Created += OnFileCreated;
      fileSystemWatcher.Deleted += OnFileDeleted;
      fileSystemWatcher.Filter = "*.uds";
      fileSystemWatcher.IncludeSubdirectories = false;
   }

   private void OnFileDeleted(object sender, FileSystemEventArgs e)
   {
      dispatcher.BeginInvoke(() => OnServerRemoved(e.FullPath));
   }

   private void OnFileCreated(object sender, FileSystemEventArgs e)
   {
      dispatcher.BeginInvoke(() => OnServerAdded(e.FullPath));
   }

   public event EventHandler<ServerEventArgs>? ServerAdded;

   public event EventHandler<ServerEventArgs>? ServerRemoved;

   protected virtual void OnServerRemoved(string socketFile)
   {
      ServerRemoved?.Invoke(this, new ServerEventArgs(socketFile));
   }

   protected virtual void OnServerAdded(string socketFile)
   {
      ServerAdded?.Invoke(this, new ServerEventArgs(socketFile));
   }

   public void Start()
   {

      fileSystemWatcher.EnableRaisingEvents = true;
   }
}