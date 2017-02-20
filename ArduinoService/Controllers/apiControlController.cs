using ArduinoService.DataModels;
using ArduinoService.Hubs;
using ArduinoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ArduinoService.Controllers
{
    public class apiControlController : ApiController
    {
        HomeModels _homemodel = new HomeModels();
        ControlHub _hub = new ControlHub();

        // GET: api/apiControl
        // id uno
        public List<ControlRawData> Get(string id)
        {
            return _homemodel.GetListControl(id);
        }

        // GET: api/apiControl
        public string Get()
        {
            return "value uno";
        }

        // POST: api/apiControl
        public bool Post([FromBody]ControlAddRawData model)
        {
            bool result = false;
            try
            {
                // add new control
                if (_homemodel.AddOrEditControl(model.DEVICE_NAME, model.TOKEN_KEY, model.DEVICE_ID, model.PIN_ID))
                    result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        // PUT: api/apiControl/5
        public ControlRawData Put(int id, [FromBody]ControlRawData model)
        {
            try
            {
                if (model != null)
                {
                    if (_homemodel.UpdateValueControl(model))
                        _hub.Send(model);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return model;
        }

        // DELETE: api/apiControl/5
        public bool Delete(string id)
        {
            return _homemodel.DeleteDevice(id);
        }
    }
}
