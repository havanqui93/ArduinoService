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

        public List<string> GetListPin(string tokenkey,string category) {
            List<string> lstresult = new List<string>();
            try
            {
                string sql = @"
                    SELECT PIN FROM S_ARDUINO_TYPE_DETAIL UNO_DETAIL
                    INNER JOIN S_GARDEN GARDEN ON UNO_DETAIL.ARDUINO_TYPE_ID = GARDEN.UNO_TYPE
                    LEFT JOIN S_DEVICE D ON D.PIN_ID = UNO_DETAIL.PIN
                    WHERE GARDEN.TOKEN_KEY = '"+ tokenkey + @"' AND CATEGORY_ID = '"+ category + @"'
                    AND D.PIN_ID IS NULL
                ";
                lstresult = dbcontext.Database.SqlQuery<string>(sql).ToList();
            }
            catch (Exception ex)
            {
                lstresult = null;
            }
            return lstresult;
        }
    }
}