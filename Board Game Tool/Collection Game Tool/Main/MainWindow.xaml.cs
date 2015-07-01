using Collection_Game_Tool.Divisions;
using Collection_Game_Tool.GameSetup;
using Collection_Game_Tool.PrizeLevels;
using Collection_Game_Tool.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using System.Windows.Forms;

namespace Collection_Game_Tool.Main
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window, Listener
    {
        private UserControlPrizeLevels pl;
        private GameSetupUC gs;
        private DivisionPanelUC divUC;
        private ProjectData savedProject;

        public Window1()
        {
            InitializeComponent();
            savedProject = new ProjectData();

            //Programmaticaly add UserControls to mainwindow.
            //Did this because couldn't find a way to access the usercontrol from within the xaml.
            UserControlPrizeLevels ucpl = new UserControlPrizeLevels();
            pl = ucpl;
            this.UserControls.Children.Add(ucpl);

            GameSetupUC gsuc = new GameSetupUC(ucpl);
            gs = gsuc;
            this.UserControls.Children.Add(gsuc);

            divUC = new DivisionPanelUC();
            this.UserControls.Children.Add(divUC);
            divUC.prizes = pl.plsObject;

            //Listener stuff between divisions and Prize Levels
            pl.addListener(divUC);
            gs.addListener(divUC);

            //Listeners for GameSetup so they can see player picks for validation
            gs.addListener(pl);
            gs.addListener(divUC);
            gs.addListener(this);

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);

            Screen screen = System.Windows.Forms.Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(this).Handle);
            this.MaxHeight = screen.WorkingArea.Height;
            this.Height = this.MaxHeight - 50;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.MaxWidth = this.Width;
            this.MinWidth = this.Width;
            toolMenu.Width = this.ActualWidth - 10;
        }

        private void Window_LayoutUpdated_1(object sender, EventArgs e)
        {
            double controlsHeight = this.ActualHeight - toolMenu.ActualHeight - windowHeader.ActualHeight - 35;
            if (controlsHeight < 0) controlsHeight = 0;
            pl.Height = controlsHeight;
            gs.Height = controlsHeight;
            gs.GameSetupMainPanel.Height = gs.ActualHeight;
            divUC.Height = controlsHeight;
            divUC.divisionsScroll.MaxHeight = ((controlsHeight - 130) > 0) ? controlsHeight - 130 : 0;
            toolMenu.Width = this.ActualWidth - 10;
        }

        public void onListen(object pass)
        {
            if (pass is String)
            {
                //if (((String)pass).Contains("generate/") && gs != null)
                //{
                //    String file = ((String)pass).Replace("generate/", "");
                //    FileGenerationService fgs = new FileGenerationService();
                //    BackgroundWorker bgWorker = new BackgroundWorker() { WorkerReportsProgress=true};
                //    bgWorker.DoWork += (s, e) =>
                //    {
                //        fgs.buildGameData(divUC.divisionsList, pl.plsObject, gs.gsObject, file, gs);
                //    };
                //    bgWorker.RunWorkerCompleted += (s, e) =>
                //    {
                //        gs.hideGeneratingAnimation();
                //    };
                //    bgWorker.RunWorkerAsync();  
                //}
            }

            divUC.validateDivision();
        }

        private void SaveItem_Clicked(object sender, RoutedEventArgs e)
        {
            savedProject.SaveProject(gs.gsObject, pl.plsObject, divUC.divisionsList);
        }

        private void SaveAsItem_Clicked(object sender, RoutedEventArgs e)
        {
            savedProject.SaveProjectAs(gs.gsObject, pl.plsObject, divUC.divisionsList);
        }

        private void OpenItem_Clicked(object sender, RoutedEventArgs e)
        {
            bool projectLoadingSuccessful = savedProject.OpenProject();

            if (projectLoadingSuccessful)
            {
                pl.plsObject = savedProject.savedPrizeLevels;
                PrizeLevels.PrizeLevels.numPrizeLevels = savedProject.savedPrizeLevels.getNumPrizeLevels();
                pl.Prizes.Children.Clear();
                for (int i = 0; i < pl.plsObject.getNumPrizeLevels(); i++)
                {
                    pl.loadExistingPrizeLevel(pl.plsObject.prizeLevels[i]);
                }
                pl.checkLoadedPrizeLevels();

                gs.loadExistingData(savedProject.savedGameSetup);

                divUC.divisionsList = savedProject.savedDivisions;
                divUC.prizes = savedProject.savedPrizeLevels;
                divUC.divisionsHolderPanel.Children.Clear();

                for (int i = 0; i < divUC.divisionsList.getSize(); i++)
                {
                    divUC.loadInDivision(divUC.divisionsList.divisions[i]);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Would you like to save the project's data before exiting?", "Exiting Application", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                savedProject.SaveProject(gs.gsObject, pl.plsObject, divUC.divisionsList);
            }
            else if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }
}