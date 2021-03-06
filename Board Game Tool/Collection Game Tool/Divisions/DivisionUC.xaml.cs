﻿using System;
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
        /// <summary>
        /// The model of the Division Panel
        /// </summary>
        public DivisionModel DivModel { get; set; }
        /// <summary>
        /// The PrizeLevels that available for selection by the divisions
        /// </summary>
        public PrizeLevels.PrizeLevels Prizes { get; set; }
        /// <summary>
        /// The GUI element that contains the Division objects
        /// </summary>
        public DivisionPanelUC SectionContainer { get; set; }

        /// <summary>
        /// Constructor that takes in the initial PrizeLevels list, and the number of Divisions to start with
        /// </summary>
        /// <param name="initialPrizeLevels">The starting list of PrizeLevels</param>
        /// <param name="number">Starting number of Divisions</param>
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

        /// <summary>
        /// Sets GUI element's DataContext to model values
        /// </summary>
        public void setDataContextToModel()
        {
            totalPicksLabel.DataContext = DivModel;
            totalValueLabel.DataContext = DivModel;
            divisionNumberLabel.DataContext = DivModel;
            divisionMaxPermutation.DataContext = DivModel;
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
            MainWindowModel.Instance.VerifyDivisions();
        }

        /// <summary>
        /// Click event for the Delete button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteDivisionButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            ErrorService.Instance.ResolveWarning("005", DivModel.errorID);
            ErrorService.Instance.ResolveWarning("007", DivModel.errorID);
            ErrorService.Instance.ResolveError("009", DivModel.errorID);
            ErrorService.Instance.ResolveError("010", DivModel.errorID);
            ErrorService.Instance.ResolveError("011", DivModel.errorID);
            int index = getIndex();
            SectionContainer.removeDivision(index);
            SectionContainer.validateDivision();

            MainWindowModel.Instance.VerifyDivisions();
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
                DivModel.TotalPlayerPicks = DivModel.calculateTotalCollections();
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
                DivModel.TotalPlayerPicks = DivModel.calculateTotalCollections();
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

        /// <summary>
        /// Reacts to shouts from objects being listened to
        /// </summary>
        /// <param name="pass">The message that was shouted</param>
        public void OnListen(object pass)
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

        /// <summary>
        /// Compares this object with the paramater object
        /// </summary>
        /// <param name="obj">The object being compared against</param>
        /// <returns>-1 if this is less than the compared object, 0 if equal to, and 1 if greater than</returns>
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