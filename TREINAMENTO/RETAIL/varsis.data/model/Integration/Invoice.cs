using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Integration
{
    public class Invoice : EntityBase
    {
        public override string EntityName => "Nota fiscal - Importação";
        public enum InvoiceIntegrationStatus
        {
            Importing = 0,
            Created = 1,
            Processed = 2,
            Error = 99,
            Journey = 5
        }

        public enum TIPOMOVTOENUM
        {
            Incoming = 0,
            Outgoing = 1
        }

        public TIPOMOVTOENUM TIPOMOVTO;
        public long ID { get; set; }
        //public List<InvoiceItem> items { get; set; }
        public string agenda { get; set; }
        public string parceironegocio { get; set; }
        public string filial { get; set; }
        public string nota { get; set; }
        public string serie { get; set; }
        public string cfop { get; set; }
        public string dataagenda { get; set; }
        public string dataemissao { get; set; }
        public string codigoservico { get; set; }
        public string situacao { get; set; }
        public DateTime LASTUPDATE { get; set; }
        public InvoiceIntegrationStatus status { get; set; }
    }
}
