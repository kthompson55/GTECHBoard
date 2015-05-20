using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Collection_Game_Tool.Services
{
    public class ValueConverter : IValueConverter
    {
        //Converts double to String
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double ret = 0;
            if (value is double)
            {
                ret = (double)value;
            }

            return ret.ToString();
        }

        //Converts String to double
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String text = (string)value;
            double ret=0;
            if (double.TryParse(text, out ret))
            {
                return ret;
            }

            return ret;
        }
    }
}
