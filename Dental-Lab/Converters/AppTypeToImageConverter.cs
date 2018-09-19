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

namespace Dental_Lab.Converters
{
    public class AppTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            AppointmentType type = value as AppointmentType;
            if (type == null)
            {
                return null;
            }
            switch (type.Name)
            {
                case "Hẹn Khám":
                    {
                        return new BitmapImage(new Uri("pack://application:,,,/Assets/Cake.png"));
                    }
                case "Điều Trị":
                    {
                        return new BitmapImage(new Uri("pack://application:,,,/Assets/Hospital.png"));
                    }
                case "Trả Kết Quả":
                    {
                        return new BitmapImage(new Uri("pack://application:,,,/Assets/Team.png"));
                    }
                default:
                    return new BitmapImage(new Uri("pack://application:,,,/Assets/Cake.png"));
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
