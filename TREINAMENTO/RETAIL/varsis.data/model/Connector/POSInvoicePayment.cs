using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;
using Newtonsoft.Json;

namespace Varsis.Data.Model.Connector
{
   public class POSInvoicePayment : EntityBase
    {
        [JsonIgnore]
        public override string EntityName => "POSInvoicePayment";

        [JsonIgnore]
        public override Guid RecId { get => base.RecId; set => base.RecId = value; }

        public POSInvoicePayment()
        {
            PaymentInvoices = new List<POSInvoicePaymentLines>() ;
            PaymentCreditCards = new List<POSInvoicePaymentCards>();
        }
        public string DocType { get; set; }
        public DateTime DocDate { get; set; }
        public string CardCode { get; set; }
        public string CashAccount { get; set; }
        public double CashSum { get; set; }
        public double CashSumSys { get; set; }
        public string DocObjectCode { get; set; }
        public string DocTypte { get; set; }
        public DateTime DueDate { get; set; }
        public long BPLID { get; set; }
        public List<POSInvoicePaymentLines> PaymentInvoices { get; set; }
        public List<POSInvoicePaymentCards> PaymentCreditCards { get; set; }

    }
}
