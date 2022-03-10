using FrontierWidgetFramework;
using FrontierWidgetFramework.WidgetUtility;
using System;
using System.Collections.Generic;

namespace ClockWidget
{
    public partial class ClockWidgetServer : IWidgetObject
    {

        // Identity
        public Guid Guid
        {
            get
            {
                return new Guid(GetType().Assembly.GetName().Name);
            }
        }
        public string Name
        {
            get
            {
                return "Clock";
            }
        }
        public string Description
        {

            get
            {
                return "A widget displaying the current time and date";
            }

        }
        public string Author
        {
            get
            {
                return "Jon Sandström";
            }
        }
        public string Website
        {
            get
            {
                return "https://www.elmorlabs.com/";
            }
        }
        public Version Version
        {
            get
            {
                return new Version(1, 0, 0);
            }
        }

        // Capabilities
        public SdkVersion TargetSdk
        {
            get
            {
                return SdkVersion.Version_0;
            }
        }
        public List<WidgetSize> SupportedSizes
        {
            get
            {
                return new List<WidgetSize>() {
                    new WidgetSize(2, 1),
                    new WidgetSize(3, 2),
                };
            }
        }

        // Functionality
        public IWidgetManager WidgetManager { get; set; }

        // Error handling
        public string LastErrorMessage { get; set; }


    }

}
