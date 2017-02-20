using ArduinoService.DataModels;
using ArduinoService.Hubs;
using ArduinoService.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ArduinoService.Controllers
{
    public class HomeController : Controller
    {
        private CommonModel commonModel = new CommonModel();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        HomeModels _model = new HomeModels();

        public ActionResult Welcome()
        {
            return View();
        }

        public ActionResult MainMenu()
        {
            if (Request.Cookies["UserSettings"] != null)
            {
                if (Request.Cookies["UserSettings"][ConstantClass.SESSION_USERNAME] != null)
                {
                    Session[ConstantClass.SESSION_USERNAME] = Request.Cookies["UserSettings"][ConstantClass.SESSION_USERNAME];
                    Session[ConstantClass.SESSION_USERID] = Request.Cookies["UserSettings"][ConstantClass.SESSION_USERID];
                }
            }

            // check permission
            if (Session[ConstantClass.SESSION_USERNAME] == null)
                return Redirect("/Account/Login");
            var listGarden = _model.GetListGarden(Session[ConstantClass.SESSION_USERID].ToString());
            return View(listGarden);
        }

        public PartialViewResult Nav()
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
            return PartialView();
        }

        public PartialViewResult MenuLeft()
        {
            if (Request.Cookies["UserSettings"] != null)
            {
                if (Request.Cookies["UserSettings"][ConstantClass.SESSION_USERNAME] != null)
                {
                    Session[ConstantClass.SESSION_USERNAME] = Request.Cookies["UserSettings"][ConstantClass.SESSION_USERNAME];
                    Session[ConstantClass.SESSION_ROLE] = Request.Cookies["UserSettings"][ConstantClass.SESSION_ROLE];
                }
            }
            // check permission
            if (Session[ConstantClass.SESSION_USERNAME] == null)
                return PartialView();
            return PartialView();
        }

        /// <summary>
        /// Lay danh sach ma pin
        /// </summary>
        /// <param name="strSelected"></param>
        /// <param name="tokenkey"></param>
        /// <param name="device_category"></param>
        /// <returns></returns>
        public JsonResult GetListPin(string strSelected, string tokenkey, string device_category)
        {
            return Json(_model.GetListPin(strSelected, tokenkey, device_category), JsonRequestBehavior.AllowGet);
        }

        #region Screen Garden
        public ActionResult Garden(string id)
        {
            if (Request.Cookies["UserSettings"] != null)
            {
                if (Request.Cookies["UserSettings"][ConstantClass.SESSION_USERNAME] != null)
                {
                    Session[ConstantClass.SESSION_USERNAME] = Request.Cookies["UserSettings"][ConstantClass.SESSION_USERNAME];
                    Session[ConstantClass.SESSION_USERID] = Request.Cookies["UserSettings"][ConstantClass.SESSION_USERID];
                }
            }
            // check permission
            if (Session[ConstantClass.SESSION_USERNAME] == null)
                return Redirect("/Account/Login");

            ViewBag.GardenId = id;
            ViewBag.IsActive = _model.GetIsActiveGarden(id);
            return View();
        }

        /// <summary>
        /// Lay danh sach loai Arduino
        /// </summary>
        /// <param name="strSelected"></param>
        /// <returns>combobox danh sach</returns>
        public JsonResult GetListUnoType(string strSelected)
        {
            return Json(_model.GetListUnoType(strSelected), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gardenname"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public JsonResult AddGarden(string gardenname, string image, int unotype,
            int acreage, string address, decimal lat, decimal lon, string description,
            string tokenkey
            )
        {
            // check session
            GardenRawData _rowdata = new GardenRawData();
            _rowdata.GARDEN_NAME = gardenname;
            if (!String.IsNullOrEmpty(image))
                _rowdata.IMAGE = image.Substring(1, image.Length - 1);
            _rowdata.ACTIVE = 1;
            _rowdata.USER_ID = Session[ConstantClass.SESSION_USERID].ToString();
            _rowdata.UNO_TYPE = unotype;
            _rowdata.ACREAGE = acreage;
            _rowdata.LATITUDE = lat;
            _rowdata.LONGITUDE = lon;
            _rowdata.ADDRESS = address;
            _rowdata.DESCRIPTION = description;
            _rowdata.TOKEN_KEY = tokenkey;

            _rowdata = _model.AddOrEditGarden(_rowdata);
            return Json(_rowdata, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteGarden(string tokenkey)
        {
            bool result = _model.DeleteGarden(tokenkey);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGardenById(string tokenkey)
        {
            return Json(_model.GetGardenById(tokenkey), JsonRequestBehavior.AllowGet);
        }

        public FilePathResult Image()
        {
            string filename = Request.Url.AbsolutePath.Replace("/home/image", "");
            string contentType = "";
            var filePath = new FileInfo(Server.MapPath("~/App_Data") + filename);

            var index = filename.LastIndexOf(".") + 1;
            var extension = filename.Substring(index).ToUpperInvariant();

            // Fix for IE not handling jpg image types
            contentType = string.Compare(extension, "JPG") == 0 ? "image/jpeg" : string.Format("image/{0}", extension);

            return File(filePath.FullName, contentType);
        }

        [HttpPost]
        public JsonResult UploadFiles()
        {
            var r = new List<UploadFilesResult>();
            string urlImg = String.Empty;
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                if (hpf.ContentLength == 0)
                    continue;
                string datetimeNow = DateTime.Now.ToString("yyyyMMddhhmmss") + Path.GetFileName(hpf.FileName);
                urlImg = Server.MapPath("~/Content/Images/Upload/") + datetimeNow;
                hpf.SaveAs(urlImg);
                urlImg = "Content/Images/Upload/" + datetimeNow;
            }

            return Json(urlImg, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Screen Control
        public ActionResult Control(string id)
        {
            if (Request.Cookies["UserSettings"] != null)
            {
                if (Request.Cookies["UserSettings"][ConstantClass.SESSION_USERNAME] != null)
                {
                    Session[ConstantClass.SESSION_USERNAME] = Request.Cookies["UserSettings"][ConstantClass.SESSION_USERNAME];
                    Session[ConstantClass.SESSION_USERID] = Request.Cookies["UserSettings"][ConstantClass.SESSION_USERID];
                }
            }
            // check permission
            if (Session[ConstantClass.SESSION_USERNAME] == null)
                return Redirect("/Account/Login");

            ViewBag.ListControl = _model.GetListControl(id);
            ViewBag.FullControl = _model.IsFullControl(id, ConstantClass.CONTROL);
            return View();
        }

        public JsonResult AddOrEditControl(string namecontrol, string tokenkey, string deviceid, string pinid)
        {
            // check permission
            if (Session[ConstantClass.SESSION_USERNAME] == null)
                return Json(false, JsonRequestBehavior.AllowGet);

            return Json(_model.AddOrEditControl(namecontrol, tokenkey, deviceid, pinid), JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region Screen Tracking
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"> id uno</param>
        /// <returns></returns>
        public ActionResult Tracking(string id)
        {
            if (Request.Cookies["UserSettings"] != null)
            {
                if (Request.Cookies["UserSettings"][ConstantClass.SESSION_USERNAME] != null)
                {
                    Session[ConstantClass.SESSION_USERNAME] = Request.Cookies["UserSettings"][ConstantClass.SESSION_USERNAME];
                    Session[ConstantClass.SESSION_USERID] = Request.Cookies["UserSettings"][ConstantClass.SESSION_USERID];
                }
            }
            // check permission
            if (Session[ConstantClass.SESSION_USERNAME] == null)
                return Redirect("/Account/Login");

            ViewBag.lstUnit = _model.GetListUnit();
            ViewBag.FullControl = _model.IsFullControl(id, ConstantClass.SENSOR);

            return View();
        }

        /// <summary>
        /// Lay danh sach data sensor dua len table
        /// </summary>
        /// <param name="tokenkey"></param>
        /// <param name="index"></param>
        /// <param name="filterDate"></param>
        /// <returns></returns>
        public JsonResult GetDeviceDetails(string tokenkey, int index, string filterDate)
        {
            var result = new
            {
                listDevice = _model.GetListDevice(tokenkey),
                listItem = _model.GetListDataDetails(tokenkey, filterDate)
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Them thiet bi (sensor)
        /// </summary>
        /// <param name="namecontrol">Ten thiet bi</param>
        /// <param name="namecontrol2">Ten thiet bi 2(dung cho group HT)</param>
        /// <param name="tokenkey"></param>
        /// <param name="deviceid">ma  thiet bi</param>
        /// <param name="groupsensor">nhom thiet bi</param>
        /// <param name="unit">don vi thiet bi thong ke</param>
        /// <param name="pinid">ma pin cam vao uno</param>
        /// <param name="unit2">don vi thiet bi 2(dung cho group HT)</param>
        /// <returns>true : thanh cong, false : that bai</returns>
        public JsonResult AddNewControlTracking(string device_id, string namecontrol, string namecontrol2, string tokenkey, int groupsensor, int unit, string pinid, int unit2)
        {
            if (Request.Cookies["UserSettings"] != null)
            {
                if (Request.Cookies["UserSettings"][ConstantClass.SESSION_USERNAME] != null)
                {
                    Session[ConstantClass.SESSION_USERNAME] = Request.Cookies["UserSettings"][ConstantClass.SESSION_USERNAME];
                    Session[ConstantClass.SESSION_USERID] = Request.Cookies["UserSettings"][ConstantClass.SESSION_USERID];
                }
            }
            // check permission
            if (Session[ConstantClass.SESSION_USERNAME] == null)
                return Json(false, JsonRequestBehavior.AllowGet);

            // check device_id
            string device_id1 = string.Empty;
            string device_id2 = string.Empty;

            if (string.IsNullOrEmpty(device_id) == false && groupsensor == ConstantClass.SENSOR_HT)
            {
                device_id1 = _model.GetGroupDeviceID(device_id)[0];
                device_id2 = _model.GetGroupDeviceID(device_id)[1];
            }
            else
            {
                device_id1 = device_id;
            }

            bool result = _model.AddNewControlTracking(device_id1, namecontrol, tokenkey, 2, groupsensor, unit, pinid);
            if (result == true && groupsensor == ConstantClass.SENSOR_HT)
                result = _model.AddNewControlTracking(device_id2, namecontrol2, tokenkey, 2, groupsensor, unit2, pinid);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Kiem tra ten thiet bi co ton tai trong danh sach thiet bi ko ?
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true : co, false : ko</returns>
        public JsonResult CheckExistsNameTracking(string name, string deviceid, string tokenkey)
        {
            return Json(_model.CheckExistsNameTracking(name, deviceid, tokenkey), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hien thi thong tin thiet bi( dung trong Screen tracking).
        /// </summary>
        /// <param name="deviceid"></param>
        /// <returns>List object hoac 1 object</returns>
        public JsonResult LoadInfoDevice(string deviceid)
        {
            return Json(_model.LoadInfoDevice(deviceid), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Screen Chart
        public ActionResult Chart(string tokenkey)
        {
            if (Request.Cookies["UserSettings"] != null)
            {
                if (Request.Cookies["UserSettings"][ConstantClass.SESSION_USERNAME] != null)
                {
                    Session[ConstantClass.SESSION_USERNAME] = Request.Cookies["UserSettings"][ConstantClass.SESSION_USERNAME];
                    Session[ConstantClass.SESSION_USERID] = Request.Cookies["UserSettings"][ConstantClass.SESSION_USERID];
                }
            }
            // check permission
            if (Session[ConstantClass.SESSION_USERNAME] == null)
                return Redirect("/Account/Login");

            return View();
        }

        /// <summary>
        /// Lay danh sach data dung de ve bieu do
        /// </summary>
        /// <param name="tokenkey"></param>
        /// <param name="datechart"></param>
        /// <returns></returns>
        public JsonResult GetDataChart(string tokenkey, string datechart)
        {
            return Json(_model.GetListDataChartDetails(tokenkey, datechart), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Common Screen

        /// <summary>
        /// Lay danh sach tu table UNIT
        /// </summary>
        /// <param name="selected"></param>
        /// <returns>Danh sach combobox unit</returns>
        public JsonResult GetListUnitAjax(string selected)
        {
            return Json(_model.GetListUnitAjax(selected), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xoa thiet bi
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true : xoa thanh cong, false : xoa that bai</returns>
        public JsonResult DeleteDevice(string id)
        {
            return Json(_model.DeleteDevice(id), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Screen Setting Garden
        public ActionResult SettingGarden(string id)
        {
            if (Request.Cookies["UserSettings"] != null)
            {
                if (Request.Cookies["UserSettings"][ConstantClass.SESSION_USERNAME] != null)
                {
                    Session[ConstantClass.SESSION_USERNAME] = Request.Cookies["UserSettings"][ConstantClass.SESSION_USERNAME];
                    Session[ConstantClass.SESSION_USERID] = Request.Cookies["UserSettings"][ConstantClass.SESSION_USERID];
                }
            }
            // check permission
            if (Session[ConstantClass.SESSION_USERNAME] == null)
                return Redirect("/Account/Login");
            ViewBag.modelgarden = _model.GetGardenById(id);
            ViewBag.datasettingcontrol = _model.GetControlDataSetting(id);

            return View();
        }

        public JsonResult UpdateShedule(string tokenkey)
        {
            return Json(_model.UpdateShedule(tokenkey), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveSettingControl(string strArr, string deviceid)
        {
            return Json(_model.SaveSettingControl(strArr, deviceid), JsonRequestBehavior.AllowGet);
        }

        #endregion




















        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChangeCulture(string lang, string returnUrl)
        {
            try
            {
                if (lang != null)
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(lang);
                }
                var langCookie = new HttpCookie("lang", lang) { HttpOnly = true };
                Response.Cookies.Add(langCookie);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Redirect(returnUrl);
        }


    }
}