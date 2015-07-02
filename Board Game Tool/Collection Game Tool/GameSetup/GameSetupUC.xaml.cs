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

namespace Collection_Game_Tool.GameSetup
{
    /// <summary>
    /// Interaction logic for GameSetupUC.xaml
    /// </summary>
    public partial class GameSetupUC : UserControl, Teller, Listener
    {
        public static int pickCheck;
        private String gsucID = null;
        private BoardGeneration boardGen;

        List<Listener> listenerList = new List<Listener>();
        private string lastAcceptableMaxPermutationValue = 0 + "";
        private string lastAcceptableBoardSizeValue = 0 + "";
        private string lastAcceptableNumMoveForwardTiles = 0 + "";
        private string lastAcceptableNumMoveBackwardTiles = 0 + "";
        private string lastAcceptableMoveForwardLength = 0 + "";
        private string lastAcceptableMoveBackwardLength = 0 + "";

        public GameSetupUC()
        {
            InitializeComponent();
            boardGen = new BoardGeneration();
        }

        public void DataBind()
        {
            MainWindowModel.gameSetupModel.canCreate = true;
            CreateButton.DataContext = MainWindowModel.gameSetupModel;
            DiceRadioButton.DataContext = MainWindowModel.gameSetupModel.diceSelected;
            ErrorTextBlock.DataContext = ErrorService.Instance;
            WarningTextBlock.DataContext = ErrorService.Instance;
            errorPanelScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            MainWindowModel.gameSetupModel.isNearWin = NearWinCheckbox.IsChecked == true;
            MainWindowModel.gameSetupModel.nearWins = (short)NumNearWinsSlider.Value;
            MainWindowModel.gameSetupModel.numTurns = (int)NumTurnsSlider.Value;
            MainWindowModel.gameSetupModel.diceSelected = DiceRadioButton.IsChecked == true;
            MainWindowModel.gameSetupModel.numDice = (int)NumDiceSlider.Value;
            MainWindowModel.gameSetupModel.spinnerMaxValue = (int)SpinnerValueSlider.Value;
            MainWindowModel.gameSetupModel.boardSize = int.Parse(BoardSizeTextBox.Text);
            MainWindowModel.gameSetupModel.numMoveForwardTiles = int.Parse(NumMoveForwardTilesTextBox.Text);
            MainWindowModel.gameSetupModel.moveForwardLength = (int)MoveForwardLengthSlider.Value;
            MainWindowModel.gameSetupModel.numMoveBackwardTiles = int.Parse(NumMoveBackwardTilesTextBox.Text);
            MainWindowModel.gameSetupModel.moveBackwardLength = (int)MoveBackwardLengthSlider.Value;
            MainWindowModel.gameSetupModel.maxPermutations = uint.Parse(MaxPermutationsTextBox.Text);
            
            int needed = PrizeLevels.PrizeLevels.totalCollections + MainWindowModel.gameSetupModel.numMoveBackwardTiles + MainWindowModel.gameSetupModel.numMoveForwardTiles;
            int actual = MainWindowModel.gameSetupModel.boardSize;
            if (needed > actual)
            {
                gsucID = ErrorService.Instance.reportError("014", new List<String> { }, gsucID);
            }
            else
            {
                ErrorService.Instance.resolveError("014", gsucID);
            }
        }

        //populates the fields from a saved cggproj file
        public void loadExistingData()
        {
            MainWindowModel.gameSetupModel.initializeListener();
            Window parentWindow = Window.GetWindow(this.Parent);
            MainWindowModel.gameSetupModel.addListener((Window1)parentWindow);

            NearWinCheckbox.IsChecked = MainWindowModel.gameSetupModel.isNearWin;
            NumNearWinsSlider.Value = MainWindowModel.gameSetupModel.nearWins;
            NumTurnsSlider.Value = MainWindowModel.gameSetupModel.numTurns;
            DiceRadioButton.IsChecked = MainWindowModel.gameSetupModel.diceSelected;
            NumDiceSlider.Value = MainWindowModel.gameSetupModel.numDice;
            SpinnerValueSlider.Value = MainWindowModel.gameSetupModel.spinnerMaxValue;
            BoardSizeTextBox.Text = MainWindowModel.gameSetupModel.boardSize.ToString();
            NumMoveForwardTilesTextBox.Text = MainWindowModel.gameSetupModel.numMoveForwardTiles.ToString();
            MoveForwardLengthSlider.Value = MainWindowModel.gameSetupModel.moveForwardLength;
            NumMoveBackwardTilesTextBox.Text = MainWindowModel.gameSetupModel.numMoveBackwardTiles.ToString();
            MoveBackwardLengthSlider.Value = MainWindowModel.gameSetupModel.moveBackwardLength;
            MaxPermutationsTextBox.Text = MainWindowModel.gameSetupModel.maxPermutations.ToString();      
        }

