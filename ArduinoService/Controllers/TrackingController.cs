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
    public class TrackingController : ApiController
    {
        CommonModel _model = new CommonModel();
        TrackingHub _hub = new TrackingHub();
        TrackingModel _trackingModel = new TrackingModel();

        // GET: api/Tracking
        public List<LTrackingRowData> Get()
        {
            return _trackingModel.GetListTrackingHistory(10);
        }

        // POST: api/Tracking
        public void Post([FromBody]UnoTrackingData model)
        {
            // update data
            if (model != null)
            {
                // Insert data
                _trackingModel.AddDataTracking(model);
                // Convert to string post all client
                //string data = _model.ConvertObjectToString(model);
                //_hub.Send(data);
            }
            else
                BadRequest();
        }

        // PUT: api/Tracking/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Tracking/5
        public void Delete(int id)
        {
        }
    }
}
