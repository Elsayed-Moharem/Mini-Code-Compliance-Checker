using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeChecker.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeChecker.RevitContext.Methods.RevitWindows
{
    public static class CheckWindowNetArea
    {
        public static List<ElementId> GetWindowWinthMiniNetArea(Double MiniWindowArea)
        {
            

            Document doc = ConstantMembers.Document;

            // Select All Window in Model
            ElementCategoryFilter windowFilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);



            // Use a filtered element collector to get all windows in the model
            ICollection<Element> windowElements =
                new FilteredElementCollector(doc).WherePasses(windowFilter).WhereElementIsNotElementType().ToElements();

            using (Transaction trans = new Transaction(doc, "Code Net Area check"))
            {

                try
                {

                    trans.Start();

                    List<ElementId> failedWindowId = new List<ElementId>();

                    double codeLimitNetArea_InMeter = MiniWindowArea;

                    double codeNetArea = UnitUtils.ConvertToInternalUnits(codeLimitNetArea_InMeter, UnitTypeId.SquareMeters);

                    // Save Window IDs in Failed Window List
                    foreach (var window in windowElements)
                    {
                        // Get Net Area value
                        double NetArea = window.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED)
                            .AsDouble();


                        // Check against the threshold
                        if (NetArea < codeNetArea)
                        {
                            failedWindowId.Add(window.Id);
                        }
                    }

                    trans.Commit();

                    return failedWindowId;



                }
                catch (Exception ex)
                {

                    trans.RollBack();
                    return new List<ElementId>();
                }
            }
            
        }


    }
}
