using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Integration
{
    public class VSITINVOICE : EntityBase
    {
        public VSITINVOICE()
        {
            vsititem = new List<VSITITEM>();
            vsitvenc = new List<VSITVENC>();
        }
        public override string EntityName => "Cadastro de VSITINVOICE";
        public enum VSITINVOICEIntegrationStatus { Importing = 0, Created = 1, Processed = 2, Error = 99 }
        public string lastupdate { get; set; }
        public string data_inclusao { get; set; }
        public string data_integracao { get; set; }
        public VSITINVOICEIntegrationStatus status { get; set; }
        public string ver { get; set; }
        public double? fnt_val { get; set; }
        public double? fun_val { get; set; }
        public double? cpl_val { get; set; }
        public string cgc_cpf { get; set; }
        public string t01_pag { get; set; }
        public string t04_fis { get; set; }
        public double? fnt_ffi { get; set; }
        public double? eft_bas { get; set; }
        public long? fin { get; set; }
        public double? fcr_bas { get; set; }
        public long? emp { get; set; }
        public double? vol { get; set; }
        public long? id { get; set; }
        public double? pir_bas { get; set; }
        public long? ven { get; set; }
        public double? ins_bas { get; set; }
        public long? ctb { get; set; }
        public long? alt_dta { get; set; }
        public double? csl_bas { get; set; }
        public long? org { get; set; }
        public double? csl_val { get; set; }
        public double? cor_val { get; set; }
        public double? fnt_gre { get; set; }
        public double? iss_val { get; set; }
        public long? fil_ven { get; set; }
        public double? ips_bas { get; set; }
        public long? cnd_pgt { get; set; }
        public string t11_tpo { get; set; }
        public double? ipi_val { get; set; }
        public string chv_nfe { get; set; }
        public double? fps_val { get; set; }
        public double? dif_val { get; set; }
        public string tpo_cbr { get; set; }
        public double? pvv_val { get; set; }
        public double? cof_val { get; set; }
        public string ser { get; set; }
        public string t03_ens { get; set; }
        public long? tpo_frt { get; set; }
        public double? eft_val { get; set; }
        public double? ipi_adi { get; set; }
        public long? cmp { get; set; }
        public double? pis_val { get; set; }
        public double? cor_bas { get; set; }
        public long? trn_cpf { get; set; }
        public double? dfo_bas { get; set; }
        public double? ips_val { get; set; }
        public long? dta { get; set; }
        public string trn_pla { get; set; }
        public long? cfs { get; set; }
        public long? dta_emi { get; set; }
        public string trn_ufp { get; set; }
        public double? pes { get; set; }
        public double? dfd_bas { get; set; }
        public string t05_rec { get; set; }
        public long? dta_apa { get; set; }
        public double? ctb_val { get; set; }
        public long? int_dta { get; set; }
        public long? tpo_pag { get; set; }
        public double? icm_bas { get; set; }
        public double? pir_val { get; set; }
        public long? int_hrs { get; set; }
        public double? irf_bas { get; set; }
        public long? por { get; set; }
        public double? pvv_adi { get; set; }
        public string t09_ite { get; set; }
        public string alt_usu { get; set; }
        public string int_ctb { get; set; }
        public double? dsc_trb { get; set; }
        public double? cpl_trb { get; set; }
        public double? fcs_bas { get; set; }
        public string t02_nta { get; set; }
        public double? cof_bas { get; set; }
        public double? mer_val { get; set; }
        public string int_usu { get; set; }
        public double? cpl_ise { get; set; }
        public long? dst { get; set; }
        public string mdl { get; set; }
        public double? icm_ise { get; set; }
        public long? crf { get; set; }
        public long? alt_hrs { get; set; }
        public double? pvv_ffi { get; set; }
        public double? iss_bas { get; set; }
        public double? frt_val { get; set; }
        public double? seg_val { get; set; }
        public long? dta_prc { get; set; }
        public double? crd_pre { get; set; }
        public long? dta_bas { get; set; }
        public long? fil_pri { get; set; }
        public double? fcp_bas { get; set; }
        public double? ins_val { get; set; }
        public long? cfo { get; set; }
        public long? sit_nfe { get; set; }
        public double? fps_bas { get; set; }
        public string ser_fis { get; set; }
        public double? des_val { get; set; }
        public double? dsp_fin { get; set; }
        public long? trn_fat { get; set; }
        public long? prv { get; set; }
        public double? ipi_bas { get; set; }
        public string t13_emi { get; set; }
        public string est { get; set; }
        public double? fcs_val { get; set; }
        public string t12_dbr { get; set; }
        public string edi { get; set; }
        public double? dfd_val { get; set; }
        public string t06_ctb { get; set; }
        public double? irf_val { get; set; }
        public double? icm_val { get; set; }
        public double? pvv_gre { get; set; }
        public long? nta { get; set; }
        public double? dfo_val { get; set; }
        public double? fcp_val { get; set; }
        public long? nta_nfe { get; set; }
        public double? fcr_val { get; set; }
        public double? fnt_adi { get; set; }
        public double? pis_bas { get; set; }
        public double? des_bas { get; set; }
        public double? dsc_ise { get; set; }
        public string sit { get; set; }
        public long? age { get; set; }
        public string tipo_pn { get; set; }
        public long? docentry { get; set; }
        public List<VSITITEM> vsititem { get; set; }
        public List<VSITVENC> vsitvenc { get; set; }

        // Campos de exibição
        public CfopToUsageMap.DocumentTypeEnum DocumentType { get; set; }
    }
}
