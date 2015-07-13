using Collection_Game_Tool.GameSetup;
using Collection_Game_Tool.Main;
using Collection_Game_Tool.Services;
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

namespace Collection_Game_Tool.Divisions
{
    /// <summary>
    /// Interaction logic for DivisionPanelUC.xaml
    /// </summary>
    [Serializable]
    public partial class DivisionPanelUC : UserControl, Listener, Teller
    {
        List<Listener> listenerList = new List<Listener>();
        private double marginAmount;
        public PrizeLevels.PrizeLevels prizes { get; set; }
        private const int MAX_DIVISIONS = 30;
        private string dpucID;

        public DivisionPanelUC()
        {
            InitializeComponent();
            marginAmount = 10;
            divisionsScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.Loaded += new RoutedEventHandler(DivisionPanelUC_Loaded);
            divisionCounterLabel.Content = divisionsHolderPanel.Children.Count;
        }

        private void DivisionPanelUC_Loaded(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this.Parent);
            AddListener((Window1)parentWindow);
            addDivision();
            divisionMaxPermutation.DataContext = MainWindowModel.Instance.DivisionsModel;
        }

        /// <summary>
        /// Adds a new division to the division's section. If the max of 30 is met, nothing is added and the add buttom is disabled
        /// </summary>
        public void addDivision()
        {
            if (MainWindowModel.Instance.DivisionsModel.getSize() < MAX_DIVISIONS)
            {
                DivisionUC divUC = new DivisionUC(prizes, MainWindowModel.Instance.DivisionsModel.getSize() + 1);
                divUC.DivModel.DivisionNumber = MainWindowModel.Instance.DivisionsModel.getSize() + 1;
                divUC.updateDivision();
                divUC.Margin = new Thickness(marginAmount, marginAmount, 0, 0);
                divUC.SectionContainer = this;

                divisionsHolderPanel.Children.Add(divUC);
                MainWindowModel.Instance.DivisionsModel.addDivision(divUC.DivModel);
                this.AddListener(divUC);
                validateDivision();
            }

            if (MainWindowModel.Instance.DivisionsModel.getSize() >= MAX_DIVISIONS)
            {
                addDivisionButton.IsEnabled = false;
                addDivisionButton.Opacity = 0.3;
            }
            divisionCounterLabel.Content = divisionsHolderPanel.Children.Count;
            isSectionEmpty();
        }

        /// <summary>
        /// Loads in an existing division into the divisions section. If the max of 30 is met, nothing is added and the add buttom is disabled
        /// </summary>
        /// <param name="div">The existing division</param>
        public void loadInDivision(DivisionModel div)
        {
            if (MainWindowModel.Instance.DivisionsModel.getSize() < MAX_DIVISIONS)
            {
                int divNumber = divisionsHolderPanel.Children.Count + 1;
                DivisionUC division = new DivisionUC(prizes, divNumber);
                division.DivModel = div;
                division.setDataContextToModel();
                division.DivModel.DivisionNumber = divNumber;
                division.DivModel.levelBoxes = div.levelBoxes;
                division.Margin = new Thickness(marginAmount, marginAmount, 0, 0);
                division.SectionContainer = this;

                division.setupLoadedDivision();
                division.updateDivision();

                divisionsHolderPanel.Children.Add(division);
                this.AddListener(division);
                validateDivision();
            }

            if (MainWindowModel.Instance.DivisionsModel.getSize() >= MAX_DIVISIONS)
            {
                addDivisionButton.IsEnabled = false;
                addDivisionButton.Opacity = 0.3;
            }

            divisionCounterLabel.Content = divisionsHolderPanel.Children.Count;
            isSectionEmpty();
        }

