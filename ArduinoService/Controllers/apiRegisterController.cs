using ArduinoService.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ArduinoService.Controllers
{
    public class apiRegisterController : ApiController
    {
        AccountModel _accountmodel = new AccountModel();

        // GET: api/apiRegister
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/apiRegister/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/apiRegister
        public int Post([FromBody]RegisterRowData model)
        {
            int result = 1;
            try
            {
                if (_accountmodel.checkUsernameIsexists(model))
                    result = 2; // email da duoc dang ky
                else
                    if (_accountmodel.Register(model) == false)
                        result = -1; // loi he thong
            }
            catch (Exception ex)
            {
                result = -1;
            }
            return result;
        }

        // PUT: api/apiRegister/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/apiRegister/5
        public void Delete(int id)
        {
        }
    }
}
