// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelayCommand.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.ServerExplorer.Commands
{
   using System;
   using System.Diagnostics;
   using System.Windows.Input;

   public class RelayCommand : ICommand
   {
      #region Constants and Fields

      private readonly Predicate<object> canExecute;

      private readonly Action<object> execute;

      #endregion

      #region Constructors and Destructors

      /// <summary>Initializes a new instance of the <see cref="RelayCommand"/> class. Creates a new command that can always execute.</summary>
      /// <param name="execute">The execution logic.</param>
      public RelayCommand(Action<object> execute)
         : this(execute, null)
      {
      }

      /// <summary>Initializes a new instance of the <see cref="RelayCommand"/> class.</summary>
      /// <param name="execute">The execution logic.</param>
      /// <param name="canExecute">The execution status logic.</param>
      public RelayCommand(Action<object> execute, Predicate<object> canExecute)
      {
         if (execute == null)
            throw new ArgumentNullException("execute");

         this.execute = execute;
         this.canExecute = canExecute;
      }

      #endregion

      #region Public Events

      /// <summary>Occurs when changes occur that affect whether or not the command should execute.</summary>
      public event EventHandler CanExecuteChanged
      {
         add => CommandManager.RequerySuggested += value;
         remove => CommandManager.RequerySuggested -= value;
      }

      #endregion

      #region Public Methods and Operators

      /// <summary>Raises the can execute changed.</summary>
      public void RaiseCanExecuteChanged()
      {
         CommandManager.InvalidateRequerySuggested();
      }

      #endregion

      #region ICommand

      /// <summary>Defines the method that determines whether the command can execute in its current state.</summary>
      /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
      /// <returns>true if this command can be executed; otherwise, false.</returns>
      [DebuggerStepThrough]
      public bool CanExecute(object parameter)
      {
         return canExecute == null ? true : canExecute(parameter);
      }

      /// <summary>Defines the method to be called when the command is invoked.</summary>
      /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
      public void Execute(object parameter)
      {
         execute(parameter);
      }

      #endregion
   }
}