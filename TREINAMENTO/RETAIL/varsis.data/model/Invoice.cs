using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
    public class Invoice : EntityBase
    {
        public Invoice()
        {
            DocumentLines = new List<InvoiceItem>();
        }

        [JsonIgnore] 
        public override string EntityName => "Capa da Nota";

        [JsonIgnore]
        public override Guid RecId { get => base.RecId; set => base.RecId = value; }

        public long? DocNum { get; set; }

        public long? DocEntry { get; set; }
        public long? SequenceSerial { get; set; }
        public string SeriesString { get; set; }
        public string SequenceModel { get; set; }
        public string DocDueDate { get; set; }
        public string CardCode { get; set; }
        public long BPL_IDAssignedToInvoice { get; set; }
        public string DocDate { get; set; }
        public string DocType { get; set; }
        public string DocObjectCode { get; set; }
        public long SequenceCode { get; set; }
        public List<InvoiceItem> DocumentLines { get; set; }
        public List<InvoiceVenc> DocumentInstallments { get; set; }
        public List<InvoiceDocumentReferences> DocumentReferences { get; set; }
        public double? DiscountPercent { get; set; }
        public string U_SUBUTILIZACAO { get; set; }
        public string TaxDate { get; set; }
        
        public long? DTA_PRC { get; set; }
        public double? FRT_VAL { get; set; }
        public long? TRN_FAT { get; set; }

        //FRT_VAL, TRN_FAT > 0
    }


}
