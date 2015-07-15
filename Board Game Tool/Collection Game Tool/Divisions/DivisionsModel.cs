using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;


namespace Collection_Game_Tool.Divisions
{
    /// <summary>
    /// The Model that represents the Division Panel of the GUI
    /// </summary>
    [Serializable]
    public class DivisionsModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Event handler of Division fields
        /// </summary>
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The divisions currently stored to the project
        /// </summary>
        public List<DivisionModel> divisions = new List<DivisionModel>();
        private int _maxLossPermutations = 1;

        /// <summary>
        /// The number of permutations that will be generated for the loss division
        /// </summary>
        public int MaxLossPermutations
        {
            get
            {
                return _maxLossPermutations;
            }

            set
            {
                if (value > 0)
                {
                    _maxLossPermutations = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("MaxLossPermutations"));
                }
            }
        }

        /// <summary>
        /// The text that will appear in the loss permutations textbox
        /// </summary>
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

        /// <summary>
        /// Retrieve the number of divisions in the model
        /// </summary>
        /// <returns>The number of divisions in the project setup</returns>
        public int getNumberOfDivisions()
        {
            return divisions.Count;
        }

        /// <summary>
        /// Add a new Division to the project
        /// </summary>
        /// <param name="newDivision">The Division being added to the project setup</param>
        public void addDivision(DivisionModel newDivision)
        {
            divisions.Add(newDivision);
            divisions.Sort();
        }

        /// <summary>
        /// Remove a division from the project
        /// </summary>
        /// <param name="divisionToRemove">The division that needs to be removed</param>
        public void removeDivision(DivisionModel divisionToRemove)
        {
            divisions.Remove(divisionToRemove);
        }

        /// <summary>
        /// Remove a division from the project at a given index
        /// </summary>
        /// <param name="index">The index of the division that needs to be removed</param>
        public void removeDivision(int index)
        {
            divisions.RemoveAt(index);
        }

        /// <summary>
        /// Removes all divisions from project setup
        /// </summary>
        public void clearDivisions()
        {
            divisions.Clear();
        }

        /// <summary>
        /// Retrieves division at given index
        /// </summary>
        /// <param name="index">Index of desired division</param>
        /// <returns>Division at the provided index</returns>
        public DivisionModel getDivision(int index)
        {
            return divisions.ElementAt(index);
        }

        /// <summary>
        /// Retrieve the number of divisions in the project setup
        /// </summary>
        /// <returns>The number of divisions in the project setup</returns>
        public int getSize()
        {
            return divisions.Count();
        }
    }
}
