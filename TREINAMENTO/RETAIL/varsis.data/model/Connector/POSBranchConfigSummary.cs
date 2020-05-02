using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Connector
{
    public class POSBranchConfigSummary 
    {
        public string RecId { get; set; }
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public string BranchIdLegacy { get; set; }
        public string DefaultCustomer { get; set; }
        public string DefaultCustomerName { get; set; }
        public long UsageCode { get; set; }
        public string UsageCodeDescription { get; set; }

        public DateTime? OpeningDate { get; set; }
    }
}
