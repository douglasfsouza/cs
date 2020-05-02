using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Varsis.Data.Serviceb1
{
    public interface IBatchProducer
    {
        public void Post(HttpMethod method, string query, string payload);
    }
}
