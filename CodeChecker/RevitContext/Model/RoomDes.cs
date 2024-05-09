using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeChecker.RevitContext.Model
{
   public class RoomDes : INotifyPropertyChanged
   {
      public RoomDes(string Id, string roomname, string roomnumber, double roomarea, string levelname, double clearheight, bool iSResidentialRoom = false
         , bool  isHeightPassed = true , bool isKitchen =false , bool isDimPassed = true , bool isAreaPassed = true)
      {
         RoomID = Id;
         RoomName = roomname;
         RoomNumber = roomnumber;
         RoomArea = roomarea;
         LevelName = levelname;
         CLearHeight = clearheight;
         ISResidentialRoom = iSResidentialRoom;
         IsHeightPassed = isHeightPassed;
         IsDimPassed = isDimPassed;
         IsAreaPassed = isAreaPassed;
         IsKitchen = isKitchen;
      }
      public string LevelName { get; set; }
      public Double CLearHeight { get; set; }
      public string RoomID { get; set; }
      public string RoomNumber { get; set; }
      public string RoomName { get; set; }
      public double RoomArea { get; set; }

      public event PropertyChangedEventHandler PropertyChanged;

      private bool iSResidentialRoom;
      public bool ISResidentialRoom
      {
         get { return iSResidentialRoom; }
         set
         {
            if (iSResidentialRoom != value)
            {
               iSResidentialRoom = value;
               OnPropertityChanged();
            }
         }
      }

      private bool isDimPassed;
      public bool IsDimPassed
      {
         get { return isDimPassed; }
         set
         {
            if (isDimPassed != value)
            {
               isDimPassed = value;
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


      private bool isKitchen;
      public bool IsKitchen
      {
         get { return isKitchen; }
         set
         {
            if (isKitchen != value)
            {
               isKitchen = value;
               OnPropertityChanged();
            }
         }
      }

      private bool isHeightPassed;
      public bool IsHeightPassed
      {
         get { return isHeightPassed; }
         set
         {
            if (isHeightPassed != value)
            {
               isHeightPassed = value;
               OnPropertityChanged();
            }
         }
      }

      public override string ToString()
      {
         return "Name : " + RoomNumber + " -No : " + RoomName;
      }


      public void OnPropertityChanged([CallerMemberName] string propertyname = null)
      {

         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
      }

   }

}
