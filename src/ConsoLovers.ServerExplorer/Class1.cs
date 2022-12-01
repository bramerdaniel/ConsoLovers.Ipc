// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Class1.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.ServerExplorer;

using System;
using System.Windows.Controls;

using ConsoLovers.Ipc;

public class TextBoxLogger : IClientLogger
{
   private readonly TextBox textBox;

   private readonly ClientLogLevel level;

   public TextBoxLogger(TextBox textBox, ClientLogLevel level)
   {
      this.textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
      this.level = level;
   }

   public bool IsEnabled(ClientLogLevel logLevel)
   {
      if (level >= logLevel)
         return true;
      return false;
   }

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

   public void Log(ClientLogLevel level, string message)
   {
      if (IsEnabled(level))
         AppendLine($"[{level}] {message}");
   }

   public void Log(ClientLogLevel level, Func<string> messageFunc)
   {
      if (IsEnabled(level))
         AppendLine($"[{level}] {messageFunc()}");
   }
}