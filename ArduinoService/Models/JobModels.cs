using ArduinoService.DataContext;
using ArduinoService.DataModels;
using ArduinoService.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArduinoService.Models
{
    public class JobModels
    {
        private ioTEntities _dbContext = new ioTEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private CommonModel commonModel = new CommonModel();
        private CommonFunction commonFunction = new CommonFunction();
        ControlHub _hub = new ControlHub();

        public void UpdateJob()
        {
            try
            {
                string sql = @"
                    BEGIN TRY
                    BEGIN TRANSACTION
                        DECLARE @GROUP_HT INT = 1
                        INSERT INTO D_DEVICE_SENSOR_HT(DEVICE_ID,VALUE,TIME_UPDATE)
                        SELECT DEVICE_ID,VALUE,TIME_UPDATE FROM D_DEVICE_SENSOR_DETAIL WHERE DEVICE_ID IN
                        (
	                        SELECT DEVICE_ID FROM S_DEVICE WHERE GROUP_SENSOR_ID = @GROUP_HT
                        )

                        DECLARE @GROUP_MOISTURE INT = 2
                        INSERT INTO D_DEVICE_SENSOR_MOISTURE(DEVICE_ID,VALUE,TIME_UPDATE)
                        SELECT DEVICE_ID,VALUE,TIME_UPDATE FROM D_DEVICE_SENSOR_DETAIL WHERE DEVICE_ID IN
                        (
	                        SELECT DEVICE_ID FROM S_DEVICE WHERE GROUP_SENSOR_ID = @GROUP_MOISTURE
                        )

                        DECLARE @GROUP_LIGHT INT = 3
                        INSERT INTO D_DEVICE_SENSOR_LIGHT(DEVICE_ID,VALUE,TIME_UPDATE)
                        SELECT DEVICE_ID,VALUE,TIME_UPDATE FROM D_DEVICE_SENSOR_DETAIL WHERE DEVICE_ID IN
                        (
	                        SELECT DEVICE_ID FROM S_DEVICE WHERE GROUP_SENSOR_ID = @GROUP_LIGHT
                        )

                        DELETE D_DEVICE_SENSOR_DETAIL

                        SELECT " + ConstantClass.IS_SUCCESS + @"
                    COMMIT
                    END TRY
                    BEGIN CATCH
                        SELECT " + ConstantClass.IS_FAILD + @"
                    ROLLBACK
                    END CATCH
                    ";
                int res = _dbContext.Database.SqlQuery<int>(sql).FirstOrDefault();
                if (res == ConstantClass.IS_SUCCESS)
                {
                    // send mail
                }
                else
                {
                    // send mail
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void UpdateValueControl(string time)
        {
            try
            {
                string sql = @"
                        DECLARE @TIME_COMPARE TIME = '" + time + @"'
                        DECLARE @TIME_MAX TIME = '" + ConstantClass.TIME_MAX + @"'
                        DECLARE @TIME_MIN TIME = '" + ConstantClass.TIME_MIN + @"'

                        SELECT D.DEVICE_ID 
                        INTO #LIST_DEVICE
                        FROM S_USER US
                        INNER JOIN (SELECT TOKEN_KEY,USER_ID FROM S_GARDEN WHERE TOKEN_KEY IS NOT NULL AND IS_SHEDULE = 1) G ON US.USER_ID = G.USER_ID
                        INNER JOIN (SELECT DEVICE_ID,TOKEN_KEY FROM S_DEVICE WHERE DEVICE_CATEGORY = 1) D ON G.TOKEN_KEY = D.TOKEN_KEY
                        WHERE US.PACKED_ID = '" + ConstantClass.PACKED_SHEDULE + @"'

                        SELECT DEVICE_ID
                        INTO #LIST_DEVICE_SETTING
                        FROM D_SETTING_CONTROL SETTING
                        WHERE TIME_ON <= @TIME_COMPARE
                         AND TIME_OFF >= @TIME_COMPARE
                         OR(TIME_ON >= TIME_OFF AND (@TIME_COMPARE <= @TIME_MAX AND @TIME_COMPARE >= TIME_ON) AND (TIME_ON >= @TIME_MIN AND TIME_OFF <= @TIME_COMPARE))
/*
                        IF NOT EXISTS (SELECT * FROM #LIST_DEVICE_SETTING)
						BEGIN
	                        --DROP TABLE #LIST_DEVICE
	                        --DROP TABLE #LIST_DEVICE_SETTING
							RETURN
						END
*/
						-- LAY DANH SACH THIET BI KO CAN PHAI UPDATE VE OFF
						SELECT D.DEVICE_ID 
                        INTO #T_UPDATE_TEMP
                        FROM S_USER US
                        INNER JOIN (SELECT TOKEN_KEY,USER_ID FROM S_GARDEN WHERE TOKEN_KEY IS NOT NULL AND IS_SHEDULE = 1) G ON US.USER_ID = G.USER_ID
                        INNER JOIN (SELECT DEVICE_ID,TOKEN_KEY FROM S_DEVICE WHERE DEVICE_CATEGORY = 1) D ON G.TOKEN_KEY = D.TOKEN_KEY
						INNER JOIN S_DEVICE_CONTROL_DETAIL SETTING ON SETTING.DEVICE_ID = D.DEVICE_ID
                        WHERE US.PACKED_ID = '" + ConstantClass.PACKED_SHEDULE + @"' AND SETTING.PRIORITY = 0

                         -- UPDATE ALL TO OFF
                        UPDATE S_DEVICE_CONTROL_DETAIL SET VALUE = 'OFF' WHERE DEVICE_ID IN (SELECT DEVICE_ID FROM #T_UPDATE_TEMP)
                        UPDATE S
                        SET S.VALUE = 'ON'
                        FROM S_DEVICE_CONTROL_DETAIL S 
                        INNER JOIN 
                        ( SELECT DISTINCT A.DEVICE_ID FROM #LIST_DEVICE_SETTING A INNER JOIN #LIST_DEVICE B ON A.DEVICE_ID = B.DEVICE_ID 
                            WHERE A.DEVICE_ID NOT IN (
							    SELECT DEVICE_ID FROM D_SETTING_CONTROL WHERE TIME_OFF = @TIME_COMPARE
						    )
                        )
                        D ON S.DEVICE_ID = D.DEVICE_ID
                        WHERE S.DEVICE_ID IN (SELECT DEVICE_ID FROM #T_UPDATE_TEMP)

                        SELECT VALUE,DEVICE_ID
                        FROM S_DEVICE_CONTROL_DETAIL

                        --DROP TABLE #LIST_DEVICE
                        --DROP TABLE #LIST_DEVICE_SETTING
                        --DROP TABLE #T_UPDATE_TEMP

                    ";
                //int res = _dbContext.Database.ExecuteSqlCommand(sql);
                List<ControlRawData> lstRes = _dbContext.Database.SqlQuery<ControlRawData>(sql).ToList();

                if (lstRes != null)
                {
                    _hub.UpdateSheduleClient(lstRes);
                    // sent mail
                }
                else
                {
                    // send mail Error
                }
            }
            catch (Exception ex)
            {
                // sent mail

            }
        }

    }
}