using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeChecker.RevitContext.Extensions;
using CodeChecker.RevitContext.Methods;
using CodeChecker.RevitContext.Methods.RevitWindows;
using CodeChecker.RevitContext.Methods.STR;
using CodeChecker.RevitContext.Model;
using CodeChecker.Utilities;
using CodeCheckerAddin.Commands;
using CodeCheckerAddin.RevitContext.Methods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CodeChecker.RevitContext.Methods.RevitDoors;

namespace CodeChecker.ViewModel
{
   public class MainWindowViewModel : INotifyPropertyChanged
   {


      #region Fields
      #region Arch-K

      string _result;

      //Check Clear Height
      double _minClearHeight;


      //Check Shaft
      double _minAreaForResidential;
      double _minAreaForServices;

      double _minDimForResidential;
      double _minDimForServices;


      // Check room
      double _minRoomDimForResidential;
      double _minRoomDimForKitchen;
      double _minRoomDimForBathroom;


      double _minRoomAreaForResidential;
      double _minRoomAreaForKitchen;
      double _minRoomAreaForBathroom;




      string _minClearHeightfromuser;
      private ObservableCollection<ModelCurveDes> modelCurveDes;
      private ObservableCollection<RoomDes> roomsDes;
      private ObservableCollection<RoomDes> roomsDesWFailDim;
      private ObservableCollection<RoomDes> roomsDesWFailArea;




      public ObservableCollection<RoomDes> allRoomsDes;


      public ObservableCollection<ShaftOpeningDes> shaftsDesFailDim;


      public ObservableCollection<ShaftOpeningDes> shaftsDesFailArea;

        #endregion

        #region Arch-S

        double _miniSillDim;
        double _miniWindowNetArea;
        double _miniRoomTotalArea;
        private List<ElementId> windowFallInSillHeight;
        private List<ElementId> windowFallInNetArea;
        private List<ElementId> windowFallInTotalArea;
        string _failedWinInSillHeight;
        string _failedWinInNetArea;
        string _failedWinInTotalArea;


        double _miniWidthService;
        double _miniWidthLiving;
        private List<ElementId> doorFallInWidth;
        string _failedRoomInWidth;

        #endregion

        #region STR
        private double _dim;
      private ObservableCollection<StrColumnsDes> _strColumnsList;
      private double _footingwidth;
      private double _footinglength;
      private double _footingthickness;
      private ObservableCollection<StrFootingDes> _strFootingList;
      #endregion


      #endregion

      #region Constructors

      public MainWindowViewModel()
      {

         DoneCommand = new RelayCommand(ExecuteDoneCommand);
         ExportCommand = new RelayCommand(ExecuteExportToEXELCommand);
         CheckCommand = new RelayCommand(ShaftOpeningCheckExecute);
         CheckRoomDimCommand = new RelayCommand(CheckRoomDimExecute);
         CheckRoomAreaCommand = new RelayCommand(CheckRoomAreaExecute);

         CheckStrColCommand = new RelayCommand(CheckStrColExecute);
         CheckFootingCommand = new RelayCommand(CheckStrFootingExecute);


         //Create List
         GetAllRoomsDes.GetAllRoomsDesM();
         //GetList
         AllRoomsDes = new ObservableCollection<RoomDes>(GetAllRoomsDes.ALLRoomDes);

            CheckSillHeight = new RelayCommand(WindowSillHeightCheckExecute);
            CheckNetArea = new RelayCommand(WindowNetAreaExecute);
            CheckTotalArea = new RelayCommand(WindowTotalAreaExecute);
            CheckDoorWidth = new RelayCommand(DoorWidthExecute);


        }




        #endregion


        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

      #region Arch-K

      public ObservableCollection<RoomDes> AllRoomsDes
      {
         get { return allRoomsDes; }
         set
         {
            allRoomsDes = value;
            OnPropertityChanged();


         }
      }

      public ObservableCollection<ModelCurveDes> ModelCurveDes
      {
         get { return modelCurveDes; }
         set
         {
            modelCurveDes = value;
            OnPropertityChanged();
         }
      }

      public ObservableCollection<RoomDes> RoomsDes
      {
         get { return roomsDes; }
         set
         {
            roomsDes = value;
            OnPropertityChanged();
         }
      }


      public ObservableCollection<RoomDes> RoomsDesWFailDim
      {
         get { return roomsDesWFailDim; }
         set
         {
            roomsDesWFailDim = value;
            OnPropertityChanged();
         }
      }


