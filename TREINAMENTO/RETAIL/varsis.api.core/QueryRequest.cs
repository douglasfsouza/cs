using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Varsis.Api.Core
{
    public class QueryRequest
    {
        public List<CriteriaRequest> criterias { get; set; }

        public long? size { get; set; } = 10000;

        public long? page { get; set; } = 1;
    }
}
