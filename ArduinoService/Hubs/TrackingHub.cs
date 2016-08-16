using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArduinoService.Hubs
{
    [HubName("TrackingData")]
    public class TrackingHub :Hub
    {
        // Send data to all client
        public void Send(string value)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<TrackingHub>();
            hubContext.Clients.All.SendClient(value);
        }

    }
}