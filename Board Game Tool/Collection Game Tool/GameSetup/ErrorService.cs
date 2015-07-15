using Collection_Game_Tool.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Collection_Game_Tool.GameSetup
{
	/// <summary>
	/// The error service
	/// </summary>
    internal class ErrorService: INotifyPropertyChanged
    {
		/// <summary>
		/// The private instance.
		/// </summary>
		private static ErrorService _instance;
		/// <summary>
		/// The public instance.
		/// </summary>
		public static ErrorService Instance
		{
			get
			{
				if ( _instance == null )
				{
					_instance = new ErrorService();
				}
				return _instance;
			}
		}

		/// <summary>
		/// The current ID
		/// </summary>
        private static int _currentId = 0;

        /// <summary>
        /// This dictionary contains all of the possible error messages in template form. 
        /// If you need additional error templates, add them here.
        /// </summary>
        private Dictionary<string, string> _errorTemplates = new Dictionary<string, string>
        {
            {"001","-{0} dun goofed. Fix it.\n"},
            {"002","-{0} and {1} dun goofed. Fix it.\n"},
            {"004", "-Prize Level {0} currently has a higher collection than Game Setup picks allows. ({1})\n"},
            {"005", "-Prize Level {0} has illegal characters found in its collection text box!\n"},
            {"006", "-Prize Level {0}'s collection text box is out of range! ({1}-{2})\n"},
            {"007", "-Number of near win prizes is greater than the amount of Prize Levels.\n"},
            {"008", "-Prize Level {0}'s collection text box cannot be empty.\n"},
            {"009", "-Division {0} is not a unique Division.\n"},
            {"010", "-The collections field in Division {0} needs to be less than or equal to the set player turns in the Game Setup.\n"},
            {"011", "-The collection in Division {0} is invalid, the possible collection must be higher so that the division cannot win other prizes.\n"},
            {"012", "-With the current setup the player cannot lose. Either decrease the amount of player picks, or increase the amount of collections one of the prize levels has."},
            {"013", "-The Board Size is too small for the current game setup."},
            {"014", "-The Board does not have enough space to accomodate the current setup. You must reduce the amount of special tiles, increase the total number of tiles, decrease the number of turns, or decrease the number of dice/spins."},
        };

        /// <summary>
        /// This dictionary contains all of the possible warning messages in template form. 
        /// If you need additional warning templates, add them here.
        /// </summary>
        private Dictionary<string, string> _warningTemplates = new Dictionary<string, string>
        {
            {"001","-{0} has no prize levels.\n"},
            {"002","-{0} is empty.\n"},
            {"003","-{0} is identical to {1}.\n"},
            {"004", "-Prize Level {0} and Prize Level {1} are the same.\n"},
            {"005", "-Division {0} has no selected prize levels.\n"},
            {"006", "-There are no divisions in this project.\n"},
            {"007", "-A spinner size of 1 will offer no variation in movement."},
            {"008", "-A spinner size of 2 will be equivalent to a coin flip."},
            {"009", "-It is possible to move beyond the board with the current movement setup."},
        };

        /// <summary>
        /// The Dictionary of unresolved errors. 
        /// Key is an Error object. 
        /// Value is the error message which will be displayed to the user.
        /// </summary>
        private Dictionary<Error,string> _unresolvedErrors = new Dictionary<Error, string>();
		/// <summary>
		/// The private error text.
		/// </summary>
        private string _errorText;
		/// <summary>
		/// The public error text.
		/// </summary>
        public string ErrorText
        {
            get
            {
                return _errorText;
            }
            set
            {
                _errorText = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ErrorText"));
                }
            }
        }

        /// <summary>
        /// Constructs a unique error message from the provided illegalObjects strings and an error template
        /// and adds a new error to the dictionary of unresolved errors. 
        /// </summary>
        /// <param name="errorCode">The error code which indicates what error template to use</param>
        /// <param name="illegalObjects">The names of all of the objects to be plugged into the error template</param>
        /// <param name="senderId">The Id of the object reporting the error</param>
        /// <returns>The id of the object reporting the error. Creates a new id if one is not provided.</returns>
        public string ReportError(string errorCode, List<string> illegalObjects, string senderId)
        {
            if(senderId == null) senderId = _currentId++ + "";
            string theErrorMessage = String.Format(_errorTemplates[errorCode], illegalObjects.ToArray());
            Error theError = new Error(senderId, errorCode);
            if (_unresolvedErrors.ContainsKey(theError))
            {
                _unresolvedErrors.Remove(theError);
            }
            _unresolvedErrors.Add(theError, theErrorMessage);
            UpdateErrorText();
            
            return senderId;
        }

        /// <summary>
        /// Removes an error from the dictionary of unresolved errors.
        /// </summary>
        /// <param name="errorCode">The error code which the error to remove contains</param>
        /// <param name="senderId">The sender Id which the error to remove contains</param>
        public void ResolveError(string errorCode, string senderId)
        {
            Error theError = new Error(senderId, errorCode);
            if (_unresolvedErrors.ContainsKey(theError))
            {
                _unresolvedErrors.Remove(theError);
                UpdateErrorText();
            }

        }

        /// <summary>
        /// Uses the errors contained in _unresolvedErrors to update the text in the error box.
        /// </summary>
        private void UpdateErrorText()
        {
			StringBuilder updatedErrorText = new StringBuilder();
            foreach(KeyValuePair<Error,string> entry in _unresolvedErrors)
            {
				updatedErrorText.AppendLine( entry.Value );
            }
            ErrorText = updatedErrorText.ToString();
        }

        /// <summary>
        /// The Dictionary of unresolved warnings. 
        /// Key is a warning object. 
        /// Value is the warning message which will be displayed to the user.
        /// </summary>
        private Dictionary<Warning, string> _unresolvedWarnings = new Dictionary<Warning, string>();
		/// <summary>
		/// The private warning text
		/// </summary>
        private string _warningText;
		/// <summary>
		/// The public warning text
		/// </summary>
        public string WarningText
        {
            get
            {
                return _warningText;
            }
            set
            {
                _warningText = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("WarningText"));
                }
            }
        }

        /// <summary>
        /// Constructs a unique warning message from the provided illegalObjects strings and a warning template
        /// and adds a new warning to the dictionary of unresolved warnings. 
        /// </summary>
        /// <param name="warningCode">The warning code which indicates what warning template to use</param>
        /// <param name="illegalObjects">The names of all of the objects to be plugged into the warning template</param>
        /// <param name="senderId">The Id of the object reporting the warning</param>
        /// <returns>The id of the object reporting the warning. Creates a new id if one is not provided.</returns>
        public string ReportWarning(string warningCode, List<string> illegalObjects, string senderId)
        {
            if (senderId == null) senderId = _currentId++ + "";
            string theWarningMessage = String.Format(_warningTemplates[warningCode], illegalObjects.ToArray());
            Warning theWarning = new Warning(senderId, warningCode);
            if (_unresolvedWarnings.ContainsKey(theWarning))
            {
                _unresolvedWarnings.Remove(theWarning);
            }
            _unresolvedWarnings.Add(theWarning, theWarningMessage);
            updateWarningText();
            return senderId;
        }

        /// <summary>
        /// Removes a warning from the dictionary of unresolved warnings.
        /// </summary>
        /// <param name="warningCode">The warning code which the warning to remove contains</param>
        /// <param name="senderId">The sender Id which the warning to remove contains</param>
        public void ResolveWarning(string warningCode, string senderId)
        {
            Warning theWarning = new Warning(senderId, warningCode);
            //string theWarningMessage = String.Format(_warningTemplates[warningCode], illegalObjects);
            if (_unresolvedWarnings.ContainsKey(theWarning))
            {
                _unresolvedWarnings.Remove(theWarning);
                updateWarningText();
            }

        }

        /// <summary>
        /// Uses the warnings contained in _unresolvedWarnings to update the text in the error box.
        /// </summary>
        private void updateWarningText()
        {
            StringBuilder updatedWarningText = new StringBuilder();
            foreach (KeyValuePair<Warning, string> entry in _unresolvedWarnings)
            {
                updatedWarningText.AppendLine(entry.Value);
            }
            WarningText = updatedWarningText.ToString();
        }
		/// <summary>
		/// Notifies listeners that a property has changed
		/// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// Determines if there are any errors with the current board setup
        /// </summary>
        /// <returns>Returns true if there are one or more errors</returns>
        public bool HasErrors()
        {
            return _unresolvedErrors.Count > 0;
        }

        /// <summary>
        /// Determines if there are any warnings with the current board setup
        /// </summary>
        /// <returns>Returns true if there are one or more warnings</returns>
        public bool HasWarnings()
        {
            return _unresolvedWarnings.Count > 0;
        }

        /// <summary>
        /// Removes all errors from the current board setup
        /// -Should only be used when opening a separate project-
        /// </summary>
        public void ClearErrors()
        {
            _unresolvedErrors.Clear();
        }

        /// <summary>
        /// Removes all warnings from the current board setup
        /// -Should only be used when opening a separate project-
        /// </summary>
        public void ClearWarnings()
        {
            _unresolvedWarnings.Clear();
        }
    }
}
