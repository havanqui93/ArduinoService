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

namespace ArduinoService.Controllers
{
    public class HomeController : Controller
    {
        TrackingModel _trackingModel = new TrackingModel();
        private CommonModel commonModel = new CommonModel();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        HomeModels _model = new HomeModels();

        public ActionResult Welcome()
        {
            return View();
        }

        public ActionResult MainMenu()
        {
            // check permission
            if (Session[ConstantClass.SESSION_USERNAME] == null)
                return Redirect("/Account/Login");
            var listGarden = _model.GetListGarden(Session[ConstantClass.SESSION_USERNAME].ToString());
            return View(listGarden);
        }

        public PartialViewResult Nav()
        {
            return PartialView();
        }

        public PartialViewResult MenuLeft()
        {
            // check permission
            if (Session[ConstantClass.SESSION_USERNAME] == null)
                return PartialView();
            return PartialView();
        }

        public ActionResult Garden(string id)
        {
            // check permission
            if (Session[ConstantClass.SESSION_USERNAME] == null)
                return Redirect("/Account/Login");

            ViewBag.GardenId = id;
            return View();
        }

        public ActionResult Control(string id)
        {
            // check permission
            if (Session[ConstantClass.SESSION_USERNAME] == null)
                return Redirect("/Account/Login");

            ViewBag.ListControl = _model.GetListControl(id);
            return View();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"> id uno</param>
        /// <returns></returns>
        public ActionResult Tracking(string id)
        {
            // check permission
            if (Session[ConstantClass.SESSION_USERNAME] == null)
                return Redirect("/Account/Login");

            //ViewBag.ListControl = _model.GetListTracking(id);
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gardenname"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public JsonResult AddGarden(string gardenname, string image)
        {
            GardenRawData _rowdata = new GardenRawData();
            _rowdata.GARDEN_NAME = gardenname;
            _rowdata.IMAGE = image;
            _rowdata.ACTIVE = 1;
            _rowdata.USER_ID = commonModel.getIdByUserName(Session[ConstantClass.SESSION_USERNAME].ToString());
            _rowdata = _model.AddGarden(_rowdata);
            return Json(_rowdata, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditGarden(string gardenname, string image, string gardenid)
        {
            GardenRawData _rowdata = new GardenRawData();
            _rowdata.GARDEN_ID = gardenid;
            _rowdata.GARDEN_NAME = gardenname;
            _rowdata.IMAGE = image;
            _rowdata.ACTIVE = 1;
            _rowdata.USER_ID = commonModel.getIdByUserName(Session[ConstantClass.SESSION_USERNAME].ToString());
            _rowdata = _model.EditGarden(_rowdata);
            return Json(_rowdata, JsonRequestBehavior.AllowGet);
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
                urlImg = "/Content/Images/Upload/" + datetimeNow;
            }

            return Json(urlImg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListGardenMenuLeft()
        {
            return Json(_model.GetListGarden(Session[ConstantClass.SESSION_USERNAME].ToString()), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddNewControl(string namecontrol, string gardenid,string deviceid)
        {
            // check permission
            if (Session[ConstantClass.SESSION_USERNAME] == null)
                return Json(false, JsonRequestBehavior.AllowGet);

            return Json(_model.AddNewControl(namecontrol, gardenid, 1, deviceid), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddNewControlTracking(string namecontrol, string gardenid, string deviceid)
        {
            // check permission
            if (Session[ConstantClass.SESSION_USERNAME] == null)
                return Json(false, JsonRequestBehavior.AllowGet);

            return Json(_model.AddNewControl(namecontrol, gardenid, 2, deviceid), JsonRequestBehavior.AllowGet);

        }




















        public ActionResult Index()
        {
            return View();
        }



        public ActionResult ControlSystem()
        {
            return View();
        }

        public ActionResult TrackingData()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetListTrackingHistory()
        {
            List<LTrackingRowData> result = new List<LTrackingRowData>();
            try
            {
                result = _trackingModel.GetListTrackingHistory(5);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
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