using ArduinoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ArduinoService.Controllers
{
    public class apiUnoControlController : ApiController
    {
        HomeModels _homemodel = new HomeModels();

        // GET: api/apiUnoControl
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/apiUnoControl/5
        public string Get(string id)
        {
            return _homemodel.GetListDataControlUno(id);
        }

        // POST: api/apiUnoControl
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/apiUnoControl/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/apiUnoControl/5
        public void Delete(int id)
        {
        }
    }
}
