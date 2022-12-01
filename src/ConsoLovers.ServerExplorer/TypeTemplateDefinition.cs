// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Class1.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.ServerExplorer;

using System;
using System.Windows;

public class TypeTemplateDefinition
{
   #region Public Properties

   /// <summary>Gets or sets the DataTemplate to select for this item type.</summary>
   public DataTemplate Template { get; set; }

   /// <summary>Gets or sets the item type to define the template for.</summary>
   public Type Type { get; set; }

   #endregion
}