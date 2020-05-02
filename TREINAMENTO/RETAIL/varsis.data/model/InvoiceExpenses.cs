using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
    public class InvoiceDocumentReferences
    {       
        
        public string ReferencedObjectType { get; set; }
        public long? ReferencedDocEntry { get; set; }
        public long? ReferencedDocNumber { get; set; }
        
        
    }
}
