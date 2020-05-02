using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Connector
{
    public class POSMonitor : EntityBase
    {
        public override string EntityName => "Monitor de importação de vendas";

        public DateTime TransactionDate { get; set; }
        public int BranchId { get; set; }
        public string BranchIdLegacy { get; set; }
        public IntegrationStatus Status { get; set; }
    }
}
