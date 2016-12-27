using ArduinoService.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArduinoService.Controllers
{
    public class DatabaseController : Controller
    {
        private ioTEntities dbcontext = new ioTEntities();

        // GET: Database
        public ActionResult CheckDatabase()
        {
            return View();
        }

        public JsonResult CheckConnectDatabase()
        {
            string result = String.Empty;
            try
            {
                string sql = "SELECT COUNT(*) FROM S_USER";
                int res = dbcontext.Database.SqlQuery<int>(sql).FirstOrDefault();
                result = res.ToString();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }




    }
}