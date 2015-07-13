using System;
using System.Globalization;
using System.Windows.Controls;

namespace Collection_Game_Tool.Services
{
	/// <summary>
	/// Validation rule for ranged controls
	/// </summary>
    public class RangeRule : ValidationRule
    {
		/// <summary>
		/// The minimum value.
		/// </summary>
		public int Min
		{
			get;
			set;
		}
		/// <summary>
		/// The maximum value.
		/// </summary>
		public int Max
		{
			get;
			set;
		}
		/// <summary>
		/// Validates a range control
		/// </summary>
		/// <param name="value">The value from the binding target to check.</param>
		/// <param name="cultureInfo">The culture to use in this rule.</param>
		/// <returns>A System.Windows.Controls.ValidationResult object.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int num = 0;

            try
            {
                if (((string)value).Length > 0)
                    num = int.Parse((String)value);
            }
            catch (Exception e)
            {
                e.GetBaseException(); //This is just so warnings don't appear anymore, yeah I'm lazy
                return new ValidationResult(false, "Illegal characters");
            }

            if ((num < Min) || (num > Max))
            {
                return new ValidationResult(false, "Please enter a number in the given range.");
            }
            else if (value.Equals(""))
            {
                return new ValidationResult(false, "Cannot be nothing");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
