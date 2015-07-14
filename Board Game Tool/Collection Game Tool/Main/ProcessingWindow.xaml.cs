using Collection_Game_Tool.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace Collection_Game_Tool.Main
{
    /// <summary>
    /// Interaction logic for ProcessingWindow.xaml
    /// </summary>
    public partial class ProcessingWindow : Window
    {
        private bool _processCanceled = false;
        private BackgroundWorker _backgroundWorker;
		/// <summary>
		/// Initializing the processing window
		/// </summary>
        public ProcessingWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (Parent != null && Parent is MainWindow) (Parent as MainWindow).IsEnabled = false;
            _backgroundWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true
            };
            _backgroundWorker.DoWork += (s, e1) =>
            {
                ProcessThread(e1);
            };
            _backgroundWorker.RunWorkerCompleted += (s, e1) =>
            {
                if (!_processCanceled)
                {
                    MessageBox.Show("File Generated!");
                }
                Close();
            };
            _backgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// The thread to run to create the board file.
        /// </summary>
        private void ProcessThread(DoWorkEventArgs e)
        {
            //open save dialog
            string filename = OpenSaveWindow();

			if ( !string.IsNullOrEmpty( filename ))
            {
                _processCanceled = false;

                int minMove = 0;
                int maxMove = 0;
                if (MainWindowModel.Instance.GameSetupModel.diceSelected)
                {
                    minMove = MainWindowModel.Instance.GameSetupModel.numDice;
                    maxMove = MainWindowModel.Instance.GameSetupModel.numDice * 6;
                }
                else
                {
                    minMove = 1;
                    maxMove = MainWindowModel.Instance.GameSetupModel.spinnerMaxValue;
                }

                Collection_Game_Tool.Services.Tiles.ITile boardFirstTile =
                    new BoardGeneration(e).GenerateBoard(
                        MainWindowModel.Instance.GameSetupModel.boardSize,
                        MainWindowModel.Instance.GameSetupModel.initialReachableSpaces,
                        minMove,
                        maxMove,
                        MainWindowModel.Instance.GameSetupModel.numMoveBackwardTiles,
                        MainWindowModel.Instance.GameSetupModel.numMoveForwardTiles,
                        MainWindowModel.Instance.PrizeLevelsModel,
                        MainWindowModel.Instance.GameSetupModel.moveForwardLength,
                        MainWindowModel.Instance.GameSetupModel.moveBackwardLength
                    );
                if (e != null && e.Cancel) return;

                List<Collection_Game_Tool.Services.Tiles.ITile> boards = new List<Collection_Game_Tool.Services.Tiles.ITile>();
                boards.Add(boardFirstTile);
                if (e != null && e.Cancel) return;

                GamePlayGeneration generator = new GamePlayGeneration(boards);

				StringBuilder formattedPlays = new StringBuilder();
                foreach (Collection_Game_Tool.Services.Tiles.ITile board in boards)
                {
                    if (e != null && e.Cancel) return;
                    formattedPlays.Append(generator.GetFormattedGameplay(boards));
                }


                if (e != null && e.Cancel) return;
                // write to file
                File.WriteAllText(filename, formattedPlays.ToString());
            }
            else
            {
                _processCanceled = true;
            }
        }

        /// <summary>
        /// Opens the standard save menu for the user to specify the save location
        /// Initiates generation of the file once the user is finished
        /// </summary>
        private string OpenSaveWindow()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "CollectionGameFile"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show save file dialog box
            bool? result = dlg.ShowDialog();

			string filename = "";
			// Process save file dialog box results
			if ( result == true )
			{
				// Save document
				filename = dlg.FileName;
				MainWindowModel.Instance.GameSetupModel.Shout( "generate/" + filename );
			}

            return filename;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_backgroundWorker != null && _backgroundWorker.IsBusy)
            {
                var result = MessageBox.Show("Not finished, cancel?", "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result.Equals(MessageBoxResult.Yes) && _backgroundWorker != null && _backgroundWorker.IsBusy)
                {
                    _backgroundWorker.CancelAsync();
                    while (_backgroundWorker.IsBusy) ;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
