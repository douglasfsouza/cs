using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Varsis.Data.Serviceb1
{
    public class BatchItem
    {
        public HttpMethod method { get; set; }
        public string query { get; set; }
        public string payload { get; set; }
    }
}
