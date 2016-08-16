using ArduinoService.DataContext;
using ArduinoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArduinoService.DataModels
{
    public class AccountModel
    {
        private ioTSystemEntities dbcontext = new ioTSystemEntities();
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
                var record = dbcontext.USERS.FirstOrDefault(x => x.EMAIL == data.Email || x.PHONE == data.Email && x.PASSWORD == pass);
                result = record != null ? true : false;
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
                var record = dbcontext.USERS.FirstOrDefault(x => x.EMAIL == data.Email || x.PHONE == data.Phone);

                if (record == null)
                {
                    record = new USER();
                    record.USER_ID = commonModel.GetAutoId("USER_ID", "USERS");
                    record.FULL_NAME = data.Fullname;
                    record.EMAIL = data.Email;
                    record.PASSWORD = commomFunction.MD5Hash(data.Password);
                    record.PHONE = data.Phone;
                    record.ADDRESS = data.Address;
                    record.PERMISSION_ID = ConstantClass.GROUP_USER; // role user
                    dbcontext.USERS.Add(record);
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
            return dbcontext.USERS.Any(x => x.EMAIL == data.Email || x.PHONE == data.Phone);
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
                result = dbcontext.USERS.Any(x => x.EMAIL == model.Email || x.PHONE == model.Email && x.PERMISSION_ID == 1);
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
                fullname = dbcontext.USERS.FirstOrDefault(x => x.EMAIL == model.Email || x.PHONE == model.Email).FULL_NAME;
            }
            catch (Exception ex)
            {
                logger.Error("GetFullName - " + ex);
            }
            return fullname;
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
    }

}