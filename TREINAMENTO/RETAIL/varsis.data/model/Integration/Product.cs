using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Integration
{
    public class Product : EntityBase
    {
        public override string EntityName => "Cadastro de Produto";
        public enum ProductIntegrationStatus
        {
            Importing = 0,
            Created = 1,
            Processed = 2,
            Error = 99
        }

        public Product()
        {
            components = new List<ProductComponent>();
            vendors = new List<ProductVendor>();
        }

        public List<ProductComponent> components { get; set; }
        public List<ProductVendor> vendors { get; set; }

        public long cod_item { get; set; }
        public string descricao { get; set; }
        public long? tipo_plu { get; set; }
        public long cod_for { get; set; }
        public string desc_coml { get; set; }
        public string  desc_reduz { get; set; }
        public string referencia { get; set; }
        public string codigo_ean13 { get; set; }
        public string tpo_emb_for { get; set; }
        public long? emb_for { get; set; }
        public string tpo_emb_venda { get; set; }
        public long? emb_venda{ get; set; }
        public double? altura_emb_vnd { get; set; }
        public double? largura_emb_vnd { get; set; }
        public double? compr_emb_vnd { get; set; }
        public long? class_fis { get; set; }
        public long? procedencia { get; set; }
        public long? dat_sai_lin { get; set; }

        public DateTime lastupdate { get; set; }

       public double? altura_emb { get; set; }
        public double? largura_emb { get; set; }

        public double? comprimento_emb { get; set; }
        public double? peso { get; set; }

        public long? dat_ent_lin { get; set; }

        public long? tipo_pro { get; set; }

        //Campos da tabela AA1DITEM

        public string cest { get; set; }
        public string controle_lote_serie { get; set; }
        public long? tip_pro_sped { get; set; }




        /*
        public long? DEPTO { get; set; }
        public long? SECAO { get; set; }
        public long? GRUPO { get; set; }
        public long? SUBGRUPO { get; set; }
        public long? CATEGORIA { get; set; }
        public long DIGITO { get; set; }
        public long? CODIGO_PAI { get; set; }
        public long? EAN_EMB_FOR { get; set; }
        public long? EMB_TRANSF { get; set; }
        public string TPO_EMB_TRANSF { get; set; }
        public long? EAN_EMB_TRANSF { get; set; }
        public long? EAN_EMB_VENDA { get; set; }
        public long? TIPO_PALLET { get; set; }
        public long? BASE_PALLET { get; set; }
        public long? ALTURA_PALLET { get; set; }
        public double? COMPRIMENTO_EMB { get; set; }
        public double? COMPR_EMB_TRF { get; set; }
        
        public double? LARGURA_EMB_TRF { get; set; }    
        
        public double? ALTURA_EMB_TRF { get; set; }
        
        public double? PESO_TRF { get; set; }
        public double? PESO_VND { get; set; }
        public long? DEPOSITO { get; set; }
        public string LINHA { get; set; }
        public string CLASSE { get; set; }
        public string POLIT_PRE { get; set; }
        public long? SIS_ABAST { get; set; }
        public long? PRZ_ENTRG { get; set; }
        public long? DIA_VISIT { get; set; }
        public long? FRQ_VISIT { get; set; }
        public long? TIPO_ETQ { get; set; }
        public long? TIPO_PRO { get; set; }
        public long? COMPRADOR { get; set; }
        public long? COND_PGTO { get; set; }
        public long? COND_PGTO_ANT { get; set; }
        public long? COND_PGTO_MAN { get; set; }
        public long? CDPG_VDA { get; set; }
        public long? TIPO_ETQ_GON { get; set; }
        public string COR { get; set; }
        
        
        public long? NAT_FISCAL { get; set; }
        public string ESTADO { get; set; }
        public long? COD_PAUTA { get; set; }
        public double? PERC_IPI { get; set; }
        public long? QTDE_ETQ_GON { get; set; }
        public double? PERC_BONIF { get; set; }
        public double? PERC_BONIF_ANT { get; set; }
        public double? PERC_BONIF_MAN { get; set; }
        public long? ENTREGA { get; set; }
        public double? FRETE { get; set; }
        public double? CUS_FOR { get; set; }
        public double? CUSF_ANT { get; set; }
        public double? CUSF_MAN { get; set; }
        public long? DAT_CUS_FOR { get; set; }
        public long? DAT_CUSF_ANT { get; set; }
        public long? DAT_CUSF_MAN { get; set; }
        public double? DESP_ACES { get; set; }
        public double? DESP_ACES_ANT { get; set; }
        public double? DESP_ACES_MAN { get; set; }
        public double? CUS_REP { get; set; }
        public double? CUSR_ANT { get; set; }
        public double? CUSR_MAN { get; set; }
        public long? DAT_CUS_REP { get; set; }
        public long? DAT_CUSR_ANT { get; set; }
        public long? DAT_CUSR_MAN { get; set; }
        public double? CUS_MED { get; set; }
        public double? CUSM_ANT { get; set; }
        public double? CUSM_MAN { get; set; }
        public long? DAT_CUS_MED { get; set; }
        public long? DAT_CUSM_ANT { get; set; }
        public long? DAT_CUSM_MAN { get; set; }
        public double? CUS_MED_C { get; set; }
        public long? DAT_CUS_MED_C { get; set; }
        public double? PRC_VEN_1 { get; set; }
        public double? PRCV_ANT_1 { get; set; }
        public double? PRCV_MAN_1 { get; set; }
        public long? MRG_LUCRO_1 { get; set; }
        public long? DSC_MAX_1 { get; set; }
        public double? COMISSAO_1 { get; set; }
        public long? DAT_PRC_VEN_1 { get; set; }
        public long? DAT_PRCV_ANT_1 { get; set; }
        public long? DAT_PRCV_MAN_1 { get; set; }
        public double? PRC_VEN_2 { get; set; }
        public double? PRCV_ANT_2 { get; set; }
        public double? PRCV_MAN_2 { get; set; }
        public long? MRG_LUCRO_2 { get; set; }
        public long? DSC_MAX_2 { get; set; }
        public double? COMISSAO_2 { get; set; }
        public long? DAT_PRC_VEN_2 { get; set; }
        public long? DAT_PRCV_ANT_2 { get; set; }
        public long? DAT_PRCV_MAN_2 { get; set; }
        public double? PRC_VEN_3 { get; set; }
        public double? PRCV_ANT_3 { get; set; }
        public double? PRCV_MAN_3 { get; set; }
        public long? MRG_LUCRO_3 { get; set; }
        public long? DSC_MAX_3 { get; set; }
        public double? COMISSAO_3 { get; set; }
        public long? DAT_PRC_VEN_3 { get; set; }
        public long? DAT_PRCV_ANT_3 { get; set; }
        public long? DAT_PRCV_MAN_3 { get; set; }
        public double? PRC_VEN_4 { get; set; }
        public double? PRCV_ANT_4 { get; set; }
        public double? PRCV_MAN_4 { get; set; }
        public long? MRG_LUCRO_4 { get; set; }
        public long? DSC_MAX_4 { get; set; }
        public double? COMISSAO_4 { get; set; }
        public long? DAT_PRC_VEN_4 { get; set; }
        public long? DAT_PRCV_ANT_4 { get; set; }
        public long? DAT_PRCV_MAN_4 { get; set; }
        public double? PRC_VEN_5 { get; set; }
        public double? PRCV_ANT_5 { get; set; }
        public double? PRCV_MAN_5 { get; set; }
        public long? MRG_LUCRO_5 { get; set; }
        public long? DSC_MAX_5 { get; set; }
        public double? COMISSAO_5 { get; set; }
        public long? DAT_PRC_VEN_5 { get; set; }
        public long? DAT_PRCV_ANT_5 { get; set; }
        public long? DAT_PRCV_MAN_5 { get; set; }
        public long? TIP_OFT_1 { get; set; }
        public long? TIP_OFT_ANT_1 { get; set; }
        public long? INI_OFT_1 { get; set; }
        public long? INI_OFT_ANT_1 { get; set; }
        public long? FIM_OFT_1 { get; set; }
        public long? FIM_OFT_ANT_1 { get; set; }
        public double? PRC_OFT_1 { get; set; }
        public double? PRC_OFT_ANT_1 { get; set; }
        public long? LIM_OFT_1 { get; set; }
        public long? LIM_OFT_ANT_1 { get; set; }
        public long? SAL_OFT_1 { get; set; }
        public long? SAL_OFT_ANT_1 { get; set; }
        public long? TIP_OFT_2 { get; set; }
        public long? TIP_OFT_ANT_2 { get; set; }
        public long? INI_OFT_2 { get; set; }
        public long? INI_OFT_ANT_2 { get; set; }
        public long? FIM_OFT_2 { get; set; }
        public long? FIM_OFT_ANT_2 { get; set; }
        public double? PRC_OFT_2 { get; set; }
        public double? PRC_OFT_ANT_2 { get; set; }
        public long? LIM_OFT_2 { get; set; }
        public long? LIM_OFT_ANT_2 { get; set; }
        public long? SAL_OFT_2 { get; set; }
        public long? SAL_OFT_ANT_2 { get; set; }
        public long? TIP_OFT_3 { get; set; }
        public long? TIP_OFT_ANT_3 { get; set; }
        public long? INI_OFT_3 { get; set; }
        public long? INI_OFT_ANT_3 { get; set; }
        public long? FIM_OFT_3 { get; set; }
        public long? FIM_OFT_ANT_3 { get; set; }
        public double? PRC_OFT_3 { get; set; }
        public double? PRC_OFT_ANT_3 { get; set; }
        public long? LIM_OFT_3 { get; set; }
        public long? LIM_OFT_ANT_3 { get; set; }
        public long? SAL_OFT_3 { get; set; }
        public long? SAL_OFT_ANT_3 { get; set; }
        public long? TIP_OFT_4 { get; set; }
        public long? TIP_OFT_ANT_4 { get; set; }
        public long? INI_OFT_4 { get; set; }
        public long? INI_OFT_ANT_4 { get; set; }
        public long? FIM_OFT_4 { get; set; }
        public long? FIM_OFT_ANT_4 { get; set; }
        public double? PRC_OFT_4 { get; set; }
        public double? PRC_OFT_ANT_4 { get; set; }
        public long? LIM_OFT_4 { get; set; }
        public long? LIM_OFT_ANT_4 { get; set; }
        public long? SAL_OFT_4 { get; set; }
        public long? SAL_OFT_ANT_4 { get; set; }
        public long? TIP_OFT_5 { get; set; }
        public long? TIP_OFT_ANT_5 { get; set; }
        public long? INI_OFT_5 { get; set; }
        public long? INI_OFT_ANT_5 { get; set; }
        public long? FIM_OFT_5 { get; set; }
        public long? FIM_OFT_ANT_5 { get; set; }
        public double? PRC_OFT_5 { get; set; }
        public double? PRC_OFT_ANT_5 { get; set; }
        public long? LIM_OFT_5 { get; set; }
        public long? LIM_OFT_ANT_5 { get; set; }
        public long? SAL_OFT_5 { get; set; }
        public long? SAL_OFT_ANT_5 { get; set; }
        public long? INI_BONUS { get; set; }
        public long? INI_BONUS_ANT { get; set; }
        public long? FIM_BONUS { get; set; }
        public double? PRES_ENT { get; set; }
        public double? PRC_BONUS { get; set; }
        public double? PRES_REP { get; set; }
        public double? ESTQ_ATUAL { get; set; }
        public double? ESTQ_DP { get; set; }
        public double? ESTQ_LJ { get; set; }
        public double? QDE_PEND { get; set; }
        public double? CUS_INV { get; set; }
        public double? ESTQ_PADRAO { get; set; }
        public long? SAI_MED_CAL { get; set; }
        public string TAMANHO { get; set; }
        public double? SAI_ACM_UN { get; set; }
        public double? SAI_ACM_CUS { get; set; }
        public double? SAI_ACM_VEN { get; set; }
        public double? CUS_ULT_ENT_BRU { get; set; }
        public double? ENT_ACM_UN { get; set; }
        public double? ENT_ACM_CUS { get; set; }
        public long? DAT_ULT_FAT { get; set; }
        public double? ULT_QDE_FAT { get; set; }
        public double? ULT_QDE_ENT { get; set; }
        public double? CUS_ULT_ENT { get; set; }
        public long? DAT_ULT_ENT { get; set; }
        public double? ABC_F { get; set; }
        public double? ABC_S { get; set; }
        public double? ABC_T { get; set; }
        public string PERECIVEL { get; set; }
        public long? PRZ_VALIDADE { get; set; }
        public long? TOT_PEDIDO { get; set; }
        public long? TOT_FALTA { get; set; }
        public long? COD_VAS { get; set; }
        public long? DIG_VAS { get; set; }
        public long? COD_ENG { get; set; }
        public long? DIG_ENG { get; set; }
        public double? VALOR_IPI { get; set; }
        public double? VALOR_IPI_ANT { get; set; }
        public double? VALOR_IPI_MAN { get; set; }
        
        public string QTD_AUT_PDV { get; set; }
        public long? MOEDA_VDA { get; set; }
        public long? BONI_MERC { get; set; }
        public long? BONI_MERC_ANT { get; set; }
        public long? BONI_MERC_MAN { get; set; }
        public string GRADE { get; set; }
        public long? MENS_AUX { get; set; }
        
        public long? CATEGORIA_ANT { get; set; }
        public long? ESTRATEGIA_REP { get; set; }
        public double? DESP_ACES_ISEN { get; set; }
        public double? DESP_ACES_ISEN_ANT { get; set; }
        public double? DESP_ACES_ISEN_MAN { get; set; }
        public double? FRETE_VALOR { get; set; }
        public double? FRETE_VALOR_ANT { get; set; }
        public double? FRETE_VALOR_MAN { get; set; }
        public double? BONIF_PER { get; set; }
        public double? BONIF_PER_ANT { get; set; }
        public double? BONIF_PER_MAN { get; set; }
        public string VASILHAME { get; set; }
        public string PERMITE_DESC { get; set; }
        public string QTD_OBRIG { get; set; }
        public string ENVIA_PDV { get; set; }
        public string ENVIA_BALANCA { get; set; }
        public string PESADO_PDV { get; set; }
        public string JPMA_FLAG1 { get; set; }
        public string JPMA_FLAG2 { get; set; }
        public string JPMA_FLAG3 { get; set; }
        public string LINHA_ANTERIOR { get; set; }
        public long? LINHA_VALIDA { get; set; }
        public long? QUANT_EAN { get; set; }
        public long? TOT_ESTOCADO { get; set; }
        public long? EMPIL_MAX { get; set; }
        public long? TIPO_END { get; set; }
        public string SAZONAL { get; set; }
        public string MARCA_PROP { get; set; }
        public long? FILLER { get; set; }
        */

        public ProductIntegrationStatus status { get; set; }
    }
}
