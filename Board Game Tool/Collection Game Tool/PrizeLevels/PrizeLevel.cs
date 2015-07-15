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
    /// <summary>
    /// Represents a prize level, with a value and collection requirement
    /// </summary>
    [Serializable]
    public class PrizeLevel : IComparable, Teller, INotifyPropertyChanged
    {
        /// <summary>
        /// Notification that a field was changed
        /// </summary>
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerializedAttribute()]
        List<Listener> audience = new List<Listener>();

        private int _prizeLevel;
        /// <summary>
        /// The index of the prize level in the collection of all Prize Levels
        /// </summary>
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
        /// <summary>
        /// The monetary worth of the prize level
        /// </summary>
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
        /// <summary>
        /// The number of collections required to win the prize level
        /// </summary>
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
        /// <summary>
        /// Able to be won with a single collection
        /// </summary>
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
        /// <summary>
        /// Enables a bonus game
        /// </summary>
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
        /// <param name="obj">The PrizeLevel being compared against</param>
        /// <returns>-1 when this object is greater than the compared against object, 0 if equal to, and 1 if less than</returns>
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            PrizeLevel pl = (PrizeLevel)obj;
            return (int)Math.Ceiling(pl.prizeValue - this.prizeValue);
        }

        /// <summary>
        /// Shouts message to all listeners
        /// </summary>
        /// <param name="pass">The message being shouted</param>
        public void Shout(object pass)
        {
            foreach (Listener fans in audience)
            {
                fans.OnListen(pass);
            }
        }

        /// <summary>
        /// Add listener that will receive shouts
        /// </summary>
        /// <param name="list"></param>
        public void AddListener(Listener list)
        {
            audience.Add(list);
        }

        /// <summary>
        /// Initialize an empty list of listeners
        /// </summary>
        public void initializeListener()
        {
            audience = new List<Listener>();
        }
    }
}
