// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetupBase.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTests.Setups
{
   using System;
   using System.Collections.Generic;

   /// <summary>Base class for setting up a test object using a fluent dependency registration</summary>
   /// <typeparam name="T"></typeparam>
   public abstract class SetupBase<T>
      where T : class
   {
      #region Constants and Fields

      private readonly Dictionary<string, object> fields = new();

      #endregion

      #region Methods

      internal T Done()
      {
         var instance = CreateInstance();
         SetupInstance(instance);
         return instance;
      }

      protected bool Add<TFt>(string name, TFt value)
      {
         if (!fields.TryGetValue(name, out var list))
         {
            Set<IList<TFt>>(name, new List<TFt> { value });
            return true;
         }

         if (list is IList<TFt> typedList)
         {
            typedList.Add(value);
            return true;
         }

         return false;
      }

      protected bool Contains<TFt>(string name)
      {
         if (fields.TryGetValue(name, out var untypedValue))
         {
            try
            {
               return untypedValue is TFt;
            }
            catch (Exception)
            {
               return false;
            }
         }

         return false;
      }

      /// <summary>Creates the instance of the setup object.// </summary>
      /// <returns>The created instance</returns>
      protected abstract T CreateInstance();

      protected TFt Get<TFt>(string name)
      {
         if (fields.TryGetValue(name, out var value))
            return (TFt)value;

         throw new InvalidOperationException($"A field with the name {name} was not specified");
      }

      protected TFt Get<TFt>(string name, Func<TFt> defaultValue)
      {
         if (fields.TryGetValue(name, out var value))
            return (TFt)value;

         return defaultValue();
      }

      protected IList<TFt> GetMany<TFt>(string name, Func<IList<TFt>> defaultValue)
      {
         if (fields.TryGetValue(name, out var value))
            return value as IList<TFt>;

         throw new InvalidOperationException($"A field with the name {name} was not specified");
      }

      protected void Set<TFt>(string name, TFt value)
      {
         fields.Add(name, value);
      }

      protected virtual void SetupInstance(T instance)
      {
      }

      protected bool TryGet<TFt>(string name, out TFt value)
      {
         if (fields.TryGetValue(name, out var untypedValue))
         {
            value = (TFt)untypedValue;
            return true;
         }

         value = default(TFt);
         return false;
      }

      #endregion
   }
}