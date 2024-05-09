using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using CodeChecker.UI.WIndows;
using Autodesk.Revit.Attributes;
using System.Windows;
using System.Windows.Media.Animation;
using CodeChecker.Helpers;

namespace CodeChecker.RevitContext.ExternalCommands
{
   /// <summary>
   /// Implements revit add-in IExternalCommand interface
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class Command : IExternalCommand
   {

      public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {


         ConstantMembers.Initialize(commandData.Application.ActiveUIDocument);

         try
         {
            //MessageBox.Show("Plugin Created");

            //Open Window
            MainWindow mainUI = new MainWindow();
            mainUI.ShowDialog();


            return Result.Succeeded;
         }
         catch (Exception Ex)
         {

            message = Ex.Message;
            return Result.Failed;
         }



         return Result.Succeeded;
      }



   }















}
