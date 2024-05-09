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
   public  class CheckRoomsArea
   {

      public static string checkRoomArea( double minResidentalArea, double minKitchenArea, double minBathroomalArea)
      {

         foreach (var roomdes in GetAllRoomsDes.ALLRoomDes)
         {

            var room = new FilteredElementCollector(ConstantMembers.Document)
            .OfClass(typeof(SpatialElement)).Cast<Room>()
               .Where(e => e.Id.ToString() == roomdes.RoomID)
               .FirstOrDefault() as Room;


            if (roomdes.ISResidentialRoom == true)
            {

               if (GetRoomArea(room) <= minResidentalArea.FromExternalUnitSquare()) roomdes.IsAreaPassed = false;
            }
            else if (roomdes.IsKitchen == true)
            {

               if (GetRoomArea(room) <= minKitchenArea.FromExternalUnitSquare()) roomdes.IsAreaPassed = false;
            }
            else
            {
               if (GetRoomArea(room) <= minBathroomalArea.FromExternalUnitSquare()) roomdes.IsAreaPassed = false;

            }

         }
         return " Check Rooms Area  is Succeeded";
      }





      /// <summary>
      /// return Room Area
      /// </summary>
      /// <param name="doc"></param>
      /// <param name="roomId"></param>
      /// <returns></returns>
      public static double GetRoomArea( Element roomElement)
      {


         if (roomElement is Room room)
         {
            // Get the room area parameter
            Parameter areaParameter = room.get_Parameter(BuiltInParameter.ROOM_AREA);

            if (areaParameter != null && areaParameter.StorageType == StorageType.Double)
            {
               // Retrieve and return the room area as a double
               double roomArea = areaParameter.AsDouble();
               return roomArea;
            }
            else
            {
               // Handle the case where the room area parameter is not found or not a double
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
