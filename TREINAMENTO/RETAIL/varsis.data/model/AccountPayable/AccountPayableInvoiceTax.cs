using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.AccountPayable
{
    public class AccountPayableInvoiceTax: EntityBase
    {
        public override string EntityName => "Título do contas a pagar - Impostos";

        public string Code { get; set; }
        public string Name { get; set; }
        public string Titulo { get; set; }
        public string Fato { get; set; }
        public DateTime DataTransacao { get; set; }
        public string CodigoImposto { get; set; }
        public double ValorBase { get; set; }
        public double Aliquota { get; set; }
        public double ValorImposto { get; set; }
        public double ValorRetencao { get; set; }
    }
}
