using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArduinoService.Models
{
    public class Jobclass : IJob
    {
        JobModels jobmodel = new JobModels();

        public void Execute(IJobExecutionContext context)
        {
            jobmodel.UpdateValueControl(DateTime.Now.ToString("HH:mm"));

            // Get datetime now
            var hourNow = DateTime.Now.ToString("%H");
            var minutes = DateTime.Now.ToString("%m");
            if ((hourNow == "00" || hourNow == "0") && (minutes == "00" || minutes == "0"))
            {
                // update database
                jobmodel.UpdateJob();
            }
        }

    }
}