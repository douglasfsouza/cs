using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Connector 
{
    public class POSInvoice : EntityBase
    {
        public override string EntityName => "Notas fiscais de saída";
        public long DocumentEntry { get; set; }
        public long DocumentNum { get; set; }
        public string DocumentType { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime DocumentTime { get; set; }
        public DateTime DueDate { get; set; }
        public string CustomerId { get; set; }
        public long BranchId { get; set; }
        public long SalesPersonId { get; set; }
        public long InvoiceId { get; set; }
        public string InvoiceSeries { get; set; }
        public string InvoiceModel { get; set; }
        public string FiscalKey { get; set; }
        public List<POSInvoiceItem> Items { get; set; }
    }
}
