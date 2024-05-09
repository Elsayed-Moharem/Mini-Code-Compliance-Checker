using Autodesk.Revit.DB;
using CodeChecker.RevitContext.Methods.RevitWindows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CodeChecker.RevitContext.Model
{
    public class revitWindow : INotifyPropertyChanged
    {
        public revitWindow (string Id, string windowname, double windowarea, string windowlevelname, double windowSillheight , XYZ loc)
        {
            WindowID = Id;
            WindowName = windowname;
            
            WindowArea = windowarea;
            WindowLevelName = windowlevelname;
            SillHeight = windowSillheight;
            WindowLocation = loc;

        }
        public string WindowLevelName { get; set; }
        public Double SillHeight { get; set; }
        public string WindowID { get; set; }
        
        public string WindowName { get; set; }
        public double WindowArea { get; set; }

        public XYZ WindowLocation { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        
      

        public override string ToString()
        {
            return "Name : " + WindowName ;
        }


        public void OnPropertityChanged([CallerMemberName] string propertyname = null)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }



    }
}
