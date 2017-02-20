using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using ArduinoService.DataModels;

namespace ArduinoService.Hubs
{
    [HubName("ControlHub")] // name of hub connet with client
    public class ControlHub : Hub
    {
        // Send status to all client
        public void Send(ControlRawData rowdata)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ControlHub>();
            hubContext.Clients.All.SendClient(rowdata);
        }

        public void UpdateSheduleClient(List<ControlRawData> rowdata)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ControlHub>();
            hubContext.Clients.All.SendAllClient(rowdata);
        }
    }
}