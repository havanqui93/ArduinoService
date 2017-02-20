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
        public int UNO_TYPE { get; set; }
        public DateTime START { get; set; }
        public DateTime? END { get; set; }
        public decimal LATITUDE { get; set; }
        public decimal LONGITUDE { get; set; }
        public int ACREAGE { get; set; }
        public string ADDRESS { get; set; }
        public string DESCRIPTION { get; set; }

        public string UNO_TYPE_NAME { get; set; }
        public string START_GARDEN { get; set; }
        public string END_GARDEN { get; set; }
        public int IS_SHEDULE { get; set; }

        public bool IS_ADD { get; set; }

    }
}