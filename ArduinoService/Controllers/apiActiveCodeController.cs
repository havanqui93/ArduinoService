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
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/apiActiveCode/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/apiActiveCode
        public void Post([FromBody]string value)
        {

        }

        // PUT: api/apiActiveCode/5
        public bool Put(string id, GardenRawData rowdata)
        {
            bool result = false;
            try
            {
                result = _homemodel.ActiveGarden(id, rowdata);
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
