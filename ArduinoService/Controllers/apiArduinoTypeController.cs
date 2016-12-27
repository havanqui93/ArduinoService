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
    public class apiArduinoTypeController : ApiController
    {
        private CommonAPI commonapi = new CommonAPI();
        // GET: api/apiArduinoType
        public List<ArduinoTypeRowData> Get()
        {
            return commonapi.GetListArduinoType();
        }

        // GET: api/apiArduinoType/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/apiArduinoType
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/apiArduinoType/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/apiArduinoType/5
        public void Delete(int id)
        {
        }
    }
}
