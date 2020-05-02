using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.AccountIncoming
{
    public class AccountIncomingInvoice : EntityBase
    {
        public override string EntityName => "Título do contas a receber";

        public string Code { get; set; }
        public string Name { get; set; }
        public double AbatimentoManual { get; set; }
        public double AbatimentoPagar { get; set; }
        public double Acrescimos { get; set; }
        public long Agencia { get; set; }       
        public string ArquivoGerado { get; set; }
        public string ArquivoImportado { get; set; }
        public long Banco { get; set; }
        public string CodigoCliente { get; set; }
        public string CodigoClienteOriginal { get; set; }   
        public string CodigoDiario { get; set; }
        public string Moeda { get; set; }
        public long CondicaoPagamento { get; set; }    
        public string ContaControle { get; set; }
        public string ContaRazao { get; set; }
        public double Desconto { get; set; }
        public double DescontoAcordo { get; set; }
        public string AgenciaDigito { get; set; }        
        public string ContaBancariaDigito { get; set; }       
        public DateTime DataVencimento { get; set; }
        public DateTime DataVencimentoOriginal { get; set; }
        public DateTime? DataAgendamento { get; set; } 
        public DateTime DataEmissao { get; set; }
        public DateTime? DataGeracao { get; set; }
        public DateTime? DataLancamento { get; set; }
        public DateTime? DataLiquida { get; set; }
        public DateTime? DataPagamento { get; set; }
        public DateTime? DataRecebimentoPrevisto { get; set; }
        public DateTime DataRecepcao { get; set; }
        public string CodigoEmpresa { get; set; }
        public string CodigoFilial { get; set; }
        public string FormaPagamento { get; set; }
        public long MeioPagamento { get; set; }
        public double Juros { get; set; }
        public long IdLinha { get; set; }
        public long LoteEDI { get; set; }      
        public string ModeloNF { get; set; }     
        public long MotivoBaixa { get; set; }
        public double Multa { get; set; }
        public string NossoNumero { get; set; }
        public long NotaFiscal { get; set; }
        public long NumeroBoleto { get; set; }
        public string NumeroConta { get; set; }
        public int Parcela { get; set; }       
        public string NotaFiscalSerie { get; set; }
        public string SeuNumero { get; set; }
        public string Situacao { get; set; }
        public string Status { get; set; }        
        public long NumeroContaTipo { get; set; }
        public string IdTransacao { get; set; }
        public string UsuarioGerouArquivo { get; set; }
        public string UsuarioImportouConfirmacao { get; set; }
        public string UsuarioImportouLiquidacao { get; set; }       
        public double ValorBruto { get; set; }
        public double ValTranMoeda { get; set; }        
        public double ValorPago { get; set; }
        public double VarCamReal { get; set; }
        public double VarCamNreal { get; set; }
        public double Retencoes { get; set; }
    }
}
