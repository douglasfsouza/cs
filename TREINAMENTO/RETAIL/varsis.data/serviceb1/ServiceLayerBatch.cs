using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Varsis.Data.Serviceb1
{
    public class ServiceLayerBatch : IBatchProducer, IBatchProvider
    {
        private List<BatchItem> _Items;

        public ServiceLayerBatch()
        {
            _Items = new List<BatchItem>();
        }

        public List<BatchItem> Items => _Items;

        public void Post(HttpMethod method, string query, string payload)
        {
            _Items.Add(new BatchItem()
            {
                method = method,
                query = query,
                payload = payload
            });
        }
    }
}
