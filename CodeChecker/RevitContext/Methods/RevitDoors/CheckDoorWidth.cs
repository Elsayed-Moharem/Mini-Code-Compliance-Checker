using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeChecker.Helpers;
using System.Windows.Forms;
using static Microsoft.IO.RecyclableMemoryStreamManager;
using System.Windows.Controls;

using Autodesk.Revit.Attributes;
using CodeChecker.RevitContext.Model;

namespace CodeChecker.RevitContext.Methods.RevitDoors
{
    public class CheckDoorWidth
    {

        

        public static Dictionary<ElementId, List<ElementId>> GetDoorForEachRoom()
        {


            Document doc = ConstantMembers.Document;

            // Select All door in Model
            ElementCategoryFilter doorFilter = new ElementCategoryFilter(BuiltInCategory.OST_Doors);

            // Select All Room in Model
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(SpatialElement));


            // Use a filtered element collector to get all doors in the model
            List<FamilyInstance> doorElements =
                new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Doors).WhereElementIsNotElementType().Cast<FamilyInstance>().ToList();

            // Use a filtered element collector to get all Rooms in the model
            IList<Element> rooms = collector.OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().Where(e => e.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble() != 0).ToList();

            using (Transaction trans = new Transaction(doc, "Code Door Width check"))

