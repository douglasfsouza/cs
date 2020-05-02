using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Varsis.Api.Connector.Contracts
{
    public class InitMonitorRequest
    {
        public string branchIdLegacy { get; set; }
        public DateTime? startDate { get; set; }
    }
}
