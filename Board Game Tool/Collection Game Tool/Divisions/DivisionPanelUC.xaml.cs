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
        public DivisionsModel divisionsList;
        private int allottedPlayerPicks;
        private double marginAmount;
        public PrizeLevels.PrizeLevels prizes { get; set; }
        private const int MAX_DIVISIONS = 30;
        private string dpucID;

        public DivisionPanelUC()
        {
            InitializeComponent();
            allottedPlayerPicks = 1;
            divisionsList = new DivisionsModel();
            marginAmount = 10;
            divisionsScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.Loaded += new RoutedEventHandler(DivisionPanelUC_Loaded);
        }

        private void DivisionPanelUC_Loaded(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this.Parent);
            addListener((Window1)parentWindow);
            addDivision();
        }

        public void addDivision()
        {
            if (divisionsList.getSize() < MAX_DIVISIONS)
            {
                DivisionUC divUC = new DivisionUC(prizes, divisionsList.getSize() + 1);
                divUC.DivModel.DivisionNumber = divisionsList.getSize() + 1;
                divUC.updateDivision();
                divUC.Margin = new Thickness(marginAmount, marginAmount, 0, 0);
                divUC.SectionContainer = this;

                divisionsHolderPanel.Children.Add(divUC);
                divisionsList.addDivision(divUC.DivModel);
                this.addListener(divUC);
                validateDivision();
            }

            if (divisionsList.getSize() >= MAX_DIVISIONS)
            {
                addDivisionButton.IsEnabled = false;
            }

            isSectionEmpty();
        }

        public void loadInDivision(int number, DivisionModel div)
        {
            if (divisionsList.getSize() < MAX_DIVISIONS)
            {
                DivisionUC division = new DivisionUC(prizes, number);
                division.DivModel = div;
                division.setDataContextToModel();
                division.DivModel.DivisionNumber = number;
                division.DivModel.levelBoxes = div.levelBoxes;
                division.Margin = new Thickness(marginAmount, marginAmount, 0, 0);
                division.SectionContainer = this;

                division.setupLoadedDivision();
                division.updateDivision();

                divisionsHolderPanel.Children.Add(division);
                this.addListener(division);
                validateDivision();
            }

            if (divisionsList.getSize() >= MAX_DIVISIONS)
            {
                addDivisionButton.IsEnabled = false;
            }
            isSectionEmpty();
        }

        public void removeDivision(int index)
        {
            for (int i = index; i < divisionsList.getSize(); i++)
            {
                DivisionUC div = (DivisionUC)divisionsHolderPanel.Children[i];
                div.DivModel.DivisionNumber = (int)div.DivModel.DivisionNumber - 1;
            }

            ErrorService.Instance.resolveWarning("005", new List<string> { ((DivisionUC)divisionsHolderPanel.Children[index]).DivModel.DivisionNumber.ToString() }, ((DivisionUC)divisionsHolderPanel.Children[index]).DivModel.errorID);
            listenerList.Remove((DivisionUC)divisionsHolderPanel.Children[index]);
            divisionsList.removeDivision(index);
            divisionsHolderPanel.Children.RemoveAt(index);

            if (divisionsList.getSize() < MAX_DIVISIONS)
            {
                addDivisionButton.IsEnabled = true;
            }

            isSectionEmpty();
        }

        private void addDivisionButton_Click(object sender, RoutedEventArgs e)
        {
            addDivision();
            divisionsScroll.ScrollToBottom();
        }

        public void validateDivision()
        {
            checkDivisionsPicks();

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
                    ErrorService.Instance.resolveWarning("005", new List<string> { divToCompare.DivisionNumber.ToString() }, divToCompare.errorID);
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
                                divToCompare.errorID = ErrorService.Instance.reportError("009", new List<string>{
                                divToCompare.DivisionNumber.ToString()
                            }, divToCompare.errorID);

                                valid = false;
                            }
                        }
                    }

                    if (valid)
                        ErrorService.Instance.resolveError("009", null, divToCompare.errorID);
                }
                else
                {
                    ErrorService.Instance.resolveError("009", null, divToCompare.errorID);
                    divToCompare.errorID = ErrorService.Instance.reportWarning("005", new List<string> { divToCompare.DivisionNumber.ToString() }, divToCompare.errorID);
                }

                int maxCollections = 0;
                for (int i = 0; i < divToCompare.getPrizeLevelsAtDivision().Count; i++)
                {
                    if (divToCompare.getPrizeLevel(i).isInstantWin)
                    {
                        maxCollections += 1;
                    }
                    else
                    {
                        maxCollections += divToCompare.getPrizeLevel(i).numCollections;
                    }
                }
                maxCollections -= GameSetupUC.pickCheck;
                for (int i = 0; i < prizes.getNumPrizeLevels(); i++)
                {
                    if(divToCompare.getPrizeLevelsAtDivision().Contains(prizes.getPrizeLevel(i)))
                        maxCollections-=(prizes.getPrizeLevel(i).numCollections-1);
                }


                if (0 < maxCollections)
                {
                    divToCompare.errorID = ErrorService.Instance.reportError("011", new List<string> { divToCompare.DivisionNumber.ToString() }, divToCompare.errorID);
                }
                else
                    ErrorService.Instance.resolveError("011", null, divToCompare.errorID);
            }

            int allCollections = 0;
            for (int i = 0; i < prizes.getNumPrizeLevels(); i++)
            {
                allCollections += prizes.getPrizeLevel(i).numCollections - 1;
            }

            if (GameSetupUC.pickCheck > allCollections)
            {
                dpucID = ErrorService.Instance.reportError("012", new List<string> { }, dpucID);
            }
            else
                ErrorService.Instance.resolveError("012", null, dpucID);
        }

        private void isSectionEmpty()
        {
            if (divisionsList.getSize() <= 0)
            {
                dpucID = ErrorService.Instance.reportWarning("006", new List<string>(), dpucID);
            }
            else
            {
                ErrorService.Instance.resolveWarning("006", null, dpucID);
            }
        }

        private void checkDivisionsPicks()
        {
            for (int index = 0; index < divisionsHolderPanel.Children.Count; index++)
            {
                DivisionModel currentDivision = ((DivisionUC)divisionsHolderPanel.Children[index]).DivModel;
                if (currentDivision.TotalPlayerPicks <= allottedPlayerPicks)
                {
                    ErrorService.Instance.resolveError("010", null, currentDivision.errorID);
                }
                else
                {
                    currentDivision.errorID = ErrorService.Instance.reportError("010", new List<string> { currentDivision.DivisionNumber.ToString() }, currentDivision.errorID);                    
                }
            }
        }

        public void onListen(object pass)
        {
            if (pass is PrizeLevels.PrizeLevels)
            {
                prizes = (PrizeLevels.PrizeLevels)pass;
            }
            else if (pass is int)
            {
                allottedPlayerPicks = (int)pass;
                checkDivisionsPicks();
            }
            shout(pass);
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
    }
}
