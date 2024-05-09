using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeChecker.RevitContext.Model
{
    public class revitDoors : INotifyPropertyChanged
    {
        public revitDoors(string Id, string doorname, string doorlevelname, double doorWidth)
        {
            DoorID = Id;
            DoorName = doorname;
            DoorLevelName = doorlevelname;
            DoorWidtht = doorWidth;

        }
        public string DoorLevelName { get; set; }
        public Double DoorWidtht { get; set; }
        public string DoorID { get; set; }
        public string DoorName { get; set; }
        

        public event PropertyChangedEventHandler PropertyChanged;




        public override string ToString()
        {
            return "Name : " + DoorName ;
        }


        public void OnPropertityChanged([CallerMemberName] string propertyname = null)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }




    }
}
