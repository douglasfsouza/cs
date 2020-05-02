using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Connector
{
    public class BusinessPartners : EntityBase
    {
        public override string EntityName => "Parceiros de negócios";

        public string Codigo { get; set; }
        public string Tipo { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public long CodigoGrupo { get; set; }
        public string ContaFinanceira { get; set; }
        public string ContaAdiantamento { get; set; }
        public string ContaBoleto { get; set; }
        public string ContaEmAberto { get; set; }
        public string CodigoGrupoImpostoRetido { get; set; }

        public string FoneFixo { get; set; }
        public string DDDFoneFixo { get; set; }
        public string Email { get; set; }

        public string TipoLogradouro { get; set; }
        public string Logradouro { get; set; }
        public string NumeroLogradouro { get; set; }
        public string ComplementoLogradouro { get; set; }
        public string CEP { get; set; }
        public string Bairro { get; set; }
        public string Municipio { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }

        public string CNPJ { get; set; }
        public string InscricaoEstadual { get; set; }
        public string InscricaoMunicipal { get; set; }
        public string CPF { get; set; }
        public string IdEstrangeiro { get; set; }
    }
}
