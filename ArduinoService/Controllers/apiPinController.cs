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
    public class apiPinController : ApiController
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private CommonAPI commonapi = new CommonAPI();
        // GET: api/apiPin
        public List<string> Get(string tokenkey, string category)
        {
            return commonapi.GetListPin(tokenkey, category);
        }

        // GET: api/apiPin/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/apiPin
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/apiPin/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/apiPin/5
        public void Delete(int id)
        {
        }
    }
}
