using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using CodeChecker.Helpers;
using CodeChecker.RevitContext.Extensions;
using CodeChecker.RevitContext.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace CodeChecker.RevitContext.Methods
{
   public class CheckClearHeight
   {

      //Create ModelCurve at where Height in less than MinClearHeight
      public static List<ModelCurveDes> ModelCurvesDes { set; get; } = new List<ModelCurveDes>();

      //public static HashSet<RoomDes> RoomsDes { set; get; } = new HashSet<RoomDes>();


      /// <summary>
      /// Check Clearance in Building and return List <modelCurves> after Drawing it 
      /// </summary>
      /// <param name="MinClearHeight"> Double Min Clear Height </param>
      /// <returns></returns>
      public static string RayProjection(Double MinClearHeight)
      {

         var doc = ConstantMembers.Document;
         var uidoc = ConstantMembers.UiDocument;


         var newLineStyle= LineStyleCreation.CreateLineStyle();

         // Find all door instances in the project by finding all elements that both belong to the door category and are family instances.
         ElementClassFilter familyInstanceFilter = new ElementClassFilter(typeof(FamilyInstance));
         ElementCategoryFilter doorsCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
         LogicalAndFilter doorInstancesFilter = new LogicalAndFilter(familyInstanceFilter, doorsCategoryfilter);


         // Apply the filter to the elements in the active document
         var doors = new FilteredElementCollector(doc).WherePasses(doorInstancesFilter).ToElements()
             .Select(e => e as FamilyInstance)
             .Where(e => (e.Host.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls));

         //Create SpecialLinestyle
         //LineStyle.CreateLineStyle();


         if (doors == null)
         {
            return " NO Doors Are Added To Model";
         }
         else
         {
            foreach (FamilyInstance door in doors)
            {

               using (Transaction trans = new Transaction(doc, "Check Clear Height"))
               {
                  try
                  {
                     trans.Start();

                     // Get Ray Lines 
                     var Lines = CalculateLineTillFloor(doc, door);




                     if (Lines != null)
                     {
                        foreach (var line in Lines)
                        {

                           if (line.Length < MinClearHeight)
                           {

                              Plane plane = null;
                              // Create a model curve to show the distance
                              if ((int)Math.Abs(door.FacingOrientation.X) == 0)
                              {
                                 plane = Plane.CreateByNormalAndOrigin(XYZ.BasisX, line.GetEndPoint(0));

                              }
                              else if ((int)Math.Abs(door.FacingOrientation.Y) == 0)
                              {
                                 plane = Plane.CreateByNormalAndOrigin(XYZ.BasisY, line.GetEndPoint(0));

                              }

                              SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
                              
                              
                              ModelCurve curve = doc.Create.NewModelCurve(line, sketchPlane);
                              curve.LineStyle = newLineStyle;

                              ModelCurvesDes.Add(

                                   new ModelCurveDes(curve.Id.ToString()
                                   , (doc.GetElement(door.LevelId) as Level).Name.ToString()
                                   , curve.GeometryCurve.Length.ToExternalUnit().RoundToOneDecimalPlace()
                                      ));

                              // Show a message with the length value
                              //  double Valueinft = UnitUtils.ConvertToInternalUnits(line.Length, UnitTypeId.Millimeters);
                              //  double valueInMillimeters = UnitUtils.ConvertFromInternalUnits(line.Length, UnitTypeId.Millimeters);
                              //TaskDialog.Show("Distance", "ClearHeight: " + String.Format("{0:f2}", valueInMillimeters));
                           }

                        }

                     }
                     trans.Commit();
                  }
                  catch (Exception ex)
                  {

                     trans.RollBack();
                     return ex.Message;

                  }
               }

            }

            return "succeed";
         }
      }

      /// <summary>
      /// >Determines the line segment that connects the Button of Door to the nearest floor.
      /// </summary>
      /// <param name="doc"> Document</param>
      /// <param name="door">Family Instance of Type Door</param>
      /// <returns></returns>
      private static List<Line> CalculateLineTillFloor(Document doc, FamilyInstance door)
      {
         // Find a 3D view to use for the ReferenceIntersector constructor
         FilteredElementCollector collector = new FilteredElementCollector(doc);
         Func<View3D, bool> isNotTemplate = v3 => !(v3.IsTemplate);
         View3D view3D = collector.OfClass(typeof(View3D)).Cast<View3D>().First<View3D>(isNotTemplate);

         // Use the BUTTOM OF Door bounding box as the start point.
         BoundingBoxXYZ box = door.get_BoundingBox(view3D);
         XYZ boundarymin = box.Min;
         XYZ boundarymax = box.Max;

         XYZ center = new XYZ((boundarymin.X + boundarymax.X) / 2, (boundarymin.Y + boundarymax.Y) / 2, boundarymin.Z);
         // XYZ center = (door.Location as LocationPoint)?.Point;

         var Points = new List<XYZ>();

         if ((int)Math.Abs(door.FacingOrientation.X) == 0)
         {
            //Get Two Point Around Door With Distance 1m

            var p1 = center + new XYZ(0.001, -3.28, 0.001);
            var p2 = center + new XYZ(0.001, 3.28, 0.001);


            //var p1 = boundarymin + new XYZ(0.0000, -3.28*(door.FacingOrientation.Y), 0.0000);
            //var p2 =new XYZ(boundarymax.X , boundarymax.Y, boundarymin.Z) ;
            Points.Add(p1);
            Points.Add(p2);

            // Points.Add(center + new XYZ(0, 3.28, 0));
            // Points.Add(center + new XYZ(0, -3.28, 0));

         }
         else if ((int)Math.Abs(door.FacingOrientation.Y) == 0)
         {


            var p1 = center + new XYZ(-3.28, 0.001, 0.001);
            var p2 = center + new XYZ(3.28, 0.001, 0.001);

            //var p1 = boundarymin + new XYZ(-3.28*(door.FacingOrientation.Y), 0.0000, 0.0000);
            //var p2 = new XYZ(boundarymax.X, boundarymax.Y, boundarymin.Z) ;
            Points.Add(p1);
            Points.Add(p2);


            //Get Two Point Around Door With Distance 1m
            //Points.Add(center + new XYZ(3.28, 0, 0));
            //Points.Add(center + new XYZ(-3.28, 0, 0));
         }

         // Project in the positive Z direction up to the next floor.
         XYZ rayDirection = new XYZ(0, 0, 1);


         var filterfloors = new ElementMulticlassFilter(new List<Type>() { typeof(Floor), typeof(RoofBase), typeof(CeilingAndFloor), typeof(BeamSystem) });

         // Create List For 2 Lines
         var lines = new List<Line>();
         Room room = null;



         foreach (var point in Points)
         {
            ReferenceIntersector refIntersectorfloors = new ReferenceIntersector(filterfloors, FindReferenceTarget.Face, view3D);

            Line line = null;
            if (refIntersectorfloors != null)
            {
               ReferenceWithContext referenceWithContext = refIntersectorfloors.FindNearest(point, rayDirection);

               if (referenceWithContext != null)
               {
                  Reference reference = referenceWithContext.GetReference();
                  XYZ intersection = reference.GlobalPoint;

                  // Create line segment from the start point and intersection point.
                  line = Line.CreateBound(point, intersection);
                  lines.Add(line);

                  foreach (var phase in doc.Phases)
                  {
                     // Adjust the coordinates as needed
                     XYZ testPoint = point + new XYZ(0.00, 0.000, 2.000);

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
                        if (item.RoomID == room.Id.ToString() )
                        {
                           item.CLearHeight = line.Length.ToExternalUnit();
                           item.IsHeightPassed = false;
                        }
                     }

                    // RoomDes roomDescription = new RoomDes(
                    // room.Id.ToString(),
                    // room.Name,
                    // room.Number,
                    // room.Area.ToExternalUnitSquare(),
                    // room.Level.Name.ToString(),
                    //// Replace this with the correct parameter for clear height
                    //line.Length.ToExternalUnit()
                    //        );

                     // Now you can use the 'roomDescription' object as needed
                   


                  }
               }



            }


         }

         return (lines);
      }



   }
}

