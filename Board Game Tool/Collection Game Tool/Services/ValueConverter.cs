using System;
using System.Globalization;
using System.Windows.Data;

namespace Collection_Game_Tool.Services
{
	/// <summary>
	/// Double to string converter
	/// </summary>
    public class ValueConverter : IValueConverter
    {
		/// <summary>
		/// Converts double to String
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double ret = 0;
            if (value is double)
            {
                ret = (double)value;
            }

            return ret.ToString();
        }

		/// <summary>
		/// Converts String to double
		/// </summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = (string)value;
            double ret=0;
            if (double.TryParse(text, out ret))
            {
                return ret;
            }

            return ret;
        }
    }
}
