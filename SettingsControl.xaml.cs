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
using System.Drawing;

namespace ClockWidget
{
    /// <summary>
    /// Interaction logic for SettingsUserControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {

        private ClockWidgetInstance parent;
        private int lastValidIntInput;

        public SettingsControl(ClockWidgetInstance parent)
        {

            this.parent = parent;

            InitializeComponent();

            useGlobalChk.IsChecked = parent.UseGlobal;
            checkbox24h.IsChecked = parent.time_24h;

            dateFontSelect.Tag = parent.UserFontDate;
            dateFontSelect.Content = new FontConverter().ConvertToInvariantString(parent.UserFontDate);
            dateFontSelect.IsEnabled = !parent.UseGlobal;

            timeFontSelect.Tag = parent.UserFontTime;
            timeFontSelect.Content = new FontConverter().ConvertToInvariantString(parent.UserFontTime);
            timeFontSelect.IsEnabled = !parent.UseGlobal;

            bgColorSelect.Content = ColorTranslator.ToHtml(parent.UserBackColor);
            bgColorSelect.IsEnabled = !parent.UseGlobal;
        }

        private void dateFontSelect_Click(object sender, RoutedEventArgs e)
        {
            Font defaultFont = parent.UserFontDate;
            Font selectedFont = parent.WidgetObject.WidgetManager.RequestFontSelection(defaultFont);

            if (sender is Button caller)
            {
                caller.Content = new FontConverter().ConvertToInvariantString(selectedFont);
                caller.Tag = selectedFont;
            }

            parent.UserFontDate = dateFontSelect.Tag as Font;

            parent.UpdateSettings();
            parent.SaveSettings();
        }

        private void timeFontSelect_Click(object sender, RoutedEventArgs e)
        {
            Font defaultFont = parent.UserFontTime;
            Font selectedFont = parent.WidgetObject.WidgetManager.RequestFontSelection(defaultFont);

            if (sender is Button caller)
            {
                caller.Content = new FontConverter().ConvertToInvariantString(selectedFont);
                caller.Tag = selectedFont;
            }

            parent.UserFontTime = timeFontSelect.Tag as Font;

            parent.UpdateSettings();
            parent.SaveSettings();
        }

        private void bgColorSelect_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button caller)
            {
                Color defaultColor = ColorTranslator.FromHtml(caller.Content.ToString());
                Color selectedColor = parent.WidgetObject.WidgetManager.RequestColorSelection(defaultColor);
                caller.Content = ColorTranslator.ToHtml(selectedColor);
            }

            parent.UserBackColor = ColorTranslator.FromHtml(bgColorSelect.Content as string);

            parent.UpdateSettings();
            parent.SaveSettings();
        }

        private void useGlobalChk_Click(object sender, RoutedEventArgs e)
        {
            parent.UseGlobal = useGlobalChk.IsChecked ?? false;
            bgColorSelect.IsEnabled = !parent.UseGlobal;
            dateFontSelect.IsEnabled = !parent.UseGlobal;
            timeFontSelect.IsEnabled = !parent.UseGlobal;

            parent.UpdateSettings();
            parent.SaveSettings();
        }

        private void checkbox24h_Click(object sender, RoutedEventArgs e)
        {
            parent.SetClock24h(checkbox24h.IsChecked ?? false);
            parent.UpdateSettings();
            parent.SaveSettings();
        }
    }
}
