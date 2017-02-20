using ArduinoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArduinoService.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Profile()
        {
            return View();
        }

        public ActionResult Message()
        {
            return View();
        }

        public ActionResult ChangeUnoType()
        {
            if (Request.Cookies["UserSettings"][ConstantClass.USER_TYPE].ToString() == "1")
                Session[ConstantClass.USER_TYPE] = "2";
            else
                Session[ConstantClass.USER_TYPE] = "1";
            return RedirectToAction("MainMenu", "Home");
        }

    }
}