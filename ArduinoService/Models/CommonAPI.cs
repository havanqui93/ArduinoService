using ArduinoService.DataContext;
using ArduinoService.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArduinoService.Models
{
    public class CommonAPI
    {
        private ioTEntities dbcontext = new ioTEntities();
        public List<ArduinoTypeRowData> GetListArduinoType() {
            List<ArduinoTypeRowData> lst = new List<ArduinoTypeRowData>();
            try
            {
                string sql = @"
                    SELECT ARDUINO_TYPE_ID,ARDUINO_TYPE_NAME FROM S_ARDUINO_TYPE
                    ";
                lst = dbcontext.Database.SqlQuery<ArduinoTypeRowData>(sql).ToList();
            }
            catch (Exception ex)
            {
                lst = null;
            }
            return lst;
        }
    }
}