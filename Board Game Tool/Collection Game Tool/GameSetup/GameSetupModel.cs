using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collection_Game_Tool.Services;
using System.Runtime.Serialization;

namespace Collection_Game_Tool.GameSetup
{
    [Serializable]
    public class GameSetupModel : INotifyPropertyChanged, Teller//, ISerializable
    {
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerializedAttribute()]
        List<Listener> audience = new List<Listener>();

        public GameSetupModel() { }

        public void initializeListener()
        {
            audience = new List<Listener>();
        }
        
        private bool inw;
        public bool isNearWin 
        {
            get
            {
                return inw;
            }
            set
            {
                inw = value;
            }
        }

        private short nw = 2;
        public short nearWins 
        {
            get
            {
                return nw;
            }
            set
            {
                nw = value;
            }
        } //Max 12

        private uint mp = 1;
        public uint maxPermutations 
        {
            get
            {
                return mp;
            }
            set
            {
                mp = value;
            }
        }

        private bool _canCreate;
        public bool canCreate
        {
            get
            {
                return _canCreate;
            }
            set
            {
                _canCreate = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("canCreate"));
            }
        }

        private bool ds = true;
        public bool diceSelected
        {
            get
            {
                return ds;
            }
            set
            {
                ds = value;
            }
        }

        private int nt = 1;
        public int numTurns
        {
            get
            {
                return nt;
            }
            set
            {
                nt = value;
            }
        }

        private int nd = 1;
        public int numDice
        {
            get
            {
                return nd;
            }
            set
            {
                nd = value;
            }
        }

        private int smv = 1;
        public int spinnerMaxValue
        {
            get
            {
                return smv;
            }
            set
            {
                smv = value;
            }
        }

        private int bs = 0;
        public int boardSize
        {
            get
            {
                return bs;
            }
            set
            {
                bs = value;
            }
        }

        private int nmft = 0;
        public int numMoveForwardTiles
        {
            get
            {
                return nmft;
            }
            set
            {
                nmft = value;
            }
        }

        private int nmbt = 0;
        public int numMoveBackwardTiles
        {
            get
            {
                return nmbt;
            }
            set
            {
                nmbt = value;
            }
        }

        private int mfl = 0;
        public int moveForwardLength
        {
            get
            {
                return mfl;
            }
            set
            {
                mfl = value;
            }
        }

        private int mbl = 0;
        public int moveBackwardLength
        {
            get
            {
                return mbl;
            }
            set
            {
                mbl = value;
            }
        }

        public int initialReachableSpaces
        {
            get
            {
                if (diceSelected)
                {
                    return (numDice * 6) * numTurns;
                }
                else
                {
                    return spinnerMaxValue * numTurns;
                }
            }
        }

        public int finalReachableSpaces
        {
            get
            {
                return initialReachableSpaces + (numMoveForwardTiles * moveForwardLength);
            }
        }

        public void toggleNearWin()
        {
            isNearWin = !isNearWin;
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
    }
}
