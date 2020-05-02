using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Integration
{
    public class VSITENTIDADECONT : EntityBase
    {
public override string EntityName => "Cadastro de VSITENTIDADECONT"; 
public enum VSITENTIDADECONTIntegrationStatus{Importing = 0, Created = 1, Processed = 2, Error = 99}
public DateTime lastupdate { get; set; }
public VSITENTIDADECONTIntegrationStatus status { get; set; }
public string  nome { get; set; }
public long  codigo { get; set; }
public long  seq { get; set; }
public string  cargo { get; set; }
public string  email_1 { get; set; }
public string  email_2 { get; set; }
public string  email_1_flags { get; set; }
public string  email_2_flags { get; set; }
public string  site { get; set; }
public long?  tipo_tel_1 { get; set; }
public long?  tipo_tel_2 { get; set; }
public long?  tipo_tel_3 { get; set; }
public long?  tipo_tel_4 { get; set; }
public long?  tipo_tel_5 { get; set; }
public long?  ddd_1 { get; set; }
public long?  ddd_2 { get; set; }
public long?  ddd_3 { get; set; }
public long?  ddd_4 { get; set; }
public long?  ddd_5 { get; set; }
public long?  fone_1 { get; set; }
public long?  fone_2 { get; set; }
public long?  fone_3 { get; set; }
public long?  fone_4 { get; set; }
public long?  fone_5 { get; set; }
public string  obs { get; set; }
public string  crm { get; set; }
public string  cnpj_auto_xml { get; set; }
public long?  cpf_auto_xml { get; set; }
        public string desc_cargo { get; set; }
    }
}
