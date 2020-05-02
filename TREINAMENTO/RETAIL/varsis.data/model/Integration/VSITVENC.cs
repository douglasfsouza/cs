using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Integration
{
    public class VSITVENC : EntityBase
    {
        public override string EntityName => "Cadastro de VSITVENC";
        public enum VSITVENCIntegrationStatus { Importing = 0, Created = 1, Processed = 2, Error = 99 }
        public string lastupdate { get; set; }
        public VSITVENCIntegrationStatus status { get; set; }
        public long? loj_org { get; set; }
        public long? dig_org { get; set; }
        public long? nro_nota { get; set; }
        public string serie { get; set; }
        public long? dta_agenda { get; set; }
        public long? oper { get; set; }
        public long? dta_vencto { get; set; }
        public double? vlr_parcela { get; set; }
        public double? vlr_desconto { get; set; }
        public double? vlr_acr_fin { get; set; }
        public long? fora_pgto { get; set; }
        public double? vlr_comissao { get; set; }
    }
}
