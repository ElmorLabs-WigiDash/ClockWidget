using WigiDashWidgetFramework;
using WigiDashWidgetFramework.WidgetUtility;
using System;
using System.Drawing;

namespace ClockWidget
{
    public partial class ClockWidgetServer : IWidgetObject
    {

        // Functionality
        public string ResourcePath;
        public WidgetError Load(string resource_path)
        {
            this.ResourcePath = resource_path;

            // Load previews
            bitmap_preview_2x1 = new Bitmap(ResourcePath + "preview_2x1.png");
            bitmap_preview_3x2 = new Bitmap(ResourcePath + "preview_3x2.png");

            return WidgetError.NO_ERROR;
        }
        public WidgetError Unload()
        {
            return WidgetError.NO_ERROR;
        }
        public Bitmap GetWidgetPreview(WidgetSize widget_size)
        {
            if (widget_size.Equals(2, 1))
            {
                return bitmap_preview_2x1;
            }
            else if (widget_size.Equals(3, 2))
            {
                return bitmap_preview_3x2;
            }
            else
            {
                return new Bitmap(ResourcePath + "preview_" + widget_size.ToString() + ".png");
            }
        }
        public IWidgetInstance CreateWidgetInstance(WidgetSize widget_size, Guid instance_guid)
        {
            ClockWidgetInstance widget_instance = new ClockWidgetInstance(this, widget_size, instance_guid);
            return widget_instance;
        }
        public bool RemoveWidgetInstance(Guid instance_guid)
        {
            throw new NotImplementedException();
        }

        // Class specific
        private Bitmap bitmap_preview_2x1;
        private Bitmap bitmap_preview_3x2;

        public ClockWidgetServer()
        {
        }
    }

}
