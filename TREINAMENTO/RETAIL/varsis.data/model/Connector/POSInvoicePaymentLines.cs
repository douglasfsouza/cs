using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Connector
{
  public  class POSInvoicePaymentLines 
    {
        public long DocEntry { get; set; }
        public long LineNum { get; set; }

        public string InvoiceType { get; set; }
        public long InstallmentId { get; set; }

        public double SumApplied { get; set; }
        public double AppliedSys { get; set; }
    }
}
