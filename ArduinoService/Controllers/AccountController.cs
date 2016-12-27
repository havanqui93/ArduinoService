﻿using ArduinoService.DataModels;
using ArduinoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArduinoService.Controllers
{
    public class AccountController : Controller
    {
        private AccountModel _account = new AccountModel();

        [HttpGet]
        public ActionResult Login()
        {
            if (Session[ConstantClass.SESSION_USERNAME] != null)
                return Redirect("/Home/MainMenu");
            return View();
        }

        [HttpPost]
        public ActionResult Login(AccountRowData model)
        {
            var result = _account.Login(model);
            if (result)
            {
                // set session
                // check group admin
                if (_account.checkGoupAdmin(model))
                    Session[ConstantClass.SESSION_ROLE] = 1;
                else
                    Session[ConstantClass.SESSION_ROLE] = 2;

                Session[ConstantClass.SESSION_USERNAME] = _account.GetSessionEmail(model);
                Session[ConstantClass.SESSION_FULLNAME] = _account.GetFullName(model);
                Session[ConstantClass.SESSION_USERID] = _account.GetSessionUserID(model);
                Session[ConstantClass.USER_TYPE] = _account.GetInfoUser(model).USER_TYPE;

                return Redirect("/Home/MainMenu");
            }
            return View();
        }

        public JsonResult CheckLogin(AccountRowData model)
        {
            var result = _account.Login(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterRowData model)
        {
            var result = _account.Register(model);
            if (result)
            {
                return Redirect("/Account/Login");
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return Redirect("/Home/Welcome");
        }

    }
}