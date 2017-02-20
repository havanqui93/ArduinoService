using ArduinoService.DataContext;
using ArduinoService.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace ArduinoService.DataModels
{
    public class AccountModel
    {
        private ioTEntities dbcontext = new ioTEntities();
        private CommonModel commonModel = new CommonModel();
        private CommonModel commomFunction = new CommonModel();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>true : login success, false : login false</returns>
        public bool Login(AccountRowData data)
        {
            bool result = false;
            try
            {
                var pass = commomFunction.MD5Hash(data.Password);
                string sql = @"
                    SELECT COUNT(*) FROM S_USER WHERE (EMAIL = '" + data.Email + "' OR PHONE = '" + data.Email + "') AND PASSWORD = '" + pass + @"'
                    ";

                int record = dbcontext.Database.SqlQuery<int>(sql).FirstOrDefault();
                result = record != 0 ? true : false;
                return result;
            }
            catch (Exception ex)
            {
                // write log
                logger.Error("Login - " + ex);
                result = false;
            }
            return result;
        }

        public bool Register(RegisterRowData data)
        {
            bool result = false;
            
            try
            {
                var record = dbcontext.S_USER.FirstOrDefault(x => x.EMAIL == data.Email || x.PHONE == data.Phone);
                if (record == null)
                {
                    record = new S_USER();
                    record.USER_ID = commonModel.GetAutoId("USER_ID", "S_USER");
                    record.FULL_NAME = data.Fullname;
                    record.EMAIL = data.Email;
                    record.PASSWORD = commomFunction.MD5Hash(data.Password);
                    record.PHONE = data.Phone;
                    record.ADDRESS = data.Address;
                    record.PACKED_ID = ConstantClass.PACKED_DEFAULT;
                    record.USER_TYPE = data.Usertype;
                    dbcontext.S_USER.Add(record);
                    dbcontext.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Register - " + ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Check user is exists in database
        /// </summary>
        /// <param name="data"></param>
        /// <returns>true : user exists, false : user not exists</returns>
        public bool checkUsernameIsexists(RegisterRowData data)
        {
            return dbcontext.S_USER.Any(x => x.EMAIL == data.Email || x.PHONE == data.Phone);
        }

        /// <summary>
        /// check group is admin or user
        /// </summary>
        /// <param name="username"></param>
        /// <returns>true : group admin, false : group user</returns>
        public bool checkGoupAdmin(AccountRowData model)
        {
            bool result = false;
            try
            {
                string sql = @"
                    SELECT COUNT(*) FROM S_USER S
                    INNER JOIN D_GROUP_PERMISSION D ON S.USER_ID = D.USER_ID
                    WHERE (S.EMAIL = '" + model.Email + @"' OR S.PHONE = '" + model.Email + @"') 
                    AND D.PERMISSION_ID = '" + ConstantClass.GROUP_ADMIN + @"'
                    ";
                int res = dbcontext.Database.SqlQuery<int>(sql).FirstOrDefault();
                result = (res > 0 ? true : false);
            }
            catch (Exception ex)
            {
                logger.Error("checkGoupAdmin - " + ex);
                result = false;
            }
            return result;
        }

        public string GetFullName(AccountRowData model)
        {
            string fullname = string.Empty;
            try
            {
                fullname = dbcontext.S_USER.FirstOrDefault(x => x.EMAIL == model.Email || x.PHONE == model.Email).FULL_NAME;
            }
            catch (Exception ex)
            {
                logger.Error("GetFullName - " + ex);
            }
            return fullname;
        }

        public string GetSessionEmail(AccountRowData model)
        {
            string email = string.Empty;
            try
            {
                email = dbcontext.S_USER.FirstOrDefault(x => x.EMAIL == model.Email || x.PHONE == model.Email).EMAIL;
            }
            catch (Exception ex)
            {
                logger.Error("GetSessionEmail - " + ex);
            }
            return email;
        }

        public string GetSessionUserID(AccountRowData model)
        {
            string userID = string.Empty;
            try
            {
                userID = dbcontext.S_USER.FirstOrDefault(x => x.EMAIL == model.Email || x.PHONE == model.Email).USER_ID;
            }
            catch (Exception ex)
            {
                logger.Error("GetSessionEmail - " + ex);
            }
            return userID;
        }

        public ResultLoginRowData CheckLoginMobile(AccountRowData data)
        {
            ResultLoginRowData result = new ResultLoginRowData();
            try
            {
                bool isResult = Login(data);
                if (isResult)
                    result = GetInfoUser(data);
                else
                    result.IS_SUCCESS = false;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public ResultLoginRowData GetInfoUser(AccountRowData data)
        {
            ResultLoginRowData result = new ResultLoginRowData();
            try
            {
                string sql = @"
                    SELECT 
	                    USER_ID,
	                    FULL_NAME,
	                    EMAIL,
	                    PHONE,
	                    PASSWORD,
	                    ADDRESS,
                        USER_TYPE
                    FROM S_USER
                    WHERE EMAIL = '" + data.Email + @"'
                    OR PHONE = '" + data.Email + @"'
                    ";
                result = dbcontext.Database.SqlQuery<ResultLoginRowData>(sql).FirstOrDefault();
                if (result != null)
                    result.IS_SUCCESS = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }




    }

    public class AccountRowData
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RemenberMe { get; set; }
    }

    public class RegisterRowData
    {
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public int Usertype { get; set; } // 1 : Nguoi trong, 2 : nguoi mua
    }

    public class ResultLoginRowData
    {
        public string USER_ID { get; set; }
        public string FULL_NAME { get; set; }
        public string EMAIL { get; set; }
        public string PHONE { get; set; }
        public string PASSWORD { get; set; }
        public string ADDRESS { get; set; }
        public bool IS_SUCCESS { get; set; }
        public int USER_TYPE { get; set; }
    }


}