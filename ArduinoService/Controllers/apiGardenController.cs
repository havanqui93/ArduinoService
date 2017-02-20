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
    public class apiGardenController : ApiController
    {
        HomeModels _homemodel = new HomeModels();

        // GET: api/apiGarden
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/apiGarden/5
        public List<GardenRawData> Get(string id)
        {
            return _homemodel.GetListGarden(id);
        }

        // POST: api/apiGarden
        public bool Post([FromBody]GardenRawData model)
        {
            bool result = false;
            try
            {
                var data = _homemodel.AddOrEditGarden(model);
                if (data != null)
                    result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        // PUT: api/apiGarden/5
        public bool Put(string id, [FromBody]GardenRawData model)
        {
            bool result = false;
            try
            {
                var data = _homemodel.AddOrEditGarden(model);
                if (data != null)
                    result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        // DELETE: api/apiGarden/5
        public bool Delete(string id)
        {
            return _homemodel.DeleteGarden(id);
        }
    }
}
