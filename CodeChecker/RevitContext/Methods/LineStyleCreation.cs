using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeChecker.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeChecker.RevitContext.Methods
{
   public static class LineStyleCreation
   {
      /// <summary>
      /// Create a new line style using NewSubcategory
      /// </summary>
      public static GraphicsStyle CreateLineStyle( string name = "ITI-CodeCheckAddin")
      {

         var doc = ConstantMembers.Document;
         var uidoc = ConstantMembers.UiDocument;
         // Use this to access the current document in a macro.
         //
         // Document doc = this.ActiveUIDocument.Document;

         // Find existing linestyle.  Can also opt to
         // create one with LinePatternElement.Create()

         FilteredElementCollector fec = new FilteredElementCollector(doc)
             .OfClass(typeof(LinePatternElement));

         LinePatternElement linePatternElem = fec
             .Cast<LinePatternElement>()
             .First<LinePatternElement>(linePattern => linePattern.Name == "Overhead");

         // The new linestyle will be a subcategory 
         // of the Lines category        

         Categories categories = doc.Settings.Categories;

         Category lineCat = categories.get_Item(BuiltInCategory.OST_Lines);

         // Category newLineStyleCat = categories.get_Item("ITI-CheckLineStyle");


         var newLineStyleCat = doc.Settings.Categories.get_Item(
      BuiltInCategory.OST_Lines).SubCategories.Cast<Category>().FirstOrDefault(e => e.Name == name);


         if (newLineStyleCat == null)
         {

            using (Transaction t = new Transaction(doc))
            {


               t.Start("Create LineStyle");
               newLineStyleCat = categories.NewSubcategory(lineCat, name);
               doc.Regenerate();

               // Set the linestyle properties 
               // (weight, color, pattern).
               newLineStyleCat.SetLineWeight(10, GraphicsStyleType.Projection);
               newLineStyleCat.LineColor = new Color(0xFF, 0x00, 0x00);
               newLineStyleCat.SetLinePatternId(linePatternElem.Id, GraphicsStyleType.Projection);

               t.Commit();





            }
         }

         return newLineStyleCat.GetGraphicsStyle(GraphicsStyleType.Projection);
      }
   }
}
