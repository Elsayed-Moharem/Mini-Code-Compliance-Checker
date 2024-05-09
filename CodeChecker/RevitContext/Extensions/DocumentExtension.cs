using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeChecker.RevitContext.Extensions
{
   public static class DocumentExtension
   {

      public static void CreateIso(this Document document)
      {
         // Find a 3D view type

         IEnumerable<ViewFamilyType> viewFamilyTypes = from elem in new FilteredElementCollector(document).OfClass(typeof(ViewFamilyType))
                                                       let type = elem as ViewFamilyType
                                                       where type.ViewFamily == ViewFamily.ThreeDimensional
                                                       select type;

         // Create a new View3D
         View3D view3D = View3D.CreateIsometric(document, viewFamilyTypes.First().Id);
         if (null != view3D)
         {
            // By default, the 3D view uses a default orientation.
            // Change the orientation by creating and setting a ViewOrientation3D 
            XYZ eye = new XYZ(10, 10, 10);
            XYZ up = new XYZ(0, 0, 1);
            XYZ forward = new XYZ(1, 1, 1);

            ViewOrientation3D viewOrientation3D = new ViewOrientation3D(eye, up, forward);
            view3D.SetOrientation(viewOrientation3D);
         }
      }


   }
}
