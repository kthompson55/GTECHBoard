using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Collection_Game_Tool.Services
{
	/// <summary>
	/// Converts prize level int to letter
	/// </summary>
    public class PrizeLevelConverter: IValueConverter
    {
		/// <summary>
		/// The prize level letters to convert
		/// </summary>
        private static List<String> _levels = new List<String>()
            {
                "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t"
            };

		/// <summary>
		/// Converts integer to letter reference
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
		public object Convert( object value, Type targetType = null, object parameter = null, CultureInfo culture = null )
        {
            int ret = 0;
            if (value is int)
            {
                ret = (int)value;
                return _levels[ret];
            }

            return "";
        }

		/// <summary>
		/// Converts letter reference back to integer
		/// </summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
		public object ConvertBack( object value, Type targetType = null, object parameter = null, CultureInfo culture  = null)
        {
            string text;
            if (value is string)
            {
                text = (string)value;

                int ret = _levels.FindIndex(0, x => x == text);
                ret += 1;
                return ret;
            }

            return -1;
        }
	}
}
