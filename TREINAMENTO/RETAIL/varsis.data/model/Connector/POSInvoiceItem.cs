using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Connector
{
    public class POSInvoiceItem : EntityBase
    {
        
        public override string EntityName => "Notas fiscais de saída (items)";
        public long LineSequence { get; set; }
        public string ItemId { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public long Usage { get; set; }
    }
}
