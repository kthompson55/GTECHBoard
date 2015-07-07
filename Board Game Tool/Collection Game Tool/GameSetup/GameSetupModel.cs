using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collection_Game_Tool.Services;
using System.Runtime.Serialization;
using Collection_Game_Tool.Main;

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
		private string _gsucID = null;
		public string gsucID { get { return _gsucID; } set { _gsucID = value; } }
        
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
				if ( inw )
				{
					if ( MainWindowModel.gameSetupModel.nearWins > PrizeLevels.PrizeLevels.numPrizeLevels )
					{
						gsucID = ErrorService.Instance.reportError( "007", new List<string> { }, gsucID );
					}
					else if ( MainWindowModel.gameSetupModel.nearWins <= PrizeLevels.PrizeLevels.numPrizeLevels )
					{
						ErrorService.Instance.resolveError( "007", gsucID );
					}
				}
				else
				{
					ErrorService.Instance.resolveError( "007", gsucID );
				}
				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "isNearWin" ) );
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
				if ( nw > PrizeLevels.PrizeLevels.numPrizeLevels )
				{
					gsucID = ErrorService.Instance.reportError( "007", new List<string> { }, gsucID );
				}
				else if ( nw <= PrizeLevels.PrizeLevels.numPrizeLevels )
				{
					ErrorService.Instance.resolveError( "007", gsucID );
				}
				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "nearWins" ) );
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
				if(ds)
				{
					if ( boardSize < MinimumBoardSize() )
					{
						gsucID = ErrorService.Instance.reportError( "014", new List<String> { }, gsucID );
					}
					else
					{
						MainWindowModel.verifyNumTiles();
						ErrorService.Instance.resolveWarning( "007", gsucID );
						ErrorService.Instance.resolveWarning( "008", gsucID );
						ErrorService.Instance.resolveError( "014", gsucID );
					}
				}
				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "diceSelected" ) );
            }
        }

		public bool SpinnerSelected 
		{ 
			get { return !diceSelected; }
			set
			{
				diceSelected = !value;
				if ( !diceSelected )
				{
					// single-value spinner
					if ( MainWindowModel.gameSetupModel.spinnerMaxValue == 1 )
					{
						gsucID = ErrorService.Instance.reportWarning( "007", new List<string> { }, gsucID );
						ErrorService.Instance.resolveWarning( "008", gsucID );
					}
					// "coin-flip" spinner
					else if ( MainWindowModel.gameSetupModel.spinnerMaxValue == 2 )
					{
						gsucID = ErrorService.Instance.reportWarning( "008", new List<string> { }, gsucID );
						ErrorService.Instance.resolveWarning( "007", gsucID );
					}
					else
					{
						ErrorService.Instance.resolveWarning( "007", gsucID );
						ErrorService.Instance.resolveWarning( "008", gsucID );
					}
					if ( boardSize < MinimumBoardSize() )
					{
						gsucID = ErrorService.Instance.reportError( "014", new List<String> { }, gsucID );
					}
					else
					{
						MainWindowModel.verifyNumTiles();
						ErrorService.Instance.resolveError( "014", gsucID );
					}
				}

				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "SpinnerSelected" ) );
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
				if ( boardSize < MinimumBoardSize() )
				{
					gsucID = ErrorService.Instance.reportError( "014", new List<String> { }, gsucID );
				}
				else
				{
					MainWindowModel.verifyNumTiles();
					MainWindowModel.verifyDivisions();
					ErrorService.Instance.resolveError( "014", gsucID );
				}
				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "numTurns" ) );
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
				//Insert error logging here
				if ( boardSize < MinimumBoardSize() )
				{
					gsucID = ErrorService.Instance.reportError( "014", new List<String> { }, gsucID );
				}
				else
				{
					ErrorService.Instance.resolveError( "014", gsucID );
					MainWindowModel.verifyNumTiles();
				}
				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "numDice" ) );
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

				// Warnings only, referring to a single value spinner or a coin-flip spinner
				if ( smv == 1 )
				{
					gsucID = ErrorService.Instance.reportWarning( "007", new List<string> { }, gsucID );
					ErrorService.Instance.resolveWarning( "008", gsucID );
				}
				else if ( smv == 2 )
				{
					gsucID = ErrorService.Instance.reportWarning( "008", new List<String> { }, gsucID );
					ErrorService.Instance.resolveWarning( "007", gsucID );
				}
				else
				{
					ErrorService.Instance.resolveWarning( "007", gsucID );
					ErrorService.Instance.resolveWarning( "008", gsucID );
				}
				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "spinnerMaxValue" ) );
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

		public int MinimumBoardSize()
		{
			if ( diceSelected )
			{
				return ( numDice ) * numTurns;
			}
			else
			{
				return 1 * numTurns;
			}
		}
    }
}
