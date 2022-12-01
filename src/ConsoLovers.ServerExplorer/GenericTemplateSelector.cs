// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericTemplateSelector.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.ServerExplorer;

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

public class GenericTemplateSelector : DataTemplateSelector
{
   #region Constructors and Destructors

   /// <summary>Initializes a new instance of the <see cref="GenericTemplateSelector"/> class. Initializes a new instance of the TypeTemplateSelector class.</summary>
   public GenericTemplateSelector()
   {
      TemplateDefinitions = new List<TypeTemplateDefinition>();
   }

   #endregion

   #region Public Properties

   /// <summary>Gets or sets the list of template definitions.</summary>
   public List<TypeTemplateDefinition> TemplateDefinitions { get; set; }

   #endregion

   #region Public Methods and Operators

   /// <summary>Selects a DataTemplate for the item, based on the item's type.</summary>
   /// <param name="item">Item to select the template for.</param>
   /// <param name="container">The container of the item.</param>
   /// <returns>The found data template</returns>
   public override DataTemplate SelectTemplate(object item, DependencyObject container)
   {
      foreach (var def in TemplateDefinitions)
      {
         if (def.Type.IsInstanceOfType(item))
            return def.Template;
      }

      return base.SelectTemplate(item, container);
   }

   #endregion
}