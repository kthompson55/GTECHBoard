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

        public void setupLoadedDivision()
        {
            for (int i = 0; i < DivModel.levelBoxes.Count; i++)
            {
                ((PrizeLevelBox)prizeLevelsGrid.Children[i]).levelModel = DivModel.levelBoxes[i];
                ((PrizeLevelBox)prizeLevelsGrid.Children[i]).levelBox.DataContext = ((PrizeLevelBox)prizeLevelsGrid.Children[i]).levelModel;
                ((PrizeLevelBox)prizeLevelsGrid.Children[i]).prizeLevelLabel.DataContext = ((PrizeLevelBox)prizeLevelsGrid.Children[i]).levelModel;
            }
        }

        private void clearDivisionButton_Click(object sender, RoutedEventArgs e)
        {
            DivModel.clearPrizeLevelList();
            for (int i = 0; i < DivisionModel.MAX_PRIZE_BOXES; i++)
            {
                DivModel.levelBoxes[i].IsSelected = false;
            }

            DivModel.TotalPrizeValue = DivModel.calculateDivisionValue();
            SectionContainer.validateDivision();
        }

        private void deleteDivisionButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorService.Instance.resolveWarning("005", null, DivModel.errorID);
            ErrorService.Instance.resolveError("009", null, DivModel.errorID);
            ErrorService.Instance.resolveError("010", null, DivModel.errorID);
            ErrorService.Instance.resolveError("011", null, DivModel.errorID);
            int index = getIndex();
            SectionContainer.removeDivision(index);
            SectionContainer.validateDivision();
        }

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

        public void updateDivision()
        {
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