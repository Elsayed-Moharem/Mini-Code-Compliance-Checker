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

namespace CodeChecker.RevitContext.Methods.RevitWindows
{
    
    public class CheckWindowRoomArea1
    {
        public static Dictionary<double, double> GetAllWindowInRoom()
        {

         
            Document doc = ConstantMembers.Document;
            // Select All Window in Model
            ElementCategoryFilter windowFilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);

            // Select All Room in Model
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(SpatialElement));


            // Use a filtered element collector to get all windows in the model
            List<FamilyInstance> windowElements =
                new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Windows).WhereElementIsNotElementType().Cast<FamilyInstance>().ToList();

            // Use a filtered element collector to get all Rooms in the model
            IList<Element> rooms = collector.OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().Where(e => e.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble() != 0).ToList();


            using (Transaction trans = new Transaction(doc, "Code Room Total Window Area check")) 
            {


                try
                {


                    trans.Start();

                    var failedWindowId = new List<ElementId>();

                    double totalAreanOfWindowinOneRoom;
                    StringBuilder messageBuilder = new StringBuilder();


                    Dictionary<double, double> roomWindowArea = new Dictionary<double, double>();
                    // Loop through each room
                    foreach (Element roomElement in rooms)
                    {
                        totalAreanOfWindowinOneRoom = 0;

                        Room room = roomElement as Room;

                        if (room != null)
                        {

                            roomWindowArea.Add(double.Parse(room.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString()), totalAreanOfWindowinOneRoom);
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
                                        totalAreanOfWindowinOneRoom = +window
                                            .get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble();

                                    }
                                }
                            }
                            roomWindowArea[double.Parse(room.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString())] = totalAreanOfWindowinOneRoom;
                        }

                    }
                    trans.Commit();
                    return roomWindowArea;
                }
                catch (Exception ex)
                {
                    trans.RollBack();
                    return new Dictionary<double, double>();
                }
            }
            
        }

    }
}
