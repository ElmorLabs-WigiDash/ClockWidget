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
    public partial class SettingsUserControl : UserControl
    {

        private ClockWidgetInstance parent;

        public SettingsUserControl(ClockWidgetInstance parent)
        {

            this.parent = parent;

            var assemblyName = GetType().Assembly.GetName();


            var asm = this.GetType().Assembly;
            string resName = asm.GetName().Name + ".g.resources";
            Stream stream = asm.GetManifestResourceStream(resName);
            ResourceReader reader = new ResourceReader(stream);
            List<string> Resources = new List<string>();
            foreach (DictionaryEntry x in reader)
            {
                Resources.Add(x.Key.ToString());
            }
            File.WriteAllLines("debug.txt", Resources.ToArray());
            
            /**
            Application.LoadComponent(
                GetType(),
                new Uri(
                    $"/{assemblyName.Name};v{assemblyName.Version};component/{GetType().Name}.xaml",
                    UriKind.Relative
                )
            );**/

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
