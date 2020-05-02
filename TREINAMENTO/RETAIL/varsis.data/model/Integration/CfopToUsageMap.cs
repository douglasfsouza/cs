using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Integration 
{
    public class CfopToUsageMap : EntityBase
    {
        public enum DocumentTypeEnum
        {
            Invoice = 1,
            PurchInvoice = 2,
            oPurchaseCreditNotes = 3,
            LancamentoManual = 4
        }

        public enum WarehouseEnum
        {
            MoveStore = 1,
            NotMoveStore = 99
        }

        public override string EntityName => "Relação entre Agenda e Utilização";

        public long Cfop { get; set; }
        public string UsageLegacy { get; set; }
        public DocumentTypeEnum DocumentType { get; set; }
        public long Warehouse { get; set; }
        public long Usage { get; set; }
        public string TaxCode { get; set; }
        public string ServiceItem { get; set; }
        
        public long? ContaPN { get; set; }
        public string ContaDebito { get; set; }
        public string ContaCredito { get; set; }
        public string ContaControle { get; set; }
        public string ContaTaxa { get; set; }
        public string Observacoes { get; set; }
    }
}
