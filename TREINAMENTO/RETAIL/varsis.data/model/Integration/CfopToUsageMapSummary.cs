using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Integration 
{
    public class CfopToUsageMapSummary 
    {
        public string recId { get; set; }
        public long cfop { get; set; }
        public string usageLegacy { get; set; }
        public string usageLegacyName { get; set; }
        public long documentType { get; set; }

        public long warehouse { get; set; }
        public long usage { get; set; }
        public string taxCode { get; set; }
        public string serviceItem { get; set; }
        public string serviceItemName { get; set; }
    }
}
