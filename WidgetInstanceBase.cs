using FrontierWidgetFramework;
using FrontierWidgetFramework.WidgetUtility;
using System;

namespace ClockWidget
{

    public partial class ClockWidgetInstance : IWidgetInstance
    {

        // Identity
        private ClockWidgetServer parent;
        public IWidgetObject WidgetObject
        {
            get
            {
                return parent;
            }
        }
        public Guid Guid { get; set; }

        // Location
        public WidgetSize WidgetSize { get; set; }

        // Events
        public event WidgetUpdatedEventHandler WidgetUpdated;

    }
}
