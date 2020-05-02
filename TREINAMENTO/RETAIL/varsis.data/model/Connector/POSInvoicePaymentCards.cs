using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Connector
{
  public   class POSInvoicePaymentCards 
    {
        public long CreditCard { get; set; }
        public string CreditCardNumber { get; set; }
        public DateTime CardValidUntil { get; set; }
        public string VoucherNum { get; set; }

        public long PaymentMethodCode { get; set; }
        public long NumOfPayments { get; set; }
        public DateTime FirstPaymentDue { get; set; }
        public double FirstPaymentSum { get; set; }
        public double CreditSum { get; set; }
        public long NumOfCreditPayments { get; set; }
               
    }
}
