using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dgsServiceLayer.Models
{
    public class ServiceLayerError
    {
        public string code { get; set; }
        public ServiceLayerErrorMessage message { get; set; }
        public class ServiceLayerErrorMessage
        {
            public string lang { get; set; }
            public string value { get; set; }
        }        
    }
}
