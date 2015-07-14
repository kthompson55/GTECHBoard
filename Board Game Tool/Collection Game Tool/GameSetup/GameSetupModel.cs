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
    public class GameSetupModel : INotifyPropertyChanged, Teller
    {
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerializedAttribute()]
        List<Listener> audience = new List<Listener>();

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
					if ( nearWins > PrizeLevels.PrizeLevels.numPrizeLevels )
					{
						gsucID = ErrorService.Instance.ReportError( "007", new List<string> { }, gsucID );
					}
					else if ( nearWins <= PrizeLevels.PrizeLevels.numPrizeLevels )
					{
						ErrorService.Instance.ResolveError( "007", gsucID );
					}
				}
				else
				{
					ErrorService.Instance.ResolveError( "007", gsucID );
				}
				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "isNearWin" ) );
            }
        }

        private short nw = 1;
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
					gsucID = ErrorService.Instance.ReportError( "007", new List<string> { }, gsucID );
				}
				else if ( nw <= PrizeLevels.PrizeLevels.numPrizeLevels )
				{
					ErrorService.Instance.ResolveError( "007", gsucID );
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

		public string MaxPermutationsTextbox
		{
			get
			{
				return maxPermutations.ToString();
			}
			set
			{
				if ( value == "" )
				{
					maxPermutations = 1;
				}
				else if ( WithinPermutationRange( value ) )
				{
					maxPermutations = Convert.ToUInt32( value );
				}
				Shout( "validate" );
				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "MaxPermutationsTextbox" ) );
			}
		}

		/// <summary>
		/// Checks that the value entered in Max Permutations is acceptable
		/// </summary>
		/// <param name="s">The input from the Max Permutations Textbox</param>
		/// <returns>whether the value is in the acceptable range</returns>
		private bool WithinPermutationRange( string s )
		{
			uint philTheOrphan;
			UInt32.TryParse( s, out philTheOrphan );
			return philTheOrphan < 100000 && philTheOrphan > 0;
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
						gsucID = ErrorService.Instance.ReportError( "014", new List<String> { }, gsucID );
					}
					else
					{
						MainWindowModel.Instance.VerifyNumTiles();
						ErrorService.Instance.ResolveWarning( "007", gsucID );
						ErrorService.Instance.ResolveWarning( "008", gsucID );
						ErrorService.Instance.ResolveError( "014", gsucID );
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
					if ( spinnerMaxValue == 1 )
					{
						gsucID = ErrorService.Instance.ReportWarning( "007", new List<string> { }, gsucID );
						ErrorService.Instance.ResolveWarning( "008", gsucID );
					}
					// "coin-flip" spinner
					else if ( spinnerMaxValue == 2 )
					{
						gsucID = ErrorService.Instance.ReportWarning( "008", new List<string> { }, gsucID );
						ErrorService.Instance.ResolveWarning( "007", gsucID );
					}
					else
					{
						ErrorService.Instance.ResolveWarning( "007", gsucID );
						ErrorService.Instance.ResolveWarning( "008", gsucID );
					}
					if ( boardSize < MinimumBoardSize() )
					{
						gsucID = ErrorService.Instance.ReportError( "014", new List<String> { }, gsucID );
					}
					else
					{
						MainWindowModel.Instance.VerifyNumTiles();
						ErrorService.Instance.ResolveError( "014", gsucID );
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
					gsucID = ErrorService.Instance.ReportError( "014", new List<String> { }, gsucID );
				}
				else
				{
					MainWindowModel.Instance.VerifyNumTiles();
					MainWindowModel.Instance.VerifyDivisions();
					ErrorService.Instance.ResolveError( "014", gsucID );
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
					gsucID = ErrorService.Instance.ReportError( "014", new List<String> { }, gsucID );
				}
				else
				{
					ErrorService.Instance.ResolveError( "014", gsucID );
					MainWindowModel.Instance.VerifyNumTiles();
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
					gsucID = ErrorService.Instance.ReportWarning( "007", new List<string> { }, gsucID );
					ErrorService.Instance.ResolveWarning( "008", gsucID );
				}
				else if ( smv == 2 )
				{
					gsucID = ErrorService.Instance.ReportWarning( "008", new List<String> { }, gsucID );
					ErrorService.Instance.ResolveWarning( "007", gsucID );
				}
				else
				{
					ErrorService.Instance.ResolveWarning( "007", gsucID );
					ErrorService.Instance.ResolveWarning( "008", gsucID );
				}
                MainWindowModel.Instance.VerifyNumTiles();
				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "spinnerMaxValue" ) );
            }
        }

        private int bs = 1;
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

		public string BoardSizeTextBox
		{
			get
			{
				return bs.ToString();
			}
			set
			{
				if ( value == "" )
				{
					boardSize = 0;
				}
				else if ( WithinViableBoardSizeRange( value ) )
				{
					int boardSizeTest;
					if ( Int32.TryParse( value, out boardSizeTest ) && boardSizeTest >= 0 )
					{
						boardSize = boardSizeTest;
					}
					MainWindowModel.Instance.VerifyNumTiles();
				}
				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "BoardSizeTextBox" ) );
			}
		}

		private bool WithinViableBoardSizeRange( string s )
		{
			int boardSizeValue;
			bool successful = Int32.TryParse( s, out boardSizeValue );
			if ( successful )
			{
				if ( boardSizeValue < MinimumBoardSize() )
				{
					gsucID = ErrorService.Instance.ReportError( "014", new List<String> { }, gsucID );
				}
				else
				{
					ErrorService.Instance.ResolveError( "014", gsucID );
				}
			}
			return successful;
		}

        private int nmft = 1;
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

        private int nmbt = 1;
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

		public string NumMoveBackwardTilesTextbox
		{
			get
			{
				return numMoveBackwardTiles + "";
			}
			set
			{
				int numMBValue;
				if ( value == "" )
				{
					numMoveBackwardTiles = 0;
				}
				else if ( Int32.TryParse( value, out numMBValue ) && numMBValue >= 0 )
				{
					// confirm that there is enough space on board for desired number of move-backward tiles
					numMoveBackwardTiles = numMBValue;
					MainWindowModel.Instance.VerifyNumTiles();
				}
				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "NumMoveBackwardTilesTextbox" ) );
			}
		}

		public string NumMoveForwardTilesTextbox
		{
			get
			{
				return numMoveForwardTiles + "";
			}
			set
			{
				int numMFValue;
				if ( value == "" )
				{
					numMoveForwardTiles = 0;
				}
				else if ( Int32.TryParse( value, out numMFValue ) && numMFValue >= 0 )
				{
					// confirm that there is enough space on board for desired number of move-backward tiles
					numMoveForwardTiles = numMFValue;
					MainWindowModel.Instance.VerifyNumTiles();
				}

				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "NumMoveForwardTilesTextbox" ) );
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
				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "moveForwardLength" ) );
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

        public void Shout(object pass)
        {
            foreach (Listener fans in audience)
            {
                fans.OnListen(pass);
            }
        }

        public void AddListener(Listener list)
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
