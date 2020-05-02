using System;
using System.Collections.Generic;
using System.Text;

namespace Varsis.Data.Serviceb1
{
    public class ServiceLayerResponse
    {
        public bool success { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string data { get; set; }

        public List<ServiceLayerResponse> internalResponses { get; set; }
    }
}
