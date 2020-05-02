using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;
using static Varsis.Data.Model.Integration.Invoice;

namespace Varsis.Data.Model.Integration
{
    public class InvoiceItem : EntityBase
    {
        //public override string EntityName => "Item da nota fiscal - Integração";

        //public string itemId { get; set; }
        //public double quantity { get; set; }
        //public double unitPrice { get; set; }
        //public string taxCodeDetermination { get; set; }
        //public string cstPIS { get; set; }
        //public string cstCOFINS { get; set; }

        public enum InvoiceItemIntegrationStatus
        {
            Importing = 0,
            Created = 1,
            Processed = 2,
            Error = 99
        }
        public int IDNOTA { get; set; }
        public long SEQUENCIAL { get; set; }
        public long ITEM { get; set; }
        public string EAN { get; set; }
        public int AGENDA { get; set; }
        public int CRF { get; set; }
        public int CFOP { get; set; }
        public int FIGURA { get; set; }
        public string CODSECAO { get; set; }
        public int TIPOEMBALAGEM { get; set; }
        public long BASEEMBALAGEM { get; set; }
        public long QUANTIUNIT { get; set; }
        public long VALORUNIT { get; set; }
        public long CUSTOUNIT { get; set; }
        public long VALORCONTABIL { get; set; }
        public string VALORMERCADORIA { get; set; }
        public int TRIBUTACAOICMS { get; set; }
        public int OPERACAOICMS { get; set; }
        public long CSTICMS { get; set; }
        public long BASEICMS { get; set; }
        public long ALIQUOTAICMS { get; set; }
        public long VALORICMS { get; set; }
        public long REDUCAOICMS { get; set; }
        public long ISENTOICMS { get; set; }
        public long NAOTRIBICMS { get; set; }
        public int OUTROSICMS { get; set; }
        public long FRONTEIRA { get; set; }
        public long ALIQSUBSTTRIB { get; set; }
        public long VALORPVV { get; set; }
        public long ALIQUOTAICMF { get; set; }
        public long VALORICMF { get; set; }
        public int VALORPAUTA { get; set; }
        public long CSTIPI { get; set; }
        public long BASEIPI { get; set; }
        public long ALIQUOTAIPI { get; set; }
        public long VALORIPI { get; set; }
        public int VALORFECOP { get; set; }
        public int CSTPIS { get; set; }
        public int SITUACAOPIS { get; set; }
        public long CATEGORIAPIS { get; set; }
        public long VALORCONTABILPIS { get; set; }
        public long BASEPIS { get; set; }
        public long ALIQUOTAPIS { get; set; }
        public int VALORPIS { get; set; }
        public int CSTCOFINS { get; set; }
        public int SITUACAOCOFINS { get; set; }
        public long CATEGORIACOFINS { get; set; }
        public long VALORCONTABILCOFINS { get; set; }
        public long BASECOFINS { get; set; }
        public long ALIQUOTACOFINS { get; set; }
        public int VALORCOFINS { get; set; }
        public int TIPOCONTRIB { get; set; }
        public long NATRECEITA { get; set; }
        public long BASEIRRF { get; set; }
        public long VALORIRRF { get; set; }
        public long ALIQUOTAIRRF { get; set; }
        public long BASEINSS { get; set; }
        public long VALORINSS { get; set; }
        public long ALIQUOTAINSS { get; set; }
        public long BASEISS { get; set; }
        public long VALORISS { get; set; }
        public long ALIQUOTAISS { get; set; }
        public long BASECSLL { get; set; }
        public long VALORCSLL { get; set; }
        public long ALIQUOTACSLL { get; set; }
        public long BASEPISRET { get; set; }
        public long VALORPIRET { get; set; }
        public long ALIQUOTARET { get; set; }
        public long BASECOFINSRET { get; set; }
        public long VALORCORET { get; set; }
        public long ALIQUOTACOFRET { get; set; }
        public long VALORFRETE { get; set; }
        public long VALORSEGURO { get; set; }
        public long VALORDODESCONTO { get; set; }
        public long VALORACRESCIMO { get; set; }
        public string VALORDESPACESS { get; set; }
        public int SITUACAO { get; set; }
        public int OPERACAO { get; set; }
        public long BASEICMSRETANT { get; set; }
        public long VALORICMSRETANT { get; set; }
        public long ALIQUOTAICMSRETANT { get; set; }
        public long BASEICMSDESO { get; set; }
        public long VALORICMSDESO { get; set; }
        public long ALIQUOTAICMSDESO { get; set; }
        public long BASEICMSRESS { get; set; }
        public long VALORICMSRESS { get; set; }
        public long ALIQUOTAICMSRESS { get; set; }
        public long BASEICMSEFETIVO { get; set; }
        public long VALORICMSEFETIVO { get; set; }
        public long ALIQUOTAICMSEFETIVO { get; set; }
        public long REDUCAOBASEICMSEFETIVO { get; set; }
        public long BASEDIFALIQUOTAORG { get; set; }
        public long VALORDIFALIQUOTAORG { get; set; }
        public long ALIQDIFALIQUOTAORG { get; set; }
        public long BASEDIFALIQUOTADST { get; set; }
        public long VALORDIFALIQUOTADST { get; set; }
        public long ALIQDIFALIQUOTADST { get; set; }
        public long BASEFECOP { get; set; }
        public long ALIQUOTAFECOP { get; set; }
        public long BASEFECOPST { get; set; }
        public long VALORFECOPST { get; set; }
        public long ALIQUOTAFECOPST { get; set; }
        public long BASEFECOPRET { get; set; }
        public long VALORFECOPRET { get; set; }
        public long ALIQUOTAFECOPRET { get; set; }
        public int NUMERODLOTE { get; set; }
        public int DTVALLOTE { get; set; }
        public string DTFABLOTE { get; set; }
        public string CLASSIFLOTE { get; set; }
        public string CODIGOAGGLOTE { get; set; }
        public string INFOADICIONAL {get;set;}
        public DateTime LASTUPDATE { get; set; }
        public InvoiceItemIntegrationStatus status { get; set; }
    }
}
