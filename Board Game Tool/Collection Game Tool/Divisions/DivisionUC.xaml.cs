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
using Collection_Game_Tool.PrizeLevels;
using Collection_Game_Tool.Services;
using Collection_Game_Tool.GameSetup;
using Collection_Game_Tool.Main;

namespace Collection_Game_Tool.Divisions
{
    /// <summary>
    /// Interaction logic for DivisionUC.xaml
    /// </summary>
    public partial class DivisionUC : UserControl, Listener, IComparable
    {
        public DivisionModel DivModel { get; set; }
        public PrizeLevels.PrizeLevels Prizes { get; set; }
        public DivisionPanelUC SectionContainer { get; set; }

        public DivisionUC(PrizeLevels.PrizeLevels initialPrizeLevels, int number)
        {
            InitializeComponent();
            DivModel = new DivisionModel();
            setDataContextToModel();
            Prizes = initialPrizeLevels;
            DivModel.DivisionNumber = number;

            for (int i = 0; i < DivisionModel.MAX_PRIZE_BOXES; i++)
            {
                LevelBox levelBox = new LevelBox(i + 1);
                DivModel.levelBoxes.Add(levelBox);
                PrizeLevelBox box = new PrizeLevelBox(this, DivModel.levelBoxes[i]);
                if (i < initialPrizeLevels.getNumPrizeLevels()) box.levelModel.IsAvailable = true;
                prizeLevelsGrid.Children.Add(box);
            }
        }

        public void setDataContextToModel()
        {
            totalValueLabel.DataContext = DivModel;
            divisionNumberLabel.DataContext = DivModel;
        }

        /// <summary>
        /// Setup a loaded division
        /// </summary>
        public void setupLoadedDivision()
        {
            for (int i = 0; i < DivModel.levelBoxes.Count; i++)
            {
                ((PrizeLevelBox)prizeLevelsGrid.Children[i]).levelModel = DivModel.levelBoxes[i];
                ((PrizeLevelBox)prizeLevelsGrid.Children[i]).levelBox.DataContext = ((PrizeLevelBox)prizeLevelsGrid.Children[i]).levelModel;
                ((PrizeLevelBox)prizeLevelsGrid.Children[i]).prizeLevelLabel.DataContext = ((PrizeLevelBox)prizeLevelsGrid.Children[i]).levelModel;
            }
        }

        /// <summary>
        /// Click event for clearing the selected prize levels in the division (Clear)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearDivisionButton_Click(object sender, RoutedEventArgs e)
        {
            DivModel.clearPrizeLevelList();
            for (int i = 0; i < DivisionModel.MAX_PRIZE_BOXES; i++)
            {
                DivModel.levelBoxes[i].IsSelected = false;
            }

            DivModel.TotalPrizeValue = DivModel.calculateDivisionValue();
            SectionContainer.validateDivision();
            MainWindowModel.Instance.verifyDivisions();
        }

        /// <summary>
        /// Click event for the Delete button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteDivisionButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            ErrorService.Instance.resolveWarning("005", DivModel.errorID);
            ErrorService.Instance.resolveWarning("007", DivModel.errorID);
            ErrorService.Instance.resolveError("009", DivModel.errorID);
            ErrorService.Instance.resolveError("010", DivModel.errorID);
            ErrorService.Instance.resolveError("011", DivModel.errorID);
            int index = getIndex();
            SectionContainer.removeDivision(index);
            SectionContainer.validateDivision();

            MainWindowModel.Instance.verifyDivisions();
        }

        /// <summary>
        /// Updates the total prize value of the division
        /// </summary>
        public void updateInfo()
        {
            if (Prizes.getNumPrizeLevels() > 0)
            {
                DivModel.clearPrizeLevelList();
                for (int i = 0; i < Prizes.getNumPrizeLevels(); i++)
                {
                    if (DivModel.levelBoxes[i].IsSelected)
                    {
                        DivModel.addPrizeLevel(Prizes.getPrizeLevel(i));
                    }
                }

                DivModel.TotalPrizeValue = DivModel.calculateDivisionValue();
            }

            SectionContainer.validateDivision();
        }

        /// <summary>
        /// Updates the prize levels and the total prize value of the division
        /// </summary>
        public void updateDivision()
        {
            // TODO
            if (Prizes.getNumPrizeLevels() > 0)
            {
                for (int i = 0; i < DivisionModel.MAX_PRIZE_BOXES; i++)
                {
                    if (DivModel.levelBoxes[i].IsAvailable && DivModel.levelBoxes[i].IsSelected)
                    {
                        DivModel.addPrizeLevel(Prizes.getPrizeLevel(i));
                    }
                    else
                    {
                        DivModel.levelBoxes[i].IsSelected = false;
                    }
                }

                DivModel.TotalPrizeValue = DivModel.calculateDivisionValue();
            }
        }

        /// <summary>
        /// Get the index of this division relative to where it is in the division panel
        /// </summary>
        /// <returns>its index</returns>
        public int getIndex()
        {
            StackPanel divisionsPanel = (StackPanel)this.Parent;
            return divisionsPanel.Children.IndexOf(this);
        }

        public void onListen(object pass)
        {
            if (pass is PrizeLevels.PrizeLevels)
            {
                Prizes = (PrizeLevels.PrizeLevels)pass;

                for (int i = 0; i < DivisionModel.MAX_PRIZE_BOXES; i++)
                {
                    DivModel.levelBoxes[i].IsAvailable = false;
                }

                for (int i = 0; i < Prizes.getNumPrizeLevels(); i++)
                {
                    DivModel.levelBoxes[i].IsAvailable = true;
                }

                DivModel.clearPrizeLevelList();
                updateDivision();
            }
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            DivisionUC div = (DivisionUC)obj;

            return div.DivModel.CompareTo(this.DivModel);
        }
    }
}