        //Initiates save process when Create Button is clicked
        public void createButton_Click(object sender, RoutedEventArgs e)
        {
			showGeneratingAnimation();
            int minMove = 0;
            int maxMove = 0;
            if (MainWindowModel.gameSetupModel.diceSelected)
            {
                minMove = MainWindowModel.gameSetupModel.numDice;
                maxMove = MainWindowModel.gameSetupModel.numDice * 6;
            }
            else 
            {
                minMove = 1;
                maxMove = MainWindowModel.gameSetupModel.spinnerMaxValue;
            }

            Collection_Game_Tool.Services.Tiles.ITile boardFirstTile = 
                boardGen.genBoard(
                    MainWindowModel.gameSetupModel.boardSize,
                    MainWindowModel.gameSetupModel.initialReachableSpaces,
                    minMove, 
                    maxMove,
                    MainWindowModel.gameSetupModel.numMoveBackwardTiles,
                    MainWindowModel.gameSetupModel.numMoveForwardTiles,
                    MainWindowModel.prizeLevelsModel,
                    MainWindowModel.gameSetupModel.moveForwardLength,
                    MainWindowModel.gameSetupModel.moveBackwardLength
                );
            List<Collection_Game_Tool.Services.Tiles.ITile> boards = new List<Collection_Game_Tool.Services.Tiles.ITile>();
            boards.Add(boardFirstTile);
            GamePlayGeneration generator = new GamePlayGeneration(boards);
            string formattedPlays = "";
            foreach(Collection_Game_Tool.Services.Tiles.ITile board in boards) 
            {
                formattedPlays = generator.GetFormattedGameplay(boards);
            }
            //open save dialog
            string filename = openSaveWindow();
            MaxPermutationsTextBox.Focus();
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
            if (result == true)
            {
                // Save document
                filename = dlg.FileName;
                MainWindowModel.gameSetupModel.shout("generate/" + filename);
            }

            return filename;
        }

        private void showGeneratingAnimation()
        {
            GeneratingFileAnimation.Visibility = Visibility.Visible;
			GeneratingFileAnimation.Margin = new Thickness(10);
			GeneratingFileLabel.FontSize = 20;
			GeneratingFileViewbox.Width = 50;
			GeneratingFileViewbox.Height = 50;
            hideGenerationCompleteMessage();
        }

        public void hideGeneratingAnimation()
        {
            GeneratingFileAnimation.Visibility = Visibility.Hidden;
			GeneratingFileAnimation.Margin = new Thickness(0);
			GeneratingFileLabel.FontSize = 1;
			GeneratingFileViewbox.Width = 0;
			GeneratingFileViewbox.Height = 0;
            showGenerationCompleteMessage();
        }
        private void hideGenerationCompleteMessage()
        {
            GeneratingCompleteMessage.Visibility = Visibility.Hidden;
			GeneratingCompleteMessage.FontSize = 1;
			GeneratingCompleteMessage.Margin = new Thickness(0);
        }
        private void showGenerationCompleteMessage()
        {
            GeneratingCompleteMessage.Visibility = Visibility.Visible;
			GeneratingCompleteMessage.FontSize = 20;
			GeneratingCompleteMessage.Margin = new Thickness(10);
        }

