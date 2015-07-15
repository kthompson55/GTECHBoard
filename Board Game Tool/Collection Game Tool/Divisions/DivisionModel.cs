using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Collection_Game_Tool.PrizeLevels;
using System.Runtime.Serialization;
using Collection_Game_Tool.Main;

namespace Collection_Game_Tool.Divisions
{
    /// <summary>
    /// Model for a division
    /// </summary>
    [Serializable]
    public class DivisionModel : IComparable, INotifyPropertyChanged
    {
        /// <summary>
        /// Event handler for changed fields
        /// </summary>
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The ID of the model for the ErrorService
        /// </summary>
        public String errorID;
        /// <summary>
        /// The PrizeLevels that the Division contains
        /// </summary>
        public List<PrizeLevel> selectedPrizes = new List<PrizeLevel>();
        /// <summary>
        /// The LevelBoxes that the Division contains
        /// </summary>
        public List<LevelBox> levelBoxes = new List<LevelBox>();
        /// <summary>
        /// The maximum number of representable PrizeLevels
        /// </summary>
        public const int MAX_PRIZE_BOXES = 12;

        private int _divisionNumber;
        private double _totalPrizeValue;
        private int _totalPlayerPicks;
        private int _maxPermutations = 1;

        /// <summary>
        /// Constructor for DivisionModel, which sets the default values for its fields
        /// </summary>
        public DivisionModel()
        {
            errorID = null;
            DivisionNumber = 0;
            TotalPrizeValue = 0.00;
            TotalPlayerPicks = 0;
            MaxPermutations = 1;
        }

        /// <summary>
        /// Adds a PrizeLevel to the Division
        /// </summary>
        /// <param name="prizeLevelToAdd">The PrizeLevel that is being added to the Division</param>
        public void addPrizeLevel(PrizeLevel prizeLevelToAdd)
        {
            selectedPrizes.Add(prizeLevelToAdd);
        }


        /// <summary>
        /// Removes a PrizeLevel from the Division
        /// </summary>
        /// <param name="prizeLevelToRemove">The PrizeLevel being removed from the Division</param>
        public void removePrizeLevel(PrizeLevel prizeLevelToRemove)
        {
            selectedPrizes.Remove(prizeLevelToRemove);
        }

        /// <summary>
        /// Removes a PrizeLevel from the Division at the given index
        /// </summary>
        /// <param name="prizeLevelIndex">The index of the PrizeLevel to be removed</param>
        public void removePrizeLevel(int prizeLevelIndex)
        {
            selectedPrizes.RemoveAt(prizeLevelIndex);
        }

        /// <summary>
        /// Removes all the PrizeLevels from the Division
        /// </summary>
        public void clearPrizeLevelList()
        {
            selectedPrizes = new List<PrizeLevel>();
        }

        /// <summary>
        /// Returns all the PrizeLevels of the Division
        /// </summary>
        /// <returns>All the PrizeLevels the Division contains</returns>
        public List<PrizeLevel> getPrizeLevelsAtDivision()
        {
            return selectedPrizes;
        }

        /// <summary>
        /// Returns the PrizeLevel at the provided index
        /// </summary>
        /// <param name="index">The index of the desired PrizeLevel</param>
        /// <returns>The PrizeLevel at the provided index</returns>
        public PrizeLevel getPrizeLevel(int index)
        {
            return selectedPrizes.ElementAt(index);
        }

        /// <summary>
        /// Gets the sum of all the selected PrizeLevels of the Division
        /// </summary>
        /// <returns>Sum of all PrizeLevels of Division</returns>
        public double calculateDivisionValue()
        {
            double divisionValue = 0.0f;
            foreach (PrizeLevel p in selectedPrizes)
            {
                divisionValue += p.prizeValue;
            }
            return divisionValue;
        }

        /// <summary>
        /// Gets the sum of all needed collections to win this Division
        /// </summary>
        /// <returns>Sum of all collections of all PrizeLevels in Division</returns>
        public int calculateTotalCollections()
        {
            int collections = 0;
            foreach (PrizeLevel p in selectedPrizes)
            {
                if (p.isInstantWin)
                    collections += 1;
                else
                    collections += p.numCollections;
            }
            return collections;
        }

        /// <summary>
        /// The index of the Division
        /// </summary>
        public int DivisionNumber
        {
            get
            {
                return _divisionNumber;
            }

            set
            {
                _divisionNumber = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("DivisionNumber"));
            }
        }

        /// <summary>
        /// Represents the sum of all PrizeLevels' values selected in the Division
        /// </summary>
        public double TotalPrizeValue
        {
            get
            {
                return _totalPrizeValue;
            }

            set
            {
                _totalPrizeValue = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TotalPrizeValue"));
            }
        }

        /// <summary>
        /// Represents the needed number of collections needed to win this Division
        /// </summary>
        public int TotalPlayerPicks
        {
            get
            {
                return _totalPlayerPicks;
            }

            set
            {
                _totalPlayerPicks = value;
                MainWindowModel.Instance.VerifyDivisions();
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TotalPlayerPicks"));
            }
        }

        /// <summary>
        /// The number of permutations that this division will receive in the generated text file
        /// </summary>
        public int MaxPermutations
        {
            get
            {
                return _maxPermutations;
            }

            set
            {
                if (value > 0)
                {
                    _maxPermutations = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("MaxPermutations"));
                }
            }
        }

        /// <summary>
        /// The text of the permutations textbox
        /// </summary>
        public string MaxPermutationsTextbox
        {
            get
            {
                return MaxPermutations + "";
            }
            set
            {
                int maxPermValue;
                if (value == "")
                {
                    MaxPermutations = 1;
                }
                else if (Int32.TryParse(value, out maxPermValue))
                {
                    MaxPermutations = maxPermValue;
                }
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("MaxPermutationsTextbox"));
            }
        }

        /// <summary>
        /// Comapares two divisions
        /// </summary>
        /// <param name="obj">The object being compared against</param>
        /// <returns>1 if this divisions is less than the other Division, 0 if equal to, -1 if greater than</returns>
        public int CompareTo(object obj)
        {
            DivisionModel dm = (DivisionModel)obj;
            return (int)Math.Ceiling(dm.calculateDivisionValue() - this.calculateDivisionValue());
        }
    }
}