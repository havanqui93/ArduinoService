using ArduinoService.DataModels;
using ArduinoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ArduinoService.Controllers
{
    public class apiActiveCodeController : ApiController
    {
        HomeModels _homemodel = new HomeModels();

        // GET: api/apiActiveCode
        public string Get()
        {
            return "true";
        }

        // GET: api/apiActiveCode/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/apiActiveCode
        public bool Post([FromBody]string value)
        {
            return true;
        }

        // PUT: api/apiActiveCode/5
        public bool Put(string id, string tokenkey)
        {
            bool result = true;
            try
            {
                result = _homemodel.ActiveGarden(id, tokenkey);
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        // DELETE: api/apiActiveCode/5
        public void Delete(int id)
        {
        }
    }
}
