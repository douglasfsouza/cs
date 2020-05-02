using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
    public class MockTeste : EntityBase
    {
        public override string EntityName => "";
        public string ParceiroDeNegocio { get; set; }
        public string Empresa { get; set; }
        public string Filial { get; set; }
        public string Conta { get; set; }
        public string TituloT { get; set; }
        public string Serie { get; set; }
        public string Vencimento { get; set; }
        public string Dias { get; set; }
        public string Val { get; set; }
        public string Forma { get; set; }
        public string ValorBruto { get; set; }
        public string ValorLiquido { get; set; }
        public string status { get; set; }
    }
}
