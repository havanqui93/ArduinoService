using ArduinoService.DataContext;
using ArduinoService.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArduinoService.Models
{
    public class HomeModels
    {
        private ioTSystemEntities _dbContext = new ioTSystemEntities();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private CommonModel commonModel = new CommonModel();
        string userID = String.Empty;

        public List<GardenRawData> GetListGarden(string userId)
        {
            List<GardenRawData> result = new List<GardenRawData>();
            try
            {
                string sql = @"
                    SELECT GARDEN_ID,GARDEN_NAME,IMAGE,TOKEN_KEY FROM GARDEN WHERE USER_ID = (SELECT USER_ID FROM USERS WHERE EMAIL = '" + userId + @"')
                    ";
                result = _dbContext.Database.SqlQuery<GardenRawData>(sql).ToList();
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }


        public GardenRawData AddGarden(GardenRawData data)
        {
            try
            {
                GARDEN _data = new GARDEN();
                _data.GARDEN_ID = commonModel.GetAutoId("GARDEN_ID", "GARDEN");
                _data.GARDEN_NAME = data.GARDEN_NAME;
                _data.IMAGE = data.IMAGE;
                _data.USER_ID = data.USER_ID;
                _data.ACTIVE = 1; // 0 : bi hu, 1 : dang hoat dong
                _data.TOKEN_KEY = commonModel.RandomString(10);
                data.GARDEN_ID = _data.GARDEN_ID;

                _dbContext.GARDENs.Add(_data);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                data = null;
            }
            return data;
        }

        public GardenRawData EditGarden(GardenRawData data)
        {
            try
            {
                var _data = _dbContext.GARDENs.FirstOrDefault(x => x.GARDEN_ID == data.GARDEN_ID);
                if (_data != null)
                {
                    _data.GARDEN_NAME = String.IsNullOrEmpty(data.GARDEN_NAME) ? _data.GARDEN_NAME : data.GARDEN_NAME;
                    _data.IMAGE = String.IsNullOrEmpty(data.IMAGE) ? _data.IMAGE : data.IMAGE;
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                data = null;
            }
            return data;
        }

        public List<ControlRawData> GetListControl(string gardenId)
        {
            List<ControlRawData> result = new List<ControlRawData>();
            try
            {
                string sql = @"
                    SELECT D.DEVICE_ID,DEVICE_NAME,VALUE
                    FROM DEVICE D
                    JOIN DEVICE_CONTROL_DETAIL C
                    ON D.DEVICE_ID = C.DEVICE_ID
                    WHERE D.TOKEN_KEY = '" + gardenId + @"'
                    ";
                result = _dbContext.Database.SqlQuery<ControlRawData>(sql).ToList();
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }


        public bool UpdateValueControl(ControlRawData model)
        {
            bool bResult = false;
            try
            {
                // udpate value control
                var module = _dbContext.DEVICE_CONTROL_DETAIL.FirstOrDefault(x => x.DEVICE_ID == model.DEVICE_ID);
                if (module != null)
                {
                    module.VALUE = model.VALUE;
                    module.TIME_UPDATE = DateTime.Now;

                    _dbContext.SaveChanges();
                    bResult = true;
                }
            }
            catch (Exception ex)
            {
                bResult = false;
            }
            return bResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namecontrol">device name</param>
        /// <param name="category">1:</param>
        /// <returns></returns>
        public bool AddNewControl(string namecontrol, string gardenid, int category, string deviceid)
        {
            bool result = false;
            try
            {
                var module = _dbContext.DEVICEs.FirstOrDefault(x => x.DEVICE_ID == deviceid);
                if (module == null) // add new
                {
                    DEVICE _device = new DEVICE();
                    _device.DEVICE_ID = commonModel.GetAutoId("DEVICE_ID", "DEVICE");
                    _device.TOKEN_KEY = gardenid;
                    _device.DEVICE_NAME = namecontrol;
                    _device.DEVICE_CATEGORY = category;
                    _dbContext.DEVICEs.Add(_device);

                    switch (category)
                    {
                        case 1: // dieu khien
                            DEVICE_CONTROL_DETAIL _devicedetail = new DEVICE_CONTROL_DETAIL();
                            _devicedetail.DEVICE_ID = _device.DEVICE_ID;
                            _devicedetail.VALUE = "OFF";
                            _devicedetail.TIME_UPDATE = DateTime.Now;
                            _dbContext.DEVICE_CONTROL_DETAIL.Add(_devicedetail);
                            break;
                        case 2: // lay du lieu
                            DEVICE_SENSOR_DETAIL _devicetracking = new DEVICE_SENSOR_DETAIL();
                            _devicetracking.DEVICE_ID = commonModel.GetAutoId("DEVICE_ID", "DEVICE");
                            _devicetracking.VALUE = "OFF";
                            _devicetracking.TIME_UPDATE = DateTime.Now;
                            _dbContext.DEVICE_SENSOR_DETAIL.Add(_devicetracking);
                            break;
                    }
                }
                else
                {
                    module.DEVICE_NAME = namecontrol;
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

        #region Api for UNO

        public string GetListDataConttrolUno(string gardenid)
        {
            string result = String.Empty;
            try
            {
                result = "[{\"Id\":\""+ gardenid + "\",\"NoDevices\":";
                string sql = "SELECT (SELECT CAST(COUNT(*) AS VARCHAR(100)) FROM DEVICE D LEFT JOIN GARDEN G ON D.GARDEN_ID = G.GARDEN_ID WHERE D.DEVICE_CATEGORY='1' AND G.GARDEN_ID = '" + gardenid + "')+'\",'";
                result += _dbContext.Database.SqlQuery<string>(sql).FirstOrDefault();

                sql = "SELECT STUFF((SELECT ',' +'\"Device' + CAST(CAST(D.DEVICE_ID AS INT) AS VARCHAR(MAX)) + '\":\"' + (CASE WHEN DC.VALUE = 'ON' THEN '1' ELSE '0' END) + '\"' FROM DEVICE D JOIN GARDEN G ON D.GARDEN_ID = G.GARDEN_ID JOIN DEVICE_CONTROL_DETAIL DC ON DC.DEVICE_ID = D.DEVICE_ID WHERE D.GARDEN_ID = '" + gardenid + "' AND D.DEVICE_CATEGORY = 1 FOR XML PATH('')), 1, 1, '') +'}]'";
                result += _dbContext.Database.SqlQuery<string>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = String.Empty;
            }
            return result;
        }

        #endregion


    }
}