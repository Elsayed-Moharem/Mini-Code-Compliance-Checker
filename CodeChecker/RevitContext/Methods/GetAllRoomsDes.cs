
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using CodeChecker.Helpers;
using CodeChecker.RevitContext.Model;
using CodeChecker.RevitContext.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace CodeChecker.RevitContext.Methods
{
   public static class GetAllRoomsDes
   {
      public static List<RoomDes> ALLRoomDes = new List<RoomDes>();  

      public  static void GetAllRoomsDesM( )
      {
         var doc = ConstantMembers.Document;
         var uidoc = ConstantMembers.UiDocument;

         List<Room> rooms = new FilteredElementCollector(doc)
            .OfClass(typeof(SpatialElement)).Cast<Room>()
            .Where(room => room.Location != null & room.Area != 0).ToList();

         rooms.ForEach(room => ALLRoomDes.Add(new RoomDes(
                     room.Id.ToString(),
                     room.Name,
                     room.Number,
                     room.Area.ToExternalUnitSquare().RoundToOneDecimalPlace(),
                     room.Level.Name.ToString(),
                     room.UnboundedHeight.ToExternalUnit().RoundToOneDecimalPlace())))
                                     ;

        
      }

   }
}
