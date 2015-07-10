using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;


namespace Collection_Game_Tool.Divisions
{
    [Serializable]
    public class DivisionsModel : INotifyPropertyChanged
    {
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        public List<DivisionModel> divisions = new List<DivisionModel>();
        private int _maxLossPermutations;

        public int MaxLossPermutations
        {
            get
            {
                return _maxLossPermutations;
            }

            set
            {
                _maxLossPermutations = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("MaxLossPermutations"));
            }
        }

        public string MaxLossPermutationsTextbox
        {
            get
            {
                return MaxLossPermutations + "";
            }
            set
            {
                int maxPermValue;
                if (value == "")
                {
                    MaxLossPermutations = 1;
                }
                else if (Int32.TryParse(value, out maxPermValue))
                {
                    MaxLossPermutations = maxPermValue;
                }
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("MaxLossPermutationsTextbox"));
            }
        }

        public int getNumberOfDivisions()
        {
            return divisions.Count;
        }

        public void addDivision(DivisionModel newDivision)
        {
            divisions.Add(newDivision);
            divisions.Sort();
        }

        public void removeDivision(DivisionModel divisionToRemove)
        {
            divisions.Remove(divisionToRemove);
        }

        public void removeDivision(int index)
        {
            divisions.RemoveAt(index);
        }

        public void clearDivisions()
        {
            divisions.Clear();
        }

        public DivisionModel getDivision(int index)
        {
            return divisions.ElementAt(index);
        }

        public int getSize()
        {
            return divisions.Count();
        }
    }
}
