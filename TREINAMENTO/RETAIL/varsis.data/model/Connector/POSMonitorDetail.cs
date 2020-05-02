using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Connector
{
    public class POSMonitorDetail : EntityBase
    {
        public override string EntityName => "Detalhes do monitoramento de importação de venda";

        public Guid POSMonitor { get; set; }
        public string POSId { get; set; }
        public DateTime TransactionTime { get; set; }
        public string InvoiceId { get; set; }
        public double totalAmount { get; set; }
        public int itemsCount { get; set; }
        public DetailIntegrationStatus status { get; set; }
        public string errorMessage { get; set; }

        public long? DocNum { get; set; }
    }
}
