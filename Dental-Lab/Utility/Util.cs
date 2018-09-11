using Dental_Lab.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dental_Lab.ViewModel.MainViewModel;

namespace Dental_Lab.Utility
{
    public static class Util
    {
        public static bool IsMenu(this string controlName, Menu menu)
        {
            return controlName.Equals(Enum.GetName(typeof(Menu), menu));
        }
    }
}
