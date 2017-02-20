using ArduinoService.DataContext;
using ArduinoService.DataModels;
using ArduinoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArduinoService.Controllers
{
    public class AdminAreaController : Controller
    {
        CommonModel _commonModel = new CommonModel();
        private ioTEntities _dbContext = new ioTEntities();

        // GET: AdminArea
        public ActionResult AdminArea()
        {
            if (Request.Cookies["UserSettings"] != null)
            {
                if (Request.Cookies["UserSettings"][ConstantClass.SESSION_USERNAME] != null)
                {
                    Session[ConstantClass.SESSION_USERNAME] = Request.Cookies["UserSettings"][ConstantClass.SESSION_USERNAME];
                    Session[ConstantClass.SESSION_USERID] = Request.Cookies["UserSettings"][ConstantClass.SESSION_USERID];
                    Session[ConstantClass.USER_TYPE] = Request.Cookies["UserSettings"][ConstantClass.USER_TYPE];
                    Session[ConstantClass.SESSION_FULLNAME] = Request.Cookies["UserSettings"][ConstantClass.SESSION_FULLNAME];
                    Session[ConstantClass.SESSION_ROLE] = Request.Cookies["UserSettings"][ConstantClass.SESSION_ROLE];
                }
            }
            // check permission
            if (Session[ConstantClass.SESSION_USERNAME] == null)
                return Redirect("/Account/Login");

            return View();
        }

        #region Manage Code

        public JsonResult GetCode()
        {
            string codeResut = String.Empty;
            try
            {
                codeResut = _commonModel.RandomString(ConstantClass.TOKEN_SIZE_DEFAULT);
                // check code is exists in database
                var data = _dbContext.S_CODE.FirstOrDefault(x => x.CODE_VALUE == codeResut);
                while (data != null)
                {
                    codeResut = _commonModel.RandomString(ConstantClass.TOKEN_SIZE_DEFAULT);
                    data = _dbContext.S_CODE.FirstOrDefault(x => x.CODE_VALUE == codeResut);
                }
            }
            catch (Exception ex)
            {
                codeResut = String.Empty;
            }
            return Json(codeResut, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 1 : SUCCESS, 2 : CODE EXISTS , -1 : ERROR SYSTEM
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public JsonResult AddCode(string code)
        {
            int result = 1;
            try
            {
                var data = _dbContext.S_CODE.FirstOrDefault(x => x.CODE_VALUE == code);
                if (data != null)
                {
                    result = 2;
                }
                else
                {
                    S_CODE rowdata = new S_CODE();
                    string sql = "SELECT CAST(MAX(CAST(CODE_ID AS INT) + 1) AS VARCHAR(20)) FROM S_CODE";
                    string id = _dbContext.Database.SqlQuery<string>(sql).FirstOrDefault();

                    rowdata.CODE_ID = String.IsNullOrEmpty(id) ? "1" : id;
                    rowdata.CODE_VALUE = code;
                    rowdata.STATUS = true;

                    _dbContext.S_CODE.Add(rowdata);
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result = -1;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RefreshCode()
        {
            List<CodeDataModel> _lst = new List<CodeDataModel>();
            try
            {
                string sql = "SELECT CODE_ID ID,CODE_VALUE CODE,CAST([STATUS] AS INT) AS [STATUS] FROM S_CODE";
                _lst = _dbContext.Database.SqlQuery<CodeDataModel>(sql).ToList();
            }
            catch (Exception ex)
            {
                _lst = null;
            }
            return Json(_lst, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Manage UNO type

        public ActionResult ManageUnoType()
        {
            // check permission
            if (Session[ConstantClass.SESSION_USERNAME] == null)
                return Redirect("/Account/Login");

            return View();
        }

        #endregion Manage UNO type

    }
}