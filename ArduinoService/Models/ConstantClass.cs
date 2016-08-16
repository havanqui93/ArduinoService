using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArduinoService.Models
{
    public static class ConstantClass
    {
        public static string TEMPERATURE_DeviceName = "TEMPERATURE";
        public static string HUMIDITY_DeviceName = "HUMIDITY";
        public static string MOISTURE_DeviceName = "MOISTURE";

        // dinh nghia ID cua tung device
        public static string TEMPERATURE_ID = "001";
        public static string HUMIDITY_ID = "002";
        public static string MOISTURE_ID = "003";

        //
        public static int GROUP_ADMIN = 1;
        public static int GROUP_USER = 2;

        // session
        public static string SESSION_USERNAME = "SESSION_USERNAME";
        public static string SESSION_ROLE = "SESSION_USERNAME";
        public static string SESSION_FULLNAME = "SESSION_FULLNAME";

    }
}