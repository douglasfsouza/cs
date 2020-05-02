using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.AccountPayable
{
    public class AccountPayableInvoice : EntityBase
    {
        public override string EntityName => "Título do contas a pagar";

        public string Code { get; set; }
        public string Name { get; set; }
        public double AbatimentoManual { get; set; }
        public double AbatimentoReceber { get; set; }
        public double Acrescimos { get; set; }
        public long Agencia { get; set; }
        public long AgenciaFornecedor { get; set; }
        public string ArquivoGerado { get; set; }
        public string ArquivoImportado { get; set; }
        public long Banco { get; set; }
        public long BancoFornecedor { get; set; }
        public long Bloqueado { get; set; }
        public string CodigoFornecedor { get; set; }
        public string CodigoFornecedorOriginal { get; set; }
        public string CodigoBarras { get; set; }
        public string CodigoAntecipacao { get; set; }
        public string CodigoPagamento { get; set; }
        public string CodigoMoeda { get; set; }
        public long CondicaoPagamento { get; set; }
        public string ContaBancariaFornecedor { get; set; }
        public string ContaControle { get; set; }
        public string ContaRazao { get; set; }
        public double Desconto { get; set; }
        public double DescontoAcordo { get; set; }
        public string AgenciaDigito { get; set; }
        public string AgenciaDigitoFornecedor { get; set; }
        public string ContaBancariaDigito { get; set; }
        public string ContaBancariaDigitoFornecedor { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime DataVencimentoOriginal { get; set; }
        public DateTime? DataAgendamento { get; set; }
        public DateTime? DataBloqueio { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime? DataGeracao { get; set; }
        public DateTime? DataLancamento { get; set; }
        public DateTime? DataLiberacao { get; set; }
        public DateTime? DataLiquida { get; set; }
        public DateTime? DataPagamento { get; set; }
        public DateTime? DataPagamentoPrevisto { get; set; }
        public DateTime DataRecepcao { get; set; }
        public string CodigoEmpresa { get; set; }
        public string CodigoFilial { get; set; }
        public string FormaPagamento { get; set; }
        public double Funrural { get; set; }
        public string InstrucaoPagamento { get; set; }
        public double Juros { get; set; }
        public double JurosMoeda { get; set; }
        public long IdLinha { get; set; }
        public long LoteEDI { get; set; }
        public long MeioPagamento { get; set; }
        public string ModeloNF { get; set; }
        public string MoedaPagamento { get; set; }
        public long MotivoBaixa { get; set; }
        public double Multa { get; set; }
        public string NossoNumero { get; set; }
        public long NotaFiscal { get; set; }
        public long NumeroBoleto { get; set; }
        public string NumeroConta { get; set; }
        public int Parcela { get; set; }
        public double Retencoes { get; set; }
        public string NotaFiscalSerie { get; set; }
        public string SeuNumero { get; set; }
        public string Situacao { get; set; }
        public string Status { get; set; }
        public string CodigoBarrasTipo { get; set; }
        public long NumeroContaTipo { get; set; }
        public string IdTransacao { get; set; }
        public string UsuarioGerouArquivo { get; set; }
        public string UsuarioImportouConfirmacao { get; set; }
        public string UsuarioImportouLiquidacao { get; set; }
        public string UsuarioLiberacao { get; set; }
        public double ValorBruto { get; set; }
        public double ValorBrutoMoeda { get; set; }
        public double ValorMoedaCmp { get; set; }
        public double ValorMoedaPagamento { get; set; }
        public double ValorPago { get; set; }
        public double VariacaoCambial { get; set; }
    }
}
