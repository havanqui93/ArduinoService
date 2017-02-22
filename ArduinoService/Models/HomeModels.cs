using ArduinoService.DataContext;
using ArduinoService.DataModels;
using ArduinoService.Hubs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace ArduinoService.Models
{
    public class HomeModels
    {
        #region Varible
        private ioTEntities _dbContext = new ioTEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private CommonModel commonModel = new CommonModel();
        private CommonFunction commonFunction = new CommonFunction();
        UpdateValueSheduleHub _hubShedule = new UpdateValueSheduleHub();
        TrackingHub _hubtracking = new TrackingHub();
        string userID = String.Empty;
        #endregion

        #region Screen garden

        public List<GardenRawData> GetListGarden(string userId)
        {
            List<GardenRawData> result = new List<GardenRawData>();
            try
            {
                string sql = @"
                    SELECT GARDEN_NAME_VN AS GARDEN_NAME,
                    IMAGE,ACTIVE,UNO_TYPE,[END],
                    TOKEN_KEY = CASE WHEN TOKEN_KEY IS NULL OR RTRIM(LTRIM(TOKEN_KEY)) = '' THEN GARDEN_ID ELSE TOKEN_KEY END 
                    FROM S_GARDEN WHERE USER_ID = '" + userId + @"'
                    ORDER BY START ASC
                    ";
                result = _dbContext.Database.SqlQuery<GardenRawData>(sql).ToList();
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }
        public GardenRawData AddOrEditGarden(GardenRawData data)
        {
            try
            {
                var _data = _dbContext.S_GARDEN.Where(x => x.TOKEN_KEY == data.TOKEN_KEY || x.GARDEN_ID == data.TOKEN_KEY).FirstOrDefault();

                if (!commonFunction.IsNullOrEmpty(data.IMAGE)) {
                    data.IMAGE = ConvertBase64ToImage(data.IMAGE);
                }

                if (commonFunction.IsNullOrEmpty(data.TOKEN_KEY))
                {
                    _data = new S_GARDEN();
                    _data.GARDEN_ID = commonModel.GetAutoId("GARDEN_ID", "S_GARDEN");
                    _data.USER_ID = data.USER_ID;
                    _data.ACTIVE = 0; // 0 : chua kich hoat , 1 : da kich hoat.
                    _data.START = DateTime.Now;
                    _data.IS_SHEDULE = false;
                    _data.IS_AUTO = false;
                    _data.UNO_TYPE = data.UNO_TYPE;

                    data.GARDEN_ID = _data.GARDEN_ID;
                    data.IS_ADD = true;
                    _dbContext.S_GARDEN.Add(_data);
                }
                else
                {
                    data.IS_ADD = false;
                    data.GARDEN_ID = commonFunction.IsNullOrEmpty(_data.TOKEN_KEY) ? _data.GARDEN_ID : _data.TOKEN_KEY;
                }

                _data.GARDEN_NAME_VN = data.GARDEN_NAME;
                _data.GARDEN_NAME_EN = data.GARDEN_NAME;
                _data.IMAGE = String.IsNullOrEmpty(data.IMAGE) ? _data.IMAGE : data.IMAGE;
                _data.STATUS = ConstantClass.LIVE;
                _data.ADDRESS = data.ADDRESS;
                _data.LATITUDE = data.LATITUDE;
                _data.LONGITUDE = data.LONGITUDE;
                _data.ACREAGE = data.ACREAGE;
                _data.DESCRIPTION = data.DESCRIPTION;

                _dbContext.SaveChanges();

                data.TOKEN_KEY = _data.TOKEN_KEY;
                data.GARDEN_NAME = _data.GARDEN_NAME_VN;
                data.UNO_TYPE = _data.UNO_TYPE;
                data.IMAGE = _data.IMAGE;
            }
            catch (Exception ex)
            {
                data = null;
                logger.Error(ex);
            }
            return data;
        }

        private string ConvertBase64ToImage(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            string fullOutputPath = "/Content/UpLoad";

            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }

            image.Save(fullOutputPath, System.Drawing.Imaging.ImageFormat.Png);
            return "";
        }

        public GardenRawData GetGardenById(string tokenkey)
        {
            GardenRawData res = new GardenRawData();
            try
            {
                string sql = @"
					SELECT
						TOKEN_KEY = CASE WHEN TOKEN_KEY IS NULL OR RTRIM(LTRIM(TOKEN_KEY)) = '' THEN GARDEN_ID ELSE TOKEN_KEY END,
						GARDEN_NAME_EN AS GARDEN_NAME,
						IMAGE,
						UNO_TYPE,
						CAST(ISNULL(LATITUDE,0.0) AS decimal(18,10)) AS LATITUDE,
						CAST(ISNULL(LONGITUDE,0.0) AS decimal(18,10)) AS LONGITUDE,
						ADDRESS,
						ISNULL(ACREAGE,0) AS ACREAGE,
						DESCRIPTION,
						D.ARDUINO_TYPE_NAME AS UNO_TYPE_NAME,
                        CONVERT(VARCHAR(20),START,111) AS START_GARDEN,
						END_GARDEN = CASE WHEN [END] IS NULL THEN CONVERT(VARCHAR(20),START,111) ELSE CONVERT(VARCHAR(20),[END],111) END,
                        CAST(IS_SHEDULE AS INT) AS IS_SHEDULE
					FROM S_GARDEN S
                    INNER JOIN S_ARDUINO_TYPE D ON S.UNO_TYPE = D.ARDUINO_TYPE_ID
					WHERE GARDEN_ID = '" + tokenkey + @"' OR TOKEN_KEY = '" + tokenkey + @"'
                    ";
                res = _dbContext.Database.SqlQuery<GardenRawData>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }
        public bool ActiveGarden(string gardenID, string tokenkey)
        {
            bool result = false;
            try
            {
                var _data = _dbContext.S_GARDEN.FirstOrDefault(x => x.GARDEN_ID == gardenID);
                if (_data != null)
                {
                    // update garden and all device
                    string sql = @"
                        DECLARE @COUNT_TOKEN INT
                        SET @COUNT_TOKEN =  (SELECT COUNT(*) FROM S_CODE WHERE CODE_VALUE = '" + tokenkey + @"' AND STATUS = 1)
                        IF(@COUNT_TOKEN = 1)
                            BEGIN
                            UPDATE S_CODE SET STATUS = 0 WHERE CODE_VALUE = '" + tokenkey + @"'
                            UPDATE S_GARDEN SET TOKEN_KEY = '" + tokenkey + "',ACTIVE = " + ConstantClass.LIVE + @" WHERE GARDEN_ID = '" + gardenID + @"'
                            UPDATE S_DEVICE SET TOKEN_KEY = '" + tokenkey + "' WHERE TOKEN_KEY = '" + gardenID + @"'
                            SELECT 1;
                            END
                        ELSE
                            BEGIN
                            SELECT 0;
                            END
                        ";
                    int res = _dbContext.Database.SqlQuery<int>(sql).FirstOrDefault();
                    result = (res == 1 ? true : false);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }
        public GardenRawData EditGarden(GardenRawData data)
        {
            try
            {
                var _data = _dbContext.S_GARDEN.FirstOrDefault(x => x.TOKEN_KEY == data.TOKEN_KEY || x.GARDEN_ID == data.TOKEN_KEY);
                if (_data != null)
                {
                    _data.GARDEN_NAME_VN = String.IsNullOrEmpty(data.GARDEN_NAME) ? _data.GARDEN_NAME_VN : data.GARDEN_NAME;
                    _data.IMAGE = String.IsNullOrEmpty(data.IMAGE) ? _data.IMAGE : data.IMAGE;
                    _data.UNO_TYPE = data.UNO_TYPE;
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                data = null;
            }
            return data;
        }
        public bool DeleteGarden(string tokenkey)
        {
            bool result = false;
            try
            {
                string sqlDel = @"
                    DELETE S_GARDEN WHERE TOKEN_KEY = '" + tokenkey + @"' OR GARDEN_ID = '" + tokenkey + @"'
                    DELETE S_CODE WHERE CODE_VALUE = '" + tokenkey + @"'
                    ";
                string sql = @"
                    BEGIN TRY
                    BEGIN TRANSACTION
                        " + sqlDel + @"
                        SELECT " + ConstantClass.IS_SUCCESS + @"
                    COMMIT
                    END TRY
                    BEGIN CATCH
                        SELECT " + ConstantClass.IS_FAILD + @"
                    ROLLBACK
                    END CATCH
                    ";
                int res = _dbContext.Database.SqlQuery<int>(sql).FirstOrDefault();
                result = (res == ConstantClass.IS_SUCCESS ? true : false);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }
        public string GetListUnoType(string strSelected)
        {
            string result = string.Empty;
            try
            {
                string sql = @"
                    SELECT STUFF((SELECT '<option value=' + CAST(ARDUINO_TYPE_ID AS VARCHAR(10)) + ' '
	                + (CASE WHEN ARDUINO_TYPE_ID = '" + strSelected + @"' THEN 'selected' ELSE '' END)+
	                 ' >' + ARDUINO_TYPE_NAME + '</option>'
	                FROM S_ARDUINO_TYPE S
	                FOR XML PATH(''), TYPE ).value('.', 'varchar(max)') , 1, 0, '')
                    ";
                result = _dbContext.Database.SqlQuery<string>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        #endregion end screen garden

        #region Screen control
        /// <summary>
        /// Lay danh sach device control
        /// </summary>
        /// <param name="tokenkey"></param>
        /// <returns></returns>
        public List<ControlRawData> GetListControl(string tokenkey)
        {
            List<ControlRawData> result = new List<ControlRawData>();
            try
            {
                string sql = @"
                    SELECT D.DEVICE_ID,DEVICE_NAME_VN AS DEVICE_NAME,VALUE,D.PIN_ID
                    , IS_FULL = (
	                    CASE WHEN((SELECT COUNT(*) FROM S_DEVICE WHERE TOKEN_KEY = '" + tokenkey + @"' AND DEVICE_CATEGORY = " + ConstantClass.CONTROL + @") 
	                    >= (SELECT COUNT(*) FROM S_ARDUINO_TYPE_DETAIL WHERE CATEGORY_ID = 1 AND ARDUINO_TYPE_ID = (SELECT UNO_TYPE FROM S_GARDEN WHERE TOKEN_KEY = '" + tokenkey + @"')))
	                    THEN CAST('TRUE'AS BIT) ELSE CAST('FALSE'AS BIT) END
                    )
                    FROM S_DEVICE D
                    JOIN S_DEVICE_CONTROL_DETAIL C
                    ON D.DEVICE_ID = C.DEVICE_ID
                    WHERE D.TOKEN_KEY = '" + tokenkey + @"'
                    ";
                result = _dbContext.Database.SqlQuery<ControlRawData>(sql).ToList();
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Kiem tra xem khu vuon co full device chua?
        /// </summary>
        /// <param name="tokenkey"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public bool IsFullControl(string tokenkey, int category)
        {
            bool result = false;
            try
            {
                string sql = @"
                    IF((SELECT COUNT(*) FROM S_DEVICE WHERE TOKEN_KEY = '" + tokenkey + @"' AND DEVICE_CATEGORY = " + category + @") 
                    >= (SELECT COUNT(*) FROM S_ARDUINO_TYPE_DETAIL WHERE CATEGORY_ID = " + category + @" AND ARDUINO_TYPE_ID = (SELECT UNO_TYPE FROM S_GARDEN WHERE TOKEN_KEY = '" + tokenkey + @"')))
                    SELECT CAST(1 AS BIT) ELSE SELECT CAST(0 AS BIT)
                    ";
                result = _dbContext.Database.SqlQuery<bool>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Get combobox list pin in uno
        /// </summary>
        /// <param name="strSelected"></param>
        /// <param name="tokenkey"></param>
        /// <returns></returns>
        public string GetListPin(string strSelected, string tokenkey, string device_category)
        {
            string result = string.Empty;
            try
            {
                string sql = string.Empty;
                if (commonFunction.IsNullOrEmpty(strSelected))
                {
                    sql = @"
                    SELECT STUFF((SELECT '<option value='+PIN+' '
                    + (CASE WHEN PIN = '" + strSelected + @"' THEN 'selected' ELSE '' END)+
                     ' >' + PIN + '</option>'
                    FROM S_ARDUINO_TYPE_DETAIL D
                    INNER JOIN S_GARDEN G ON D.ARDUINO_TYPE_ID = G.UNO_TYPE
					LEFT JOIN (SELECT * FROM S_DEVICE WHERE TOKEN_KEY = '" + tokenkey + @"') DE ON D.PIN = DE.PIN_ID
                    WHERE G.TOKEN_KEY = '" + tokenkey + @"'
                    AND D.CATEGORY_ID = '" + device_category + @"' AND DE.PIN_ID IS NULL
                    FOR XML PATH(''), TYPE ).value('.', 'varchar(max)') , 1, 0, '')
                    ";
                }
                else
                {
                    sql = @"
                        SELECT STUFF((SELECT '<option value=' + PIN + ' '
                        + (CASE WHEN PIN = '" + strSelected + @"' THEN 'selected' ELSE '' END)+
                         ' >' + PIN + '</option>'
                        FROM S_ARDUINO_TYPE_DETAIL D
                        FOR XML PATH(''), TYPE ).value('.', 'varchar(max)') , 1, 0, '')
                        ";
                }
                result = _dbContext.Database.SqlQuery<string>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Cap nhat gia tri cua control : ON/OFF
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateValueControl(ControlRawData model)
        {
            bool bResult = false;
            try
            {
                // udpate value control
                var module = _dbContext.S_DEVICE_CONTROL_DETAIL.FirstOrDefault(x => x.DEVICE_ID == model.DEVICE_ID);
                if (module != null)
                {
                    module.VALUE = model.VALUE;
                    module.TIME_UPDATE = DateTime.Now;
                    module.PRIORITY = true;

                    _dbContext.SaveChanges();
                    bResult = true;
                }
            }
            catch (Exception ex)
            {
                bResult = false;
                logger.Error(ex);
            }
            return bResult;
        }

        /// <summary>
        /// Them/sua thiet bi
        /// </summary>
        /// <param name="namecontrol"></param>
        /// <param name="tokenkey"></param>
        /// <param name="deviceid"></param>
        /// <param name="pinid"></param>
        /// <returns></returns>
        public bool AddOrEditControl(string namecontrol, string tokenkey, string deviceid, string pinid)
        {
            bool result = false;
            try
            {
                string autoid = commonModel.GetAutoId("DEVICE_ID", "S_DEVICE");
                string sql = @"
                    IF(EXISTS(SELECT * FROM S_DEVICE WHERE DEVICE_ID = '" + deviceid + @"'))
                    -- UPDATE
                    BEGIN
	                    UPDATE S_DEVICE SET DEVICE_NAME_EN = N'" + namecontrol + @"',DEVICE_NAME_VN = N'" + namecontrol + @"'
	                    WHERE DEVICE_ID = '" + deviceid + @"'
                    END
                    ELSE
                    -- ADD
                    BEGIN
	                    INSERT INTO S_DEVICE(DEVICE_ID,TOKEN_KEY,DEVICE_NAME_EN,DEVICE_NAME_VN,DEVICE_CATEGORY,STATUS,PIN_ID)
	                    VALUES ('" + autoid + @"','" + tokenkey + @"',N'" + namecontrol + @"',N'" + namecontrol + @"'," + ConstantClass.CONTROL + @"," + ConstantClass.LIVE + @",'" + pinid + @"')
	                    INSERT INTO S_DEVICE_CONTROL_DETAIL(DEVICE_ID,VALUE,TIME_UPDATE,PRIORITY)
	                    VALUES ('" + autoid + @"','" + ConstantClass.VALUE_INIT_CONTROL_DEFAULT + @"',GETDATE(),0)
                        INSERT INTO D_SETTING_CONTROL([DEVICE_ID],[TIME_ON],[TIME_OFF])
                        VALUES('" + autoid + @"','06:00','18:00')
                    END
                    ";
                string sqlTrans = @"
                    BEGIN TRY
                    BEGIN TRANSACTION
                        " + sql + @"
                        SELECT " + ConstantClass.IS_SUCCESS + @"
                    COMMIT
                    END TRY
                    BEGIN CATCH
                        SELECT " + ConstantClass.IS_FAILD + @"
                    ROLLBACK
                    END CATCH
                    ";

                result = (_dbContext.Database.SqlQuery<int>(sqlTrans).FirstOrDefault() == ConstantClass.IS_SUCCESS ? true : false);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Delete device
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteDevice(string id)
        {
            bool result = false;
            try
            {
                string sql = @"
                    SELECT D2.DEVICE_ID 
                    INTO #TEMP_DELETE
                    FROM S_DEVICE D1
                    INNER JOIN S_DEVICE D2 ON D1.GROUP_SENSOR_ID = D2.GROUP_SENSOR_ID
                    AND D1.PIN_ID = D2.PIN_ID AND D1.DEVICE_ID = '" + id + @"'
                    ORDER BY CAST(D2.DEVICE_ID AS INT)

                    IF((SELECT COUNT(DEVICE_ID) FROM #TEMP_DELETE) > 1)
                    BEGIN
	                    DELETE S_DEVICE WHERE DEVICE_ID IN (SELECT DEVICE_ID FROM #TEMP_DELETE)
                        DELETE S_DEVICE_CONTROL_DETAIL WHERE DEVICE_ID IN (SELECT DEVICE_ID FROM #TEMP_DELETE)
                        DELETE D_DEVICE_SENSOR_DETAIL WHERE DEVICE_ID IN (SELECT DEVICE_ID FROM #TEMP_DELETE)
                        DELETE D_DEVICE_SENSOR_HT WHERE DEVICE_ID IN (SELECT DEVICE_ID FROM #TEMP_DELETE)
                        DELETE D_DEVICE_SENSOR_LIGHT WHERE DEVICE_ID IN (SELECT DEVICE_ID FROM #TEMP_DELETE)
                        DELETE D_DEVICE_SENSOR_MOISTURE WHERE DEVICE_ID IN (SELECT DEVICE_ID FROM #TEMP_DELETE)
                        DELETE D_SETTING_CONTROL WHERE DEVICE_ID IN (SELECT DEVICE_ID FROM #TEMP_DELETE)
                    END

                    DELETE S_DEVICE WHERE DEVICE_ID = '" + id + @"'
                    DELETE S_DEVICE_CONTROL_DETAIL WHERE DEVICE_ID = '" + id + @"'
                    DELETE D_DEVICE_SENSOR_DETAIL WHERE DEVICE_ID = '" + id + @"'
                    DELETE D_DEVICE_SENSOR_HT WHERE DEVICE_ID = '" + id + @"'
                    DELETE D_DEVICE_SENSOR_LIGHT WHERE DEVICE_ID = '" + id + @"'
                    DELETE D_DEVICE_SENSOR_MOISTURE WHERE DEVICE_ID = '" + id + @"'
                    DELETE D_SETTING_CONTROL WHERE DEVICE_ID = '" + id + @"'
                    ";
                _dbContext.Database.ExecuteSqlCommand(sql);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        #endregion

        #region Screen tracking sensor
        /// <summary>
        /// Them moi control
        /// </summary>
        /// <param name="namecontrol">device name</param>
        /// <param name="category">1:</param>
        /// <returns></returns>
        public bool AddNewControlTracking(string device_id, string namecontrol, string tokenkey, int category, int groupsensor, int unit, string pinid)
        {
            bool result = false;
            try
            {
                var module = _dbContext.S_DEVICE.FirstOrDefault(x => x.DEVICE_ID == device_id);
                if (module == null) // add new
                {
                    S_DEVICE _device = new S_DEVICE();
                    _device.DEVICE_ID = commonModel.GetAutoId("DEVICE_ID", "S_DEVICE");
                    _device.TOKEN_KEY = tokenkey;
                    _device.DEVICE_NAME_EN = namecontrol;
                    _device.DEVICE_NAME_VN = namecontrol;
                    _device.DEVICE_CATEGORY = category;
                    _device.GROUP_SENSOR_ID = groupsensor;
                    _device.UNIT = unit;
                    _device.PIN_ID = pinid;
                    _device.STATUS = ConstantClass.LIVE;
                    _dbContext.S_DEVICE.Add(_device);

                    switch (category)
                    {
                        case 1: // dieu khien
                            S_DEVICE_CONTROL_DETAIL _devicedetail = new S_DEVICE_CONTROL_DETAIL();
                            _devicedetail.DEVICE_ID = _device.DEVICE_ID;
                            _devicedetail.VALUE = "OFF";
                            _devicedetail.TIME_UPDATE = DateTime.Now;
                            _dbContext.S_DEVICE_CONTROL_DETAIL.Add(_devicedetail);
                            break;
                    }
                }
                else
                {
                    module.DEVICE_NAME_VN = namecontrol;
                    module.DEVICE_NAME_VN = namecontrol;
                    module.UNIT = unit;
                }

                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// Lay nhom thiet bi ( dung cho group HT)
        /// </summary>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public List<string> GetGroupDeviceID(string deviceID)
        {
            List<string> result = new List<string>();
            try
            {
                string sql = @"SELECT D2.DEVICE_ID FROM S_DEVICE D1
                                INNER JOIN S_DEVICE D2 ON D1.GROUP_SENSOR_ID = D2.GROUP_SENSOR_ID
                                AND D1.PIN_ID = D2.PIN_ID AND D1.DEVICE_ID = '" + deviceID + @"'
                                ORDER BY CAST(D2.DEVICE_ID AS INT)";
                result = _dbContext.Database.SqlQuery<string>(sql).ToList();
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        /// <summary>
        /// Lay danh sach thiet bi sensor
        /// </summary>
        /// <param name="tokenkey"></param>
        /// <returns></returns>
        public List<TrackingRawData> GetListDevice(string tokenkey)
        {
            string sql = @"
                SELECT DEVICE_NAME = DEVICE_NAME_VN,DEVICE_ID FROM S_DEVICE
	            WHERE TOKEN_KEY = '" + tokenkey + @"'
	            AND DEVICE_CATEGORY = " + ConstantClass.SENSOR + @"
	            ORDER BY CAST(DEVICE_ID AS INT) ASC
                ";
            return _dbContext.Database.SqlQuery<TrackingRawData>(sql).ToList();
        }
        /// <summary>
        /// Loc qua tung thiet bi lay danh sach gia tri tung thiet bi
        /// </summary>
        /// <param name="tokenkey"></param>
        /// <returns></returns>
        public List<List<string>> GetListDataDetails(string tokenkey, string dateinput)
        {
            List<List<string>> lstResult = new List<List<string>>();
            try
            {
                dateinput = commonFunction.IsNullOrEmpty(dateinput) ? DateTime.Now.ToString("yyyy/MM/dd") : commonModel.ConverDDMMYYYY_YYYYMMDD(dateinput);

                string sql = @"
                    SELECT DEVICE_ID FROM S_DEVICE
                    WHERE TOKEN_KEY = '" + tokenkey + @"'
                    AND DEVICE_CATEGORY = " + ConstantClass.SENSOR + @"
                    ORDER BY CAST(DEVICE_ID AS INT) ASC
                    ";

                List<string> lstDevices = _dbContext.Database.SqlQuery<string>(sql).ToList();
                string firstDevice = (lstDevices.Count != 0 ? lstDevices[0] : "");

                if (String.IsNullOrEmpty(firstDevice))
                    return lstResult;
                else
                {
                    sql = @"
                        DECLARE @Date DATETIME
                        --SELECT @Date = CONVERT(VARCHAR(10),GETDATE(),111)
                        SELECT @Date = (SELECT CONVERT(VARCHAR(20),GETDATE() + 1,111) + ' 00:00')

                        ;WITH Dates AS
                        (
                            SELECT DATEPART(HOUR,DATEADD(HOUR,-1,@Date)) [Hour], 
                              DATEADD(HOUR,-1,@Date) [Date], 1 Num
                            UNION ALL
                            SELECT DATEPART(HOUR,DATEADD(HOUR,-1,[Date])), 
                              DATEADD(HOUR,-1,[Date]), Num+1
                            FROM Dates
                            WHERE Num <= 23
                        )
                        SELECT 
                        CONVERT(VARCHAR(5),[Date],108)
                        FROM Dates
                        ORDER BY [Date] ASC
                        ";
                    List<string> lstTime = _dbContext.Database.SqlQuery<string>(sql).ToList();
                    lstResult.Add(lstTime);
                }

                for (int i = 0; i < lstDevices.Count; i++)
                {
                    List<string> lst = GetItemDetais(lstDevices[i], dateinput);
                    lstResult.Add(lst);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return lstResult;
        }

        /// <summary>
        /// Lay danh sach gia tri tung device sensor
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="dateinput">Default : YYYY/MM/DD</param>
        /// <returns></returns>
        private List<string> GetItemDetais(string deviceid, string dateinput)
        {
            List<string> lstItems = new List<string>();
            try
            {
                // Check using table
                string tablename = string.Empty;
                if (dateinput == DateTime.Now.ToString("yyyy/MM/dd"))
                    tablename = "D_DEVICE_SENSOR_DETAIL";
                else
                    tablename = "( SELECT * FROM " + Gettablename(deviceid) + " WHERE CONVERT(VARCHAR(10), TIME_UPDATE, 111) = @DateInput AND DEVICE_ID = '" + deviceid + @"' )";

                string sql = @"
                    DECLARE @DateInput DATETIME = '" + dateinput + @"' -- yyyy/mm/dd
                    DECLARE @Date DATETIME
                    --SELECT @Date = CONVERT(VARCHAR(10),GETDATE(),111)
                    SELECT @Date = (SELECT CONVERT(VARCHAR(20),@DateInput + 1,111) + ' 00:00')

                    ;WITH Dates AS
                    (
                        SELECT DATEPART(HOUR,DATEADD(HOUR,-1,@Date)) [Hour], 
                          DATEADD(HOUR,-1,@Date) [Date], 1 Num
                        UNION ALL
                        SELECT DATEPART(HOUR,DATEADD(HOUR,-1,[Date])), 
                          DATEADD(HOUR,-1,[Date]), Num+1
                        FROM Dates
                        WHERE Num <= 23
                    )
                    SELECT [Hour], [Date]
                    into #TEMP
                    FROM Dates

                    SELECT
                    CASE WHEN A.VALUE IS NULL THEN 'NULL' ELSE A.VALUE + ' ' + A.UNIT END
                    FROM #TEMP T
                    LEFT JOIN
                    (
	                    SELECT D.VALUE,D.TIME_UPDATE,U.UNIT
                        FROM " + tablename + @" D
                        INNER JOIN
                        (
                        SELECT U.UNIT_NAME UNIT,D.DEVICE_ID FROM S_DEVICE D
                        LEFT JOIN S_UNIT U ON D.UNIT = U.UNIT_ID
                        WHERE DEVICE_ID = '" + deviceid + @"'
                        ) U ON U.DEVICE_ID = D.DEVICE_ID
                        WHERE D.DEVICE_ID = '" + deviceid + @"' 
                     ) A ON (CONVERT(VARCHAR(10),A.TIME_UPDATE,111) + CONVERT(VARCHAR(10),DATEPART(HOUR,A.TIME_UPDATE))) 
					 --LIKE T.Date    
					 = (CONVERT(VARCHAR(10),T.Date,111) + CONVERT(VARCHAR(10),DATEPART(HOUR,T.Date)))    
                    ORDER BY [Date]
                    ";
                lstItems = _dbContext.Database.SqlQuery<string>(sql).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return lstItems;
        }

        /// <summary>
        /// lAY TEN TABLE
        /// </summary>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        private string Gettablename(string deviceid)
        {
            string tableResult = string.Empty;
            int? groupid = 1;
            var datadevice = _dbContext.S_DEVICE.FirstOrDefault(x => x.DEVICE_ID == deviceid);
            if (datadevice != null)
                groupid = datadevice.GROUP_SENSOR_ID;

            switch (groupid)
            {
                case 1:
                    tableResult = ConstantClass.TABLE_GROUP_HT;
                    break;
                case 2:
                    tableResult = ConstantClass.TABLE_GROUP_MOISTURE;
                    break;
                case 3:
                    tableResult = ConstantClass.TABLE_GROUP_LIGHT;
                    break;
            }
            return tableResult;
        }

        /// <summary>
        /// Lay danh sach don vi
        /// </summary>
        /// <returns></returns>
        public List<UnitRawData> GetListUnit()
        {
            List<UnitRawData> result = new List<UnitRawData>();
            try
            {
                string sql = "SELECT GROUP_SENSOR_ID AS ID,GROUP_SENSOR_NAME AS NAME FROM S_GROUP_SENSOR";
                result = _dbContext.Database.SqlQuery<UnitRawData>(sql).ToList();
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// Kiem tra ten cua thiet bi sensor co ton tai chua?
        /// </summary>
        /// <param name="strname"></param>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        public bool CheckExistsNameTracking(string strname, string deviceid, string tokenkey)
        {
            bool result = false;
            try
            {
                string sql = string.Empty;

                if (commonFunction.IsNullOrEmpty(deviceid) == false)
                {
                    sql = @"
                    SELECT D2.DEVICE_ID 
                    INTO #TEMP_DELETE
                    FROM S_DEVICE D1
                    INNER JOIN S_DEVICE D2 ON D1.GROUP_SENSOR_ID = D2.GROUP_SENSOR_ID
                    AND D1.PIN_ID = D2.PIN_ID AND D1.DEVICE_ID = '" + deviceid + @"'
                    ORDER BY CAST(D2.DEVICE_ID AS INT)

                    IF((SELECT COUNT(*) FROM #TEMP_DELETE) > 1)
                    BEGIN
	                    DECLARE @DEVICE_ID_1 VARCHAR(20) = (SELECT TOP 1 DEVICE_ID FROM #TEMP_DELETE)
	                    DELETE #TEMP_DELETE WHERE DEVICE_ID = @DEVICE_ID_1
	                    DECLARE @DEVICE_ID_2 VARCHAR(20) = (SELECT TOP 1 DEVICE_ID FROM #TEMP_DELETE)

	                    DROP TABLE #TEMP_DELETE
	                    IF(
	                    (SELECT COUNT(*) FROM S_DEVICE WHERE DEVICE_CATEGORY = 2 AND DEVICE_NAME_VN = N'" + strname + @"'  
	                    AND (DEVICE_ID <> @DEVICE_ID_1 AND DEVICE_ID <> @DEVICE_ID_2) AND TOKEN_KEY = '" + tokenkey + @"') > 0)
		                    SELECT 1
	                    ELSE 
		                    SELECT 0
	                    RETURN;
                    END
                    ELSE
	                    SELECT COUNT(*) FROM S_DEVICE WHERE DEVICE_CATEGORY = 2 AND DEVICE_NAME_VN = N'" + strname + @"'
	                    AND DEVICE_ID <> '" + deviceid + @"' AND TOKEN_KEY = '" + tokenkey + @"'
                        ";
                }
                else
                {
                    sql = "SELECT COUNT(*) FROM S_DEVICE WHERE DEVICE_CATEGORY = 2 AND DEVICE_NAME_VN = N'" + strname + @"' AND TOKEN_KEY = '" + tokenkey + @"'";
                }
                int res = _dbContext.Database.SqlQuery<int>(sql).FirstOrDefault();
                result = (res > 0 ? true : false);
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error(ex);
            }
            return result;
        }
        /// <summary>
        /// Lay thong tin cua thiet bi (sensor)
        /// </summary>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        public List<DeviceRowData> LoadInfoDevice(string deviceid)
        {
            List<DeviceRowData> result = new List<DeviceRowData>();
            try
            {
                string sql = @"SELECT D2.DEVICE_ID,D2.GROUP_SENSOR_ID,D2.DEVICE_NAME_VN AS DEVICE_NAME,D2.UNIT,D2.PIN_ID FROM S_DEVICE D1
                                INNER JOIN S_DEVICE D2 ON D1.GROUP_SENSOR_ID = D2.GROUP_SENSOR_ID
                                AND D1.PIN_ID = D2.PIN_ID AND D1.DEVICE_ID = '" + deviceid + @"'
                                ORDER BY CAST(D2.DEVICE_ID AS INT)
                                ";
                result = _dbContext.Database.SqlQuery<DeviceRowData>(sql).ToList();
            }
            catch (Exception ex)
            {
                result = null;
                logger.Error(ex);
            }
            return result;
        }

        #endregion

        #region Screen chart
        /// <summary>
        /// Lay danh sach thiet bi va foreach tung thiet bi de ve bieu do.
        /// </summary>
        /// <param name="tokenkey"></param>
        /// <param name="datechart"></param>
        /// <returns></returns>
        public List<ChartRowData> GetListDataChartDetails(string tokenkey, string dateinput)
        {
            List<ChartRowData> lstResult = new List<ChartRowData>();
            try
            {
                dateinput = commonFunction.IsNullOrEmpty(dateinput) ? DateTime.Now.ToString("yyyy/MM/dd") : commonModel.ConverDDMMYYYY_YYYYMMDD(dateinput);

                string sql = @"
                    SELECT CHART_ID = DEVICE_ID,CHART_NAME = DEVICE_NAME_VN,GROUP_SENSOR_ID,UNIT_NAME
                    FROM S_DEVICE INNER JOIN S_UNIT U ON S_DEVICE.UNIT = U.UNIT_ID WHERE DEVICE_CATEGORY = 2 
                    AND TOKEN_KEY = '" + tokenkey + @"' ORDER BY CAST(DEVICE_ID AS INT) ASC
                    ";

                lstResult = _dbContext.Database.SqlQuery<ChartRowData>(sql).ToList();
                for (int i = 0; i < lstResult.Count; i++)
                {
                    lstResult[i].CHART_DATA = GetItemChartDetais(lstResult[i].CHART_ID, dateinput);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return lstResult;
        }
        /// <summary>
        /// Lay danh sach tung item de ve bieu do
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="datechart"></param>
        /// <returns></returns>
        private List<ChartData> GetItemChartDetais(string deviceid, string dateinput)
        {
            List<ChartData> lstItems = new List<ChartData>();
            try
            {
                #region chart not customize
                /*
                string sql = @"
                    DECLARE @TABLE_TEMP_24H TABLE (ID VARCHAR(20))
                    DECLARE @TODAY VARCHAR(20) = CONVERT(VARCHAR(10),GETDATE(),111)

                    DECLARE @I INT = 0
                    WHILE(@I <= 23)
                    BEGIN
	                    INSERT INTO @TABLE_TEMP_24H VALUES (@I)
	                    SET @I = @I + 1
                    END

                    DECLARE @DATE_CHART INT = " + datechart + @"
					DECLARE @DEVICE_ID VARCHAR(10) = '" + deviceid + @"' -- DEVICE_ID

					IF(@DATE_CHART = 1) -- THONG KE NGAY HIEN TAI
					BEGIN
						SELECT ISNULL(B.VALUE,0) AS VALUE,A.ID AS [DAY] FROM @TABLE_TEMP_24H A
						LEFT JOIN
						(
							SELECT CAST(VALUE AS VARCHAR(20)) as VALUE, CAST(DATEPART(hh,TIME_UPDATE) AS VARCHAR(20)) as [DAY] 
							FROM D_DEVICE_SENSOR_DETAIL WHERE DEVICE_ID = @DEVICE_ID AND CONVERT(VARCHAR(10),TIME_UPDATE,111) = @TODAY
						) B ON A.ID = B.DAY   
                    END
					--================================ THONG KE CHART THEO TUAN/THANG ===========================
					
					DECLARE @TABLE_DATA_CHART TABLE (DEVICE_ID VARCHAR(50),VALUE VARCHAR(10),TIME_UPDATE DATETIME)
					DECLARE @DATE DATETIME = GETDATE()
					DECLARE @NUM_PART INT

					DECLARE @GROUP_SENSOR_ID INT = (SELECT GROUP_SENSOR_ID FROM S_DEVICE WHERE DEVICE_ID = @DEVICE_ID)
					IF(@GROUP_SENSOR_ID = 1)
						INSERT INTO @TABLE_DATA_CHART 
						SELECT DEVICE_ID,VALUE,TIME_UPDATE FROM D_DEVICE_SENSOR_HT WHERE DEVICE_ID = @DEVICE_ID
					IF(@GROUP_SENSOR_ID = 2)
						INSERT INTO @TABLE_DATA_CHART 
						SELECT DEVICE_ID,VALUE,TIME_UPDATE FROM D_DEVICE_SENSOR_MOISTURE WHERE DEVICE_ID = @DEVICE_ID
					IF(@GROUP_SENSOR_ID = 3)
						INSERT INTO @TABLE_DATA_CHART 
						SELECT DEVICE_ID,VALUE,TIME_UPDATE FROM D_DEVICE_SENSOR_LIGHT WHERE DEVICE_ID = @DEVICE_ID

					IF(@DATE_CHART = 2) -- THONG KE THEO TUAN HIEN TAI
					BEGIN
						SET @NUM_PART = (DATEPART(DW, @DATE) - 1)

						IF(@NUM_PART = 0)
							SET @NUM_PART = 1

						DECLARE @FIRST_DAY_OF_WEEK DATETIME = (SELECT DATEADD(DAY, 1-DATEPART(DW, @DATE), CONVERT(DATE,@DATE)))
						DECLARE @LAST_DAY_OF_WEEK DATETIME = (SELECT DATEADD(DAY, @NUM_PART-DATEPART(DW, @DATE), CONVERT(DATE,@DATE)))

						IF(@NUM_PART - 1 = 0)
							SET @NUM_PART = 2

						SELECT ISNULL(B.VALUE,0) AS VALUE,A.ID AS [DAY] FROM @TABLE_TEMP_24H A
						LEFT JOIN
						(
							SELECT CAST(SUM(CAST(VALUE AS INT)) / (@NUM_PART - 1) AS VARCHAR(20)) AS VALUE,CAST(DATEPART(hh,TIME_UPDATE) AS VARCHAR(20)) AS [DAY] 
							FROM @TABLE_DATA_CHART WHERE DEVICE_ID = @DEVICE_ID
							AND TIME_UPDATE >= @FIRST_DAY_OF_WEEK AND TIME_UPDATE <= @LAST_DAY_OF_WEEK
							GROUP BY CAST(DATEPART(hh,TIME_UPDATE) AS VARCHAR(20))
						) B ON A.ID = B.DAY   
                    END

					IF(@DATE_CHART = 3) -- THONG KE THEO THANG HIEN TAI
					BEGIN
						
						SET @NUM_PART = (DATEPART(DW, @DATE) - 1)

						IF(@NUM_PART = 0)
							SET @NUM_PART = 1

                        DECLARE @FIRST_DAY_OF_MONTH DATETIME = (SELECT DATEADD(MONTH, DATEDIFF(MONTH,0,@DATE),0))
                        DECLARE @LAST_DAY_OF_MONTH DATETIME = (SELECT DATEADD(DAY,-1,DATEADD(MONTH,1,@FIRST_DAY_OF_MONTH)))

						IF(@NUM_PART - 1 = 0)
							SET @NUM_PART = 2

						SELECT ISNULL(B.VALUE,0) AS VALUE,A.ID AS [DAY] FROM @TABLE_TEMP_24H A
						LEFT JOIN
						(
							SELECT CAST(SUM(CAST(VALUE AS INT)) / (@NUM_PART - 1) AS VARCHAR(20)) AS VALUE,CAST(DATEPART(hh,TIME_UPDATE) AS VARCHAR(20)) AS [DAY] 
							FROM @TABLE_DATA_CHART WHERE DEVICE_ID = @DEVICE_ID
							AND TIME_UPDATE >= @FIRST_DAY_OF_MONTH AND TIME_UPDATE <= @LAST_DAY_OF_MONTH
							GROUP BY CAST(DATEPART(hh,TIME_UPDATE) AS VARCHAR(20))
						) B ON A.ID = B.DAY   
                    END
   
                    ";
                    */
                #endregion

                // Check using table
                string tablename = string.Empty;
                if (dateinput == DateTime.Now.ToString("yyyy/MM/dd"))
                    tablename = "D_DEVICE_SENSOR_DETAIL";
                else
                    tablename = "( SELECT * FROM " + Gettablename(deviceid) + " WHERE CONVERT(VARCHAR(10), TIME_UPDATE, 111) = @DateInput AND DEVICE_ID = '" + deviceid + @"' )";

                string sql = @"
                    DECLARE @DateInput DATETIME = '" + dateinput + @"' -- yyyy/mm/dd
                    DECLARE @Date DATETIME
                    --SELECT @Date = CONVERT(VARCHAR(10),GETDATE(),111)
                    SELECT @Date = (SELECT CONVERT(VARCHAR(20),@DateInput + 1,111) + ' 00:00')

                    ;WITH Dates AS
                    (
                        SELECT DATEPART(HOUR,DATEADD(HOUR,-1,@Date)) [Hour], 
                          DATEADD(HOUR,-1,@Date) [Date], 1 Num
                        UNION ALL
                        SELECT DATEPART(HOUR,DATEADD(HOUR,-1,[Date])), 
                          DATEADD(HOUR,-1,[Date]), Num+1
                        FROM Dates
                        WHERE Num <= 23
                    )
                    SELECT [Hour], [Date]
                    into #TEMP
                    FROM Dates

                    SELECT
                    VALUE = ISNULL(A.VALUE,0),
					DAY = CAST(Hour AS VARCHAR(5))
                    FROM #TEMP T
                    LEFT JOIN
                    (
	                    SELECT D.VALUE,D.TIME_UPDATE,U.UNIT
                        FROM " + tablename + @" D
                        INNER JOIN
                        (
                        SELECT U.UNIT_NAME UNIT,D.DEVICE_ID FROM S_DEVICE D
                        LEFT JOIN S_UNIT U ON D.UNIT = U.UNIT_ID
                        WHERE DEVICE_ID = '" + deviceid + @"'
                        ) U ON U.DEVICE_ID = D.DEVICE_ID
                        WHERE D.DEVICE_ID = '" + deviceid + @"' 
                     ) A ON (CONVERT(VARCHAR(10),A.TIME_UPDATE,111) + CONVERT(VARCHAR(10),DATEPART(HOUR,A.TIME_UPDATE))) 
					 --LIKE T.Date    
					 = (CONVERT(VARCHAR(10),T.Date,111) + CONVERT(VARCHAR(10),DATEPART(HOUR,T.Date)))    
                    ORDER BY [Date]
                    ";

                lstItems = _dbContext.Database.SqlQuery<ChartData>(sql).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return lstItems;
        }

        #endregion

        #region Screen setting garden

        public List<SettingControlRawData> GetControlDataSetting(string tokenkey)
        {
            List<SettingControlRawData> lstResult = new List<SettingControlRawData>();
            try
            {
                string sql = @"SELECT DEVICE_ID,DEVICE_NAME = DEVICE_NAME_VN FROM S_DEVICE 
                                WHERE TOKEN_KEY = '" + tokenkey + @"' 
                                AND DEVICE_CATEGORY = 1 AND STATUS = 1";
                lstResult = _dbContext.Database.SqlQuery<SettingControlRawData>(sql).ToList();
                for (int i = 0; i < lstResult.Count; i++)
                {
                    sql = "SELECT SETTING_CONTROL_ID = CAST(SETTING_CONTROL_ID AS VARCHAR(20)),TIME_ON = CONVERT(VARCHAR(20),TIME_ON,108),TIME_OFF = CONVERT(VARCHAR(20),TIME_OFF,108) FROM D_SETTING_CONTROL WHERE DEVICE_ID = '" + lstResult[i].DEVICE_ID + @"'";
                    lstResult[i].LIST_SETTING = _dbContext.Database.SqlQuery<SettingControlDetailRawData>(sql).ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return lstResult;
        }

        /// <summary>
        /// Cap nhat tat mo shedule. ON/OFF shedule
        /// </summary>
        /// <param name="tokenkey"></param>
        /// <returns></returns>
        public int UpdateShedule(string tokenkey)
        {
            int result = 0;
            try
            {
                string sql = @"
                        DECLARE @USER_ID VARCHAR(50) = (SELECT USER_ID FROM S_GARDEN WHERE TOKEN_KEY = '" + tokenkey + @"')
                        IF((SELECT PACKED_ID FROM S_USER WHERE USER_ID = @USER_ID) = '" + ConstantClass.PACKED_DEFAULT + @"')
						BEGIN
							SELECT 2
						END
						ELSE
						BEGIN
                            DECLARE @IS_SHEDULE INT = (SELECT IS_SHEDULE FROM S_GARDEN WHERE TOKEN_KEY = '" + tokenkey + @"')
                            IF( @IS_SHEDULE = 1)
                                UPDATE S_GARDEN SET IS_SHEDULE = 0 WHERE TOKEN_KEY = '" + tokenkey + @"'
                            ELSE
                            BEGIN
                                UPDATE S_GARDEN SET IS_SHEDULE = 1 WHERE TOKEN_KEY = '" + tokenkey + @"'
                                UPDATE S_DEVICE_CONTROL_DETAIL SET PRIORITY = 0
                            END
                            SELECT CAST(@IS_SHEDULE AS INT)
                        END
                        ";
                result = _dbContext.Database.SqlQuery<int>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = -1;
                logger.Error(ex);
            }
            // Send client
            UpdateValueSheduleHub _hubShedule = new UpdateValueSheduleHub();
            _hubShedule.UpdateShedule(result.ToString());
            return result;
        }

        public bool SaveSettingControl(string strArr, string deviceid)
        {
            bool result = false;
            try
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                List<SettingControlDetailRawData> listSetting = jss.Deserialize<List<SettingControlDetailRawData>>(strArr);

                string sql = @" 
                    -- Table data full
                    DECLARE @DEVICE_ID VARCHAR(20) = '" + deviceid + @"'
                    CREATE TABLE #TABLE_DATA_FULL
                    (
                    SETTING_CONTROL_ID BIGINT,
                    DEVICE_ID VARCHAR(50),
                    TIME_ON TIME(7),
                    TIME_OFF TIME(7)
                    )

                    " + (listSetting.Count > 0 ? " INSERT INTO #TABLE_DATA_FULL VALUES " : "") + @"
                    
                    ";
                foreach (var itemx in listSetting)
                {
                    sql += "('" + itemx.SETTING_CONTROL_ID + @"'," + deviceid + @",'" + itemx.TIME_ON + @"','" + itemx.TIME_OFF + @"'),";
                }
                sql = sql.Substring(0, sql.Length - 1);

                sql += @"
                    -- DELETE DATA NOT IN TABLE FULL
                    DELETE FROM D_SETTING_CONTROL WHERE DEVICE_ID = @DEVICE_ID
                    AND SETTING_CONTROL_ID NOT IN
                    (
                    SELECT D.SETTING_CONTROL_ID 
                    FROM D_SETTING_CONTROL D INNER JOIN #TABLE_DATA_FULL F ON F.SETTING_CONTROL_ID = D.SETTING_CONTROL_ID
                    )

                    -- INSERT DATA NEW
                    INSERT INTO D_SETTING_CONTROL
                    SELECT DEVICE_ID,TIME_ON,TIME_OFF FROM #TABLE_DATA_FULL WHERE SETTING_CONTROL_ID = 0

                    -- UPDATE DATA OLD
                    UPDATE D
	                    SET D.TIME_ON = A.TIME_ON,
	                    D.TIME_OFF = A.TIME_OFF
                    FROM D_SETTING_CONTROL D
                    INNER JOIN (
                    SELECT SETTING_CONTROL_ID,DEVICE_ID,TIME_ON,TIME_OFF FROM #TABLE_DATA_FULL WHERE SETTING_CONTROL_ID != 0
                    ) A ON A.SETTING_CONTROL_ID = D.SETTING_CONTROL_ID

                    DROP TABLE #TABLE_DATA_FULL
                    ";

                // run transaction
                string sqlRun = @"
                    BEGIN TRY
                    BEGIN TRANSACTION
                        " + sql + @"
                        SELECT " + ConstantClass.IS_SUCCESS + @"
                    COMMIT
                    END TRY
                    BEGIN CATCH
                        SELECT " + ConstantClass.IS_FAILD + @"
                    ROLLBACK
                    END CATCH
                    ";
                int res = _dbContext.Database.SqlQuery<int>(sqlRun).FirstOrDefault();
                result = (res == ConstantClass.IS_SUCCESS ? true : false);
                if (result == true)
                {
                    string sqlSelect = @"
                        SELECT SETTING_CONTROL_ID = CAST(SETTING_CONTROL_ID AS VARCHAR(20)),
                        TIME_ON = CONVERT(VARCHAR(20),TIME_ON,108),
                        TIME_OFF = CONVERT(VARCHAR(20),TIME_OFF,108)
                        FROM D_SETTING_CONTROL WHERE DEVICE_ID = '" + deviceid + @"'
                    ";
                    List<SettingControlDetailRawData> lst = _dbContext.Database.SqlQuery<SettingControlDetailRawData>(sqlSelect).ToList();
                    _hubShedule.Send(lst,deviceid);
                }

            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        #endregion




        #region Api for UNO

        public string GetListDataControlUno(string token_key)
        {
            string result = String.Empty;
            try
            {
                // get control device
                result = "{\"ID\":\"" + token_key + "\",\"GET_DEVICES\":[{\"NO_DEVICES\":";
                string sql = "SELECT COUNT(*) FROM S_DEVICE WHERE TOKEN_KEY = '" + token_key + "' AND DEVICE_CATEGORY = 1";
                result += "\"" + _dbContext.Database.SqlQuery<int>(sql).FirstOrDefault() + "\",";

                sql = "SELECT STUFF((SELECT ',' +'\"' + CAST(CAST(D.DEVICE_ID AS INT) AS VARCHAR(MAX)) + '\":\"' + (CASE WHEN DC.VALUE = 'ON' THEN '1' ELSE '0' END) + '\"' FROM S_DEVICE D JOIN S_GARDEN G ON D.TOKEN_KEY = G.TOKEN_KEY JOIN S_DEVICE_CONTROL_DETAIL DC ON DC.DEVICE_ID = D.DEVICE_ID WHERE D.TOKEN_KEY = '" + token_key + "' AND D.DEVICE_CATEGORY = 1 FOR XML PATH('')), 1, 1, '') +'}]'";
                result += _dbContext.Database.SqlQuery<string>(sql).FirstOrDefault();

                // get sensor device
                result += ",\"GET_SENSORS\":[{\"HT\":[";
                // TH
                sql = "SELECT STUFF((SELECT ',' +'\"' + CAST(CAST(DEVICE_ID AS INT) AS VARCHAR(MAX)) + '\"' FROM S_DEVICE D INNER JOIN S_GROUP_SENSOR G ON D.GROUP_SENSOR_ID = G.GROUP_SENSOR_ID WHERE TOKEN_KEY = '" + token_key + "' AND DEVICE_CATEGORY = 2 AND G.GROUP_SENSOR_ID = 1 AND D.DEVICE_NAME_VN LIKE N'%Nhiệt độ%' FOR XML PATH('')), 1, 1, '')";
                result += _dbContext.Database.SqlQuery<string>(sql).FirstOrDefault() + "],\"MOISTURE\":[";

                // MOISTURE
                sql = "SELECT STUFF((SELECT ',' +'\"' + CAST(CAST(DEVICE_ID AS INT) AS VARCHAR(MAX)) + '\"' FROM S_DEVICE D INNER JOIN S_GROUP_SENSOR G ON D.GROUP_SENSOR_ID = G.GROUP_SENSOR_ID WHERE TOKEN_KEY = '" + token_key + "' AND DEVICE_CATEGORY = 2 AND G.GROUP_SENSOR_ID = 2 FOR XML PATH('')), 1, 1, '')";
                result += _dbContext.Database.SqlQuery<string>(sql).FirstOrDefault() + "],\"LIGHT\":[";

                // LIGHT
                sql = "SELECT STUFF((SELECT ',' +'\"' + CAST(CAST(DEVICE_ID AS INT) AS VARCHAR(MAX)) + '\"' FROM S_DEVICE D INNER JOIN S_GROUP_SENSOR G ON D.GROUP_SENSOR_ID = G.GROUP_SENSOR_ID WHERE TOKEN_KEY = '" + token_key + "' AND DEVICE_CATEGORY = 2 AND G.GROUP_SENSOR_ID = 3 FOR XML PATH('')), 1, 1, '')";
                result += _dbContext.Database.SqlQuery<string>(sql).FirstOrDefault() + "]}]}";

            }
            catch (Exception ex)
            {
                result = String.Empty;
            }
            return result;
        }


        public bool InsertDataTracking(List<ListValueTrackingUno> _collection)
        {
            bool result = false;
            List<ListValueTrackingUno> _newcolection = new List<ListValueTrackingUno>();
            try
            {
                char constanstkey = 'z';
                string sql = @" 
                    CREATE TABLE #TABLE_TEMP
                     (
                        DEVICE_ID VARCHAR(20),
                        VALUE VARCHAR(10),
                        TIME_UPDATE DATETIME
                     )
                    INSERT INTO #TABLE_TEMP VALUES ";

                foreach (var itemx in _collection)
                {
                    if (itemx.VALUE.Contains(constanstkey))
                        sql += "('" + itemx.DEVICE_ID + @"'," + itemx.VALUE.Split(constanstkey)[0] + @",GETDATE())," + "('" + commonModel.GetLastIdInGrouHT(itemx.DEVICE_ID) + @"'," + itemx.VALUE.Split(constanstkey)[1] + @",GETDATE()),";
                    else
                        sql += "('" + itemx.DEVICE_ID + @"'," + itemx.VALUE + @",GETDATE()),";
                }
                sql = sql.Substring(0, sql.Length - 1);
                sql += @"
                    -- FIND AND DELETE WHEN DOUBLE ROW IN HOUR
                    DELETE D_DEVICE_SENSOR_DETAIL
                    WHERE DEVICE_SENSOR_DETAIL_ID IN
                    (
                        SELECT B.DEVICE_SENSOR_DETAIL_ID
                        FROM #TABLE_TEMP A
                        INNER JOIN D_DEVICE_SENSOR_DETAIL B
                        ON A.DEVICE_ID = B.DEVICE_ID
                        AND CONVERT(VARCHAR(10), A.TIME_UPDATE, 101) + CONVERT(VARCHAR(20),DATEPART(HOUR,A.TIME_UPDATE)) =
                        CONVERT(VARCHAR(10), B.TIME_UPDATE, 101) + CONVERT(VARCHAR(20),DATEPART(HOUR,B.TIME_UPDATE))
                    )

                    -- INSERT DATA
                    INSERT INTO D_DEVICE_SENSOR_DETAIL(DEVICE_ID,VALUE,TIME_UPDATE)
                    SELECT DEVICE_ID,VALUE,TIME_UPDATE FROM #TABLE_TEMP
                    ";

                // run transaction
                string sqlRun = @"
                    BEGIN TRY
                    BEGIN TRANSACTION
                        " + sql + @"
                        SELECT " + ConstantClass.IS_SUCCESS + @"
                    COMMIT
                    END TRY
                    BEGIN CATCH
                        SELECT " + ConstantClass.IS_FAILD + @"
                    ROLLBACK
                    END CATCH
                    ";
                int res = _dbContext.Database.SqlQuery<int>(sqlRun).FirstOrDefault();

                if (res == ConstantClass.IS_SUCCESS)
                    _hubtracking.Send("OK");
                // insert data tracking to database
                //foreach (var item in _collection)
                //{
                //    D_DEVICE_SENSOR_DETAIL _detail = new D_DEVICE_SENSOR_DETAIL();
                //    _detail.DEVICE_ID = item.DEVICE_ID;
                //    _detail.VALUE = item.VALUE;
                //    _detail.TIME_UPDATE = DateTime.Now;

                //    _dbContext.D_DEVICE_SENSOR_DETAIL.Add(_detail);
                //    _newcolection.Add(new ListValueTrackingUno()
                //    {
                //        DEVICE_ID = _detail.DEVICE_ID,
                //        VALUE = _detail.VALUE,
                //        TIME = DateTime.Parse(_detail.TIME_UPDATE.ToString()).ToShortDateString() + " " + DateTime.Parse(_detail.TIME_UPDATE.ToString()).ToShortTimeString().Replace("PM", "").Replace("AM", ""),
                //        UNIT = GetUnitByDeviceID(_detail.DEVICE_ID)
                //    });
                //    _dbContext.SaveChanges();
                //}

                //var json = new JavaScriptSerializer().Serialize(_newcolection);
                //_hubtracking.Send(json);

                result = true;
            }
            catch (Exception ex)
            {
                logger.Error("InsertDataTracking - " + ex);
                result = false;
            }
            return result;
        }

        private string GetUnitByDeviceID(string deviceid)
        {
            string result = string.Empty;
            try
            {
                string sql = @"SELECT G.UNIT FROM DEVICE D
                                JOIN S_GROUP_SENSOR G
                                ON D.GROUP_SENSOR_ID = G.GROUP_SENSOR_ID
                                WHERE D.DEVICE_ID = '" + deviceid + "'";
                result = _dbContext.Database.SqlQuery<string>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// Kiem tra khu vuon do co active chua?
        /// </summary>
        /// <param name="tokenkey"></param>
        /// <returns></returns>
        public int GetIsActiveGarden(string tokenkey)
        {
            int result = 0;
            string sql = "SELECT ACTIVE FROM S_GARDEN WHERE TOKEN_KEY = '" + tokenkey + @"'";
            int res = (_dbContext.Database.SqlQuery<int>(sql).FirstOrDefault());
            if (res == 1)
                result = 1;
            return result;
        }

        public string GetListUnitAjax(string strSelected)
        {
            string result = string.Empty;
            try
            {
                string sql = @"
                    SELECT STUFF((SELECT '<option value=' + CAST(UNIT_ID AS VARCHAR(10)) + ' '
	                + (CASE WHEN UNIT_ID = '" + strSelected + @"' THEN 'selected' ELSE '' END)+
	                 ' >' + UNIT_NAME + '</option>'
	                FROM S_UNIT S
	                FOR XML PATH(''), TYPE ).value('.', 'varchar(max)') , 1, 0, '')
                    ";
                result = _dbContext.Database.SqlQuery<string>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        #endregion


        #region APi for Mobile

        public List<TrackingRawData> GetListTrackingMobile(string tokenkey)
        {
            List<TrackingRawData> lstResult = new List<TrackingRawData>();
            try
            {
                string sql = @"
                    SELECT A.DEVICE_ID,DEVICE_NAME_EN AS DEVICE_NAME,PIN_ID 
                    ,VALUE = CASE WHEN D.VALUE IS NULL THEN '0 ' + U.UNIT_NAME ELSE D.VALUE + ' ' + U.UNIT_NAME END

                    FROM S_DEVICE A
                    JOIN (SELECT * FROM S_GARDEN WHERE TOKEN_KEY= '" + tokenkey + @"') B
                    ON A.TOKEN_KEY = B.TOKEN_KEY
                    
                    LEFT JOIN (SELECT * FROM D_DEVICE_SENSOR_DETAIL 
					WHERE FORMAT(GETDATE(),'HH') = FORMAT(TIME_UPDATE,'HH')) D 
					ON D.DEVICE_ID = A.DEVICE_ID
					INNER JOIN S_UNIT U ON A.UNIT= U.UNIT_ID

                    WHERE A.DEVICE_CATEGORY = 2
                ";
                lstResult = _dbContext.Database.SqlQuery<TrackingRawData>(sql).ToList();
            }
            catch (Exception ex)
            {
                lstResult = null;
            }
            return lstResult;
        }

        public List<ListValueTracking> GetListValueTrackingMobile(string tokenkey, string deviceid)
        {
            List<ListValueTracking> lstItems = new List<ListValueTracking>();
            try
            {
                string sql = @"
                    DECLARE @Date DATETIME
                    --SELECT @Date = CONVERT(VARCHAR(10),GETDATE(),111)
                    SELECT @Date = (SELECT CONVERT(VARCHAR(20),GETDATE() + 1,111) + ' 00:00')

                    ;WITH Dates AS
                    (
                        SELECT DATEPART(HOUR,DATEADD(HOUR,-1,@Date)) [Hour], 
                          DATEADD(HOUR,-1,@Date) [Date], 1 Num
                        UNION ALL
                        SELECT DATEPART(HOUR,DATEADD(HOUR,-1,[Date])), 
                          DATEADD(HOUR,-1,[Date]), Num+1
                        FROM Dates
                        WHERE Num <= 23
                    )
                    SELECT [Hour], [Date]
                    into #TEMP
                    FROM Dates

                    SELECT
                    VALUE = CASE WHEN A.VALUE IS NULL THEN 'NULL' ELSE A.VALUE + ' ' + A.UNIT END
                    , VALUE_CHART = ISNULL(A.VALUE,0)
					, [TIME] = CONVERT(VARCHAR(5),Date,108) 
                    FROM #TEMP T
                    LEFT JOIN
                    (
	                    SELECT D.VALUE,D.TIME_UPDATE,U.UNIT
                        FROM D_DEVICE_SENSOR_DETAIL D
                        INNER JOIN
                        (
                        SELECT U.UNIT_NAME UNIT,D.DEVICE_ID FROM S_DEVICE D
                        LEFT JOIN S_UNIT U ON D.UNIT = U.UNIT_ID
                        WHERE DEVICE_ID = '" + deviceid + @"'
                        ) U ON U.DEVICE_ID = D.DEVICE_ID
                        WHERE D.DEVICE_ID = '" + deviceid + @"' 
                     ) A ON (CONVERT(VARCHAR(10),A.TIME_UPDATE,111) + CONVERT(VARCHAR(10),DATEPART(HOUR,A.TIME_UPDATE))) 
					 --LIKE T.Date    
					 = (CONVERT(VARCHAR(10),T.Date,111) + CONVERT(VARCHAR(10),DATEPART(HOUR,T.Date)))    
                    ORDER BY [Date]
                    ";
                lstItems = _dbContext.Database.SqlQuery<ListValueTracking>(sql).ToList();
            }
            catch (Exception ex)
            {
                lstItems = null;
            }
            return lstItems;
        }


        #endregion

    }
}