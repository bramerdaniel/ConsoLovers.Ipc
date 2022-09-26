// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpectreHandler.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Client;

using ConsoLovers.ConsoleToolkit.Core;

using Spectre.Console;

public class SpectreHandler : IExceptionHandler
{
   #region IExceptionHandler Members

   public bool Handle(Exception exception)
   {
      AnsiConsole.WriteException(exception, ExceptionFormats.ShortenTypes);
      Console.ReadLine();
      return true;
   }

   #endregion
}