        /// <summary>
        /// Removes a division at the specified index
        /// </summary>
        /// <param name="index">index to remove at</param>
        public void removeDivision(int index)
        {
            for (int i = index; i < MainWindowModel.Instance.DivisionsModel.getSize(); i++)
            {
                DivisionUC div = (DivisionUC)divisionsHolderPanel.Children[i];
                div.DivModel.DivisionNumber = (int)div.DivModel.DivisionNumber - 1;
            }

            ErrorService.Instance.ResolveWarning("005", ((DivisionUC)divisionsHolderPanel.Children[index]).DivModel.errorID);
            listenerList.Remove((DivisionUC)divisionsHolderPanel.Children[index]);
            MainWindowModel.Instance.DivisionsModel.removeDivision(index);
            divisionsHolderPanel.Children.RemoveAt(index);

            if (MainWindowModel.Instance.DivisionsModel.getSize() < MAX_DIVISIONS)
            {
                addDivisionButton.IsEnabled = true;
                addDivisionButton.Opacity = 1.0;
            }

            divisionCounterLabel.Content = divisionsHolderPanel.Children.Count;
            isSectionEmpty();
        }

        /// <summary>
        /// Click event for the AddDivision button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addDivisionButton_Click(object sender, RoutedEventArgs e)
        {
            addDivision();
            divisionsScroll.ScrollToBottom();
            MainWindowModel.Instance.verifyDivisions();
        }

        /// <summary>
        /// Checks whether the available divisions are valid by checking if the divisions are not empty and/or unique from the other divisions
        /// </summary>
        public void validateDivision()
        {
            for (int index = 0; index < divisionsHolderPanel.Children.Count; index++)
            {
                DivisionModel divToCompare = ((DivisionUC)divisionsHolderPanel.Children[index]).DivModel;
                bool empty = true;
                for (int i = 0; i < DivisionModel.MAX_PRIZE_BOXES && empty; i++)
                {
                    if (divToCompare.levelBoxes[i].IsSelected)
                    {
                        empty = false;
                    }
                }

                if (!empty)
                {
                    ErrorService.Instance.ResolveWarning("005", divToCompare.errorID);
                    bool valid = true;
                    for (int i = 0; i < divisionsHolderPanel.Children.Count && valid; i++)
                    {
                        DivisionModel currentDiv = ((DivisionUC)divisionsHolderPanel.Children[i]).DivModel;
                        if (divToCompare.DivisionNumber != currentDiv.DivisionNumber)
                        {
                            bool isUnique = false;
                            for (int prizeIndex = 0; prizeIndex < prizes.getNumPrizeLevels() && !isUnique; prizeIndex++)
                            {
                                if (divToCompare.levelBoxes[prizeIndex].IsSelected != currentDiv.levelBoxes[prizeIndex].IsSelected)
                                {
                                    isUnique = true;
                                }
                            }

                            if (!isUnique)
                            {
                                divToCompare.errorID = ErrorService.Instance.ReportError("009", new List<string>{
                                divToCompare.DivisionNumber.ToString()
                            }, divToCompare.errorID);

                                valid = false;
                            }
                        }
                    }

                    if (valid)
                        ErrorService.Instance.ResolveError("009", divToCompare.errorID);
                }
                else
                {
                    ErrorService.Instance.ResolveError("009", divToCompare.errorID);
                    divToCompare.errorID = ErrorService.Instance.ReportWarning("005", new List<string> { divToCompare.DivisionNumber.ToString() }, divToCompare.errorID);
                }
            }
        }

        /// <summary>
        /// Verifies whether or not there are divisions in the divisions section
        /// </summary>
        private void isSectionEmpty()
        {
            if (MainWindowModel.Instance.DivisionsModel.getSize() <= 0)
            {
                dpucID = ErrorService.Instance.ReportWarning("006", new List<string>(), dpucID);
            }
            else
            {
                ErrorService.Instance.ResolveWarning("006", dpucID);
            }
        }

        public void OnListen(object pass)
        {
            if (pass is PrizeLevels.PrizeLevels)
            {
                prizes = (PrizeLevels.PrizeLevels)pass;
            }
            Shout(pass);
        }

        public void Shout(object pass)
        {
            foreach (Listener list in listenerList)
            {
                list.OnListen(pass);
            }
        }

        public void AddListener(Listener list)
        {
            listenerList.Add(list);
        }
    }
}
