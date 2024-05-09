using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using CodeChecker.RevitContext.ExternalCommands;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Windows.Media.Imaging;
using CodeChecker.Utilities;

namespace CodeChecker
{
   /// <summary>
   /// Implements the Revit add-in interface IExternalApplication
   /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class Application : IExternalApplication
    {
        /// <summary>
        /// Implements the on Shutdown event
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }


        /// <summary>
        /// Implements the OnStartup event
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel panel = RibbonPanel(application);
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            if(panel.AddItem(new PushButtonData("Code Checker", "Code Checker", thisAssemblyPath, typeof(Command).FullName)) 
                is PushButton button)
            {
                button.ToolTip = "Code Checker";
                button.LargeImage = ImageUtils.LoadImage(Assembly.GetExecutingAssembly(), "_32x32.ico");
               

            }

            return Result.Succeeded;    

        }


        /// <summary>
        /// Function that creates RibbonPanel
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public RibbonPanel RibbonPanel(UIControlledApplication a)
        {
            string tab = "Code Checker";

            RibbonPanel ribbonPanel = null;

            try
            {
                a.CreateRibbonTab(tab);
            }   
            catch (Exception ex) 
            {
               Debug.WriteLine(ex.Message);
            }

            try
            {
                RibbonPanel panel = a.CreateRibbonPanel(tab, "Code Checker");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            List<RibbonPanel> panels = a.GetRibbonPanels(tab);
            foreach (RibbonPanel p in panels.Where(p => p.Name == "Code Checker"))
            {
                ribbonPanel = p;
            }

            return ribbonPanel;


        }


    }

}
