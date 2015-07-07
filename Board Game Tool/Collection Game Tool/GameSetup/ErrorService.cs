using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Collection_Game_Tool.Services;

namespace Collection_Game_Tool.GameSetup
{
    public class ErrorService: INotifyPropertyChanged
    {
        private static ErrorService instance;
        
        private ErrorService() { }

        private static int currentId = 0;

        public static ErrorService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ErrorService();
                }
                return instance;
            }
        }

        /// <summary>
        /// This dictionary contains all of the possible error messages in template form. 
        /// If you need additional error templates, add them here.
        /// </summary>
        private Dictionary<string, string> errorTemplates = new Dictionary<string, string>
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
            {"014", "-The Board does not have enough space to accomodate the current setup. You must reduce the amount of special tiles, increase the total number of tiles, decrease the number of turns, or decrease the number of dice/spins."}
        };

        /// <summary>
        /// This dictionary contains all of the possible warning messages in template form. 
        /// If you need additional warning templates, add them here.
        /// </summary>
        private Dictionary<string, string> warningTemplates = new Dictionary<string, string>
        {
            {"001","-{0} has no prize levels.\n"},
            {"002","-{0} is empty.\n"},
            {"003","-{0} is identical to {1}.\n"},
            {"004", "-Prize Level {0} and Prize Level {1} are the same.\n"},
            {"005", "-Division {0} has no selected prize levels.\n"},
            {"006", "-There are no divisions in this project.\n"},
            {"007", "-A spinner size of 1 will offer no variation in movement."},
            {"008", "-A spinner size of 2 will be equivalent to a coin flip."},
        };

        /// <summary>
        /// The Dictionary of unresolved errors. 
        /// Key is an Error object. 
        /// Value is the error message which will be displayed to the user.
        /// </summary>
        private Dictionary<Error,string> unresolvedErrors = new Dictionary<Error, string>();
        private string _errorText;
        public string errorText
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
                    PropertyChanged(this, new PropertyChangedEventArgs("errorText"));
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
        public string reportError(string errorCode, List<string> illegalObjects, string senderId)
        {
            if(senderId == null) senderId = currentId++ + "";
            string theErrorMessage = String.Format(errorTemplates[errorCode], illegalObjects.ToArray());
            Error theError = new Error(senderId, errorCode);
            if (unresolvedErrors.ContainsKey(theError))
            {
                unresolvedErrors.Remove(theError);
            }
            unresolvedErrors.Add(theError, theErrorMessage);
            updateErrorText();
            
            return senderId;
        }

        /// <summary>
        /// Removes an error from the dictionary of unresolved errors.
        /// </summary>
        /// <param name="errorCode">The error code which the error to remove contains</param>
        /// <param name="senderId">The sender Id which the error to remove contains</param>
        public void resolveError(string errorCode, string senderId)
        {
            Error theError = new Error(senderId, errorCode);
            if (unresolvedErrors.ContainsKey(theError))
            {
                unresolvedErrors.Remove(theError);
                updateErrorText();
            }

        }

        /// <summary>
        /// Uses the errors contained in unresolvedErrors to update the text in the error box.
        /// </summary>
        private void updateErrorText()
        {
            string updatedErrorText = "";
            foreach(KeyValuePair<Error,string> entry in unresolvedErrors)
            {
                updatedErrorText += entry.Value;
                updatedErrorText += System.Environment.NewLine;
            }
            errorText = updatedErrorText;
        }

        /// <summary>
        /// The Dictionary of unresolved warnings. 
        /// Key is a warning object. 
        /// Value is the warning message which will be displayed to the user.
        /// </summary>
        private Dictionary<Warning, string> unresolvedWarnings = new Dictionary<Warning, string>();
        private string _warningText;
        public string warningText
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
                    PropertyChanged(this, new PropertyChangedEventArgs("warningText"));
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
        public string reportWarning(string warningCode, List<string> illegalObjects, string senderId)
        {
            if (senderId == null) senderId = currentId++ + "";
            string theWarningMessage = String.Format(warningTemplates[warningCode], illegalObjects.ToArray());
            Warning theWarning = new Warning(senderId, warningCode);
            if (unresolvedWarnings.ContainsKey(theWarning))
            {
                unresolvedWarnings.Remove(theWarning);
            }
            unresolvedWarnings.Add(theWarning, theWarningMessage);
            updateWarningText();
            return senderId;
        }

        /// <summary>
        /// Removes a warning from the dictionary of unresolved warnings.
        /// </summary>
        /// <param name="warningCode">The warning code which the warning to remove contains</param>
        /// <param name="senderId">The sender Id which the warning to remove contains</param>
        public void resolveWarning(string warningCode, string senderId)
        {
            Warning theWarning = new Warning(senderId, warningCode);
            //string theWarningMessage = String.Format(warningTemplates[warningCode], illegalObjects);
            if (unresolvedWarnings.ContainsKey(theWarning))
            {
                unresolvedWarnings.Remove(theWarning);
                updateWarningText();
            }

        }

        /// <summary>
        /// Uses the warnings contained in unresolvedWarnings to update the text in the error box.
        /// </summary>
        private void updateWarningText()
        {
            string updatedWarningText = "";
            foreach (KeyValuePair<Warning, string> entry in unresolvedWarnings)
            {
                updatedWarningText += entry.Value;
                updatedWarningText += System.Environment.NewLine;
            }
            warningText = updatedWarningText;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
