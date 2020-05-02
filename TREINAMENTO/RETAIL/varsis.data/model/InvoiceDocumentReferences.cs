using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
    public class InvoiceExpenses
    {       
        //public long? LineNum { get; set; }
        public long? ExpenseCode { get; set; }
        public double? LineTotal { get; set; }
        public double? LineTotalSys { get; set; }
        public string TaxCode { get; set; }
        

    }
}
