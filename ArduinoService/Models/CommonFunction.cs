using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArduinoService.Models
{
    public class CommonFunction
    {
        public bool IsNullOrEmpty(object str)
        {
            if (str == null || str.ToString() == string.Empty || string.IsNullOrEmpty(str.ToString()))
                return true;
            return false;
        }
    }
}