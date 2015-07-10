using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Collection_Game_Tool.Services
{
    public class PrizeLevelConverter
    {
        List<String> levels = new List<String>()
            {
                "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T"
            };

        //Converts integer to letter reference
        public object Convert(object value)
        {
            int ret = 0;
            if (value is int)
            {
                ret = (int)value;
                return levels[ret - 1];
            }

            return "";
        }

        //Converts letter reference back to integer
        public object ConvertBack(object value)
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
