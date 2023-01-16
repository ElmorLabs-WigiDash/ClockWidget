using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
        private int lastValidIntInput;

        public SettingsControl(ClockWidgetInstance parent)
        {

            this.parent = parent;

            InitializeComponent();

            checkbox24h.IsChecked = parent.time_24h;

            dateFontSelect.Tag = parent.FontDate;
            dateFontSelect.Content = new FontConverter().ConvertToInvariantString(parent.FontDate);

            timeFontSelect.Tag = parent.FontTime;
            timeFontSelect.Content = new FontConverter().ConvertToInvariantString(parent.FontTime);

            bgColorSelect.Content = ColorTranslator.ToHtml(parent.BackgroundTint);

            tintOpacitySelect.Text = parent.BackgroundTintOpacity.ToString();
        }

        private void dateFontSelect_Click(object sender, RoutedEventArgs e)
        {
            Font defaultFont = parent.FontDate;
            Font selectedFont = parent.WidgetObject.WidgetManager.RequestFontSelection(defaultFont);

            if (sender is Button caller)
            {
                caller.Content = new FontConverter().ConvertToInvariantString(selectedFont);
                caller.Tag = selectedFont;
            }
        }

        private void timeFontSelect_Click(object sender, RoutedEventArgs e)
        {
            Font defaultFont = parent.FontTime;
            Font selectedFont = parent.WidgetObject.WidgetManager.RequestFontSelection(defaultFont);

            if (sender is Button caller)
            {
                caller.Content = new FontConverter().ConvertToInvariantString(selectedFont);
                caller.Tag = selectedFont;
            }
        }

        private void bgColorSelect_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button caller)
            {
                Color defaultColor = ColorTranslator.FromHtml(caller.Content.ToString());
                Color selectedColor = parent.WidgetObject.WidgetManager.RequestColorSelection(defaultColor);
                caller.Content = ColorTranslator.ToHtml(selectedColor);
            }
        }

        private void tintOpacitySelect_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse(tintOpacitySelect.Text, out int parsedInput))
            {
                tintOpacitySelect.Text = parsedInput.ToString();
                return;
            }

            if (parsedInput <= 100 && parsedInput >= 0)
            {
                lastValidIntInput = parsedInput;
            } else if (parsedInput > 100)
            {
                lastValidIntInput = 100;
                tintOpacitySelect.Text = "100";
            } else
            {
                lastValidIntInput = 0;
                tintOpacitySelect.Text = "0";
            }
        }

        private void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            parent.SetClock24h(checkbox24h.IsChecked ?? false);

            parent.BackgroundTint = ColorTranslator.FromHtml(bgColorSelect.Content as string);
            parent.BackgroundTintOpacity = lastValidIntInput;

            parent.FontDate = dateFontSelect.Tag as Font;
            parent.FontTime = timeFontSelect.Tag as Font;

            parent.SaveSettings();
        }
    }
}