      public ObservableCollection<RoomDes> RoomsDesWFailArea
      {
         get { return roomsDesWFailArea; }
         set
         {
            roomsDesWFailArea = value;
            OnPropertityChanged();
         }
      }





      public ObservableCollection<ShaftOpeningDes> ShaftsDesFailDim
      {
         get { return shaftsDesFailDim; }
         set
         {
            shaftsDesFailDim = value;
            OnPropertityChanged();
         }
      }


      public ObservableCollection<ShaftOpeningDes> ShaftsDesFailArea
      {
         get { return shaftsDesFailArea; }
         set
         {
            shaftsDesFailArea = value;
            OnPropertityChanged();
         }
      }


      //--------------------------------------------
      public double MinRoomDimForResidential
      {
         get { return _minRoomDimForResidential; }
         set
         {
            _minRoomDimForResidential = value;
            OnPropertityChanged();
         }
      }


      public double MinRoomDimForKitchen
      {
         get { return _minRoomDimForKitchen; }
         set
         {
            _minRoomDimForKitchen = value;
            OnPropertityChanged();
         }
      }


      public double MinRoomDimForBathroom
      {
         get { return _minRoomDimForBathroom; }
         set
         {
            _minRoomDimForBathroom = value;
            OnPropertityChanged();
         }
      }

      public double MinRoomAreaForResidential
      {
         get { return _minRoomAreaForResidential; }
         set
         {
            _minRoomAreaForResidential = value;
            OnPropertityChanged();
         }
      }

      public double MinRoomAreaForKitchen
      {
         get { return _minRoomAreaForKitchen; }
         set
         {
            _minRoomAreaForKitchen = value;
            OnPropertityChanged();
         }
      }

      public double MinRoomAreaForBathroom
      {
         get { return _minRoomAreaForBathroom; }
         set
         {
            _minRoomAreaForBathroom = value;
            OnPropertityChanged();
         }
      }
      //--------------------------------------------





      public string Result
      {
         get { return _result; }
         set
         {
            _result = value;
            OnPropertityChanged();
         }
      }


      public double MinClearHeight
      {
         get { return _minClearHeight; }
         set
         {
            _minClearHeight = value;
            OnPropertityChanged();
         }
      }



      public double MinAreaForResidential
      {
         get { return _minAreaForResidential; }
         set
         {
            _minAreaForResidential = value;
            OnPropertityChanged();
         }
      }



      public double MinAreaForServices
      {
         get { return _minAreaForServices; }
         set
         {
            _minAreaForServices = value;
            OnPropertityChanged();
         }
      }



      public double MinDimForResidential
      {
         get { return _minDimForResidential; }
         set
         {
            _minDimForResidential = value;
            OnPropertityChanged();
         }
      }



      public double MinDimForServices
      {
         get { return _minDimForServices; }
         set
         {
            _minDimForServices = value;
            OnPropertityChanged();
         }
      }







      public string MinClearHeightfromuser
      {
         get { return _minClearHeightfromuser; }
         set
         {
            bool isDouble = true;
            while (isDouble)
            {
               if (double.TryParse(_minClearHeightfromuser, out double result))
               {
                  _minClearHeightfromuser = value;
                  MinClearHeight = result;
                  isDouble = false;
               }
               else { TaskDialog.Show("Invalid Input", _minClearHeight.ToString() + "is not Double"); }

            }
            OnPropertityChanged();
         }
      }


      #endregion

      #region STRProp
      public double Dim
      {
         get { return _dim; }
         set
         {
            _dim = value;
            OnPropertityChanged();
         }
      }

      public ObservableCollection<StrColumnsDes> StrColumnsList
      {
         get { return _strColumnsList; }
         set
         {
            _strColumnsList = value;
            OnPropertityChanged();
         }
      }


      public double FootingWidth
      {
         get { return _footingwidth; }
         set
         {
            _footingwidth = value;
            OnPropertityChanged();
         }
      }
      public double FootingLength
      {
         get { return _footinglength; }
         set
         {
            _footinglength = value;
            OnPropertityChanged();
         }
      }
      public double FootingThickness
      {
         get { return _footingthickness; }
         set
         {
            _footingthickness = value;
            OnPropertityChanged();
         }
      }
      public ObservableCollection<StrFootingDes> StrFootingList
      {
         get { return _strFootingList; }
         set
         {
            _strFootingList = value;
            OnPropertityChanged();
         }
      }
      #endregion

      #region Arch-K Command

      public RelayCommand DoneCommand { get; set; }
      public RelayCommand ExportCommand { get; set; }
      public RelayCommand CheckCommand { get; set; }

