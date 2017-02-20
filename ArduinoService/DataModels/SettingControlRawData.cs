using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArduinoService.DataModels
{
    public class SettingControlRawData
    {
        public string DEVICE_ID { get; set; }
        public string DEVICE_NAME { get; set; }
        public List<SettingControlDetailRawData> LIST_SETTING { get; set; }
    }
    public class SettingControlDetailRawData
    {
        public string SETTING_CONTROL_ID { get; set; }
        public string TIME_ON { get; set; }
        public string TIME_OFF { get; set; }
    }
}