        private void NumNearWinsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MainWindowModel.gameSetupModel != null)
            {
                Slider slider = sender as Slider;
                MainWindowModel.gameSetupModel.nearWins = Convert.ToInt16(slider.Value);

                if (MainWindowModel.gameSetupModel.nearWins > PrizeLevels.PrizeLevels.numPrizeLevels)
                {
                    gsucID = ErrorService.Instance.reportError("007", new List<string>{}, gsucID);
                }
                else if (MainWindowModel.gameSetupModel.nearWins <= PrizeLevels.PrizeLevels.numPrizeLevels)
                {
                    ErrorService.Instance.resolveError("007", gsucID);
                }
            }
        }

        private void NearWinCheckbox_Click(object sender, RoutedEventArgs e)
        {
            MainWindowModel.gameSetupModel.toggleNearWin();
        }

        private void MaxPermutationsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindowModel.gameSetupModel != null)
            {
                TextBox textBox = sender as TextBox;
                if (textBox.Text == "")
                {
                    textBox.Text = 0 +"";
                }
                else if (!WithinPermutationRange(textBox.Text))
                {
                    textBox.Text = lastAcceptableMaxPermutationValue;
                }
                else
                {
                    MainWindowModel.gameSetupModel.maxPermutations = Convert.ToUInt32(textBox.Text);
                }
                MainWindowModel.gameSetupModel.shout("validate");
            }
        }


        private void MaxPermutationsTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;
            lastAcceptableMaxPermutationValue = tb.Text;
           
        }

        /// <summary>
        /// Checks that the value entered in Max Permutations is acceptable
        /// </summary>
        /// <param name="s">The input from the Max Permutations Textbox</param>
        /// <returns>whether the value is in the acceptable range</returns>
        private bool WithinPermutationRange(string s)
        {
            uint philTheOrphan;
            return (UInt32.TryParse(s, out philTheOrphan) && philTheOrphan < 100000);
        }

       
        private void GameSetupUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this.Parent);
            MainWindowModel.gameSetupModel.addListener((Window1)parentWindow);
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
            else if (pass is string && ((String)pass).Equals("FileFinished"))
            {
                hideGeneratingAnimation();
            }
        }

        //Do we even need this? Maybe for warnings. Don't use for errors.
        private int maximumBoardSize()
        {
            if ((bool)DiceRadioButton.IsChecked)
            {
                return ((int)NumDiceSlider.Value * 6) * (int)NumTurnsSlider.Value;
            }
            else if ((bool)SpinnerRadioButton.IsChecked)
            {
                return ((int)SpinnerValueSlider.Value) * (int)NumTurnsSlider.Value;
            }
            return 0;
        }

        //Calculates the smallest possible board size for the current settings
        private int minimumBoardSize()
        {
            if ((bool)DiceRadioButton.IsChecked)
            {
                return ((int)NumDiceSlider.Value) * (int)NumTurnsSlider.Value;
            }
            else if ((bool)SpinnerRadioButton.IsChecked)
            {
                return 1 * (int)NumTurnsSlider.Value;
            }
            return 0;
        }

        /// <summary>
        /// Ensures that the Create Button can only be pressed when there are no errors
        /// </summary>
        public void adjustCreateButtonEnabled()
        {
            if (ErrorService.Instance.errorText == "" || ErrorService.Instance.errorText == null)
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
            if ((ErrorService.Instance.errorText == "" || ErrorService.Instance.errorText == null) &&
                (ErrorService.Instance.warningText == "" || ErrorService.Instance.warningText == null))
            {
                ErrorBoxBorder.Visibility = Visibility.Hidden;
				ErrorRow.Height = new GridLength(0);
            }
            else
            {
                ErrorBoxBorder.Visibility = Visibility.Visible;
				ErrorRow.Height = GridLength.Auto;
            }
        }

        private void NumDiceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MainWindowModel.gameSetupModel != null)
            {
                Slider slider = sender as Slider;
                MainWindowModel.gameSetupModel.numDice = Convert.ToInt32(slider.Value);

                //Insert error logging here
                if (int.Parse(BoardSizeTextBox.Text) < minimumBoardSize())
                {
                    gsucID = ErrorService.Instance.reportError("014", new List<String> { }, gsucID);
                    gsucID = ErrorService.Instance.reportError("013", new List<String> { }, gsucID);
                }
                else
                {
                    int needed = PrizeLevels.PrizeLevels.totalCollections + MainWindowModel.gameSetupModel.numMoveBackwardTiles + MainWindowModel.gameSetupModel.numMoveForwardTiles;
                    int actual = MainWindowModel.gameSetupModel.boardSize;
                    if (needed > actual)
                    {
                        gsucID = ErrorService.Instance.reportError("014", new List<String> { }, gsucID);
                    }
                    else
                    {
                        ErrorService.Instance.resolveError("014", gsucID);
                    }
                    ErrorService.Instance.resolveError("013", gsucID);
                }
            }

        }

        private void SpinnerValueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MainWindowModel.gameSetupModel != null)
            {
                Slider slider = sender as Slider;
                MainWindowModel.gameSetupModel.spinnerMaxValue = Convert.ToInt32(slider.Value);

                //Insert error logging here
                if (MainWindowModel.gameSetupModel.spinnerMaxValue == 1)
                {
                    gsucID = ErrorService.Instance.reportWarning("007", new List<string> { }, gsucID);
                    ErrorService.Instance.resolveWarning("008", gsucID);

                }
                else if (int.Parse(BoardSizeTextBox.Text) > maximumBoardSize() || int.Parse(BoardSizeTextBox.Text) < minimumBoardSize() || MainWindowModel.gameSetupModel.spinnerMaxValue == 2)
                {
                    gsucID = ErrorService.Instance.reportWarning("008", new List<String> { }, gsucID);
                    ErrorService.Instance.resolveWarning("007", gsucID);
                }
                else
                {
                    ErrorService.Instance.resolveWarning("007", gsucID);
                    ErrorService.Instance.resolveWarning("008", gsucID);
                }
            }
        }

        private void BoardSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindowModel.gameSetupModel != null)
            {
                TextBox textBox = sender as TextBox;
                if (textBox.Text == "")
                {
                    textBox.Text = 0 + "";
                }
                else if (WithinViableBoardSizeRange(textBox.Text))
                {
                    int needed = PrizeLevels.PrizeLevels.totalCollections + MainWindowModel.gameSetupModel.numMoveBackwardTiles + MainWindowModel.gameSetupModel.numMoveForwardTiles;
                    int actual = Convert.ToInt32(textBox.Text);
                    if (needed > actual)
                    {
                        gsucID = ErrorService.Instance.reportError("014", new List<String> { }, gsucID);
                    }
                    else
                    {
                        ErrorService.Instance.resolveError("014", gsucID);
                        MainWindowModel.gameSetupModel.boardSize = Convert.ToInt32(textBox.Text);
                    }
                }
                else
                {
                    textBox.Text = lastAcceptableBoardSizeValue;
                }
                MainWindowModel.gameSetupModel.shout("validate");
            }
        }

        private bool WithinViableBoardSizeRange(string s)
        {
            int boardSizeValue;
            bool successful = Int32.TryParse(s, out boardSizeValue);
            if (successful)
            {
                if (boardSizeValue < minimumBoardSize())
                {
                    gsucID = ErrorService.Instance.reportError("014", new List<String> { }, gsucID);
                }
                else
                {
                    ErrorService.Instance.resolveError("014", gsucID);
                }
            }
            return successful;
        }

        private void BoardSizeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;
            lastAcceptableBoardSizeValue = tb.Text;
        }

        private void DiceRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (int.Parse(BoardSizeTextBox.Text) < minimumBoardSize())
            {
                gsucID = ErrorService.Instance.reportError("014", new List<String> { }, gsucID);
                gsucID = ErrorService.Instance.reportError("013", new List<String> { }, gsucID);
            }
            else
            {
                int needed = PrizeLevels.PrizeLevels.totalCollections + MainWindowModel.gameSetupModel.numMoveBackwardTiles + MainWindowModel.gameSetupModel.numMoveForwardTiles;
                int actual = MainWindowModel.gameSetupModel.boardSize;
                if (needed > actual)
                {
                    gsucID = ErrorService.Instance.reportError("014", new List<String> { }, gsucID);
                }
                else
                {
                    ErrorService.Instance.resolveError("014", gsucID);
                }
                ErrorService.Instance.resolveError("013", gsucID);
                ErrorService.Instance.resolveWarning("007", gsucID);
                ErrorService.Instance.resolveWarning("008", gsucID);
            }
        }

        private void SpinnerRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (MainWindowModel.gameSetupModel.spinnerMaxValue == 1)
            {
                gsucID = ErrorService.Instance.reportWarning("007", new List<string> { }, gsucID);
                ErrorService.Instance.resolveWarning("008", gsucID);
            }
            else if (MainWindowModel.gameSetupModel.spinnerMaxValue == 2)
            {
                gsucID = ErrorService.Instance.reportWarning("008", new List<string> { }, gsucID);
                ErrorService.Instance.resolveWarning("007", gsucID);
            }
            if (int.Parse(BoardSizeTextBox.Text) < minimumBoardSize())
            {
                gsucID = ErrorService.Instance.reportError("014", new List<String> { }, gsucID);
                gsucID = ErrorService.Instance.reportError("013", new List<String> { }, gsucID);
            }
            else
            {
                ErrorService.Instance.resolveError("014", gsucID);
                ErrorService.Instance.resolveError("013", gsucID);
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

        private void NumMoveForwardTilesTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindowModel.gameSetupModel != null)
            {
                TextBox textBox = sender as TextBox;
                if (textBox.Text == "")
                {
                    textBox.Text = 0 + "";
                }
                int numMFValue;
                if (Int32.TryParse(textBox.Text, out numMFValue) && numMFValue >= 0)
                {
                    MainWindowModel.gameSetupModel.numMoveForwardTiles = numMFValue;
                    int spacesAvailableForMoveForward = MainWindowModel.gameSetupModel.initialReachableSpaces - (PrizeLevels.PrizeLevels.totalCollections);
                    int specialSpaces = numMFValue + MainWindowModel.gameSetupModel.numMoveBackwardTiles;
                    if (specialSpaces > spacesAvailableForMoveForward && specialSpaces != 0)
                    {
                        gsucID = ErrorService.Instance.reportError("014", new List<string> { }, gsucID);
                    }
                    else
                    {
                        ErrorService.Instance.resolveError("014", gsucID);
                    }
                }
                else
                {
                    textBox.Text = lastAcceptableNumMoveForwardTiles;
                }
            }
        }

        private void NumMoveForwardTilesTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;
            lastAcceptableNumMoveForwardTiles = tb.Text;
        }

        private void MoveForwardLengthTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;
            lastAcceptableMoveForwardLength = tb.Text;
        }

        private void NumMoveBackwardTilesTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MainWindowModel.gameSetupModel != null)
            {
                TextBox textBox = sender as TextBox;
                if (textBox.Text == "")
                {
                    textBox.Text = 0 + "";
                }
                int numMBValue;
                //int spacesAvailableForMoveBackward = MainWindowModel.gameSetupModel.initialReachableSpaces - (PrizeLevels.PrizeLevels.totalCollections + MainWindowModel.gameSetupModel.numMoveForwardTiles);
                if (Int32.TryParse(textBox.Text, out numMBValue) && numMBValue >= 0)
                {
                    MainWindowModel.gameSetupModel.numMoveBackwardTiles = numMBValue;
                    int spacesAvailableForMoveBackward = MainWindowModel.gameSetupModel.initialReachableSpaces - (PrizeLevels.PrizeLevels.totalCollections);
                    int specialSpaces = numMBValue + MainWindowModel.gameSetupModel.numMoveForwardTiles;
                    if (specialSpaces > spacesAvailableForMoveBackward && specialSpaces != 0)
                    {
                        gsucID = ErrorService.Instance.reportError("014", new List<string> { }, gsucID);
                    }
                    else
                    {
                        ErrorService.Instance.resolveError("014", gsucID);
                    }
                }
                else
                {
                    textBox.Text = lastAcceptableNumMoveBackwardTiles;
                }
            }
        }

        private void NumMoveBackwardTilesTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;
            lastAcceptableNumMoveBackwardTiles = tb.Text;
        }


        private void MoveBackwardLengthTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;
            lastAcceptableMoveBackwardLength = tb.Text;
        }

        private void MoveForwardLengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MainWindowModel.gameSetupModel != null)
            {
                Slider slider = sender as Slider;
                MainWindowModel.gameSetupModel.moveForwardLength = Convert.ToInt32(slider.Value);
            }
        }

        private void MoveBackwardLengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MainWindowModel.gameSetupModel != null)
            {
                Slider slider = sender as Slider;
                MainWindowModel.gameSetupModel.moveBackwardLength = Convert.ToInt32(slider.Value);
            }
        }

        private void NumTurnsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MainWindowModel.gameSetupModel != null)
            {
                Slider slider = sender as Slider;
                MainWindowModel.gameSetupModel.numTurns = Convert.ToInt32(slider.Value);

                if (int.Parse(BoardSizeTextBox.Text) < minimumBoardSize())
                {
                    gsucID = ErrorService.Instance.reportError("014", new List<String> { }, gsucID);
                }
                if (int.Parse(BoardSizeTextBox.Text) < minimumBoardSize())
                {
                    gsucID = ErrorService.Instance.reportError("013", new List<String> { }, gsucID);
                }
                else
                {
                    ErrorService.Instance.resolveError("014", gsucID);
                    ErrorService.Instance.resolveError("013", gsucID);
                }
            }
        }
    }
}
