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
        public static string GROUP_ADMIN = "000";

        // session
        public static string SESSION_USERNAME = "SESSION_USERNAME";
        public static string SESSION_ROLE = "SESSION_ROLE";
        public static string SESSION_FULLNAME = "SESSION_FULLNAME";
        public static string SESSION_USERID = "SESSION_USERID";

        // pagging
        public const int PAGE_SIZE_DEFAULT = 15;

        // token length
        public const int TOKEN_SIZE_DEFAULT = 10;

        public const int SENSOR_HT = 1;

        // constants new version
        public const string PACKED_DEFAULT = "001"; // packed default : control by hand
        public const string PACKED_SHEDULE = "002"; // packed shedule : control by shedule
        public const string PACKED_AUTO = "003"; // packed automatic : control automatic
        public const int IS_SUCCESS = 1;
        public const int IS_FAILD = -1;

        public const int CONTROL = 1;
        public const int SENSOR = 2;

        public const int LIVE = 1; // status = 1 => live
        public const int DIE = 2;

        public const string VALUE_INIT_CONTROL_DEFAULT = "OFF";

        public static string USER_TYPE = "USER_TYPE";

        // GROUP SENSOR
        public static int GROUP_HT = 1; // humidity and temperature
        public static int GROUP_MOISTURE = 2;
        public static int GROUP_LIGHT = 3;

        public static string TIME_MIN = "00:00";
        public static string TIME_MAX = "23:00";

        public static string TABLE_GROUP_HT = "D_DEVICE_SENSOR_HT";
        public static string TABLE_GROUP_LIGHT = "D_DEVICE_SENSOR_LIGHT";
        public static string TABLE_GROUP_MOISTURE = "D_DEVICE_SENSOR_MOISTURE";
    }
}