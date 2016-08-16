using ArduinoService.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ArduinoService.Controllers
{
    public class apiLoginController : ApiController
    {
        AccountModel _accountmodel = new AccountModel();

        // GET: api/apiLogin
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/apiLogin/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/apiLogin
        public bool Post([FromBody]AccountRowData model)
        {
            try
            {
                return _accountmodel.Login(model);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // PUT: api/apiLogin/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/apiLogin/5
        public void Delete(int id)
        {
        }
    }
}
