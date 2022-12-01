// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerViewModel.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.ServerExplorer;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

public class ServerViewModel
{
   #region Constructors and Destructors

   public ServerViewModel(string socketFile)
   {
      SocketFile = socketFile ?? throw new ArgumentNullException(nameof(socketFile));
      Name = Path.GetFileNameWithoutExtension(socketFile);
      var strings = Name.Split('.');
      if (strings.Length == 2)
      {
         ProcessName = strings[0];
         if (int.TryParse(strings[1], out var id))
            ProcessId = id;

         var process = Process.GetProcesses().FirstOrDefault(x => x.Id == ProcessId);
         if (process != null && process.ProcessName == ProcessName)
            ProcessFound = true;
      }
   }

   #endregion

   #region Public Properties

   public string Name { get; }

   public bool ProcessFound { get; }

   public int ProcessId { get; }

   public string ProcessName { get; }

   public string SocketFile { get; }

   #endregion
}