using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeChecker.Helpers;
using CodeChecker.RevitContext.Methods;
using CodeChecker.RevitContext.Model;
using CodeChecker.RevitContext.Extensions;

namespace CodeCheckerAddin.RevitContext.Methods
{
   public  class CheckRoomsDim
   {


      public static string checkRoomDim( double minResidentalDim , double minKitchenDim, double minBathroomalDim)
      {

         foreach (var roomdes in GetAllRoomsDes.ALLRoomDes)
         {

            var room = new FilteredElementCollector(ConstantMembers.Document)
            .OfClass(typeof(SpatialElement)).Cast<Room>()
               .Where(e => e.Id.ToString() == roomdes.RoomID)
               .FirstOrDefault() as Room;


            if (roomdes.ISResidentialRoom==true)
            {

               if (GetMinBoundarySegmentLength(room)<= minResidentalDim.FromExternalUnit()) roomdes.IsDimPassed = false;
            }
            else if (roomdes.IsKitchen==true)
            {

               if (GetMinBoundarySegmentLength(room) <= minKitchenDim.FromExternalUnit()) roomdes.IsDimPassed = false;
            }
            else
            {
               if (GetMinBoundarySegmentLength(room) <= minBathroomalDim.FromExternalUnit()) roomdes.IsDimPassed = false;

            }

         }
         return " Check Rooms  Dim is Succeeded";
      }


     
      public static double GetMinBoundarySegmentLength( Element roomElement)
      {

         if (roomElement is Room room)
         {
            // Get the room boundary segments
            IList<IList<BoundarySegment>> boundarySegments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());

            if (boundarySegments.Count > 0)
            {
               // Find the minimum segment length among all boundary segments
               double minSegmentLength = double.MaxValue;

               foreach (IList<BoundarySegment> segmentList in boundarySegments)
               {
                  foreach (BoundarySegment segment in segmentList)
                  {
                     if (segment.GetCurve().Length < minSegmentLength)
                     {
                        minSegmentLength = segment.GetCurve().Length;
                     }
                  }
               }

               // Return the minimum boundary segment length
               return minSegmentLength;
            }
            else
            {
               // Handle the case where there are no boundary segments
               // You may want to throw an exception, log a message, or handle it in another way
               // based on your specific requirements.
               // For simplicity, this example returns 0.0 in case of any issues.
               return 0.0;
            }
         }
         else
         {
            // Handle the case where the element with the provided Id is not a room
            // You may want to throw an exception, log a message, or handle it in another way
            // based on your specific requirements.
            // For simplicity, this example returns 0.0 in case of any issues.
            return 0.0;
         }
      }




   }
}
