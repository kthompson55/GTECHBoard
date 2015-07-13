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
using Collection_Game_Tool.Services;
using Collection_Game_Tool.Main;
using System.Windows.Threading;
using System.IO;
using System.Threading;
using System.ComponentModel;

namespace Collection_Game_Tool.GameSetup
{
    /// <summary>
    /// Interaction logic for GameSetupUC.xaml
    /// </summary>
    public partial class GameSetupUC : UserControl, Teller, Listener
    {
        private List<Listener> listenerList = new List<Listener>();
		private ProcessingWindow processingWindow;
        public GameSetupUC()
        {
            InitializeComponent();
        }

        public void DataBind()
        {
            MainWindowModel.Instance.GameSetupModel.canCreate = true;
			NumTurnsSlider.DataContext = MainWindowModel.Instance.GameSetupModel;
			NearWinCheckbox.DataContext = MainWindowModel.Instance.GameSetupModel;
            CreateButton.DataContext = MainWindowModel.Instance.GameSetupModel;
			DiceRadioButton.DataContext = MainWindowModel.Instance.GameSetupModel;
			NumNearWinsSlider.DataContext = MainWindowModel.Instance.GameSetupModel;
			SpinnerRadioButton.DataContext = MainWindowModel.Instance.GameSetupModel;
			NumDiceSlider.DataContext = MainWindowModel.Instance.GameSetupModel;
			SpinnerValueSlider.DataContext = MainWindowModel.Instance.GameSetupModel;
			BoardSizeTextBox.DataContext = MainWindowModel.Instance.GameSetupModel;
			NumMoveForwardTilesTextBox.DataContext = MainWindowModel.Instance.GameSetupModel;
			MoveForwardLengthSlider.DataContext = MainWindowModel.Instance.GameSetupModel;
			NumMoveBackwardTilesTextBox.DataContext = MainWindowModel.Instance.GameSetupModel;
			MoveBackwardLengthSlider.DataContext = MainWindowModel.Instance.GameSetupModel;
            ErrorTextBlock.DataContext = ErrorService.Instance;
            WarningTextBlock.DataContext = ErrorService.Instance;
            errorPanelScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            MainWindowModel.Instance.verifyNumTiles();
        }

        //populates the fields from a saved cggproj file
        public void loadExistingData()
        {
			MainWindowModel.Instance.GameSetupModel.initializeListener();
            Window parentWindow = Window.GetWindow(this.Parent);
			MainWindowModel.Instance.GameSetupModel.addListener( ( Window1 )parentWindow );

			NumTurnsSlider.DataContext = MainWindowModel.Instance.GameSetupModel;
			NearWinCheckbox.DataContext = MainWindowModel.Instance.GameSetupModel;
			CreateButton.DataContext = MainWindowModel.Instance.GameSetupModel;
			DiceRadioButton.DataContext = MainWindowModel.Instance.GameSetupModel;
			NumNearWinsSlider.DataContext = MainWindowModel.Instance.GameSetupModel;
			SpinnerRadioButton.DataContext = MainWindowModel.Instance.GameSetupModel;
			NumDiceSlider.DataContext = MainWindowModel.Instance.GameSetupModel;
			SpinnerValueSlider.DataContext = MainWindowModel.Instance.GameSetupModel;
			BoardSizeTextBox.DataContext = MainWindowModel.Instance.GameSetupModel;
			NumMoveForwardTilesTextBox.DataContext = MainWindowModel.Instance.GameSetupModel;
			MoveForwardLengthSlider.DataContext = MainWindowModel.Instance.GameSetupModel;
			NumMoveBackwardTilesTextBox.DataContext = MainWindowModel.Instance.GameSetupModel;
			MoveBackwardLengthSlider.DataContext = MainWindowModel.Instance.GameSetupModel;

			NearWinCheckbox.IsChecked = MainWindowModel.Instance.GameSetupModel.isNearWin;
			NumNearWinsSlider.Value = MainWindowModel.Instance.GameSetupModel.nearWins;
			NumTurnsSlider.Value = MainWindowModel.Instance.GameSetupModel.numTurns;
			DiceRadioButton.IsChecked = MainWindowModel.Instance.GameSetupModel.diceSelected;
			NumDiceSlider.Value = MainWindowModel.Instance.GameSetupModel.numDice;
			SpinnerValueSlider.Value = MainWindowModel.Instance.GameSetupModel.spinnerMaxValue;
			BoardSizeTextBox.Text = MainWindowModel.Instance.GameSetupModel.boardSize.ToString();
			NumMoveForwardTilesTextBox.Text = MainWindowModel.Instance.GameSetupModel.numMoveForwardTiles.ToString();
			MoveForwardLengthSlider.Value = MainWindowModel.Instance.GameSetupModel.moveForwardLength;
			NumMoveBackwardTilesTextBox.Text = MainWindowModel.Instance.GameSetupModel.numMoveBackwardTiles.ToString();
			MoveBackwardLengthSlider.Value = MainWindowModel.Instance.GameSetupModel.moveBackwardLength;
        }

