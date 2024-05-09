using CodeChecker.RevitContext.ExternalCommands;
using CodeChecker.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeChecker.UI.WIndows
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();

         DataContext = new MainWindowViewModel();

      }


      #region EVENTS

      /// <summary>
      /// Main ActiveWindow close event.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
      private void MainWindowClose(object sender, System.EventArgs e)
      {
         Close();
      }





      #endregion

      private void Button_Click(object sender, RoutedEventArgs e)
      {

      }
   }
}
