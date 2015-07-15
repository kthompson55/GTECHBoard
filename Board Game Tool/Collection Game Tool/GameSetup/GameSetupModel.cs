using Collection_Game_Tool.Main;
using Collection_Game_Tool.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Collection_Game_Tool.GameSetup
{
	/// <summary>
	/// The game setup model
	/// </summary>
    [Serializable]
    public class GameSetupModel : INotifyPropertyChanged, Teller
    {
        /// <summary>
        /// Event handler for fields being changed
        /// </summary>
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerializedAttribute()]
        private List<Listener> _audience = new List<Listener>();

		/// <summary>
		/// Initialize the listener
		/// </summary>
        public void InitializeListener()
        {
            _audience = new List<Listener>();
        }
		private string _gsucID = null;
        /// <summary>
        /// The ID of the model for the ErrorService
        /// </summary>
		public string gsucID { get { return _gsucID; } set { _gsucID = value; } }
        
        private bool _isNearWin;
		/// <summary>
		/// Is near win bool
		/// </summary>
        public bool IsNearWin 
        {
            get
            {
                return _isNearWin;
            }
            set
            {
                _isNearWin = value;
				if ( _isNearWin )
				{
					if ( NearWins > PrizeLevels.PrizeLevels.numPrizeLevels )
					{
						gsucID = ErrorService.Instance.ReportError( "007", new List<string> { }, gsucID );
					}
					else if ( NearWins <= PrizeLevels.PrizeLevels.numPrizeLevels )
					{
						ErrorService.Instance.ResolveError( "007", gsucID );
					}
				}
				else
				{
					ErrorService.Instance.ResolveError( "007", gsucID );
				}
				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "IsNearWin" ) );
            }
        }

        private short _nearWins = 1;
		/// <summary>
		/// The near win amount
		/// </summary>
        public short NearWins 
        {
            get
            {
                return _nearWins;
            }
            set
            {
                _nearWins = value;
				if ( _nearWins > PrizeLevels.PrizeLevels.numPrizeLevels )
				{
					gsucID = ErrorService.Instance.ReportError( "007", new List<string> { }, gsucID );
				}
				else if ( _nearWins <= PrizeLevels.PrizeLevels.numPrizeLevels )
				{
					ErrorService.Instance.ResolveError( "007", gsucID );
				}
				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "NearWins" ) );
            }
        } //Max 12


        private uint _maxPermutations = 1;
		/// <summary>
		/// The max permutations
		/// </summary>
        public uint MaxPermutations 
        {
            get
            {
                return _maxPermutations;
            }
            set
            {
                _maxPermutations = value;
            }
        }

		/// <summary>
		/// Set and get max permutations by string
		/// </summary>
		public string MaxPermutationsTextbox
		{
			get
			{
				return MaxPermutations.ToString();
			}
			set
			{
				if ( value == "" )
				{
					MaxPermutations = 1;
				}
				else if ( WithinPermutationRange( value ) )
				{
					MaxPermutations = Convert.ToUInt32( value );
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
		/// <summary>
		/// Boolean if can create
		/// </summary>
        public bool CanCreate
        {
            get
            {
                return _canCreate;
            }
            set
            {
                _canCreate = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CanCreate"));
            }
        }

        private bool _diceSelected = true;
		/// <summary>
		/// Boolean if dice is selected
		/// </summary>
        public bool DiceSelected
        {
            get
            {
                return _diceSelected;
            }
            set
            {
                _diceSelected = value;
				if(_diceSelected)
				{
					if ( BoardSize < MinimumBoardSize() )
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
					PropertyChanged( this, new PropertyChangedEventArgs( "DiceSelected" ) );
            }
        }

		/// <summary>
		/// Boolean if spinner is selected. Uses the DiceSelected value for the bool.
		/// </summary>
		public bool SpinnerSelected 
		{ 
			get { return !DiceSelected; }
			set
			{
				DiceSelected = !value;
				if ( !DiceSelected )
				{
					// single-value spinner
					if ( SpinnerMaxValue == 1 )
					{
						gsucID = ErrorService.Instance.ReportWarning( "007", new List<string> { }, gsucID );
						ErrorService.Instance.ResolveWarning( "008", gsucID );
					}
					// "coin-flip" spinner
					else if ( SpinnerMaxValue == 2 )
					{
						gsucID = ErrorService.Instance.ReportWarning( "008", new List<string> { }, gsucID );
						ErrorService.Instance.ResolveWarning( "007", gsucID );
					}
					else
					{
						ErrorService.Instance.ResolveWarning( "007", gsucID );
						ErrorService.Instance.ResolveWarning( "008", gsucID );
					}
					if ( BoardSize < MinimumBoardSize() )
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

        private int _numTurns = 1;

		/// <summary>
		/// Number of turns
		/// </summary>
        public int NumTurns
        {
            get
            {
                return _numTurns;
            }
			set
			{
				_numTurns = value;
				if ( BoardSize < MinimumBoardSize() )
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
					PropertyChanged( this, new PropertyChangedEventArgs( "NumTurns" ) );
			}
        }

        private int _numDice = 1;
		/// <summary>
		/// The number of dice
		/// </summary>
        public int NumDice
        {
            get
            {
                return _numDice;
            }
            set
            {
                _numDice = value;

				if ( BoardSize < MinimumBoardSize() )
				{
					gsucID = ErrorService.Instance.ReportError( "014", new List<String> { }, gsucID );
				}
				else
				{
					ErrorService.Instance.ResolveError( "014", gsucID );
					MainWindowModel.Instance.VerifyNumTiles();
				}
				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "NumDice" ) );
            }
        }

        private int _spinnerMaxValue = 1;
		/// <summary>
		/// The max value of the spinner
		/// </summary>
        public int SpinnerMaxValue
        {
            get
            {
                return _spinnerMaxValue;
            }
            set
            {
                _spinnerMaxValue = value;

				// Warnings only, referring to a single value spinner or a coin-flip spinner
				if ( _spinnerMaxValue == 1 )
				{
					gsucID = ErrorService.Instance.ReportWarning( "007", new List<string> { }, gsucID );
					ErrorService.Instance.ResolveWarning( "008", gsucID );
				}
				else if ( _spinnerMaxValue == 2 )
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
					PropertyChanged( this, new PropertyChangedEventArgs( "SpinnerMaxValue" ) );
            }
        }

        private int _boardSize = 1;
		/// <summary>
		/// The board size
		/// </summary>
        public int BoardSize
        {
            get
            {
                return _boardSize;
            }
            set
            {
                _boardSize = value;
            }
        }
		/// <summary>
		/// The string version of board size
		/// </summary>
		public string BoardSizeTextBox
		{
			get
			{
				return _boardSize.ToString();
			}
			set
			{
				if ( value == "" )
				{
					BoardSize = 0;
				}
				else if ( WithinViableBoardSizeRange( value ) )
				{
					int boardSizeTest;
					if ( Int32.TryParse( value, out boardSizeTest ) && boardSizeTest >= 0 )
					{
						BoardSize = boardSizeTest;
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
			bool successful = int.TryParse( s, out boardSizeValue );
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

        private int _numMoveForwardTiles = 1;
		/// <summary>
		/// Number of move forward tiles
		/// </summary>
        public int NumMoveForwardTiles
        {
            get
            {
                return _numMoveForwardTiles;
            }
            set
            {
                _numMoveForwardTiles = value;
            }
        }

        private int _numMoveBackwardTiles = 1;
		/// <summary>
		/// Number of move backward tiles
		/// </summary>
        public int NumMoveBackwardTiles
        {
            get
            {
                return _numMoveBackwardTiles;
            }
            set
            {
                _numMoveBackwardTiles = value;
            }
        }

		/// <summary>
		/// String version of NumMoveBackwardTiles
		/// </summary>
		public string NumMoveBackwardTilesTextbox
		{
			get
			{
				return NumMoveBackwardTiles + "";
			}
			set
			{
				int numMBValue;
				if ( value == "" )
				{
					NumMoveBackwardTiles = 0;
				}
				else if ( Int32.TryParse( value, out numMBValue ) && numMBValue >= 0 )
				{
					// confirm that there is enough space on board for desired number of move-backward tiles
					NumMoveBackwardTiles = numMBValue;
					MainWindowModel.Instance.VerifyNumTiles();
				}
				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "NumMoveBackwardTilesTextbox" ) );
			}
		}
		/// <summary>
		/// String version of NumMoveForwardTiles
		/// </summary>
		public string NumMoveForwardTilesTextbox
		{
			get
			{
				return NumMoveForwardTiles + "";
			}
			set
			{
				int numMFValue;
				if ( value == "" )
				{
					NumMoveForwardTiles = 0;
				}
				else if ( Int32.TryParse( value, out numMFValue ) && numMFValue >= 0 )
				{
					// confirm that there is enough space on board for desired number of move-backward tiles
					NumMoveForwardTiles = numMFValue;
					MainWindowModel.Instance.VerifyNumTiles();
				}

				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "NumMoveForwardTilesTextbox" ) );
			}
		}

        private int _moveForwardLength = 0;
		/// <summary>
		/// Steps for move forward
		/// </summary>
        public int MoveForwardLength
        {
            get
            {
                return _moveForwardLength;
            }
            set
            {
                _moveForwardLength = value;
				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "MoveForwardLength" ) );
            }
        }

        private int _moveBackwardLength = 0;
		/// <summary>
		/// Steps for move backward
		/// </summary>
        public int MoveBackwardLength
        {
            get
            {
                return _moveBackwardLength;
            }
            set
            {
                _moveBackwardLength = value;
				if ( PropertyChanged != null )
					PropertyChanged( this, new PropertyChangedEventArgs( "MoveBackwardLength" ) );
            }
        }

		/// <summary>
		/// The number of initial reachable spaces
		/// </summary>
        public int InitialReachableSpaces
        {
            get
            {
                if (DiceSelected)
                {
                    return (NumDice * 6) * NumTurns;
                }
                else
                {
                    return SpinnerMaxValue * NumTurns;
                }
            }
        }

		/// <summary>
		/// The number of initial reachable spaces
		/// </summary>
        public int FinalReachableSpaces
        {
            get
            {
                return InitialReachableSpaces + (NumMoveForwardTiles * MoveForwardLength);
            }
        }

		/// <summary>
		/// Shouts to the listeners/audience.
		/// </summary>
		/// <param name="pass">The object to pass</param>
        public void Shout(object pass)
        {
            foreach (Listener fans in _audience)
            {
                fans.OnListen(pass);
            }
        }
		/// <summary>
		/// Adds a listener
		/// </summary>
		/// <param name="list">The listener to add</param>
        public void AddListener(Listener list)
        {
            _audience.Add(list);
        }

		/// <summary>
		/// The minimum board size
		/// </summary>
		/// <returns>The minimum board size</returns>
		public int MinimumBoardSize()
		{
			if ( DiceSelected )
			{
				return ( NumDice ) * NumTurns;
			}
			else
			{
				return 1 * NumTurns;
			}
		}
    }
}
