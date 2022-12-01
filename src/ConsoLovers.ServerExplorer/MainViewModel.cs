// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.ServerExplorer;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

using ConsoLovers.ServerExplorer.Annotations;
using ConsoLovers.ServerExplorer.Commands;

public class MainViewModel : INotifyPropertyChanged
{
   #region Constants and Fields

   private int selectedIndex;

   private readonly ServerWatcher serverWatcher;

   #endregion

   #region Constructors and Destructors

   public MainViewModel()
   {
      var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
      serverWatcher = new ServerWatcher(path, Dispatcher.CurrentDispatcher);
      serverWatcher.ServerAdded += OnServerAdded;
      serverWatcher.ServerRemoved += OnServerRemoved;
      serverWatcher.Start();

      RefreshCommand = new AsyncCommand(Refresh, _ => true);
      CleanCommand = new AsyncCommand(Clean, _ => true);
      Servers = new ObservableCollection<ServerViewModel>();
      OpenServers = new ObservableCollection<object>();
      OpenServers.Add(this);
      Refresh();
   }

   private void OnServerRemoved(object? sender, ServerEventArgs e)
   {
      var serversToRemove = Servers.Where(x => string.Equals(x.SocketFile, e.SocketFile, StringComparison.OrdinalIgnoreCase)).ToArray();
      foreach (var serverViewModel in serversToRemove)
         RemoveServer(serverViewModel);
   }

   private void RemoveServer(ServerViewModel server)
   {
      if (server == null)
         throw new ArgumentNullException(nameof(server));
      
      server.OpenRequest -= OnOpenRequested;
      Servers.Remove(server);
      server.NotifyRemoved();
   }

   private void OnServerAdded(object? sender, ServerEventArgs e)
   {
      Servers.Add(CreateModel(e.SocketFile));
   }

   #endregion

   #region Public Events

   public event PropertyChangedEventHandler? PropertyChanged;

   #endregion

   #region Public Properties

   public ICommand CleanCommand { get; }

   public ObservableCollection<object> OpenServers { get; set; }

   public ICommand RefreshCommand { get; set; }

   public int SelectedIndex
   {
      get => selectedIndex;
      set
      {
         if (value == selectedIndex)
            return;
         selectedIndex = value;
         RaisePropertyChanged();
      }
   }

   public ObservableCollection<ServerViewModel> Servers { get; set; }

   public string Title { get; private set; } = "Servers";

   #endregion

   #region Methods

   [NotifyPropertyChangedInvocator]
   protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
   {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }

   private void Activate(ServerViewModel serverViewModel)
   {
      var indx = OpenServers.IndexOf(serverViewModel);
      SelectedIndex = indx;
   }

   private Task Clean()
   {
      foreach (var server in Servers.Where(x => !x.ProcessFound))
         File.Delete(server.SocketFile);

      return Refresh();
   }

   private void Clear()
   {
      foreach (var viewModel in Servers)
         viewModel.OpenRequest -= OnOpenRequested;
      Servers.Clear();
   }

   private ServerViewModel CreateModel(string file)
   {
      var viewModel = new ServerViewModel(file);
      viewModel.OpenRequest += OnOpenRequested;
      return viewModel;
   }

   private void OnOpenRequested(object? sender, EventArgs e)
   {
      if (sender is ServerViewModel serverViewModel)
      {
         if (!OpenServers.Contains(serverViewModel))
            OpenServers.Add(serverViewModel);

         Activate(serverViewModel);
      }
   }

   private Task Refresh()
   {
      Clear();

      var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
      foreach (var file in Directory.EnumerateFiles(path, "*.uds"))
      {
         Servers.Add(CreateModel(file));
      }

      return Task.CompletedTask;
   }

   #endregion
}