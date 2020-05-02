using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Connector
{
    public class POSBranchConfig : EntityBase
    {
        public override string EntityName => "Configuração de POS por filial";
        public long BranchId { get; set; }
        public string BranchIdLegacy { get; set; }
        public string DefaultCustomer { get; set; }
        public long UsageCode { get; set; }
        public DateTime OpeningDate { get; set; }
        public string CashAccount { get; set; }
        public long CreditCardId { get; set; }
        public long DebitCardId { get; set; }
        public string Code { get; set; }
    }
}
