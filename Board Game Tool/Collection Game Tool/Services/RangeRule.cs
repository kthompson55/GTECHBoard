using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Collection_Game_Tool.Services
{
    public class RangeRule : ValidationRule
    {
        private int _min;
        private int _max;

        public int Min
        {
            get
            {
                return _min;
            }
            set
            {
                _min = value;
            }
        }

        public int Max
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
            }
        }

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
