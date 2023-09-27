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
            if (drawing_mutex.WaitOne(1000))
            {
                DrawClock(DateTime.Now);
                drawing_mutex.ReleaseMutex();
            }
        }

        public void ClickEvent(ClickType click_type, int x, int y)
        {
            parent.WidgetManager.OnTriggerOccurred(clicked_trigger_guid);
        }

        private SettingsControl settingsControl;
        public UserControl GetSettingsControl()
        {
            return settingsControl;
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
        }

        // Class specific
        private Thread task_thread;
        private volatile bool run_task;
        private volatile bool pause_task;

        public Font DrawFontDate;
        public Font UserFontDate;
        public Font DrawFontTime;
        public Font UserFontTime;
        Bitmap BitmapCurrent;
        private DateTime timestamp_last;

        Mutex drawing_mutex = new Mutex();

        public bool time_24h = true;

        private Guid clicked_trigger_guid = new Guid("F6228B98-B94B-4088-8CA6-484A36436E2A");
        private Guid toggle_24h_action_guid = new Guid("A48F7EAC-BFB2-494E-8DB7-D5F6678B831E");

        public Color DrawBackColor;
        public Color UserBackColor;
        public Color DrawForeColor;
        public Color UserForeColor;
        public bool UseGlobal = false;

        public ClockWidgetInstance(ClockWidgetServer parent, WidgetSize widget_size, Guid instance_guid)
        {
            this.parent = parent;
            this.WidgetSize = widget_size;
            this.Guid = instance_guid;

            if (widget_size.Equals(2, 1))
            {
                DrawFontDate = new Font("Verdana", 18, FontStyle.Bold);
                DrawFontTime = new Font("Basic Square 7", 56);
            }
            else
            {
                DrawFontDate = new Font("Verdana", 24, FontStyle.Bold);
                DrawFontTime = new Font("Basic Square 7", 72);
            }

            BitmapCurrent = new Bitmap(widget_size.ToSize().Width, widget_size.ToSize().Height);

            LoadSettings();

            settingsControl = new SettingsControl(this);

            UpdateSettings();

            // Register widget clicked
            parent.WidgetManager.RegisterTrigger(this, clicked_trigger_guid, "Clicked");

            // Register toggle time
            parent.WidgetManager.RegisterAction(this, toggle_24h_action_guid, "Toggle 12/24h");

            // Register for action events
            parent.WidgetManager.ActionRequested += WidgetManager_ActionRequested;

            parent.WidgetManager.GlobalThemeUpdated += WidgetManager_GlobalThemeUpdated;

            // Start thread
            ThreadStart thread_start = new ThreadStart(UpdateTask);
            task_thread = new Thread(thread_start);
            task_thread.IsBackground = true;
            run_task = true;
            pause_task = false;
            timestamp_last = DateTime.MinValue;
            task_thread.Start();
        }

        private void WidgetManager_GlobalThemeUpdated()
        {
            if (UseGlobal)
            {
                UpdateSettings();
            }
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
                string date = timestamp.ToString("ddd, MMM. d, yyyy", CultureInfo.GetCultureInfo("en-US"));
                string time = time_24h ? timestamp.ToString("HH:mm") : timestamp.ToString("h:mm", CultureInfo.InvariantCulture);
                using (Graphics g = Graphics.FromImage(BitmapCurrent))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                    g.Clear(DrawBackColor);
                    Brush textBrush = new SolidBrush(DrawForeColor);

                    SizeF str_size_date = g.MeasureString(date, DrawFontDate);
                    g.DrawString(date, DrawFontDate, textBrush, (BitmapCurrent.Width - str_size_date.Width) / 2 + 5, BitmapCurrent.Height - str_size_date.Height - 10);
                    SizeF str_size_time = g.MeasureString(time, DrawFontTime);
                    g.DrawString(time, DrawFontTime, textBrush, (BitmapCurrent.Width - str_size_time.Width) / 2 + 10, (BitmapCurrent.Height - str_size_time.Height) / 2);
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
                for (int i = 0; i < 1 * 5 || pause_task; i++)
                {
                    if (!run_task) return;
                    Thread.Sleep(100);
                }
            }
        }

        public void SaveSettings()
        {
            // Save setting
            parent.WidgetManager.StoreSetting(this, nameof(UseGlobal), UseGlobal.ToString());
            parent.WidgetManager.StoreSetting(this, nameof(UserBackColor), ColorTranslator.ToHtml(UserBackColor));
            parent.WidgetManager.StoreSetting(this, nameof(UserForeColor), ColorTranslator.ToHtml(UserForeColor));
            parent.WidgetManager.StoreSetting(this, nameof(DrawFontDate), new FontConverter().ConvertToInvariantString(DrawFontDate));
            parent.WidgetManager.StoreSetting(this, nameof(DrawFontTime), new FontConverter().ConvertToInvariantString(DrawFontTime));
        }

        public void LoadSettings()
        {
            if (parent.WidgetManager.LoadSetting(this, nameof (UseGlobal), out string useGlobalStr))
            {
                UseGlobal = bool.Parse(useGlobalStr);
            } else
            {
                UseGlobal = parent.WidgetManager.PreferGlobalTheme;
            }

            if (parent.WidgetManager.LoadSetting(this, nameof(UserBackColor), out string bgTintStr))
            {
                UserBackColor = ColorTranslator.FromHtml(bgTintStr);
            } else
            {
                Random rnd = new Random();
                UserBackColor = Color.FromArgb(rnd.Next(0, 150), rnd.Next(0, 150), rnd.Next(0, 150));
            }

            if (parent.WidgetManager.LoadSetting(this, nameof(UserForeColor), out string fgColorStr))
            {
                UserForeColor = ColorTranslator.FromHtml(fgColorStr);
            }
            else
            {
                UserForeColor = Color.FromArgb(255 - UserBackColor.R, 255 - UserBackColor.G, 255 - UserBackColor.B);
            }

            if (parent.WidgetManager.LoadSetting(this, nameof(UserFontDate), out var strDateFont))
            {
                UserFontDate = new FontConverter().ConvertFromInvariantString(strDateFont) as Font;
            } else
            {
                UserFontDate = DrawFontDate;
            }

            if (parent.WidgetManager.LoadSetting(this, nameof(UserFontTime), out var strTimeFont))
            {
                UserFontTime = new FontConverter().ConvertFromInvariantString(strTimeFont) as Font;
            } else
            {
                UserFontTime = DrawFontTime;
            }
        }

        public void UpdateSettings()
        {
            if (UseGlobal)
            {
                DrawBackColor = parent.WidgetManager.GlobalWidgetTheme.PrimaryBgColor;
                DrawForeColor = parent.WidgetManager.GlobalWidgetTheme.PrimaryFgColor;
                DrawFontDate = new Font(parent.WidgetManager.GlobalWidgetTheme.SecondaryFont.FontFamily, DrawFontDate.Size, DrawFontDate.Style);
                DrawFontTime = new Font(parent.WidgetManager.GlobalWidgetTheme.PrimaryFont.FontFamily, DrawFontTime.Size, DrawFontTime.Style);
            }
            else
            {
                DrawBackColor = UserBackColor;
                DrawForeColor = UserForeColor;
                DrawFontDate = UserFontDate;
                DrawFontTime = UserFontTime;
            }

            RequestUpdate();
        }
    }
}

