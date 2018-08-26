using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental_Lab.ViewModel
{
    class ScheduleViewModel: BaseViewModel
    {
        private string _scheduleType;
        public string ScheduleType
        {
            get => this._scheduleType;
            set => SetProperty(ref this._scheduleType, value);
        }

        public ScheduleViewModel()
        {
            ScheduleType = "Week";
            //System.Console.WriteLine("ScheduleType" + ScheduleType);
        }
    }
}
