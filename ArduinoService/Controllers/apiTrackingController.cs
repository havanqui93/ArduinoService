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
    public class apiTrackingController : ApiController
    {
        HomeModels _homemodel = new HomeModels();

        // GET: api/apiTracking
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/apiTracking/tokenkey
        public List<TrackingRawData> Get(string id)
        {
            return _homemodel.GetListTrackingMobile(id);
        }

        // GET: api/apiTracking/tokenkey,id
        public List<ListValueTracking> Get(string tokenkey, string id)
        {
            return _homemodel.GetListValueTrackingMobile(tokenkey, id);
        }

        // POST: api/apiTracking
        public void Post([FromBody]string value)
        {

        }

        // PUT: api/apiTracking/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/apiTracking/5
        public void Delete(int id)
        {
        }
    }
}
