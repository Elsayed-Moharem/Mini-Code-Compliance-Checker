using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodeChecker.RevitContext.Methods.STR
{
    public class GetDimensions
    {
        public static double GetWidth(Solid solid) => solid.GetBoundingBox().Max.X - solid.GetBoundingBox().Min.X;

        public static double GetLength(Solid solid) => solid.GetBoundingBox().Max.Y - solid.GetBoundingBox().Min.Y;

        public static double GetThickness(Solid solid) => solid.GetBoundingBox().Max.Z - solid.GetBoundingBox().Min.Z;
    }
}