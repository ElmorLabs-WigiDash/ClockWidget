﻿using FrontierWidgetFramework;
using FrontierWidgetFramework.WidgetUtility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Controls;

namespace ClockWidget
{
    public partial class ClockWidgetInstance : IWidgetInstance
    {

        // Functionality
        public void RequestUpdate()
        {
            if (drawing_mutex.WaitOne(1000))
            {
                DrawClock(DateTime.Now);
                drawing_mutex.ReleaseMutex();
            }
        }

        public void ClickEvent(ClickType click_type, int x, int y)
        {

        }

        public UserControl GetSettingsControl()
        {
            return new SettingsUserControl(this);
        }

        public void Dispose()
        {
            run_task = false;
            pause_task = false;
        }

        public void EnterSleep()
        {
            pause_task = true;
        }

        public void ExitSleep()
        {
            pause_task = false;
            timestamp_last = DateTime.MinValue;
            RequestUpdate();
        }

        // Class specific
        private Thread task_thread;
        private volatile bool run_task;
        private volatile bool pause_task;

        private Font FontDate;
        private Font FontTime;
        private Font FontTime12h;
        Bitmap BitmapCurrent;
        private Bitmap BitmapBackground;
        private DateTime timestamp_last;

        Mutex drawing_mutex = new Mutex();

        private bool time_24h = true;

        public ClockWidgetInstance(ClockWidgetServer parent, WidgetSize widget_size, Guid instance_guid)
        {

            this.parent = parent;
            this.WidgetSize = widget_size;
            this.Guid = instance_guid;

            if (widget_size.Equals(2, 1))
            {
                FontDate = new Font("Verdana", 18, FontStyle.Bold);
                FontTime = new Font("Basic Square 7", 72);
                FontTime12h = new Font("Basic Square 7", 56);
            }
            else
            {
                FontDate = new Font("Verdana", 24, FontStyle.Bold);
                FontTime = new Font("Basic Square 7", 100);
                FontTime12h = new Font("Basic Square 7", 80);
            }

            parent.WidgetManager.LoadSetting(this.Guid, "24h", out string clock_type);
            Boolean.TryParse(clock_type, out bool twenty_four_hours);
            time_24h = twenty_four_hours;

            BitmapCurrent = new Bitmap(widget_size.ToSize().Width, widget_size.ToSize().Height);
            BitmapBackground = new Bitmap(parent.ResourcePath + "widget_506x194_grey_gradient_dithered.png");

            // Start thread
            ThreadStart thread_start = new ThreadStart(UpdateTask);
            task_thread = new Thread(thread_start);
            task_thread.IsBackground = true;
            run_task = true;
            pause_task = false;
            timestamp_last = DateTime.MinValue;
            task_thread.Start();

        }

        public void SetClock24h(bool value)
        {
            time_24h = value;
            timestamp_last = DateTime.MinValue;
            parent.WidgetManager.StoreSetting(this, "24h", value.ToString());
        }

        private void DrawClock(DateTime timestamp)
        {

            if (drawing_mutex.WaitOne(1000))
            {
                string date = timestamp.ToString("D", CultureInfo.GetCultureInfo("en-US"));
                string time = time_24h ? timestamp.ToString("HH:mm") : timestamp.ToString("h:mm tt", CultureInfo.InvariantCulture);
                using (Graphics g = Graphics.FromImage(BitmapCurrent))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    g.DrawImage(BitmapBackground, 0, 0, BitmapCurrent.Width, BitmapCurrent.Height);
                    SizeF str_size_date = g.MeasureString(date, FontDate);
                    g.DrawString(date, FontDate, Brushes.LightGray, (BitmapCurrent.Width - str_size_date.Width) / 2 + 5, BitmapCurrent.Height - str_size_date.Height - 10);
                    SizeF str_size_time = g.MeasureString(time, time_24h ? FontTime : FontTime12h);
                    g.DrawString(time, time_24h ? FontTime : FontTime12h, Brushes.White, (BitmapCurrent.Width - str_size_time.Width) / 2 + 10, (BitmapCurrent.Height - str_size_time.Height) / 2);
                }
                timestamp_last = timestamp;
                UpdateWidget();
                drawing_mutex.ReleaseMutex();
            }
        }

        private void UpdateWidget()
        {
            WidgetUpdatedEventArgs e = new WidgetUpdatedEventArgs();
            e.WidgetBitmap = BitmapCurrent;
            e.WaitMax = 1000;
            WidgetUpdated?.Invoke(this, e);
        }

        private void UpdateTask()
        {
            while (run_task)
            {
                DateTime timestamp = DateTime.Now;
                if (timestamp.Minute != timestamp_last.Minute || timestamp_last == DateTime.MinValue)
                {
                    DrawClock(timestamp);
                }
                for (int i = 0; i < 1 || pause_task; i++)
                {
                    Thread.Sleep(500);
                }
            }
        }

    }
}

