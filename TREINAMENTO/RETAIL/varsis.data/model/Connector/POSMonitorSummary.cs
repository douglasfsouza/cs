using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Connector
{
    public class POSMonitorSummary
    {
        public string RecId { get; set; }
        public DateTime TransactionDate { get; set; }
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public string BranchIdLegacy { get; set; }
        public IntegrationStatus Status { get; set; }
        public long CountErrors { get; set; }
        public long CountTickets { get; set; }
        public double SumTickets { get; set; }
    }
}
