using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Paint
{
    public class ActivableButton : Button
    {
        public static readonly DependencyProperty IsActivedProperty = DependencyProperty.Register(
            "IsActived", typeof(bool), typeof(ActivableButton), new PropertyMetadata(default(bool)));

        public bool IsActived
        {
            get { return (bool)GetValue(IsActivedProperty); }
            set { SetValue(IsActivedProperty, value); }
        }
    }
}
