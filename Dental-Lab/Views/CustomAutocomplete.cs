using Syncfusion.Windows.Controls.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Dental_Lab.Views
{
    class CustomAutocomplete : SfTextBoxExt
    {
        public CustomAutocomplete()
        {
            var button = new Button() {
                Content = "Xóa",
                ToolTip = "Xóa",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            //button.Style= this.FindResource(MaterialDesignToolForegroundButton);
            button.Click += Button_Click; //Hooking up to event 

            DependencyObject parentObject = VisualTreeHelper.GetParent(this);

            if (parentObject != null)
            {
                (parentObject as Grid).Children.Add(button);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e) //Event which will be triggerd on click of ya button
        {
            throw new NotImplementedException();
        }
    }
}
