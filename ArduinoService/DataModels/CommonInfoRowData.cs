using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArduinoService.DataModels
{
    public class CommonInfoRowData
    {
    }

    public class ArduinoTypeRowData
    {
        public int ARDUINO_TYPE_ID { get; set; }
        public string ARDUINO_TYPE_NAME { get; set; }
    }

    public class ActiveCodeRowData
    {
        public string GARDEN_ID { get; set; }
        public string TOKEN_KEY { get; set; }
    }

}