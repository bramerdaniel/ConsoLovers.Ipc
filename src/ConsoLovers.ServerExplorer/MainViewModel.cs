// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.ServerExplorer;

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using ConsoLovers.ServerExplorer.Commands;

public class MainViewModel
{
   #region Constructors and Destructors

   public MainViewModel()
   {
      RefreshCommand = new AsyncCommand(Refresh, _ => true);
      CleanCommand = new AsyncCommand(Clean, _ => true);
      Servers = new ObservableCollection<ServerViewModel>();
   }

   private Task Clean()
   {
      foreach (var server in Servers.Where(x => !x.ProcessFound))
         File.Delete(server.SocketFile);

      return Refresh();
   }

   public ICommand CleanCommand { get; }

   #endregion

   #region Public Properties

   public ICommand RefreshCommand { get; set; }

   public ObservableCollection<ServerViewModel> Servers { get; set; }

   #endregion

   #region Methods

   private Task Refresh()
   {
      Servers.Clear();

      var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
      foreach (var file in Directory.EnumerateFiles(path, "*.uds"))
         Servers.Add(new ServerViewModel(file));

      return Task.CompletedTask;
   }

   #endregion
}