        //Initiates save process when Create Button is clicked
        public void createButton_Click(object sender, RoutedEventArgs e)
        {

			processingWindow = new ProcessingWindow();
			processingWindow.ShowDialog();
        }

        /// <summary>
        /// Checks that the value entered in Max Permutations is acceptable
        /// </summary>
        /// <param name="s">The input from the Max Permutations Textbox</param>
        /// <returns>whether the value is in the acceptable range</returns>
        private bool WithinPermutationRange(string s)
        {
            uint philTheOrphan;
            UInt32.TryParse(s, out philTheOrphan);
            return philTheOrphan < 100000 && philTheOrphan > 0;
        }


        private void GameSetupUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this.Parent);
			MainWindowModel.Instance.GameSetupModel.addListener( ( Window1 )parentWindow );
        }

        public void shout(object pass)
        {
            foreach (Listener list in listenerList)
            {
                list.onListen(pass);
            }
        }

        public void addListener(Listener list)
        {
            listenerList.Add(list);
        }

        public void onListen(object pass)
        {
            if (pass is int)
            {
                int pick = (int)pass;
            }
        }

        /// <summary>
        /// Ensures that the Create Button can only be pressed when there are no errors
        /// </summary>
        public void adjustCreateButtonEnabled()
        {
            if (ErrorService.Instance.ErrorText == "" || ErrorService.Instance.ErrorText == null)
            {
                CreateButton.IsEnabled = true;
            }
            else
            {
                CreateButton.IsEnabled = false;
            }
        }

        private void ErrorTextBlock_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            adjustBorderVisibility();
            adjustCreateButtonEnabled();
        }

        private void WarningTextBlock_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            adjustBorderVisibility();
        }

        /// <summary>
        /// Ensures the Error/Warning box is only visible if there is at least one warning or error
        /// </summary>
        private void adjustBorderVisibility()
        {
            if (ErrorService.Instance.HasErrors() || ErrorService.Instance.HasWarnings())
            {
                ErrorBoxBorder.Visibility = Visibility.Visible;
                // hides error box if no errors
                if (!ErrorService.Instance.HasErrors())
                {
                    ErrorHeader.Visibility = Visibility.Collapsed;
                    ErrorTextBlock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ErrorHeader.Visibility = Visibility.Visible;
                    ErrorTextBlock.Visibility = Visibility.Visible;
                }
                // hides warning box if no warnings
                if (!ErrorService.Instance.HasWarnings())
                {
                    WarningHeader.Visibility = Visibility.Collapsed;
                    WarningTextBlock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    WarningHeader.Visibility = Visibility.Visible;
                    WarningTextBlock.Visibility = Visibility.Visible;
                }
                ErrorRow.Height = GridLength.Auto;
            }
            else
            {
                ErrorBoxBorder.Visibility = Visibility.Hidden;
                ErrorRow.Height = new GridLength(0);
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.SelectAll();
        }

        private void TextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.SelectAll();
        }

		private void BoardSizeTextBox_TextChanged( object sender, TextChangedEventArgs e )
		{
			var textboxSender = sender as TextBox;
			if ( textboxSender != null ) MainWindowModel.Instance.GameSetupModel.BoardSizeTextBox = textboxSender.Text;
		}

		private void NumMoveForwardTilesTextBox_TextChanged( object sender, TextChangedEventArgs e )
		{
			var textboxSender = sender as TextBox;
			if ( textboxSender != null ) MainWindowModel.Instance.GameSetupModel.NumMoveForwardTilesTextbox = textboxSender.Text;
		}

		private void NumMoveBackwardTilesTextBox_TextChanged( object sender, TextChangedEventArgs e )
		{
			var textboxSender = sender as TextBox;
			if ( textboxSender != null ) MainWindowModel.Instance.GameSetupModel.NumMoveBackwardTilesTextbox = textboxSender.Text;
		}

		private void MaxPermutationsTextBox_TextChanged( object sender, TextChangedEventArgs e )
		{
			var textboxSender = sender as TextBox;
			if ( textboxSender != null ) MainWindowModel.Instance.GameSetupModel.MaxPermutationsTextbox = textboxSender.Text;
		}
    }
}
