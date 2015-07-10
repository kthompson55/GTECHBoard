using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Collection_Game_Tool.Services
{
    public class PrizeLevelConverter: IValueConverter
    {
        List<String> levels = new List<String>()
            {
                "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T"
            };

		/// <summary>
		/// Converts integer to letter reference
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object Convert( object value, Type targetType = null, object parameter = null, System.Globalization.CultureInfo culture = null )
        {
            int ret = 0;
            if (value is int)
            {
                ret = (int)value;
                return levels[ret - 1];
            }

            return "";
        }

		/// <summary>
		/// Converts letter reference back to integer
		/// </summary>
		/// <param name="value"></param>
		/// <param name="targetType"></param>
		/// <param name="parameter"></param>
		/// <param name="culture"></param>
		/// <returns></returns>
		public object ConvertBack( object value, Type targetType = null, object parameter = null, System.Globalization.CultureInfo culture  = null)
        {
            String text;
            if (value is string)
            {
                text = (string)value;

                int ret = levels.FindIndex(0, x => x == text);
                ret += 1;
                return ret;
            }

            return -1;
        }
	}
}
