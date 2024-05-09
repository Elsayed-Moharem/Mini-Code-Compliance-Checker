using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CodeChecker.RevitContext.Model;
using CodeChecker.Helpers;

namespace CodeChecker.RevitContext.Methods.STR
{
    /// <summary>
    /// Class for checking structural footings in Revit models.
    /// </summary>
    public class CheckStrFooting
    {
        /// <summary>
        /// List to store mismatched footing descriptions.
        /// </summary>
        public static List<StrFootingDes> MismatchedFooting { get; set; }

        /// <summary>
        /// Method to check structural footings against specified dimensions.
        /// </summary>
        /// <param name="footingWidthInput">Input width of footing to check against.</param>
        /// <param name="footingLengthInput">Input length of footing to check against.</param>
        /// <param name="footingThicknessInput">Input thickness of footing to check against.</param>
        /// <returns>A string indicating success or failure of the operation.</returns>
        public static string StrFooting(double footingWidthInput, double footingLengthInput, double footingThicknessInput)
        {
            var doc = ConstantMembers.Document;
            MismatchedFooting = new List<StrFootingDes>(); // Initialize list to store mismatched footings

            try
            {
                // Retrieve all structural footings in the document
                var footings = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_StructuralFoundation)
                    .WhereElementIsNotElementType()
                    .ToElements()
                    .Cast<FamilyInstance>();

                // Iterate through each footing to check dimensions
                foreach (var footing in footings)
                {
                    // Check dimensions of each geometry instance of the footing
                    CheckFootingDimensions(footing, footingWidthInput, footingLengthInput, footingThicknessInput);
                }

                // Display message indicating completion of checks
                MessageBox.Show("Code Checker: All Isolated have been checked see 3d views", "Code Checker");

                return "Succeeded"; // Operation succeeded
            }
            catch (Exception ex)
            {
                // Operation failed with exception
                return "Failed" + ex.Message;
            }
        }

        /// <summary>
        /// Method to check dimensions of a footing against specified inputs.
        /// </summary>
        private static void CheckFootingDimensions(FamilyInstance footing, double footingWidthInput, double footingLengthInput, double footingThicknessInput)
        {
            var doc = ConstantMembers.Document;
            Options opt = new Options();
            GeometryElement geo = footing.get_Geometry(opt);
            if (geo == null) return; // Skip if geometry is null

            foreach (GeometryObject obj in geo)
            {
                if (!(obj is GeometryInstance inst)) continue; // Skip if not a geometry instance

                GeometryElement instGeomElem = inst.SymbolGeometry;

                foreach (GeometryObject instObj in instGeomElem)
                {
                    if (!(instObj is Solid geomSolid)) continue; // Skip if not a solid

                    // Get dimensions of the solid
                    double width = GetDimensions.GetWidth(geomSolid);
                    double length = GetDimensions.GetLength(geomSolid);
                    double thickness = GetDimensions.GetThickness(geomSolid);

                    // Convert dimensions to millimeters
                    width = UnitUtils.ConvertFromInternalUnits(width, UnitTypeId.Millimeters);
                    length = UnitUtils.ConvertFromInternalUnits(length, UnitTypeId.Millimeters);
                    thickness = UnitUtils.ConvertFromInternalUnits(thickness, UnitTypeId.Millimeters);

                    width = Math.Round(width);
                    length = Math.Round(length);
                    thickness = Math.Round(thickness);

                    // Set footing dimensions for description
                    StrFootingDes.StrFootingWidth = width;
                    StrFootingDes.StrFootingLength = length;
                    StrFootingDes.StrFootingThickness = thickness;

                    // Update input dimensions if they are not provided
                    if (footingWidthInput == 0)
                        footingWidthInput = width;
                    if (footingLengthInput == 0)
                        footingLengthInput = length;
                    if (footingThicknessInput == 0)
                        footingThicknessInput = thickness;

                    // Check against input dimensions
                    if (width > footingWidthInput || length > footingLengthInput || thickness > footingThicknessInput)
                    {
                        // Create a 3D view showing the mismatched footing
                        CreateMismatchedFootingView(doc, footing, width, length, thickness);
                        return; // No need to continue checking if mismatch found
                    }
                }
            }
        }

        /// <summary>
        /// Method to create a 3D view showing the mismatched footing.
        /// </summary>
        private static void CreateMismatchedFootingView(Document doc, FamilyInstance footing, double width, double length, double thickness)
        {
            ViewFamilyType viewFamilyType = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault(x => x.ViewFamily == ViewFamily.ThreeDimensional);

            using (Transaction trans = new Transaction(doc, "Check Footing"))
            {
                trans.Start();
                View3D sectionView = View3D.CreateIsometric(doc, viewFamilyType.Id);
                BoundingBoxXYZ boundingBox = footing.get_BoundingBox(doc.ActiveView);

                if (boundingBox != null)
                {
                    XYZ min = boundingBox.Min;
                    XYZ max = boundingBox.Max;

                    // Set section box to focus on the footing
                    sectionView.SetSectionBox(new BoundingBoxXYZ
                    {
                        Min = new XYZ(min.X, min.Y, min.Z),
                        Max = new XYZ(max.X, max.Y, max.Z)
                    });

                    // Name the view
                    sectionView.Name = $" Code Checker - {footing.Symbol.FamilyName} - {footing.Id}";

                    // Add the mismatched footing to the list
                    MismatchedFooting.Add(new StrFootingDes(
                        footing.Id.ToString(),
                        length,
                        width,
                        thickness
                    ));

                    trans.Commit();
                }
            }
        }
    }
}
