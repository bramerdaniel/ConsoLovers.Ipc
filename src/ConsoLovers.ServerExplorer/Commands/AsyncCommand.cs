// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncCommand.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.ServerExplorer.Commands;

using System;
using System.Threading.Tasks;
using System.Windows.Input;

/// <summary>An implementation of IAsyncCommand. Allows Commands to safely be used asynchronously with Task.</summary>
public sealed class AsyncCommand<T> : IAsyncCommand<T>
{
   #region Constructors and Destructors

   #region Constructors

   /// <summary>Initializes a new instance of the <see cref="T:TaskExtensions.MVVM.AsyncCommand`1"/> class.</summary>
   /// <param name="execute">
   ///    The Function executed when Execute or ExecuteAsync is called. This does not check canExecute before executing and will execute
   ///    even if canExecute is false
   /// </param>
   /// <param name="canExecute">The Function that verifies whether or not AsyncCommand should execute.</param>
   /// <param name="onException">
   ///    If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be
   ///    re-thrown
   /// </param>
   /// <param name="continueOnCapturedContext">
   ///    If set to <c>true</c> continue on captured context; this will ensure that the Synchronization Context returns
   ///    to the calling thread. If set to <c>false</c> continue on a different context; this will allow the Synchronization Context to continue on a
   ///    different thread
   /// </param>
   public AsyncCommand(Func<T, Task> execute, Func<object, bool>? canExecute = null,
      Action<Exception>? onException = null,
      bool continueOnCapturedContext = true)
   {
      this.execute = execute ?? throw new ArgumentNullException(nameof(execute), $"{nameof(execute)} cannot be null");
      this.canExecute = canExecute ?? (_ => true);
      this.onException = onException;
      this.continueOnCapturedContext = continueOnCapturedContext;
   }

   #endregion

   #endregion

   #region Public Events

   /// <summary>Occurs when changes occur that affect whether or not the command should execute</summary>
   public event EventHandler? CanExecuteChanged
   {
      add => CommandManager.RequerySuggested += value;
      remove => CommandManager.RequerySuggested -= value;
   }

   #endregion

   #region IAsyncCommand<T> Members

   /// <summary>Determines whether the command can execute in its current state</summary>
   /// <returns><c>true</c>, if this command can be executed; otherwise, <c>false</c>.</returns>
   /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
   public bool CanExecute(object? parameter) => canExecute(parameter);

   /// <summary>Executes the Command as a Task</summary>
   /// <returns>The executed Task</returns>
   /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
   public Task ExecuteAsync(T parameter) => execute(parameter);

   void ICommand.Execute(object? parameter)
   {
      if (parameter is T validParameter)
         ExecuteAsync(validParameter).SafeFireAndForget(continueOnCapturedContext, onException);
      else if (parameter is null && !typeof(T).IsValueType)
         ExecuteAsync((T)parameter).SafeFireAndForget(continueOnCapturedContext, onException);
      else
         throw new InvalidOperationException();
   }

   #endregion

   #region Constant Fields

   readonly Func<T, Task> execute;

   readonly Func<object?, bool> canExecute;

   readonly Action<Exception>? onException;

   readonly bool continueOnCapturedContext;

   #endregion
}

/// <summary>An implementation of IAsyncCommand. Allows Commands to safely be used asynchronously with Task.</summary>
public sealed class AsyncCommand : IAsyncCommand
{
   #region Constructors and Destructors

   #region Constructors

   /// <summary>Initializes a new instance of the <see cref="T:TaskExtensions.MVVM.AsyncCommand`1"/> class.</summary>
   /// <param name="execute">
   ///    The Function executed when Execute or ExecuteAsync is called. This does not check canExecute before executing and will execute
   ///    even if canExecute is false
   /// </param>
   /// <param name="canExecute">The Function that verifies whether or not AsyncCommand should execute.</param>
   /// <param name="onException">
   ///    If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be
   ///    re-thrown
   /// </param>
   /// <param name="continueOnCapturedContext">
   ///    If set to <c>true</c> continue on captured context; this will ensure that the Synchronization Context returns
   ///    to the calling thread. If set to <c>false</c> continue on a different context; this will allow the Synchronization Context to continue on a
   ///    different thread
   /// </param>
   public AsyncCommand(Func<Task> execute, Func<object, bool>? canExecute = null, Action<Exception>? onException = null,
      bool continueOnCapturedContext = true)
   {
      this.execute = execute ?? throw new ArgumentNullException(nameof(execute), $"{nameof(execute)} cannot be null");
      this.canExecute = canExecute ?? (_ => true);
      this.onException = onException;
      this.continueOnCapturedContext = continueOnCapturedContext;
   }

   #endregion

   #endregion

   #region Public Events

   /// <summary>Occurs when changes occur that affect whether or not the command should execute</summary>
   public event EventHandler? CanExecuteChanged
   {
      add => CommandManager.RequerySuggested += value;
      remove => CommandManager.RequerySuggested -= value;
   }

   #endregion

   #region IAsyncCommand Members

   /// <summary>Determines whether the command can execute in its current state</summary>
   /// <returns><c>true</c>, if this command can be executed; otherwise, <c>false</c>.</returns>
   /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
   public bool CanExecute(object parameter) => canExecute(parameter);

   /// <summary>Executes the Command as a Task</summary>
   /// <returns>The executed Task</returns>
   public Task ExecuteAsync() => execute();

   void ICommand.Execute(object parameter) => execute().SafeFireAndForget(continueOnCapturedContext, onException);

   #endregion

   #region Constant Fields

   readonly Func<Task> execute;

   readonly Func<object, bool> canExecute;

   readonly Action<Exception>? onException;

   readonly bool continueOnCapturedContext;
   
   #endregion
}