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

        static bool flag = false;
        public void Execute(IJobExecutionContext context)
        {
            jobmodel.UpdateValueControl(DateTime.Now.ToString("HH:mm"));

            // Get datetime now
            var hourNow = DateTime.Now.ToString("%h");
            if (hourNow == "12" && flag == false)
            {
                flag = true;
                // update database
                jobmodel.UpdateJob();
                if (hourNow == "1")
                    flag = false;
            }
        }

    }
}