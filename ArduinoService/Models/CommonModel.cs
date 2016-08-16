using ArduinoService.DataContext;
using ArduinoService.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;

namespace ArduinoService.Models
{
    public class CommonModel
    {
        static string root = AppDomain.CurrentDomain.BaseDirectory + @"Data/Database.xml";
        private ioTSystemEntities _dbContext = new ioTSystemEntities();

        public string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        public string GetAutoId(string column, string table)
        {
            string id = String.Empty;
            try
            {
                string sql = @"
                    DECLARE @MAX_ID VARCHAR(20)
                    SET @MAX_ID = (SELECT ISNULL(MAX(CAST(" + column + @" AS INT)),0) + 1 FROM " + table + @")
                    SELECT @MAX_ID
                    ";
                id = _dbContext.Database.SqlQuery<string>(sql).FirstOrDefault();
            }
            catch (Exception ex)
            {
                id = string.Empty;
            }
            return id;
        }

        private readonly Random _rng = new Random();
        private const string _chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public string RandomString(int size)
        {
            string result = String.Empty;

            while(CheckTokenKey(result))
            {
                char[] buffer = new char[size];

                for (int i = 0; i < size; i++)
                {
                    buffer[i] = _chars[_rng.Next(_chars.Length)];
                }
                result = new string(buffer);
            }

            return result;
        }

        /// <summary>
        /// Kiem tra key co ton tai trong Garden ko ?
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true : co ton tai, false : ko ton tai</returns>
        private bool CheckTokenKey(string key)
        {
            var record = _dbContext.GARDENs.FirstOrDefault(x => x.TOKEN_KEY == key);
            if (record != null || key == "")
                return true;
            return false;
        }

        /// <summary>
        /// Lay ID theo User name
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public string getIdByUserName(string email)
        {
            string result = string.Empty;
            try
            {
                result = _dbContext.USERS.FirstOrDefault(x => x.EMAIL == email).USER_ID;
            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            return result;
        }



    }
}