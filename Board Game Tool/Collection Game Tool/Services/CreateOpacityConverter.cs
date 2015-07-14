using System;
using System.Globalization;
using System.Windows.Data;

namespace Collection_Game_Tool.Services
{
	/// <summary>
	/// The create opacity converter
	/// </summary>
    public class CreateOpacityConverter : IValueConverter
    {
		/// <summary>
		/// Converts from boolean to opacity value
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
			return value is bool && (bool)value ? 1.0 : 0.3;
        }

		/// <summary>
		/// Converts from Opacity value to boolean
		/// </summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
			return value is double && (double)value > 0.3;
        }
    }
}
