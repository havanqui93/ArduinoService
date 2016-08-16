using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArduinoService.DataModels
{
    public class GardenRawData
    {
        public string GARDEN_ID { get; set; }
        public string GARDEN_NAME { get; set; }
        public string IMAGE { get; set; }
        public string USER_ID { get; set; }
        public int ACTIVE { get; set; }
        public string TOKEN_KEY { get; set; }
    }
}