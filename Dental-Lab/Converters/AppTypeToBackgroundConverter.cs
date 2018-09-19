using Dental_Lab.Utility;
using Dental_Lab.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Dental_Lab.Converters
{
    public class AppTypeToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            AppointmentType type = value as AppointmentType;
            if (type == null)
            {
                return new SolidColorBrush(Color.FromArgb(255, 236, 12, 12));
            }
            switch (type.Name)
            {
                case "Hẹn Khám":
                    {
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("LightBlue"));
                    }
                case "Điều Trị":
                    {
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("LightBlue"));
                    }
                case "Trả Kết Quả":
                    {
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("LightBlue"));
                    }
                default:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("LightBlue"));
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
