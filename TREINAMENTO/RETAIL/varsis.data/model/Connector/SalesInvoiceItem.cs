using System;
using System.Collections.Generic;
using System.Text;

namespace Varsis.Data.Model.Connector
{
    public class SalesInvoiceItem
    {
        public string CodigoProduto { get; set; }
        public double Quantidade { get; set; }
        public double ValorLiquido { get; set; }
        public string VerticalNegocio { get; set; }
        public string Programacao { get; set; }
        public string Escritorio { get; set; }
    }
}
