using System;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeChecker.Helpers
{
   public static class ConstantMembers
   {

      #region PUBLIC PROPERTIES

      /// <summary>
      /// Gets the Revit document.
      /// </summary>
      /// <value>
      /// The document.
      /// </value>
      public static Document Document { get; private set; }

      /// <summary>
      /// Gets the Revit UIDocument.
      /// </summary>
      /// <value>
      /// The UI document.
      /// </value>
      public static UIDocument UiDocument { get; private set; }

      /// <summary>
      /// Gets or sets the Revit UIApplication.
      /// </summary>
      /// <value>
      /// The uiapp.
      /// </value>
      public static UIApplication UiApplication { get; set; }


      #endregion

      #region CONSTRUCTORS

      /// <summary>
      /// Defualt constructor.
      /// Initializes a new instance of the <see cref="ConstantMembers"/> class.
      /// </summary>
      /// <param name="doc">The document.</param>
      public static void Initialize(UIDocument uidoc)
      {
         UiDocument = uidoc;
         Document = uidoc.Document;

      }

      #endregion

   }
}
