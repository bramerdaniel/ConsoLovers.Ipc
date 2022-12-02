// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerTextBoxLogger.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace WpfServer;

using System;
using System.Windows.Controls;

using ConsoLovers.Ipc;

public class ServerTextBoxLogger : IServerLogger
{
   #region Constants and Fields

   private readonly ServerLogLevel minLevel;

   private readonly TextBox textBox;

   #endregion

   #region Constructors and Destructors

   public ServerTextBoxLogger(TextBox textBox, ServerLogLevel level)
   {
      this.textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
      minLevel = level;
   }

   #endregion

   #region IServerLogger Members

   public bool IsEnabled(ServerLogLevel logLevel)
   {
      if (minLevel >= logLevel)
         return true;
      return false;
   }

   public void Log(ServerLogLevel level, string message)
   {
      if (IsEnabled(level))
         AppendLine($"[{level}] {message}");
   }

   public void Log(ServerLogLevel level, Func<string> messageFunc)
   {
      if (IsEnabled(level))
         AppendLine($"[{level}] {messageFunc()}");
   }

   #endregion

   #region Methods

   private void AppendLine(string message)
   {
      if (textBox.Dispatcher.CheckAccess())
      {
         textBox.AppendText($"{message}{Environment.NewLine}");
         textBox.ScrollToEnd();
      }
      else
      {
         textBox.Dispatcher.BeginInvoke(() => AppendLine(message));
      }
   }

   #endregion
}