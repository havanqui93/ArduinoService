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

    }
}