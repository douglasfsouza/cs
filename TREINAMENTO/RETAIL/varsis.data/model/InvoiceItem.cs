using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
    public class InvoiceItem : EntityBase
    {
        [JsonIgnore]
        public override string EntityName => "Items da Nota";

        [JsonIgnore]
        public override Guid RecId { get => base.RecId; set => base.RecId = value; }

        public string ItemCode { get; set; }
        public long? Usage { get; set; }
        public string MeasureUnit { get; set; }
        public double? PackageQuantity { get; set; }
        public double? Price { get; set; }
        public double? Quantity { get; set; }
        public string TaxCode { get; set; }

        public long? CFOPCode { get; set; }

        public double? UnitPrice { get; set; }

        public double? DiscountPercent { get; set; }

        public List<InvoiceExpenses> DocumentLineAdditionalExpenses { get; set; }


        public string CSTCode { get; set; }
        public string CSTforCOFINS { get; set; }
        public string CSTforIPI { get; set; }
        public string CSTforPIS { get; set; }

        public List<WithholdingTaxLines> WithholdingTaxLines { get; set; }

    }

    public class WithholdingTaxLines
    {
        public long? WTCode { get; set; }
    }
}
