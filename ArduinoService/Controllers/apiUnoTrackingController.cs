using ArduinoService.DataModels;
using ArduinoService.Hubs;
using ArduinoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace ArduinoService.Controllers
{
    public class apiUnoTrackingController : ApiController
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        HomeModels _homemodel = new HomeModels();
        TrackingHub _hub = new TrackingHub();

        // GET: api/apiUnoTracking
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/apiUnoTracking/5
        public string Get(string id)
        {
            return "";
            //return _homemodel.GetListTrackingUno(id);
        }

        // POST: api/apiUnoTracking
        public void Post([FromBody]DataSensor value)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            List<ListValueTrackingUno> listSensor = jss.Deserialize<List<ListValueTrackingUno>>(value.str);

            // Insert data
            bool result = _homemodel.InsertDataTracking(listSensor);
            if (result == false)
                BadRequest();
        }

        // PUT: api/apiUnoTracking/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/apiUnoTracking/5
        public void Delete(int id)
        {
        }
    }

    public class DataSensor
    {
        public string str { get; set; }
    }
}
