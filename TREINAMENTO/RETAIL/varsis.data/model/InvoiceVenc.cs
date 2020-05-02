using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
    public class InvoiceVenc 
    {
        public string DueDate { get; set; }
        public double? Percentage { get; set; }
        //public double? Total { get; set; }
        //public DateTime? LastDunningDate { get; set; }
        //public long? DunningLevel { get; set; }
        //public double? TotalFC { get; set; }
        public long? InstallmentId { get; set; }
        //public string PaymentOrdered { get; set; }

        

    }
}
