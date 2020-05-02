using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
    public class JournalEntry : EntityBase
    {
        public override string EntityName => "JournalEntry";
        public string TaxDate { get; set; }
        public string ReferenceDate { get; set; }
        public string Memo { get; set; }
        public string DueDate { get; set; }
        public string ECDPostingType { get; set; }
        public List<JournalEntryLines> JournalEntryLines { get; set; }
    }

    public class JournalEntryLines
    {
        public long Line_ID { get; set; }
        //public string AccountCode { get; set; }
        public double? Credit { get; set; }
        public double? Debit { get; set; }
        public string DueDate { get; set; }
        public string ShortName { get; set; }
        public string ControlAccount { get; set; }
        public string TaxDate { get; set; }
        public long? BPLID { get; set; }
        
    }

    

    

}