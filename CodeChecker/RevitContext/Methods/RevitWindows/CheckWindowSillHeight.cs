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
    
    public static class CheckWindowSillHeight
    {
        public static List<ElementId> GetFailedWindowInSillHeight (double miniSillHight)
        {

            

            Document doc = ConstantMembers.Document;

            // Select All Window in Model
            ElementCategoryFilter windowFilter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);



            // Use a filtered element collector to get all windows in the model
            ICollection<Element> windowElements =
                new FilteredElementCollector(doc).WherePasses(windowFilter).WhereElementIsNotElementType().ToElements();

            using (Transaction trans = new Transaction(doc, "Code Sill Height check"))
            {


                try
                {


                    trans.Start();

                    var failedWindowId = new List<ElementId>();

                    double codeLimitSillHeight_InMiliMeter = miniSillHight;


                    double codeSillHeight = UnitUtils.ConvertToInternalUnits(codeLimitSillHeight_InMiliMeter, UnitTypeId.Millimeters);

                    // Save Window IDs in Failed Window List
                    foreach (var window in windowElements)
                    {
                        // Get sill height value
                        double sillHeight = window.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM)
                            .AsDouble();

                        // Check against the threshold
                        if (sillHeight < codeSillHeight)
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
