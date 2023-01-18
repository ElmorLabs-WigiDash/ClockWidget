using WigiDashWidgetFramework;
using WigiDashWidgetFramework.WidgetUtility;
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
            DrawClock(DateTime.Now);
        }

        public void ClickEvent(ClickType click_type, int x, int y)
        {
            parent.WidgetManager.OnTriggerOccurred(clicked_trigger_guid);
        }

        public UserControl GetSettingsControl()
        {
            return new SettingsControl(this);
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

        public Font FontDate;
        public Font FontTime;
        Bitmap BitmapCurrent;
        private Bitmap BitmapBackground;
        private DateTime timestamp_last;

        Mutex drawing_mutex = new Mutex();

        public bool time_24h = true;

        private Guid clicked_trigger_guid = new Guid("F6228B98-B94B-4088-8CA6-484A36436E2A");
        private Guid toggle_24h_action_guid = new Guid("A48F7EAC-BFB2-494E-8DB7-D5F6678B831E");

        public Color BackgroundTint = Color.Red;
        public int BackgroundTintOpacity = 0;

        public ClockWidgetInstance(ClockWidgetServer parent, WidgetSize widget_size, Guid instance_guid)
        {

            this.parent = parent;
            this.WidgetSize = widget_size;
            this.Guid = instance_guid;

            if (widget_size.Equals(2, 1))
            {
                FontDate = new Font("Verdana", 18, FontStyle.Bold);
                FontTime = new Font("Basic Square 7", 56);
            }
            else
            {
                FontDate = new Font("Verdana", 24, FontStyle.Bold);
                FontTime = new Font("Basic Square 7", 72);
            }

            BitmapCurrent = new Bitmap(widget_size.ToSize().Width, widget_size.ToSize().Height);
            BitmapBackground = new Bitmap(parent.ResourcePath + "widget_506x194_grey_gradient_dithered.png");

            LoadSettings();

            // Register widget clicked
            parent.WidgetManager.RegisterTrigger(this, clicked_trigger_guid, "Clicked");

            // Register toggle time
            parent.WidgetManager.RegisterAction(this, toggle_24h_action_guid, "Toggle 12/24h");

            // Register for action events
            parent.WidgetManager.ActionRequested += WidgetManager_ActionRequested;

            // Start thread
            ThreadStart thread_start = new ThreadStart(UpdateTask);
            task_thread = new Thread(thread_start);
            task_thread.IsBackground = true;
            run_task = true;
            pause_task = false;
            timestamp_last = DateTime.MinValue;
            task_thread.Start();
        }

        private void WidgetManager_ActionRequested(Guid action_guid) {
            if(action_guid == toggle_24h_action_guid) {
                SetClock24h(!time_24h);
                //timestamp_last.AddMinutes(1);
                if(drawing_mutex.WaitOne(1000)) {
                    using(Graphics g = Graphics.FromImage(BitmapCurrent)) {
                        g.Clear(Color.Red);
                    }
                    UpdateWidget();
                    drawing_mutex.ReleaseMutex();
                }
            }
        }

        public void SetClock24h(bool value)
        {
            time_24h = value;
            timestamp_last = DateTime.MinValue;
        }

        private void DrawClock(DateTime timestamp)
        {

            if (drawing_mutex.WaitOne(1000))
            {
                string date = timestamp.ToString("D", CultureInfo.GetCultureInfo("en-US"));
                string time = time_24h ? timestamp.ToString("HH:mm") : timestamp.ToString("h:mm", CultureInfo.InvariantCulture);
                using (Graphics g = Graphics.FromImage(BitmapCurrent))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    g.DrawImage(BitmapBackground, 0, 0, BitmapCurrent.Width, BitmapCurrent.Height);

                    Brush warnBrush = new SolidBrush(Color.FromArgb(255 / 100 * BackgroundTintOpacity, BackgroundTint));
                    g.FillRectangle(warnBrush, new Rectangle(0, 0, BitmapCurrent.Width, BitmapCurrent.Height));

                    SizeF str_size_date = g.MeasureString(date, FontDate);
                    g.DrawString(date, FontDate, Brushes.LightGray, (BitmapCurrent.Width - str_size_date.Width) / 2 + 5, BitmapCurrent.Height - str_size_date.Height - 10);
                    SizeF str_size_time = g.MeasureString(time, FontTime);
                    g.DrawString(time, FontTime, Brushes.White, (BitmapCurrent.Width - str_size_time.Width) / 2 + 10, (BitmapCurrent.Height - str_size_time.Height) / 2);
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

        public void SaveSettings()
        {
            // Save setting
            parent.WidgetManager.StoreSetting(this, nameof(BackgroundTint), ColorTranslator.ToHtml(BackgroundTint));
            parent.WidgetManager.StoreSetting(this, nameof(BackgroundTintOpacity), BackgroundTintOpacity.ToString());
            parent.WidgetManager.StoreSetting(this, nameof(FontDate), new FontConverter().ConvertToInvariantString(FontDate));
            parent.WidgetManager.StoreSetting(this, nameof(FontTime), new FontConverter().ConvertToInvariantString(FontTime));
        }

        public void LoadSettings()
        {
            if (parent.WidgetManager.LoadSetting(this, nameof(BackgroundTint), out string bgTintStr))
            {
                BackgroundTint = ColorTranslator.FromHtml(bgTintStr);
            }

            if (parent.WidgetManager.LoadSetting(this, nameof(BackgroundTintOpacity), out string bgTintOpacityStr))
            {
                int.TryParse(bgTintOpacityStr, out BackgroundTintOpacity);
            }

            if (parent.WidgetManager.LoadSetting(this, nameof(FontDate), out var strDateFont))
            {
                FontDate = new FontConverter().ConvertFromInvariantString(strDateFont) as Font;
            }

            if (parent.WidgetManager.LoadSetting(this, nameof(FontTime), out var strTimeFont))
            {
                FontTime = new FontConverter().ConvertFromInvariantString(strTimeFont) as Font;
            }
        }
    }
}

