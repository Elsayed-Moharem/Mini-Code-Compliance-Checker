using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeChecker.Helpers;
using CodeChecker.RevitContext.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CodeChecker.RevitContext.Methods.STR
{
    /// <summary>
    /// Class for checking structural columns in Revit models.
    /// </summary>
    public class CheckStrColumns
    {
        /// <summary>
        /// List to store mismatched column descriptions.
        /// </summary>
        public static List<StrColumnsDes> MismatchedColumns { get; set; }

        /// <summary>
        /// Method to check structural columns against specified dimensions.
        /// </summary>
        /// <param name="columnWidthInput">Input width of column to check against.</param>
        /// <param name="columnLengthInput">Input length of column to check against.</param>
        /// <returns>A string indicating success or failure of the operation.</returns>
        public static string StrColumns(double columnWidthInput, double columnLengthInput)
        {
            var doc = ConstantMembers.Document;

            // Initialize list to store mismatched columns
            MismatchedColumns = new List<StrColumnsDes>();

            try
            {
                // Start a new transaction
                using (Transaction trans = new Transaction(doc, "Check Structural Columns"))
                {
                    trans.Start();

                    // Retrieve all structural columns in the document
                    var columns = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_StructuralColumns)
                        .WhereElementIsNotElementType()
                        .ToElements()
                        .Cast<FamilyInstance>();

                    // Iterate through each column
                    foreach (var column in columns)
                    {
                        // Check dimensions of each geometry instance of the column
                        CheckColumnDimensions(column, columnWidthInput, columnLengthInput);
                    }

                    // Commit the transaction
                    trans.Commit();

                    // Operation succeeded
                    return "Succeeded";
                }
            }
            catch (Exception ex)
            {
                // Operation failed with exception
                return "Failed" + ex.Message;
            }
        }

        /// <summary>
        /// Method to check dimensions of a column against specified inputs.
        /// </summary>
        private static void CheckColumnDimensions(FamilyInstance column, double columnWidthInput, double columnLengthInput)
        {
            var doc = ConstantMembers.Document;
            Options opt = new Options();
            GeometryElement geo = column.get_Geometry(opt);
            if (geo == null) return; // Skip if geometry is null

            foreach (GeometryObject obj in geo)
            {
                if (!(obj is GeometryInstance inst)) continue; // Skip if not a geometry instance

                GeometryElement instGeomElem = inst.SymbolGeometry;

                foreach (GeometryObject instObj in instGeomElem)
                {
                    if (!(instObj is Solid geomSolid)) continue; // Skip if not a solid

                    // Get dimensions of the column
                    double width = GetDimensions.GetWidth(geomSolid);
                    double length = GetDimensions.GetLength(geomSolid);

                    // Set column dimensions for description
                    StrColumnsDes.StrColumnsWidth = width;
                    StrColumnsDes.StrColumnsLength = length;



                    // Convert dimensions to millimeters
                    width = UnitUtils.ConvertFromInternalUnits(width, UnitTypeId.Millimeters);
                    length = UnitUtils.ConvertFromInternalUnits(length, UnitTypeId.Millimeters);

                    width = Math.Round(width);
                    length = Math.Round(length);

                    // Update input dimensions if they are not provided
                    if (columnWidthInput == 0)
                        columnWidthInput = width;
                    if (columnLengthInput == 0)
                        columnLengthInput = length;

                    // Check against input dimensions
                    if (width > columnWidthInput || length > columnLengthInput)
                    {
                        // Create a new 3D view
                        ViewFamilyType viewFamilyType = new FilteredElementCollector(doc)
                            .OfClass(typeof(ViewFamilyType))
                            .Cast<ViewFamilyType>()
                            .FirstOrDefault(x => x.ViewFamily == ViewFamily.ThreeDimensional);

                        View3D sectionView = View3D.CreateIsometric(doc, viewFamilyType.Id);

                        BoundingBoxXYZ boundingBox = column.get_BoundingBox(doc.ActiveView);
                        if (boundingBox != null)
                        {
                            XYZ min = boundingBox.Min;
                            XYZ max = boundingBox.Max;

                            // Create a crop box
                            sectionView.SetSectionBox(new BoundingBoxXYZ
                            {
                                Min = new XYZ(min.X, min.Y, min.Z),
                                Max = new XYZ(max.X, max.Y, max.Z)
                            });

                            // Name the view
                            sectionView.Name = $" Code Checker - {column.Symbol.FamilyName} - {column.Id}";

                            // Add the mismatched column to the list
                            MismatchedColumns.Add(new StrColumnsDes(
                                column.Id.ToString(),
                                length,
                                width
                            ));

                            break;
                        }
                    }
                }
            }
        }
    }
}
