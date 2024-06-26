﻿using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Model.AccountPayable;

namespace Varsis.Data.Model.AccountPayable
{
    public class AccountPayableInvoiceSummary : AccountPayableInvoice
    {
        public string NomeFornecedor { get; set; }
        public string NomeFornecedorOriginal { get; set; }
        public string NomeContaRazao { get; set; }
        public string NomeContaControle { get; set; }
        public string NomeCondicaoPagamento { get; set; }
        public string NomeEmpresa { get; set; }
        public string NomeFilial { get; set; }
        public string NomeBanco { get; set; }

        public double ValorLiquido
        {
            get
            {
                return ValorBruto -
                       AbatimentoManual -
                       AbatimentoReceber -
                       Desconto +
                       Juros +
                       Acrescimos +
                       Multa -
                       Retencoes;
            }
        }
        public int DiasAtraso
        {
            get
            {
                TimeSpan diferenca;

                if (DataPagamento == null)
                {
                    diferenca = DataVencimento - DateTime.Now.Date;
                }
                else
                {
                    diferenca = DataVencimento - DataPagamento.Value;
                }

                return diferenca.Days;
            }
        }

        public int Liquidado
        {
            get
            {
                return DataLiquida == null ? 0 : 1;
            }
        }
    }
}

