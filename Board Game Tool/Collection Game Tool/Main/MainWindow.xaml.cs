using Collection_Game_Tool.Divisions;
using Collection_Game_Tool.GameSetup;
using Collection_Game_Tool.PrizeLevels;
using Collection_Game_Tool.Services;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Forms;

namespace Collection_Game_Tool.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, Listener
    {
        private UserControlPrizeLevels _userControlPrizeLevels;
        private GameSetupUC _gameSetupUserControl;
        private DivisionPanelUC _divisionPanelUserControl;
        private ProjectData _savedProjectData;

		/// <summary>
		/// Construct the main window
		/// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _savedProjectData = new ProjectData();

            //Programmaticaly add UserControls to mainwindow.
            //Did this because couldn't find a way to access the usercontrol from within the xaml.

            // Prize Levels Column
            _userControlPrizeLevels = new UserControlPrizeLevels();
            this.UserControls.Children.Add(_userControlPrizeLevels);

            // Game Setup Column
            _gameSetupUserControl = new GameSetupUC();
            this.UserControls.Children.Add(_gameSetupUserControl);

            // Divisions Column
            _divisionPanelUserControl = new DivisionPanelUC();
            this.UserControls.Children.Add(_divisionPanelUserControl);

            // Prize levels logic
            MainWindowModel.Instance.PrizeLevelsModel = new PrizeLevels.PrizeLevels();
            _divisionPanelUserControl.prizes = MainWindowModel.Instance.PrizeLevelsModel;
            _userControlPrizeLevels.AddDefaultPrizeLevels();

            // Game setup logic
			MainWindowModel.Instance.GameSetupModel = new GameSetupModel();
            _gameSetupUserControl.DataBind();

            // Divisions logic
            MainWindowModel.Instance.DivisionsModel = new DivisionsModel();

            //Listener stuff between divisions and Prize Levels
            _userControlPrizeLevels.AddListener(_divisionPanelUserControl);

            //Listeners for GameSetup so they can see player picks for validation
            _gameSetupUserControl.AddListener(_userControlPrizeLevels);
            _gameSetupUserControl.AddListener(_divisionPanelUserControl);
            _gameSetupUserControl.AddListener(this);            

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);

            Screen screen = System.Windows.Forms.Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(this).Handle);
            this.MaxHeight = screen.WorkingArea.Height;
            this.Height = this.MaxHeight - 50;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.MaxWidth = this.Width;
            this.MinWidth = this.Width;
            toolMenu.Width = this.ActualWidth - 10;
        }

        private void WindowLayoutUpdated(object sender, EventArgs e)
        {
            double controlsHeight = this.ActualHeight - toolMenu.ActualHeight - windowHeader.ActualHeight - 35;
            if (controlsHeight < 0) controlsHeight = 0;
            _userControlPrizeLevels.Height = controlsHeight;
            _gameSetupUserControl.Height = controlsHeight;
            _gameSetupUserControl.GameSetupMainPanel.Height = _gameSetupUserControl.ActualHeight;
            _divisionPanelUserControl.Height = controlsHeight;
            _divisionPanelUserControl.divisionsScroll.MaxHeight = ((controlsHeight - 130) > 0) ? controlsHeight - 130 : 0;
            toolMenu.Width = this.ActualWidth - 10;
        }

		/// <summary>
		/// Called when shouted
		/// </summary>
		/// <param name="pass">The object that was passed</param>
        public void OnListen(object pass)
        {
			if(pass is string && (pass as string) == "validate")
            _divisionPanelUserControl.validateDivision();
        }

        private void New_Clicked(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Would you like to save the current project's data?", "Exiting Application", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
				_savedProjectData.SaveProject( MainWindowModel.Instance.GameSetupModel, MainWindowModel.Instance.PrizeLevelsModel, MainWindowModel.Instance.DivisionsModel );
            }

            if (result != MessageBoxResult.Cancel)
            {
                string projectFileName = "../../DEFAULT.bggproj";
                string temp = System.AppDomain.CurrentDomain.BaseDirectory;
                IFormatter format = new BinaryFormatter();
                Stream stream = new FileStream(projectFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                ProjectData loadedProject = (ProjectData)format.Deserialize(stream);
                _savedProjectData.savedPrizeLevels = loadedProject.savedPrizeLevels;
                _savedProjectData.savedGameSetup = loadedProject.savedGameSetup;
                _savedProjectData.savedDivisions = loadedProject.savedDivisions;

                loadProject();                
            }
        }

        private void SaveItem_Clicked(object sender, RoutedEventArgs e)
        {
			_savedProjectData.SaveProject( MainWindowModel.Instance.GameSetupModel, MainWindowModel.Instance.PrizeLevelsModel, MainWindowModel.Instance.DivisionsModel );
        }

        private void SaveAsItem_Clicked(object sender, RoutedEventArgs e)
        {
			_savedProjectData.SaveProjectAs( MainWindowModel.Instance.GameSetupModel, MainWindowModel.Instance.PrizeLevelsModel, MainWindowModel.Instance.DivisionsModel );
        }

        private void OpenItem_Clicked(object sender, RoutedEventArgs e)
        {
            bool projectLoadingSuccessful = _savedProjectData.OpenProject();

            if (projectLoadingSuccessful)
                loadProject();
        }

        private void loadProject()
        {
            MainWindowModel.Instance.PrizeLevelsModel = _savedProjectData.savedPrizeLevels;
            PrizeLevels.PrizeLevels.numPrizeLevels = _savedProjectData.savedPrizeLevels.getNumPrizeLevels();
            _userControlPrizeLevels.Prizes.Children.Clear();
            for (int i = 0; i < MainWindowModel.Instance.PrizeLevelsModel.getNumPrizeLevels(); i++)
            {
                _userControlPrizeLevels.loadExistingPrizeLevel(MainWindowModel.Instance.PrizeLevelsModel.prizeLevels[i]);
            }
            _userControlPrizeLevels.checkLoadedPrizeLevels();

			MainWindowModel.Instance.GameSetupModel = _savedProjectData.savedGameSetup;
            _gameSetupUserControl.loadExistingData();

            MainWindowModel.Instance.DivisionsModel = _savedProjectData.savedDivisions;
            _divisionPanelUserControl.prizes = _savedProjectData.savedPrizeLevels;
            _divisionPanelUserControl.divisionsHolderPanel.Children.Clear();

            for (int i = 0; i < MainWindowModel.Instance.DivisionsModel.getSize(); i++)
            {
                _divisionPanelUserControl.loadInDivision(MainWindowModel.Instance.DivisionsModel.divisions[i]);
            }

            ErrorService.Instance.ClearErrors();
            ErrorService.Instance.ClearWarnings();

            MainWindowModel.Instance.VerifyNumTiles();
            MainWindowModel.Instance.VerifyDivisions();
			MainWindowModel.Instance.GameSetupModel.Shout( "validate" );
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Would you like to save the project's data before exiting?", "Exiting Application", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
				_savedProjectData.SaveProject( MainWindowModel.Instance.GameSetupModel, MainWindowModel.Instance.PrizeLevelsModel, MainWindowModel.Instance.DivisionsModel );
            }
            else if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }
}