using System;
using System.Collections.Generic;
using System.Text;

namespace Varsis.Data.Serviceb1
{
    /*
     * "error" : {
            "code" : -2035,
            "message" : {
                "lang" : "en-us",
                "value" : "This entry already exists in the following tables (ODBC -2035)"
            }
        }
   */

    public class ServiceLayerErrorMessage
    {
        public string lang { get; set; }
        public string value { get; set; }
    }
    public class ServiceLayerError
    {
        public string code { get; set; }
        public ServiceLayerErrorMessage message { get; set; }
    }
}
