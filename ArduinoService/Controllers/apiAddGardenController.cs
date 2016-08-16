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
    public class apiAddGardenController : ApiController
    {
        HomeModels _homemodel = new HomeModels();

        // GET: api/apiAddGarden
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/apiAddGarden/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/apiAddGarden
        public GardenRawData Post([FromBody]GardenRawData model)
        {
            try
            {
                _homemodel.AddGarden(model);
            }
            catch (Exception ex)
            {
                model = null;
            }
            return model;
        }

        // PUT: api/apiAddGarden/5
        public GardenRawData Put(int id, [FromBody]GardenRawData model)
        {
            try
            {
                _homemodel.AddGarden(model);
            }
            catch (Exception ex)
            {
                model = null;
            }
            return model;
        }

        // DELETE: api/apiAddGarden/5
        public void Delete(int id)
        {
        }
    }
}
