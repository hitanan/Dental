using Dental_Lab.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Dental_Lab.Converters
{
    public class OneInTwoConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { 

            string string1 = null;
            object object1 = values[0];
            if (object1 is Client)
            {
                if (object1 != null)
                {
                    string1 = (object1 as Client).Name;
                }
            } else {
                string1 = object1 as string;
            }
            var result = String.IsNullOrEmpty(string1) ? values[1] as string : string1;
            if (!String.IsNullOrEmpty(result) && "UPPER".Equals(parameter))
            {
                result = result.ToUpper();
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
