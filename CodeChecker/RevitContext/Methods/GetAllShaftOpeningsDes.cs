using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using CodeChecker.Helpers;
using CodeChecker.RevitContext.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeChecker.RevitContext.Extensions;

namespace CodeChecker.RevitContext.Methods
{
   public static class GetAllShaftOpeningsDes
   {
      public static List<ShaftOpeningDes> AllShaftOpeningDes ;

      public static double MaxAllowableDis { set; get; } = 1000;

      public static void AllShaftOpeningDesM()
      {
         AllShaftOpeningDes = new List<ShaftOpeningDes>();

         var doc = ConstantMembers.Document;
         var uidoc = ConstantMembers.UiDocument;

         var shaftOpenings = new FilteredElementCollector(doc)
            .OfCategory(BuiltInCategory.OST_ShaftOpening)
            .Where(shaft => shaft.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsValueString() != null)
            .Cast<Opening>().ToList();



         shaftOpenings.ForEach(shaftOpening => AllShaftOpeningDes.Add(new ShaftOpeningDes(
                     shaftOpening.Id.ToString(),
                     PolygonArea(shaftOpening).RoundToOneDecimalPlace(),
                     MinSegmentLength(shaftOpening).RoundToOneDecimalPlace(),
                     (shaftOpening.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT)).AsValueString(),
                     (doc.GetElement((shaftOpening.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE)).AsElementId()).Name)
                                )));
      }

      // Calculate the area of a polygon given by a list of points
      public static double PolygonArea(Opening opening)
      {

         //List<XYZ> points = Polygonpoints(opening);

         //int n = points.Count;
         //double area = 0.0;

         //// Calculate value of shoelace formula
         //int j = n - 1;
         //for (int i = 0; i < n; i++)
         //{
         //   area += (points[j].X + points[i].X) * (points[j].Y - points[i].Y);
         //   j = i; // j is previous vertex to i
         //}

         //// Return absolute value
         //return Math.Abs(area / 2.0);

         //--------------------------------
         // Create a new options object
         Options options = new Options();

         // Set the compute references to true
         options.IncludeNonVisibleObjects = true;

         var face = (opening.get_Geometry(options).First() as Solid)
                             .Faces.Cast<PlanarFace>()
                             .Where(f => (int)(f.FaceNormal.Z)!=0).FirstOrDefault();

         return face.Area.ToExternalUnitSquare();

      }



      // Calculate the  a list of points of Polygon
      public static List<XYZ> Polygonpoints(Opening opening)
      {
         // Get the boundary curves of the opening
         CurveArray boundaryCurves = opening.BoundaryCurves;

         // Create an empty list of points
         List<XYZ> points = new List<XYZ>();

         // Loop through the curves and get their end points
         foreach (Curve curve in boundaryCurves)
         {
            // Get the start point of the curve
            XYZ startPoint = curve.GetEndPoint(0);

            // Add the point to the list if it is not already there
            if (!points.Contains(startPoint))
            {
               points.Add(startPoint);
            }

            // Get the end point of the curve
            XYZ endPoint = curve.GetEndPoint(1);

            // Add the point to the list if it is not already there
            if (!points.Contains(endPoint))
            {
               points.Add(endPoint);
            }
         }

         return points;

      }



      // Get a list of sums of segments with the same direction and small distance between them
      public static double MinSegmentLength(Opening opening)
      {

         // Get the boundary curves of the opening
         CurveArray boundaryCurves = opening.BoundaryCurves;

         // Create a list of points
         List<XYZ> points = Polygonpoints(opening);


         // Create an empty list of sums
         List<double> sums = new List<double>();


         // Loop through the curves and get their end points
         foreach (Curve curve in boundaryCurves)
         {
            // Calculate the slope of the segment
            XYZ p1 = curve.GetEndPoint(0);
            XYZ p2 = curve.GetEndPoint(1); ;
            double comparedslope = (p2.Y - p1.Y) / (p2.X - p1.X);


            // Initialize the sum to zero
            double sum = 0;


            foreach (Curve checkedcurve in boundaryCurves)
            {
               // Calculate the slope of the segment
               XYZ p3 = curve.GetEndPoint(0);
               XYZ p4 = curve.GetEndPoint(1); ;
               double checkeddslope = (p4.Y - p3.Y) / (p4.X - p3.X);

               // Check if the slope is the same as the previous slope
               if (checkeddslope == comparedslope)
               {

                  var distancebetween2curve = ShortestDistanceBetweenCurves(curve, checkedcurve).ToExternalUnit();

                  if (distancebetween2curve != 0 & distancebetween2curve <= MaxAllowableDis)
                  {

                     sum += checkedcurve.Length;
                  }

               }

            }
            // Add the last sum to the list
            sums.Add(sum);
         }

         // Return the list of sums
         return sums.Min();
      }

      //// Get the minimum segment length betwen two Curves
      public static double ShortestDistanceBetweenCurves(Curve curve1, Curve curve2)
      {
         // Use the Project method to find the closest points on the curves
         IntersectionResult closestPointsResult = curve1.Project(curve2.GetEndPoint(0));
         XYZ closestPointCurve1 = closestPointsResult.XYZPoint;

         closestPointsResult = curve2.Project(curve1.GetEndPoint(0));
         XYZ closestPointCurve2 = closestPointsResult.XYZPoint;

         // Calculate the distance between the closest points
         double distance = closestPointCurve1.DistanceTo(closestPointCurve2);

         return distance.ToExternalUnit();
      }

   }
}
