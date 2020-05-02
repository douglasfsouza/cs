using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Integration
{
    public class Store : EntityBase
    {
        public override string EntityName => "Cadastro de Lojas";
        public enum StoreIntegrationStatus
        {
            Importing = 0,
            Created = 1,
            Processed = 2,
            Error = 99
        }
        public long codigo { get; set; }
        public long digito { get; set; }
        public string razao_social { get; set; }
        public string nome_fantasia { get; set; }
        public string endereco { get; set; }
        public string bairro { get; set; }
        public string cidade { get; set; }
        public string estado { get; set; }
        public long? cep { get; set; }
        public string loj_cli { get; set; }
        public string natureza { get; set; }
        public long? data_cad { get; set; }
        public long? cli_emp_princ { get; set; }
        public long? cxpostal { get; set; }
        public long? fax_ddd { get; set; }
        public long? fax_num { get; set; }
        public long? fone_ddd { get; set; }
        public long? fone_num { get; set; }
        public long? telex_ddd { get; set; }
        public long? telex_num { get; set; }
        public long? renpar_ddd { get; set; }
        public long? renpar_num { get; set; }
        public string stm400 { get; set; }
        public string cod_edi { get; set; }
        public string cod_x25 { get; set; }
        public string fis_jur { get; set; }
        public long? cgc_cpf { get; set; }
        public string insc_est_ident { get; set; }
        public string insc_mun { get; set; }
        public long? regiao { get; set; }
        public long? divisao { get; set; }
        public long? distrito { get; set; }
        public long? empresa { get; set; }
        public string linha_tabela { get; set; }
        public string tem_obser { get; set; }
        public long? cod_van { get; set; }
        public long? dig_van { get; set; }
        public long? filler { get; set; }


        public StoreIntegrationStatus status { get; set; }

    }
}