            {


                try
                {


                    trans.Start();

                    Dictionary<ElementId, List<ElementId>> roomDoor = new Dictionary<ElementId, List<ElementId>>();
                    // Loop through each room
                    foreach (var door in doorElements)
                    {
                        // Get Window Point Location
                        LocationPoint doorLocation = door.Location as LocationPoint;
                        XYZ doorPoint = doorLocation.Point;
                        double X0 = doorPoint.X;
                        double Y0 = doorPoint.Y;

                        List<ElementId> roomsForDoor = new List<ElementId>();

                        roomDoor.Add(door.Id, roomsForDoor);



                        roomsForDoor = new List<ElementId>();
                        foreach (Element roomElement in rooms)
                        {

                            Room room = roomElement as Room;

                            if (room != null)
                            {
                                if (room.get_Parameter(BuiltInParameter.ROOM_LEVEL_ID).AsElementId() ==
                                            door.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId())
                                {

                                    // Get the boundary segments of the room
                                    IList<IList<BoundarySegment>> boundarySegments =
                                        room.GetBoundarySegments(new SpatialElementBoundaryOptions());

                                    // Loop through each boundary segment
                                    IList<BoundarySegment> segments = boundarySegments[0];

                                    foreach (BoundarySegment segment in segments)
                                    {


                                        // Get the curve of the segment
                                        Curve curve = segment.GetCurve();
                                        XYZ startPoint = curve.GetEndPoint(0);
                                        double X1 = startPoint.X;
                                        double Y1 = startPoint.Y;
                                        XYZ endPoint = curve.GetEndPoint(1);
                                        double X2 = endPoint.X;
                                        double Y2 = endPoint.Y;


                                        double WallThick1 = .250;

                                        double WallThick2 = UnitUtils.ConvertToInternalUnits(WallThick1, UnitTypeId.Meters);

                                        double miniY = Math.Abs(Y1);
                                        double maxY = Math.Abs(Y2);

                                        if (Math.Abs(Y1) > Math.Abs(Y2))
                                        {
                                            miniY = Math.Abs(Y2);
                                            maxY = Math.Abs(Y1);

                                        }

                                        double miniX = Math.Abs(X1);
                                        double maxX = Math.Abs(X2);

                                        if (Math.Abs(X1) > Math.Abs(X2))
                                        {
                                            miniX = Math.Abs(X2);
                                            maxX = Math.Abs(X1);

                                        }



                                        //HZ Door 
                                        if ((int)Y0 <= (int)(Y1 + WallThick2) &&
                                            (int)Y0 >= (int)(Y1 - WallThick2) &&
                                            Math.Abs((int)X0) <= ((int)maxX + WallThick2) &&
                                            Math.Abs((int)X0) >= ((int)miniX - WallThick2))
                                        {

                                            roomsForDoor.Add(room.Id);


                                        }

                                        //vl Door 
                                        if ((int)X0 <= (int)(X1 + WallThick2) &&
                                            (int)X0 >= (int)(X1 - WallThick2) &&
                                            Math.Abs((int)Y0) <= ((int)maxY + WallThick2) &&
                                            Math.Abs((int)Y0) >= ((int)miniY - WallThick2))
                                        {

                                            roomsForDoor.Add(room.Id);


                                        }


                                    }
                                }
                            }


                        }


                    }


                    trans.Commit();
                    return roomDoor;
                }
                catch (Exception ex)
                {
                    return new Dictionary<ElementId, List<ElementId>>();
                }

            }




        }
       
        public static List<ElementId> GetServiceRoom(Dictionary<ElementId, List<ElementId>> AllDoors, double ServiceWidth)
        {
            Document doc = ConstantMembers.Document;
            List<ElementId> ServiceDoors = new List<ElementId>();

            foreach (var kvp in AllDoors)
            {
                ElementId doorId = kvp.Key;
                List<ElementId> AllRoomsForDoor = kvp.Value;

                foreach (ElementId roomId in AllRoomsForDoor)
                {
                    // Get the room from the Revit model
                    Room room = doc.GetElement(roomId) as Room;
                    if (room != null)
                    {
                        if (room.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)?.AsString() == "Service")
                        {
                            // Get the door from the Revit model
                            FamilyInstance door = doc.GetElement(doorId) as FamilyInstance;
                            if (door != null)
                            {
                                // Get the geometry of the door
                                Autodesk.Revit.DB.Options options = new Autodesk.Revit.DB.Options();
                                GeometryElement doorGeometry = door.get_Geometry(options);

                                double doorWidth = 0.0;

                                // Iterate through the geometry to find the width of the door
                                foreach (GeometryObject geomObj in doorGeometry)
                                {
                                    if (geomObj is Solid solid)
                                    {
                                        // Assuming the width is the length in the X direction of the bounding box
                                        BoundingBoxXYZ boundingBox = solid.GetBoundingBox();
                                        double width = boundingBox.Max.X - boundingBox.Min.X;

                                        // Accumulate the width
                                        doorWidth += width;
                                    }
                                }

                                // Compare the door width with the LivingWidth threshold
                                if (doorWidth < UnitUtils.ConvertToInternalUnits(ServiceWidth, UnitTypeId.Meters))
                                {
                                    // If the door width is less than the threshold, add the door to the list of service doors
                                    ServiceDoors.Add(doorId);
                                }
                            }
                        }
                            
                    }
                }
            }

            return ServiceDoors;
        }
       
        public static List<ElementId> GetLivingDoors(Dictionary<ElementId, List<ElementId>> AllDoors, List<ElementId> ServiceRoom, double LivingWidth)
        {
            List<ElementId> LivingDoors = new List<ElementId>();

            Document doc = ConstantMembers.Document;

            foreach (var kvp in AllDoors)
            {
                ElementId doorId = kvp.Key;
                List<ElementId> AllRoomsForDoor = kvp.Value;

                // Check if the door is not in the list of service rooms
                if (!ServiceRoom.Contains(doorId))
                {
                    // Get the door from the Revit model
                    FamilyInstance door = doc.GetElement(doorId) as FamilyInstance;
                    if (door != null)
                    {
                        // Get the geometry of the door
                        Autodesk.Revit.DB.Options options = new Autodesk.Revit.DB.Options();
                        GeometryElement doorGeometry = door.get_Geometry(options);

                        double doorWidth = 0.0;

                        // Iterate through the geometry to find the width of the door
                        foreach (GeometryObject geomObj in doorGeometry)
                        {
                            if (geomObj is Solid solid)
                            {
                                // Assuming the width is the length in the X direction of the bounding box
                                BoundingBoxXYZ boundingBox = solid.GetBoundingBox();
                                double width = boundingBox.Max.X - boundingBox.Min.X;

                                // Accumulate the width
                                doorWidth += width;
                            }
                        }

                        // Compare the door width with the LivingWidth threshold
                        if (doorWidth < UnitUtils.ConvertToInternalUnits(LivingWidth, UnitTypeId.Meters))
                        {
                            // If the door width is less than the threshold, add the door to the list of living doors
                            LivingDoors.Add(doorId);
                        }
                    }
                }
            }

            return LivingDoors;
        }

    }
}

