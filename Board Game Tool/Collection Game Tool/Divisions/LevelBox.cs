using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Collection_Game_Tool.PrizeLevels;

namespace Collection_Game_Tool.Divisions
{
    [Serializable]
    public class LevelBox : INotifyPropertyChanged
    {
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isSelected;
        private bool _isAvailable;
        private int _prizeBoxLevel;

        public LevelBox(int level)
        {
            IsSelected = false;
            IsAvailable = false;
            PrizeBoxLevel = level;
        }

        public void switchIsSelected()
        {
            IsSelected = !IsSelected;
        }

        public void clear()
        {
            IsAvailable = false;
            IsSelected = false;
        }

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
