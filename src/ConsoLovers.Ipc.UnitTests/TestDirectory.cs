// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestDirectory.cs" company="KUKA Roboter GmbH">
//   Copyright (c) KUKA Roboter GmbH 2006 - 2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTests
{
   using System;
   using System.Diagnostics;
   using System.IO;
   using System.Linq;

   using Microsoft.VisualStudio.TestTools.UnitTesting;

   /// <summary>The test directory.</summary>
   public class TestDirectory : IDisposable
   {
      #region Constructors and Destructors

      internal static TestDirectory ForContext(TestContext testContext)
      {
         var path = Path.Combine(Path.GetTempPath(), $"{Process.GetCurrentProcess().Id}");
         return new TestDirectory(path);
      }

      /// <summary>Initializes a new instance of the <see cref="TestDirectory"/> class.</summary>
      private TestDirectory(string root)
      {
         Root = root;
         if (Directory.Exists(Root))
            Directory.Delete(Root, true);
         EnsureRoot();
      }

      #endregion

      #region IDisposable Members

      /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      #endregion

      #region Public Properties

      /// <summary>Gets the root dir.</summary>
      public string Root { get; }

      #endregion

      #region Public Methods and Operators

      /// <summary>Saves the embedded file with the specified name.</summary>
      /// <param name="name">The name.</param>
      /// <returns>The full path of the saved file.</returns>
      public string SaveEmbeddedFile(string name)
      {
         var resourceNames = GetType().Assembly.GetManifestResourceNames().Where(n => n.EndsWith($".{name}", StringComparison.InvariantCultureIgnoreCase)).ToArray();
         if (resourceNames.Length == 0)
            throw new ArgumentException("No resource with specified name found.", name);
         if (resourceNames.Length > 1)
            throw new ArgumentException("Multiple resources with specified name found.", name);

         using (var embeddedStream = GetType().Assembly.GetManifestResourceStream(resourceNames[0]))
         {
            EnsureRoot();
            string fullPath = Path.Combine(Root, name);
            using (Stream fileStream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
               // ReSharper disable once PossibleNullReferenceException
               embeddedStream.CopyTo(fileStream);
            }
            return fullPath;
         }
      }

      /// <summary>Saves the specified content to a file inside the <see cref="TestDirectory"/>.</summary>
      /// <param name="fileName">Name of the file in the directory.</param>
      /// <param name="content">The content of the file.</param>
      /// <returns>The full path of the file</returns>
      public string SaveContent(string fileName, string content)
      {
         EnsureRoot();

         var fullPath = Path.Combine(Root, fileName);
         File.WriteAllText(fullPath, content);
         return fullPath;
      }

      #endregion

      #region Methods

      /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
      /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
      protected virtual void Dispose(bool disposing)
      {
         if (Directory.Exists(Root))
            Directory.Delete(Root, true);
      }

      private void EnsureRoot()
      {
         if (!Directory.Exists(Root))
            Directory.CreateDirectory(Root);
      }

      #endregion
   }
}