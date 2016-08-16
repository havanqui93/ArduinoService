using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArduinoService.DataModels
{
    // Class get for MOBILE
    public class ControlRawData
    {
        public string DEVICE_ID { get; set; }
        public string DEVICE_NAME { get; set; }
        public string VALUE { get; set; }
    }

    // Class addd new control for mobile
    public class ControlAddRawData
    {
        public string GARDEN_ID { get; set; }
        public string DEVICE_NAME { get; set; }
        public int CATEGORY_ID { get; set; }
        public string DEVICE_ID { get; set; }
    }

}