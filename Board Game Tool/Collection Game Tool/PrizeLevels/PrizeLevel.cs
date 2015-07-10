using Collection_Game_Tool.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Runtime.Serialization;

namespace Collection_Game_Tool.PrizeLevels
{
    [Serializable]
    public class PrizeLevel : IComparable, Teller, INotifyPropertyChanged
    {
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerializedAttribute()]
        List<Listener> audience = new List<Listener>();

        private int _prizeLevel;
        public int prizeLevel
        {
            get
            {
                return _prizeLevel;
            }
            set
            {
                _prizeLevel = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("prizeLevel"));
            }
        }

        private double _prizeValue;
        public double prizeValue
        {
            get
            {
                return _prizeValue;
            }
            set
            {
                _prizeValue = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("prizeValue"));
            }
        }

        private int _numCollections;
        public int numCollections
        {
            get
            {
                return _numCollections;
            }
            set
            {
                _numCollections = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("numCollections"));
            }
        }

        private bool _isInstantWin;
        public bool isInstantWin
        {
            get
            {
                return _isInstantWin;
            }
            set
            {
                _isInstantWin = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("isInstantWin"));
            }
        }

        private bool _isBonusGame;
        public bool isBonusGame
        {
            get
            {
                return _isBonusGame;
            }
            set
            {
                _isBonusGame = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("isBonusGame"));
            }
        }

        /// <summary>
        /// Returns negative when first object is greater than second object.
        /// Returns positive when first object is less than second object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            PrizeLevel pl = (PrizeLevel)obj;
            return (int)Math.Ceiling(pl.prizeValue - this.prizeValue);
        }

        public void shout(object pass)
        {
            foreach (Listener fans in audience)
            {
                fans.onListen(pass);
            }
        }

        public void addListener(Listener list)
        {
            audience.Add(list);
        }

        public void initializeListener()
        {
            audience = new List<Listener>();
        }
    }
}
