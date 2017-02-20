
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArduinoService.DataModels
{
    public class ChartRowData
    {
        public string CHART_ID { get; set; }
        public string CHART_NAME { get; set; }
        public int GROUP_SENSOR_ID { get; set; }
        public string UNIT_NAME { get; set; }
        public List<ChartData> CHART_DATA { get; set; }
    }

    public class ChartData
    {
        public string VALUE { get; set; }
        public string DAY { get; set; }
    }
}