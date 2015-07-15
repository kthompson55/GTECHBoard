using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Collection_Game_Tool.PrizeLevels;

namespace Collection_Game_Tool.Divisions
{
    /// <summary>
    /// The PrizeLevel representation model in the division panel
    /// </summary>
    [Serializable]
    public class LevelBox : INotifyPropertyChanged
    {
        /// <summary>
        /// Event handler for fields being changed
        /// </summary>
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isSelected;
        private bool _isAvailable;
        private int _prizeBoxLevel;

        /// <summary>
        /// Constructor for LevelBox in Division GUI object
        /// </summary>
        /// <param name="level">The index of the PrizeLevel being represented</param>
        public LevelBox(int level)
        {
            IsSelected = false;
            IsAvailable = false;
            PrizeBoxLevel = level;
        }

        /// <summary>
        /// The current Division contains the PrizeLevel
        /// </summary>
        public void switchIsSelected()
        {
            IsSelected = !IsSelected;
        }

        /// <summary>
        /// Gray out selected PrizeLevel, used when a PrizeLevel no longer exists in the PrizeLevel panel
        /// </summary>
        public void clear()
        {
            IsAvailable = false;
            IsSelected = false;
        }

        /// <summary>
        /// The division's inclusion of the PrizeLevel
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        /// <summary>
        /// The PrizeLevel exists in the project setup
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                return _isAvailable;
            }
            set
            {
                _isAvailable = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsAvailable"));
            }
        }

        /// <summary>
        /// The PrizeLevel being represented
        /// </summary>
        public int PrizeBoxLevel 
        {
            get
            {
                return _prizeBoxLevel;
            }
            set
            {
                _prizeBoxLevel = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("PrizeBoxLevel"));
            }
        }
    }
}
