using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using CodeChecker.Helpers;
using CodeChecker.RevitContext.Extensions;
using CodeChecker.RevitContext.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace CodeChecker.RevitContext.Methods
{
   public static class CheckShaftsAreaDim
   {


      public static void ShaftOpeningCheckISResidential()
      {

         GetAllShaftOpeningsDes.AllShaftOpeningDesM();

         var doc = ConstantMembers.Document;

         foreach (var shaftOpeningDes in GetAllShaftOpeningsDes.AllShaftOpeningDes)
         {
            Level baselevel = new FilteredElementCollector(doc)
               .OfClass(typeof(Level))
               .Where(x => x.Name == shaftOpeningDes.BaseLevelName)
               .FirstOrDefault() as Level;

            Level Toplevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
               .Where(x => x.Name == shaftOpeningDes.TopLevelName)
               .FirstOrDefault() as Level;



            var shaftOpening = new FilteredElementCollector(doc)
              .OfCategory(BuiltInCategory.OST_ShaftOpening)
               .Cast<Opening>()
               .Where(e => e.Id.ToString() == shaftOpeningDes.ShaftOpeningID)
               .FirstOrDefault() as Opening;


            var levels = new FilteredElementCollector(doc)
               .OfClass(typeof(Level))
               .Cast<Level>()
               .Where(e => e.Elevation >= baselevel.Elevation && e.Elevation <= Toplevel.Elevation)
               .ToList();


            for (int i = 0; i < levels.Count - 1; i++)
            {
               double zposition = (levels[i].Elevation + levels[i + 1].Elevation)/2;

               // Create a new options object
               Options options = new Options();
               options.IncludeNonVisibleObjects = true;

               // Get the geometry object of the shaft opening
               var shaftOpeningSolid = shaftOpening.get_Geometry(options).First() as Solid;


               // Get the face array of the solid as PlanarFace
               var faces = shaftOpeningSolid
                  .Faces.Cast<PlanarFace>()
                  .Where(f => (int)(f.FaceNormal.Z) == 0).ToList();

               // Loop over the faces and do something with them
               foreach (PlanarFace face in faces)
               {
                  // Get the origin point of the face
                  XYZ origin = face.Origin;

                  // Get the normal vector of the face
                  XYZ normal = face.FaceNormal;


                  // Find a 3D view to use for the ReferenceIntersector constructor
                  View3D view3D = new FilteredElementCollector(doc)
                     .OfClass(typeof(View3D)).Cast<View3D>()
                     .First<View3D>(v3 => !(v3.IsTemplate));

                  XYZ point = new XYZ(origin.X, origin.Y, zposition);


                  var filterwalls = new ElementMulticlassFilter(new List<Type>() { typeof(Wall) });

                  ReferenceIntersector refIntersectorWall = new ReferenceIntersector(
                     filterwalls,
                     FindReferenceTarget.Face,
                     view3D);

                  if (refIntersectorWall != null)
                  {
                     ReferenceWithContext referenceWithContext = refIntersectorWall.FindNearest(point, normal);

                     if (referenceWithContext != null)
                     {
                        Reference reference = referenceWithContext.GetReference();
                        XYZ intersection = reference.GlobalPoint;

                        Room room = null;
                        foreach (var phase in doc.Phases)
                        {
                           // Adjust the coordinates as needed
                           XYZ testPoint = intersection + normal * 2;

                           // Try to find a room at the adjusted point in the current phase
                           var foundRoom = doc.GetRoomAtPoint(testPoint, phase as Phase);

                           // If a room is found, assign it and break out of the loop
                           if (foundRoom != null)
                           {
                              room = foundRoom;
                              break;
                           }
                        }


                        if (room != null)
                        {
                           foreach (var item in GetAllRoomsDes.ALLRoomDes)
                           {
                              if (item.RoomID == room.Id.ToString())
                              {
                                 if (item.ISResidentialRoom == true)
                                 {
                                    shaftOpeningDes.ISResidentialshaft = true;
                                    // "the Shaft with "+ShaftOpeningDes.ShaftOpeningID + "IS Residential shaft";
                                 }
                              }
                              
                           }
                        }
                     }
                  }
               }
            }
         }


      }



      public static string ShaftOpeningCheckArea(double MinAreaForResidential, double MinAreaForServices)
      {

         foreach (var ShaftOpeningDes in GetAllShaftOpeningsDes.AllShaftOpeningDes)
         {
            if (ShaftOpeningDes.ISResidentialshaft == true)
            {

               if (ShaftOpeningDes.ShaftOpeningArea.FromExternalUnitSquare() < MinAreaForResidential)
               {
                  ShaftOpeningDes.IsAreaPassed = false;
               }
            }
            else
            {

               if (ShaftOpeningDes.ShaftOpeningArea.FromExternalUnitSquare() < MinAreaForServices)
               {
                  ShaftOpeningDes.IsAreaPassed = false;
               }

            }




         }

         return "ShaftOpeningCheckArea is Determined well";
      }


      public static string ShaftOpeningCheckDim(double MinDimForResidential, double MinDimForServices)
      {

         foreach (var ShaftOpeningDes in GetAllShaftOpeningsDes.AllShaftOpeningDes)
         {
            if (ShaftOpeningDes.ISResidentialshaft == true)
            {

               if (ShaftOpeningDes.ShaftOpeningArea.FromExternalUnit() < MinDimForResidential)
               {
                  ShaftOpeningDes.IsMinDimPassed = false;
               }
            }
            else
            {

               if (ShaftOpeningDes.ShaftOpeningArea.FromExternalUnit() < MinDimForServices)
               {
                  ShaftOpeningDes.IsMinDimPassed = false;
               }

            }

         }


         return "ShaftOpeningCheckDim is Determined well ";
      }

   }
}
