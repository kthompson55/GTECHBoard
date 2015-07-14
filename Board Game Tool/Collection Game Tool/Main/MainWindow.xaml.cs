﻿using Collection_Game_Tool.Divisions;
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
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

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

            // Prize Levels Column
            pl = new UserControlPrizeLevels();
            this.UserControls.Children.Add(pl);

            // Game Setup Column
            gs = new GameSetupUC();
            this.UserControls.Children.Add(gs);

            // Divisions Column
            divUC = new DivisionPanelUC();
            this.UserControls.Children.Add(divUC);

            // Prize levels logic
            MainWindowModel.Instance.PrizeLevelsModel = new PrizeLevels.PrizeLevels();
            divUC.prizes = MainWindowModel.Instance.PrizeLevelsModel;
            pl.AddDefaultPrizeLevels();

            // Game setup logic
			MainWindowModel.Instance.GameSetupModel = new GameSetupModel();
            gs.DataBind();

            // Divisions logic
            MainWindowModel.Instance.DivisionsModel = new DivisionsModel();

            //Listener stuff between divisions and Prize Levels
            pl.AddListener(divUC);

            //Listeners for GameSetup so they can see player picks for validation
            gs.AddListener(pl);
            gs.AddListener(divUC);
            gs.AddListener(this);            

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

        public void OnListen(object pass)
        {
			if(pass is string && (pass as string) == "validate")
            divUC.validateDivision();
        }

        private void New_Clicked(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Would you like to save the current project's data?", "Exiting Application", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
				savedProject.SaveProject( MainWindowModel.Instance.GameSetupModel, MainWindowModel.Instance.PrizeLevelsModel, MainWindowModel.Instance.DivisionsModel );
            }

            if (result != MessageBoxResult.Cancel)
            {
                string projectFileName = "../../DEFAULT.bggproj";
                string temp = System.AppDomain.CurrentDomain.BaseDirectory;
                IFormatter format = new BinaryFormatter();
                Stream stream = new FileStream(projectFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                ProjectData loadedProject = (ProjectData)format.Deserialize(stream);
                savedProject.savedPrizeLevels = loadedProject.savedPrizeLevels;
                savedProject.savedGameSetup = loadedProject.savedGameSetup;
                savedProject.savedDivisions = loadedProject.savedDivisions;

                loadProject();                
            }
        }

        private void SaveItem_Clicked(object sender, RoutedEventArgs e)
        {
			savedProject.SaveProject( MainWindowModel.Instance.GameSetupModel, MainWindowModel.Instance.PrizeLevelsModel, MainWindowModel.Instance.DivisionsModel );
        }

        private void SaveAsItem_Clicked(object sender, RoutedEventArgs e)
        {
			savedProject.SaveProjectAs( MainWindowModel.Instance.GameSetupModel, MainWindowModel.Instance.PrizeLevelsModel, MainWindowModel.Instance.DivisionsModel );
        }

        private void OpenItem_Clicked(object sender, RoutedEventArgs e)
        {
            bool projectLoadingSuccessful = savedProject.OpenProject();

            if (projectLoadingSuccessful)
                loadProject();
        }

        private void loadProject()
        {
            MainWindowModel.Instance.PrizeLevelsModel = savedProject.savedPrizeLevels;
            PrizeLevels.PrizeLevels.numPrizeLevels = savedProject.savedPrizeLevels.getNumPrizeLevels();
            pl.Prizes.Children.Clear();
            for (int i = 0; i < MainWindowModel.Instance.PrizeLevelsModel.getNumPrizeLevels(); i++)
            {
                pl.loadExistingPrizeLevel(MainWindowModel.Instance.PrizeLevelsModel.prizeLevels[i]);
            }
            pl.checkLoadedPrizeLevels();

			MainWindowModel.Instance.GameSetupModel = savedProject.savedGameSetup;
            gs.loadExistingData();

            MainWindowModel.Instance.DivisionsModel = savedProject.savedDivisions;
            divUC.prizes = savedProject.savedPrizeLevels;
            divUC.divisionsHolderPanel.Children.Clear();

            for (int i = 0; i < MainWindowModel.Instance.DivisionsModel.getSize(); i++)
            {
                divUC.loadInDivision(MainWindowModel.Instance.DivisionsModel.divisions[i]);
            }

            ErrorService.Instance.ClearErrors();
            ErrorService.Instance.ClearWarnings();

            MainWindowModel.Instance.verifyNumTiles();
            MainWindowModel.Instance.verifyDivisions();
			MainWindowModel.Instance.GameSetupModel.Shout( "validate" );
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Would you like to save the project's data before exiting?", "Exiting Application", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
				savedProject.SaveProject( MainWindowModel.Instance.GameSetupModel, MainWindowModel.Instance.PrizeLevelsModel, MainWindowModel.Instance.DivisionsModel );
            }
            else if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }
}