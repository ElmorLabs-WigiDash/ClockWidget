using FrontierWidgetFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClockWidget {
    /// <summary>
    /// Interaction logic for SettingsUserControl.xaml
    /// </summary>
    public partial class SettingsUserControl : UserControl {

        private ClockWidgetInstance parent;

        public SettingsUserControl(ClockWidgetInstance parent) {

            this.parent = parent;

            InitializeComponent();
        }


        private void checkbox24h_Checked(object sender, RoutedEventArgs e) {
            parent.SetClock24h(true);
        }

        private void checkbox24h_Unchecked(object sender, RoutedEventArgs e) {
            parent.SetClock24h(false);
        }
    }
}
