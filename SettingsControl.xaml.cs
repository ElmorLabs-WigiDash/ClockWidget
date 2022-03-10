using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace ClockWidget
{
    /// <summary>
    /// Interaction logic for SettingsUserControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {

        private ClockWidgetInstance parent;

        public SettingsControl(ClockWidgetInstance parent)
        {

            this.parent = parent;

            InitializeComponent();
        }


        private void checkbox24h_Checked(object sender, RoutedEventArgs e)
        {
            parent.SetClock24h(true);
        }

        private void checkbox24h_Unchecked(object sender, RoutedEventArgs e)
        {
            parent.SetClock24h(false);
        }
    }
}
