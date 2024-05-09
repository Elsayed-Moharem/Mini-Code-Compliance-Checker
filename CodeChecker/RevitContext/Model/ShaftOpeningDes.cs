using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeChecker.RevitContext.Model
{
   public  class ShaftOpeningDes
   {

      public ShaftOpeningDes(string Id, double shaftOpeningarea, double shaftOpeningMinDim
         , string baseLevelName, string topLevelName, bool iSResidentialshaft = false
         , bool isAreaPassed = true, bool isMinDimPassed = true)
      {
         ShaftOpeningID = Id;
         ShaftOpeningArea = shaftOpeningarea;
         BaseLevelName = baseLevelName;
         TopLevelName = topLevelName;
         ShaftOpeningMinDim = shaftOpeningMinDim;


         IsMinDimPassed = isMinDimPassed;
         ISResidentialshaft = iSResidentialshaft;
         IsAreaPassed = isAreaPassed;
      }

      public string BaseLevelName { get; set; }
      public string TopLevelName { get; set; }
      public string ShaftOpeningID { get; set; }
      public double ShaftOpeningArea { get; set; }
      public double ShaftOpeningMinDim { get; set; }

      public event PropertyChangedEventHandler PropertyChanged;

      private bool iSResidentialshaft;

      public bool ISResidentialshaft
      {
         get { return iSResidentialshaft; }
         set
         {
            if (iSResidentialshaft != value)
            {
               iSResidentialshaft = value;
               OnPropertityChanged();
            }
         }
      }

      private bool isAreaPassed;
      public bool IsAreaPassed
      {
         get { return isAreaPassed; }
         set
         {
            if (isAreaPassed != value)
            {
               isAreaPassed = value;
               OnPropertityChanged();
            }
         }
      }

      private bool isMinDimPassed;
      public bool IsMinDimPassed
      {
         get { return isMinDimPassed; }
         set
         {
            if (isMinDimPassed != value)
            {
               isMinDimPassed = value;
               OnPropertityChanged();
            }
         }
      }




      public void OnPropertityChanged([CallerMemberName] string propertyname = null)
      {

         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
      }


   }
}
