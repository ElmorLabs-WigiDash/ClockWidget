using System.Windows;
using System.Windows.Controls;

namespace ClockWidget
{
    /// <summary>
    /// Interaction logic for SettingsUserControl.xaml
    /// </summary>
    public partial class SettingsUserControl : UserControl
    {

        private ClockWidgetInstance parent;

        public SettingsUserControl(ClockWidgetInstance parent)
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