      public RelayCommand CheckRoomDimCommand { get; set; }
      public RelayCommand CheckRoomAreaCommand { get; set; } 
      #endregion

      #region STRCommand
      public RelayCommand CheckStrColCommand { get; set; }
      public RelayCommand CheckFootingCommand { get; set; }
        #endregion

        #region Arch-S

        public List<ElementId> WindowFallInSillHeight
        {
            get { return windowFallInSillHeight; }
            set
            {
                windowFallInSillHeight = value;
                OnPropertityChanged();


            }
        }
        public List<ElementId> WindowFallInNetArea
        {
            get { return windowFallInNetArea; }
            set
            {
                windowFallInNetArea = value;
                OnPropertityChanged();


            }
        }

        public double MiniSillDim
        {
            get { return _miniSillDim; }
            set
            {
                _miniSillDim = value;
                OnPropertityChanged();
            }
        }
        public double MiniWindowNetArea
        {
            get { return _miniWindowNetArea; }
            set
            {
                _miniWindowNetArea = value;
                OnPropertityChanged();
            }
        }

        public RelayCommand CheckSillHeight { get; set; }
        public RelayCommand CheckNetArea { get; set; }

        public string FailedWinInSillHeight
        {
            get { return _failedWinInSillHeight; }
            set
            {
                _failedWinInSillHeight = value;
                OnPropertityChanged();
            }
        }
        public string FailedWinInNetArea
        {
            get { return _failedWinInNetArea; }
            set
            {
                _failedWinInNetArea = value;
                OnPropertityChanged();
            }
        }


        public List<ElementId> WindowFallInTotalArea
        {
            get { return windowFallInTotalArea; }
            set
            {
                windowFallInTotalArea = value;
                OnPropertityChanged();


            }
        }

        public double MiniRoomTotalArea
        {
            get { return _miniRoomTotalArea; }
            set
            {
                _miniRoomTotalArea = value;
                OnPropertityChanged();
            }
        }

        public string FailedWinInTotalArea
        {
            get { return _failedWinInTotalArea; }
            set
            {
                _failedWinInTotalArea = value;
                OnPropertityChanged();
            }
        }

        public RelayCommand CheckTotalArea { get; set; }


        public double MiniWidthService
        {
            get { return _miniWidthService; }
            set
            {
                _miniWidthService = value;
                OnPropertityChanged();
            }
        }
        public double MiniWidthLiving
        {
            get { return _miniWidthLiving; }
            set
            {
                _miniWidthLiving = value;
                OnPropertityChanged();
            }
        }

        public List<ElementId> DoorFallInWidth
        {
            get { return doorFallInWidth; }
            set
            {
                doorFallInWidth = value;
                OnPropertityChanged();


            }
        }
        public string FailedRoomInWidth
        {
            get { return _failedRoomInWidth; }
            set
            {
                _failedRoomInWidth = value;
                OnPropertityChanged();
            }
        }
        public RelayCommand CheckDoorWidth { get; set; }



        #endregion
        #endregion


        #region Methods

        #region Arch-K
        public void OnPropertityChanged([CallerMemberName] string propertyname = null)
      {

         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
      }



      public void ExecuteDoneCommand()
      {

         Result = CheckClearHeight.RayProjection(MinClearHeight.FromExternalUnit());

         RoomsDes = new ObservableCollection<RoomDes>(allRoomsDes.Where(room => !room.IsHeightPassed));

         ModelCurveDes = new ObservableCollection<ModelCurveDes>(CheckClearHeight.ModelCurvesDes);

      }




      



      public void ShaftOpeningCheckExecute()
      {

         CheckShaftsAreaDim.ShaftOpeningCheckISResidential();

         Result = CheckShaftsAreaDim.ShaftOpeningCheckArea(MinAreaForResidential.FromExternalUnitSquare(), MinAreaForServices.FromExternalUnitSquare());
         Result = CheckShaftsAreaDim.ShaftOpeningCheckDim(MinDimForResidential.FromExternalUnit(), MinDimForServices.FromExternalUnit());

         ShaftsDesFailArea = new ObservableCollection<ShaftOpeningDes>(GetAllShaftOpeningsDes.AllShaftOpeningDes.Where(shaft => !shaft.IsAreaPassed));

         ShaftsDesFailDim = new ObservableCollection<ShaftOpeningDes>(GetAllShaftOpeningsDes.AllShaftOpeningDes.Where(shaft => !shaft.IsMinDimPassed));

      }




