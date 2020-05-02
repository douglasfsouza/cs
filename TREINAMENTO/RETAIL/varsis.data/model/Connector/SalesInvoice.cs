using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Connector
{
    public class SalesInvoice : EntityBase
    {
        public override string EntityName => "Nota fiscal de venda";

        public string CodigoCliente { get; set; }
        public long CodigoEmpresa { get; set; }
        public string DataLancamento { get; set; }
        public string DataLiberacao { get; set; }
        public string NumeroRP { get; set; }
        public string NumeroNegociacao { get; set; }
        public string CodigoCondicaoPagto { get; set; }
        public string Observacao { get; set; }
        public string Natureza { get; set; }
        public string TipoPessoa { get; set; }
        public string TipoOrdemFat { get; set; }

        public List<SalesInvoiceItem> Itens { get; set; }
        public List<SalesInvoiceInstallment> Parcelas { get; set; }

        [JsonIgnore]
        public SalesInvoiceResponse Response { get; set; }
    }
}
