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

        public GameSetupModel gsObject;
        List<Listener> listenerList = new List<Listener>();
        private string lastAcceptableMaxPermutationValue = 0 + "";

        public GameSetupUC()
        {
            InitializeComponent();
            gsObject = new GameSetupModel();
            gsObject.canCreate = true;
            CreateButton.DataContext = gsObject;
            ErrorTextBlock.DataContext = ErrorService.Instance;
            WarningTextBlock.DataContext = ErrorService.Instance;
            errorPanelScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            //gameSetupScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        public void loadExistingData(GameSetupModel savedSetup)
        {
            TotalPicksSlider.Value = savedSetup.totalPicks;
            NearWinCheckbox.IsChecked = savedSetup.isNearWin;
            NumNearWinsSlider.Value = savedSetup.nearWins;
            MaxPermutationsTextBox.Text = savedSetup.maxPermutations.ToString();
            gsObject = savedSetup;
            gsObject.initializeListener();
            Window parentWindow = Window.GetWindow(this.Parent);
            gsObject.addListener((Window1)parentWindow);
            pickCheck = gsObject.totalPicks;
        }

        //When Create is clicked, validates data and creates a text file
        public void createButton_Click(object sender, RoutedEventArgs e)
        {
            //open save dialog
            openSaveWindow();
            MaxPermutationsTextBox.Focus();
        }

        private void openSaveWindow()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "CollectionGameFile"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                showGeneratingAnimation();
                gsObject.shout("generate/" + filename);
            }
        }

        private void showGeneratingAnimation()
        {
            GeneratingFileAnimation.Visibility = Visibility.Visible;
            hideGenerationCompleteMessage();
        }

        public void hideGeneratingAnimation()
        {
            GeneratingFileAnimation.Visibility = Visibility.Hidden;
            showGenerationCompleteMessage();
        }
        private void hideGenerationCompleteMessage()
        {
            GeneratingCompleteMessage.Visibility = Visibility.Hidden;

        }
        private void showGenerationCompleteMessage()
        {
            GeneratingCompleteMessage.Visibility = Visibility.Visible;

        }

        private void TotalPicksSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (gsObject != null)
            {
                Slider slider = sender as Slider;
                gsObject.totalPicks = Convert.ToInt16(slider.Value);
                shout((int)gsObject.totalPicks);
                pickCheck = gsObject.totalPicks;
            }
        }

        private void NumNearWinsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (gsObject != null)
            {
                Slider slider = sender as Slider;
                gsObject.nearWins = Convert.ToInt16(slider.Value);

                if (gsObject.nearWins > PrizeLevels.PrizeLevels.numPrizeLevels)
                {
                    gsucID = ErrorService.Instance.reportError("007", new List<string>{}, gsucID);
                }
                else if(gsObject.nearWins<=PrizeLevels.PrizeLevels.numPrizeLevels)
                {
                    ErrorService.Instance.resolveError("007", new List<string> { }, gsucID);
                }
            }
        }

        private void NearWinCheckbox_Click(object sender, RoutedEventArgs e)
        {
            gsObject.toggleNearWin();
           
        }

        private void MaxPermutationsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (gsObject != null)
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
                    gsObject.maxPermutations = Convert.ToUInt32(textBox.Text);
                }
                gsObject.shout("validate");
            }
        }

        private void MaxPermutationsTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.SelectAll();
        }

        private void MaxPermutationsTextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.SelectAll();
        }

        private void MaxPermutationsTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = sender as TextBox;
            lastAcceptableMaxPermutationValue = tb.Text;
           
        }

        private bool WithinPermutationRange(string s)
        {
            uint philTheOrphan;
            return UInt32.TryParse(s, out philTheOrphan);
        }

       
        private void GameSetupUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this.Parent);
            gsObject.addListener((Window1)parentWindow);
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

        private void ErrorTextBlock_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            adjustBorderVisibility();
            adjustCreateButtonEnabled();
        }

        private void WarningTextBlock_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            adjustBorderVisibility();
        }


        private void adjustCreateButtonEnabled()
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

        private void adjustBorderVisibility()
        {
            if ((ErrorService.Instance.errorText == "" || ErrorService.Instance.errorText == null) && 
                (ErrorService.Instance.warningText == "" || ErrorService.Instance.warningText == null))
            {
                ErrorBoxBorder.Visibility = Visibility.Hidden;
            }
            else
            {
                ErrorBoxBorder.Visibility = Visibility.Visible;
            }
        }
    }
}
