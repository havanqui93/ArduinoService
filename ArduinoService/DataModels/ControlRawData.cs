using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArduinoService.DataModels
{
    public class DeviceRowData
    {
        public string DEVICE_ID { get; set; }
        public string DEVICE_NAME { get; set; }
        public int UNIT { get; set; }
        public int GROUP_SENSOR_ID { get; set; }
        public string PIN_ID { get; set; }
    }

    // Class get for MOBILE
    public class ControlRawData
    {
        public string DEVICE_ID { get; set; }
        public string DEVICE_NAME { get; set; }
        public string VALUE { get; set; }
        public string PIN_ID { get; set; }
    }

    // Class add new control for mobile
    public class ControlAddRawData
    {
        public string GARDEN_ID { get; set; }
        public string DEVICE_NAME { get; set; }
        public int CATEGORY_ID { get; set; }
        public string DEVICE_ID { get; set; }
        public int UNIT_ID { get; set; }
        public string PIN_ID { get; set; }
    }

    public class TrackingRawData
    {
        public string DEVICE_ID { get; set; }
        public string DEVICE_NAME { get; set; }
        public string UNIT { get; set; }
        public List<ListValueTracking> LIST_TRACKING { get; set; }
    }

    public class ListValueTracking
    {
        public string TIME { get; set; }
        public string VALUE { get; set; }
    }

    #region Class for UNO Tracking

    public class ListValueTrackingUno
    {
        public string DEVICE_ID { get; set; }
        public string VALUE { get; set; }
        public string TIME { get; set; }
        public string UNIT { get; set; }
    }

    #endregion
}