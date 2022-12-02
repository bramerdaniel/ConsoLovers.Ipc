// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerViewModel.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.ServerExplorer;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using ConsoLovers.Ipc;
using ConsoLovers.Ipc.ProcessMonitoring;
using ConsoLovers.ServerExplorer.Annotations;
using ConsoLovers.ServerExplorer.Commands;

public class ServerViewModel : IClientLogger, INotifyPropertyChanged
{
   #region Constants and Fields

   private IClientFactory? clientFactory;

   private int exitCode;

   private StringBuilder messageBuilder = new();

   private int progress;

   private string progressText;

   private string resultMessage;

   private CancellationTokenSource? resultTokenSource;

   private CancellationTokenSource? progressTokenSource;

   private bool showProgress;

   private bool showResult;

   #endregion

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

      OpenCommand = new RelayCommand(_ => Open(), _ => CanOpen());
      ProgressCommand = new AsyncCommand(AwaitProgress, _ => CanAwaitProgress());
      ResultCommand = new AsyncCommand(AwaitResult, _ => CanAwaitResult());
      CancelResultCommand = new RelayCommand(_ => CancelResult());
      CancelProgressCommand = new RelayCommand(_ => CancelProgress());
   }

   #endregion

   #region Public Events

   public event EventHandler OpenRequest;

   public event PropertyChangedEventHandler? PropertyChanged;

   #endregion

   #region IClientLogger Members

   public bool IsEnabled(ClientLogLevel logLevel)
   {
      return true;
   }

   public void Log(ClientLogLevel level, string message)
   {
      if (!IsEnabled(level))
         return;

      messageBuilder.Append($"[{level}]".PadRight(10));
      messageBuilder.AppendLine(message);
      RaisePropertyChanged(nameof(Messages));
   }

   public void Log(ClientLogLevel level, Func<string> messageFunc)
   {
      if (!IsEnabled(level))
         return;

      messageBuilder.Append($"[{level}]".PadRight(10));
      messageBuilder.AppendLine(messageFunc());
      RaisePropertyChanged(nameof(Messages));
   }

   #endregion

   #region Public Properties

   public ICommand CancelResultCommand { get; }
   public ICommand CancelProgressCommand { get; }

   public int ExitCode
   {
      get => exitCode;
      set
      {
         if (value == exitCode)
            return;
         exitCode = value;
         RaisePropertyChanged();
      }
   }

   public string Messages => messageBuilder.ToString();

   public string Name { get; }

   public ICommand OpenCommand { get; set; }

   public bool ProcessFound { get; }

   public int ProcessId { get; }

   public string ProcessName { get; }

   public int Progress
   {
      get => progress;
      set
      {
         if (value == progress)
            return;
         progress = value;
         RaisePropertyChanged();
      }
   }

   public IAsyncCommand ProgressCommand { get; }

   public string ProgressText
   {
      get => progressText;
      set
      {
         if (value == progressText)
            return;
         progressText = value;
         RaisePropertyChanged();
      }
   }

   public IAsyncCommand ResultCommand { get; }

   public string ResultMessage
   {
      get => resultMessage;
      set
      {
         if (value == resultMessage)
            return;
         resultMessage = value;
         RaisePropertyChanged();
      }
   }

   public bool ShowProgress
   {
      get => showProgress;
      set
      {
         if (value == showProgress)
            return;
         showProgress = value;
         RaisePropertyChanged();
      }
   }

   public bool ShowResult
   {
      get => showResult;
      set
      {
         if (value == showResult)
            return;
         showResult = value;
         RaisePropertyChanged();
      }
   }

   public string SocketFile { get; }

   public string Title => ProcessName;

   #endregion

   #region Public Methods and Operators

   public void NotifyRemoved()
   {
   }

   #endregion

   #region Methods

   [NotifyPropertyChangedInvocator]
   protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
   {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }

   private async Task AwaitProgress()
   {
      ShowProgress = true;
      progressTokenSource = new CancellationTokenSource();

      var factory = GetOrCreateClientFactory();
      var progressClient = factory.CreateProgressClient();

      try
      {
         ProgressText = "Waiting for server";
         await progressClient.WaitForServerAsync(progressTokenSource.Token);

         progressClient.ProgressChanged += OnProgressChanged;
         await progressClient.WaitForCompletedAsync(progressTokenSource.Token);
      }
      catch (OperationCanceledException e)
      {
         Log(ClientLogLevel.Info, e.Message);
      }
      catch (IpcException e)
      {
         Log(ClientLogLevel.Info, e.Message);
      }
      finally
      {
         ShowProgress = false;
         progressClient.Dispose();
      }
   }

   private async Task AwaitResult()
   {
      resultTokenSource = new CancellationTokenSource();

      var factory = GetOrCreateClientFactory();
      var resultClient = factory.CreateResultClient();
      ShowResult = true;

      try
      {
         await resultClient.WaitForServerAsync(resultTokenSource.Token);

         var resultInfo = await resultClient.WaitForResultAsync(resultTokenSource.Token);
         ResultMessage = resultInfo.Message;
         ExitCode = resultInfo.ExitCode;
      }
      catch (OperationCanceledException e)
      {
         Log(ClientLogLevel.Info, e.Message);
      }
      catch (Exception e)
      {
         Log(ClientLogLevel.Info, e.Message);
      }
      finally
      {
         ShowResult = false;
         resultClient.Dispose();
      }
   }

   private bool CanAwaitProgress()
   {
      return !ShowProgress;
   }

   private bool CanAwaitResult()
   {
      if (ShowResult)
         return false;
      return true;
   }

   private void CancelResult()
   {
      if (resultTokenSource == null)
         return;

      resultTokenSource.Cancel();
      resultTokenSource.Dispose();
      resultTokenSource = null;
   }

   private void CancelProgress()
   {
      if (progressTokenSource == null)
         return;

      progressTokenSource.Cancel();
      progressTokenSource.Dispose();
      progressTokenSource = null;
   }

   private bool CanOpen()
   {
      return ProcessFound;
   }

   private IClientFactory GetOrCreateClientFactory()
   {
      if (clientFactory != null)
         return clientFactory;

      clientFactory = IpcClient.CreateClientFactory()
         .WithSocketFile(SocketFile)
         .WithDefaultCulture("de-DE")
         .WithLogger(this)
         .AddProcessMonitoringClients()
         .Build();

      return clientFactory;
   }

   private void OnProgressChanged(object? sender, ProgressEventArgs e)
   {
      Progress = e.Percentage;
      ProgressText = e.Message;
   }

   private void Open()
   {
      OpenRequest?.Invoke(this, EventArgs.Empty);
   }

   #endregion
}