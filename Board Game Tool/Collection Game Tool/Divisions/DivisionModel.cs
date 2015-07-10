using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Collection_Game_Tool.PrizeLevels;
using System.Runtime.Serialization;

namespace Collection_Game_Tool.Divisions
{
    [Serializable]
    public class DivisionModel : IComparable, INotifyPropertyChanged
    {
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        public String errorID;
        public List<PrizeLevel> selectedPrizes = new List<PrizeLevel>();
        public List<LevelBox> levelBoxes = new List<LevelBox>();
        public const int MAX_PRIZE_BOXES = 12;

        private int _divisionNumber;
        private double _totalPrizeValue;
        private int _maxPermutations = 1;

        public DivisionModel()
        {
            errorID = null;
            DivisionNumber = 0;
            TotalPrizeValue = 0.00;
            MaxPermutations = 1;
        }

        public void addPrizeLevel(PrizeLevel prizeLevelToAdd)
        {
            selectedPrizes.Add(prizeLevelToAdd);
        }

        public void removePrizeLevel(PrizeLevel prizeLevelToRemove)
        {
            selectedPrizes.Remove(prizeLevelToRemove);
        }

        public void removePrizeLevel(int prizeLevelIndex)
        {
            selectedPrizes.RemoveAt(prizeLevelIndex);
        }

        public void clearPrizeLevelList()
        {
            selectedPrizes = new List<PrizeLevel>();
        }

        public List<PrizeLevel> getPrizeLevelsAtDivision()
        {
            return selectedPrizes;
        }

        public PrizeLevel getPrizeLevel(int index)
        {
            return selectedPrizes.ElementAt(index);
        }

        public double calculateDivisionValue()
        {
            double divisionValue = 0.0f;
            foreach (PrizeLevel p in selectedPrizes)
            {
                divisionValue += p.prizeValue;
            }
            return divisionValue;
        }

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

        public int CompareTo(object obj)
        {
            DivisionModel dm = (DivisionModel)obj;
            return (int)Math.Ceiling(dm.calculateDivisionValue() - this.calculateDivisionValue());
        }
    }
}