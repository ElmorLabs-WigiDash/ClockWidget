using WigiDashWidgetFramework;
using WigiDashWidgetFramework.WidgetUtility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ClockWidget
{
    public partial class ClockWidgetServer : IWidgetObject
    {

        // Identity
        public Guid Guid => new Guid(GetType().Assembly.GetName().Name);
        public string Name => ClockWidget.Properties.Resources.ClockWidgetServer_Clock;

        public string Description => ClockWidget.Properties.Resources.ClockWidgetServer_DisplayTheCurrentTimeAndDate;

        public string Author => "ElmorLabs";

        public string Website => "https://elmorlabs.com/";

        public Version Version => new Version(1, 0, 1);

        // Capabilities
        public SdkVersion TargetSdk => SdkVersion.Version_0;

        public List<WidgetSize> SupportedSizes =>
            new List<WidgetSize>() {
                new WidgetSize(2, 1),
                new WidgetSize(3, 2),
            };

        public Bitmap PreviewImage => new Bitmap(ResourcePath + "preview_2x1.png");

        // Functionality
        public IWidgetManager WidgetManager { get; set; }

        // Error handling
        public string LastErrorMessage { get; set; }


    }

}
