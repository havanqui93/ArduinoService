using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using ArduinoService.DataModels;

namespace ArduinoService.Hubs
{
    [HubName("UpdateValueSheduleHub")] // name of hub connet with client
    public class UpdateValueSheduleHub : Hub
    {
        // Send status to all client
        public void Send(List<SettingControlDetailRawData> rowdata,string deviceid)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<UpdateValueSheduleHub>();
            hubContext.Clients.All.SendClient(rowdata, deviceid);
        }

        /// <summary>
        /// Thuc hien chuc nang Bat/Tat khu vuon
        /// </summary>
        public void UpdateShedule(string isOn)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<UpdateValueSheduleHub>();
            hubContext.Clients.All.SendStatusClient(isOn);
        }

    }
}