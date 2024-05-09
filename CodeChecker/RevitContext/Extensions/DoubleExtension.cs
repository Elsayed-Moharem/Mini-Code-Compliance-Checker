using Autodesk.Revit.DB;
using System;
using System.Linq;

namespace CodeChecker.RevitContext.Extensions
{
   public static class DoubleExtension
   {
      public static double FromExternalUnit(this double value)
      { return UnitUtils.ConvertToInternalUnits(value, UnitTypeId.Millimeters); }

      public static double FromExternalUnitCubic(this double value)
      { return UnitUtils.ConvertToInternalUnits(value, UnitTypeId.CubicMillimeters); }

      public static double FromExternalUnitSquare(this double value)
      { return UnitUtils.ConvertToInternalUnits(value, UnitTypeId.SquareMillimeters); }

      public static double ToExternalUnit(this double value)
      { return UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.Millimeters); }

      public static double ToExternalUnitCubic(this double value)
      { return UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.CubicMillimeters); }

      public static double ToExternalUnitSquare(this double value)
      { return UnitUtils.ConvertFromInternalUnits(value, UnitTypeId.SquareMillimeters); }



      public static double RoundToOneDecimalPlace(this double number)
      {
         return Math.Round(number, 1);
      }

   }
}
