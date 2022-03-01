using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using FrontierWidgetFramework;
using System.Timers;
using System.Threading;

namespace ClockWidget_old {
    public class ClockWidget : IWidget {

        private IWidgetManager widget_manager;
        private Bitmap bitmap_preview_2x1;

        public Guid WidgetGuid {
            get {
                return new Guid("04E2CAFC-4BAB-4601-BD40-0D365C1C435C");
            }
        }

        public string Name {
            get {
                return "ClockWidget";
            }
        }

        public string Description => throw new NotImplementedException();

        public string Author => throw new NotImplementedException();

        public string Email => throw new NotImplementedException();

        public string Website => throw new NotImplementedException();

        public SdkVersion TargetSdk {
            get {
                return SdkVersion.Version_0;
            }
        }

        public WidgetSize Size {
            get {
                return WidgetSize.SIZE_2X1;
            }
        }

        private string last_error_message;
        public string LastErrorMessage {
            get {
                return last_error_message;
            }
        }

        public IWidgetManager WidgetManager {
            get {
                return widget_manager;
            }
            set {
                widget_manager = value;
            }
        }

        int IWidget.Version => throw new NotImplementedException();

        public string VersionString => throw new NotImplementedException();

        public List<WidgetSize> SupportedSizes {
            get {
                return new List<WidgetSize>() { WidgetSize.SIZE_2X1 };
            }
        }

        public ErrorCode Load() {
            widget_manager.RegisterWidget(this);
            return ErrorCode.NoError;
        }

        public ErrorCode Unload() {
            //throw new NotImplementedException();
            return ErrorCode.NoError;
        }

        public Bitmap GetWidgetPreview(WidgetSize widget_size) {
            return bitmap_preview_2x1;
        }

        public IWidgetInstance CreateWidgetInstance(WidgetSize widget_size, Guid instance_guid) {
            ClockWidgetInstance widget_instance = new ClockWidgetInstance(this, widget_size, instance_guid);
            return widget_instance;
        }

        public bool RemoveWidgetInstance(Guid instance_guid) {
            throw new NotImplementedException();
        }

        public ClockWidget() {

            // Generate previews
            ClockWidgetInstance temp_widget_instance = new ClockWidgetInstance(this, WidgetSize.SIZE_2X1, new Guid());
            //while(!temp_widget_instance.Invalidated);
            if(temp_widget_instance.GetBitmapMutex()) {
                bitmap_preview_2x1 = temp_widget_instance.GetWidgetBitmap();
                temp_widget_instance.ReturnBitmapMutex();
            }
            temp_widget_instance.Dispose();

        }

    }

    public class ClockWidgetInstance : IWidgetInstance {

        private ClockWidget parent;
        private Guid guid;
        private int id;
        private Point location;
        private Point location_abs;
        private WidgetSize widget_size;
        private bool invalidated;
        private int page;
        
        private Mutex drawing_mutex = new Mutex();
        private const int mutex_timeout = 100;
        private Thread task_thread;
        private bool run_task;

        private Font FontDate;
        private Font FontTime;
        private Bitmap widget_bitmap;
        private Bitmap BitmapBackground;
        private DateTime timestamp_last;

        public event WidgetUpdatedEventHandler WidgetUpdated;

        public IWidget Parent {
            get {
                return parent;
            }
        }

        public Guid InstanceGuid {
            get {
                return guid;
            }
        }

        public int Id {
            get {
                return id;
            }
            set {
                id = value;
            }
        }

        public WidgetSize Size {
            get {
                return widget_size;
            }
        }

        public bool SupportsClick {
            get {
                return false;
            }
        }

        public int Page {
            get {
                return page;
            }
            set {
                page = value;
            }
        }

        public Point Location {
            get {
                return location;
            }
            set {
                location = value;
            }
        }

        public Point LocationAbs {
            get {
                return location_abs;
            }
            set {
                location_abs = value;
            }
        }

        public bool Invalidated {
            get {
                return invalidated;
            }
        }

        public void ClickEvent(ClickType click_type, int x, int y) {
            //throw new NotImplementedException();
        }

        public bool GetBitmapMutex() {
            return drawing_mutex.WaitOne(mutex_timeout);
        }

        public void ReturnBitmapMutex() {
            drawing_mutex.ReleaseMutex();
        }

        public Bitmap GetWidgetBitmap() {
            invalidated = false;
            return widget_bitmap;
        }

        public bool HasSettings {
            get {
                return false;
            }
        }

        Size IWidgetInstance.Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void ShowSettings() {
            //throw new NotImplementedException();
        }

        public ClockWidgetInstance(ClockWidget parent, WidgetSize widget_size, Guid instance_guid) {

            this.parent = parent;
            this.widget_size = widget_size;
            this.guid = instance_guid;

            FontDate = new Font("Verdana", 18, FontStyle.Bold);
            FontTime = new Font("Basic Square 7", 72);

            Size size = Helper.WidgetSizeToSize(widget_size);
            widget_bitmap = new Bitmap(size.Width, size.Height);
            BitmapBackground = new Bitmap("widget_506x194_grey_gradient_dithered.png");
            invalidated = false;

            timestamp_last = DateTime.MinValue;
            DrawClock();

            ThreadStart thread_start = new ThreadStart(UpdateTask);
            task_thread = new Thread(thread_start);
            run_task = true;
            task_thread.Start();
        }

        private void DrawClock() {
            DateTime timestamp = DateTime.Now;

            if(timestamp.Minute != timestamp_last.Minute || timestamp_last != DateTime.MinValue) {
                string date = timestamp.ToString("D", CultureInfo.GetCultureInfo("en-US"));
                string time = timestamp.ToString("t");
                if(drawing_mutex.WaitOne(mutex_timeout)) {
                    using(Graphics g = Graphics.FromImage(widget_bitmap)) {
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                        g.DrawImage(BitmapBackground, 0, 0, widget_bitmap.Width, widget_bitmap.Height);
                        SizeF str_size_date = g.MeasureString(date, FontDate);
                        g.DrawString(date, FontDate, Brushes.LightGray, (widget_bitmap.Width - str_size_date.Width) / 2 + 5, widget_bitmap.Height - str_size_date.Height - 20);
                        SizeF str_size_time = g.MeasureString(time, FontTime);
                        g.DrawString(time, FontTime, Brushes.White, (widget_bitmap.Width - str_size_time.Width) / 2 + 10, (widget_bitmap.Height - str_size_time.Height) / 2);
                    }
                    invalidated = true;
                    timestamp_last = timestamp;
                    drawing_mutex.ReleaseMutex();
                }
            }
        }

        private void UpdateTask() {
            while(run_task) {
                DrawClock();
                Thread.Sleep(10000);
            }
        }

        public void Dispose() {
            run_task = false;
        }

        public void RequestUpdate() {
            throw new NotImplementedException();
        }
    }

}


