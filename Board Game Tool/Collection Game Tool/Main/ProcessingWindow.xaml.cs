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
		private BackgroundWorker bgWorker;
		public ProcessingWindow()
		{
			InitializeComponent();
		}

		private void Window_Initialized( object sender, EventArgs e )
		{
			if ( Parent != null && Parent is Window1 ) ( Parent as Window1 ).IsEnabled = false;
			bgWorker = new BackgroundWorker()
			{
				WorkerReportsProgress = true
			};
			bgWorker.DoWork += ( s, e1 ) =>
			{
				processThread(e1);
			};
			bgWorker.RunWorkerCompleted += ( s, e1 ) =>
			{
				MessageBox.Show( "File Generated!" );
				Close();
			};
			bgWorker.RunWorkerAsync();
		}

		/// <summary>
		/// The thread to run to create the board file.
		/// </summary>
		private void processThread( DoWorkEventArgs e)
		{
			int minMove = 0;
			int maxMove = 0;
			if ( MainWindowModel.Instance.GameSetupModel.diceSelected )
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
				new BoardGeneration(e).genBoard(
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
			if ( e != null && e.Cancel ) return;

			List<Collection_Game_Tool.Services.Tiles.ITile> boards = new List<Collection_Game_Tool.Services.Tiles.ITile>();
			boards.Add( boardFirstTile );
			if ( e != null && e.Cancel ) return;

			GamePlayGeneration generator = new GamePlayGeneration( boards );
			string formattedPlays = "";
			foreach ( Collection_Game_Tool.Services.Tiles.ITile board in boards )
			{
				if ( e != null && e.Cancel ) return;
				formattedPlays += generator.GetFormattedGameplay( boards );
			}
			//open save dialog
			string filename = openSaveWindow();

			if ( e != null && e.Cancel ) return;
			// write to file
			File.WriteAllText( filename, formattedPlays );
		}

		/// <summary>
		/// Opens the standard save menu for the user to specify the save location
		/// Initiates generation of the file once the user is finished
		/// </summary>
		private string openSaveWindow()
		{
			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
			dlg.FileName = "CollectionGameFile"; // Default file name
			dlg.DefaultExt = ".txt"; // Default file extension
			dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

			// Show save file dialog box
			Nullable<bool> result = dlg.ShowDialog();

			string filename = "";
			// Process save file dialog box results
			if ( result == true )
			{
				// Save document
				filename = dlg.FileName;
				MainWindowModel.Instance.GameSetupModel.shout( "generate/" + filename );
			}

			return filename;
		}

		private void Window_Closing( object sender, CancelEventArgs e )
		{
			if(bgWorker != null && bgWorker.IsBusy)
			{
				var result = MessageBox.Show( "Not finished, cancel?", "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Warning );
				if ( result.Equals( MessageBoxResult.Yes ) && bgWorker != null && bgWorker.IsBusy )
				{
					bgWorker.CancelAsync();
					while ( bgWorker.IsBusy ) ;
				}
				else
				{
					e.Cancel = true;
				}
			}
		}
	}
}
