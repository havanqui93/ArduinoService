using ArduinoService.DataContext;
using ArduinoService.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArduinoService.Models
{
    public class TrackingModel
    {
        private ioTSystemEntities _dbContext = new ioTSystemEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Get list temperature
        /// </summary>
        /// <returns></returns>
        public List<TrackingData> GetListDataTracking(string _deviceID, int top)
        {
            List<TrackingData> _list = new List<TrackingData>();
            try
            {
                string sql = @"
                SELECT TOP " + top + @"
                    DeviceId,
	                Value,
                    UserId,
	                CONVERT(VARCHAR(10), TimeUpdate, 101) + ' ' + SUBSTRING( convert(varchar, TimeUpdate,108),1,5) AS Time 
                FROM DeviceDetail
                WHERE DeviceId = '" + _deviceID + "' ORDER BY ID DESC";

                _list = _dbContext.Database.SqlQuery<TrackingData>(sql).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            return _list;
        }

        /// <summary>
        /// Add temperature
        /// </summary>
        /// <param name="_temperature"></param>
        /// <returns>true : Add success, False : Add Faild</returns>
        public bool AddDataTracking(UnoTrackingData rowdata)
        {
            try
            {
                string sql = @"
                    DECLARE @TEMPERATURE VARCHAR(20) = " + rowdata.TEMPERATURE + @"
                    DECLARE @HUMIDITY VARCHAR(20) = " + rowdata.HUMIDITY + @"
                    DECLARE @MOISTURE VARCHAR(20) = " + rowdata.MOISTURE + @"
                    DECLARE @USER_ID VARCHAR(100) = 'Admin'
                    DECLARE @DATE_TIME DATETIME = GETDATE()

                    INSERT INTO[DeviceDetail]
                    VALUES('" + ConstantClass.TEMPERATURE_ID + @"', @TEMPERATURE, @USER_ID, @DATE_TIME)
                    INSERT INTO[DeviceDetail]
                    VALUES('" + ConstantClass.HUMIDITY_ID + @"', @HUMIDITY, @USER_ID, @DATE_TIME)
                    INSERT INTO[DeviceDetail]
                    VALUES('" + ConstantClass.MOISTURE_ID + @"', @MOISTURE, @USER_ID, @DATE_TIME)
                    SELECT 1;
                    ";

                int res = _dbContext.Database.SqlQuery<int>(sql).FirstOrDefault();
                // delete data
                DeleteRecord();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool DeleteRecord()
        {
            bool result = false;
            try
            {
                string sql = @"
                    DECLARE @COUNT INT
                    SET @COUNT = (SELECT COUNT(*) FROM DeviceDetail WHERE DeviceId = '" + ConstantClass.HUMIDITY_ID + @"')

                    IF(@COUNT > 100)
                    BEGIN
                        DELETE DeviceDetail
                        WHERE ID IN (
                        SELECT TOP 100 Id
                        FROM DeviceDetail
                        ORDER BY Id DESC)
                        SELECT 1;
                    END
                    ELSE
						SELECT 0;
                    ";
                int res = _dbContext.Database.SqlQuery<int>(sql).FirstOrDefault();
                result = (res == 1 ? true : false);
            }
            catch (Exception ex)
            {
                result = true;
            }
            return result;
        }

        public List<LTrackingRowData> GetListTrackingHistory(int top)
        {
            List<LTrackingRowData> result = new List<LTrackingRowData>();
            try
            {
                var lstTemperature = GetListDataTracking(ConstantClass.TEMPERATURE_ID, top);
                var lstHumidity = GetListDataTracking(ConstantClass.HUMIDITY_ID, top);
                var lstMoisture = GetListDataTracking(ConstantClass.MOISTURE_ID, top);

                for (int i = 0; i < lstTemperature.Count; i++)
                {
                    result.Add(new LTrackingRowData()
                    {
                        ID = i + 1,
                        TEMPERATURE = lstTemperature[i].Value,
                        HUMIDITY = lstHumidity[i].Value,
                        MOISTURE = lstMoisture[i].Value,
                        Time = lstTemperature[i].Time
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }


    }
}