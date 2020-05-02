using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Integration
{
    public class VSITENTIDADE : EntityBase
    {
        public override string EntityName => "Dados complementares Cadastro de Entidade";
        public enum VSITENTIDADEIntegrationStatus
        {
            Importing = 0,
            Created = 1,
            Processed = 2,
            Error = 99
        }
        public string natureza { get; set; } //aa2ctipo
        public long codigo { get; set; } //aa2ctipo
        public string nome_fantasia { get; set; } //aa2ctipo
        public string razao_social { get; set; } //aa2ctipo
        public string cgc_cpf { get; set; } //aa2ctipo
        public string insc_est_ident { get; set; } //aa2ctipo
        public string insc_mun { get; set; } //aa2ctipo
        public long? cep { get; set; } //aa2ctipo
        public string bairro { get; set; } //aa2ctipo
        public string cidade { get; set; } //aa2ctipo
        public string estado { get; set; } //aa2ctipo
        public string cod_x25 { get; set; } //aa2ctipo
        public string loj_cli { get; set; } //aa2ctipo
        public string insc_est_subst { get; set; } //AA1DTIPO
        public string cod_municipio { get; set; } //AA1DTIPO
        public string nire { get; set; } //AA1DTIPO
        public string suframa { get; set; }  //AA1DTIPO
        public string localidade { get; set; } //AG1CDCNF
        public string nr_interior { get; set; } //AG1CDCNF
        public string nr_exterior { get; set; } //AG1CDCNF
        public long? data_fecha { get; set; } //AA2CLOJA
        //campos faltantes
        //ocrd_cli_for
        public string for_contato { get; set; }
        public string cli_contato { get; set; }
        public long? forpri { get; set; }
        public long? data_cad { get; set; }
        public long? dta_flinha { get; set; }
        public string situacao { get; set; }
        //crd1end
        //crd7
        //ocrb
        public long? banco { get; set; }
        public long? conta { get; set; }
        public long? agencia { get; set; }
        public long? dig_conta { get; set; }
        public long? dig_agen { get; set; }
        //ocpr
        public long? cargo { get; set; }

        public List<Model.Integration.VSITENTIDADECONT> contatos { get; set; }
         
        public DateTime? lastupdate { get; set; }
        public DateTime? data_inclusao { get; set; }
        public DateTime? data_integracao { get; set; }
        public VSITENTIDADEIntegrationStatus status { get; set; }
    }
}