      public void CheckRoomDimExecute()
      {
         Result = CheckRoomsDim.checkRoomDim(MinRoomDimForResidential, MinRoomDimForKitchen, MinRoomDimForBathroom);
         RoomsDesWFailDim = new ObservableCollection<RoomDes>(allRoomsDes.Where(room => !room.IsDimPassed));
      }


      public void CheckRoomAreaExecute()
      {
         Result = CheckRoomsArea.checkRoomArea(MinRoomAreaForResidential, MinRoomAreaForKitchen, MinRoomAreaForBathroom);
         RoomsDesWFailArea = new ObservableCollection<RoomDes>(allRoomsDes.Where(room => !room.IsAreaPassed));
      }

      #endregion

      #region STR
      private void CheckStrColExecute()
      {

         Result = CheckStrColumns.StrColumns(Dim, Dim);
         StrColumnsList = (new ObservableCollection<StrColumnsDes>(CheckStrColumns.MismatchedColumns));


      }
      private void CheckStrFootingExecute()
      {

         Result = CheckStrFooting.StrFooting(FootingWidth, FootingLength, FootingThickness);
         StrFootingList = (new ObservableCollection<StrFootingDes>(CheckStrFooting.MismatchedFooting));


      }
      #endregion

      #region CommonMethod Exel
      public void ExecuteExportToEXELCommand(object parameter)
      {
         switch (parameter)
         {
            case "Roomsparameter":
               ExcelExporter.ExportToExcel<RoomDes>(RoomsDes);
               break;

            case "Curvesparameter":
               ExcelExporter.ExportToExcel<ModelCurveDes>(ModelCurveDes);
               break;

            case "Shaftareaparameter":
               ExcelExporter.ExportToExcel<ShaftOpeningDes>(ShaftsDesFailArea);
               break;

            case "shaftdimparameter":
               ExcelExporter.ExportToExcel<ShaftOpeningDes>(ShaftsDesFailDim);
               break;

            case "RoomsWFailedDimparameter":
               ExcelExporter.ExportToExcel<RoomDes>(RoomsDesWFailDim);
               break;

            case "RoomsWFailedAreaparameter":
               ExcelExporter.ExportToExcel<RoomDes>(RoomsDesWFailArea);
               break;
            case "Columnssparameter":
               ExcelExporter.ExportToExcel<StrColumnsDes>(StrColumnsList);
               break;
            case "Footingparameter":
               ExcelExporter.ExportToExcel<StrFootingDes>(StrFootingList);
               break;



                case "Sillparameter":
                    ExcelExporter.ExportToExcel<ElementId>(WindowFallInSillHeight);
                    break;
                case "NetAreaparameter":
                    ExcelExporter.ExportToExcel<ElementId>(WindowFallInNetArea);
                    break;
                case "TotalAreaparameter":
                    ExcelExporter.ExportToExcel<ElementId>(WindowFallInTotalArea);
                    break;
                case "doorparameter":
                    ExcelExporter.ExportToExcel<ElementId>(DoorFallInWidth);
                    break;







            }

      }
        #endregion

        #region Arch-S
        public void WindowSillHeightCheckExecute()
        {



            WindowFallInSillHeight = CheckWindowSillHeight.GetFailedWindowInSillHeight(MiniSillDim);
        }

        public void WindowNetAreaExecute()
        {



            WindowFallInNetArea = CheckWindowNetArea.GetWindowWinthMiniNetArea(MiniWindowNetArea);
        }


        public void WindowTotalAreaExecute()
        {

            var x = CheckWindowRoomArea.GetAllWindowInRoom();

            WindowFallInTotalArea = CheckWindowRoomArea.CompareRoomAreasWithRevit(x, MiniRoomTotalArea);
        }

        public void DoorWidthExecute()
        {

            Dictionary<ElementId, List<ElementId>> ALLDoors = CodeChecker.RevitContext.Methods.RevitDoors.CheckDoorWidth.GetDoorForEachRoom();

            List<ElementId> ALLServceDoors = CodeChecker.RevitContext.Methods.RevitDoors.CheckDoorWidth.GetServiceRoom(ALLDoors, MiniWidthService);
            List<ElementId> ALLLivingDoors = CodeChecker.RevitContext.Methods.RevitDoors.CheckDoorWidth.GetLivingDoors(ALLDoors, ALLServceDoors, MiniWidthLiving);

            
            
            DoorFallInWidth = new List<ElementId>();
            DoorFallInWidth.AddRange(ALLServceDoors);
            DoorFallInWidth.AddRange(ALLLivingDoors);

        }

        #endregion
        #endregion






    }
}
