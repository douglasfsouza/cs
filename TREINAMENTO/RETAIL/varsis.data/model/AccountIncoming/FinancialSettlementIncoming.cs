using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.AccountIncoming
{
    public class FinancialSettlementIncoming : EntityBase
    {
        public override string EntityName => "Abatimento Contas a pagar x Contas a receber";
        public string Code { get; set; }
        public string Name { get; set; }
        public string CodigoFornecedor { get; set; }
        public string TituloPagar { get; set; }
        public string TituloReceber { get; set; }
        public double ValorAbatimento { get; set; }
        public DateTime DataTransacao { get; set; }
        public string UsuarioTransacao { get; set; }
    }
}
