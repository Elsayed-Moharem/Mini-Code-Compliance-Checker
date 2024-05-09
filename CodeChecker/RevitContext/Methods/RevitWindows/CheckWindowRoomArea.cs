using Autodesk.Revit.Attributes;
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
using CodeChecker.RevitContext.Model;

namespace CodeChecker.RevitContext.Methods.RevitWindows
{
    
    public class CheckWindowRoomArea
    {

        public static Dictionary<ElementId, double> GetAllWindowInRoom()
        {


            Document doc = ConstantMembers.Document;
            

            // Use a filtered element collector to get all windows in the model
            List<FamilyInstance> windowElements =
                new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Windows).WhereElementIsNotElementType().Cast<FamilyInstance>().ToList();

            // Use a filtered element collector to get all Rooms in the model
            IList<Element> rooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().Where(e => e.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble() != 0).ToList();


            using (Transaction trans = new Transaction(doc, "Code Room Total Window Area check"))
            {


                try
                {


                    trans.Start();
                    var failedWindowId = new List<ElementId>();

                    double totalAreanOfWindowinOneRoom;
                    


                    Dictionary<ElementId, double> roomWindowArea = new Dictionary<ElementId, double>();



                    // Loop through each room
                    foreach (Element roomElement in rooms)
                    {
                        totalAreanOfWindowinOneRoom = 0;

                        Room room = roomElement as Room;

                        if (room != null)
                        {

                            roomWindowArea.Add(room.Id, totalAreanOfWindowinOneRoom);
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

                                foreach (var window in windowElements)
                                {
                                    // Get Window Point Location
                                    LocationPoint windowLocation = window.Location as LocationPoint;
                                    XYZ windowPoint = windowLocation.Point;
                                    double X0 = windowPoint.X;
                                    double Y0 = windowPoint.Y;

                                    // Calculate the slope and y-intercept for the line passing through (x1, y1) and (x2, y2)
                                    double m = (Y2 - Y1) / (X2 - X1);
                                    double b = Y1 - m * X1;

                                    // Calculate the expected y-coordinate on the line
                                    double expectedY = m * X0 + b;

                                    // Check if the actual y-coordinate is equal to the expected y-coordinate
                                    if (room.get_Parameter(BuiltInParameter.ROOM_LEVEL_ID).AsElementId() ==
                                        window.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId() &&
                                        Math.Abs((int)Y0 - (int)expectedY) < 1e-9)
                                    {
                                        totalAreanOfWindowinOneRoom += window
                                            .get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble();

                                    }
                                }
                            }
                            roomWindowArea[room.Id] = totalAreanOfWindowinOneRoom;
                        }

                    }
                    trans.Commit();
                    return roomWindowArea;
                }
                catch (Exception ex)
                {
                    trans.RollBack();
                    return new Dictionary<ElementId, double>();
                }
            }

        }

        public static List<ElementId> CompareRoomAreasWithRevit(Dictionary<ElementId, double> roomAreas , double pre)
        {
            Document doc = ConstantMembers.Document;
            List<ElementId> roomsWithLargerAreas = new List<ElementId>();

            foreach (var kvp in roomAreas)
            {
                ElementId roomId = kvp.Key;
                double dictionaryArea = kvp.Value;

                // Get the room from the Revit model
                Room room = doc.GetElement(roomId) as Room;

                if (room != null)
                {
                    // Get the area of the room from the Revit model
                    Parameter roomAreaParameter = room.get_Parameter(BuiltInParameter.ROOM_AREA);
                    if (roomAreaParameter != null)
                    {
                        double revitArea = roomAreaParameter.AsDouble();

                        // Compare the areas
                        if (dictionaryArea < revitArea * pre)
                        {
                            roomsWithLargerAreas.Add(roomId);
                        }
                    }
                }
            }

            return roomsWithLargerAreas;
        }


    }
}
