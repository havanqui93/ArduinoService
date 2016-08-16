using ArduinoService.DataModels;
using ArduinoService.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ArduinoService.Hubs
{
    [HubName("LedHub")] // name of hub connet with client
    public class LedHub : Hub
    {
        // Send status to all client
        public void Send(string list)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<LedHub>();
            hubContext.Clients.All.SendClient(list);

            //var mystring = JsonConvert.DeserializeObject<string>(list);
            // Add message to all client
            //Clients.All.SendClient(list);
        }
    }
}