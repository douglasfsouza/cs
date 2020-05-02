using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
 public   class LegacyFiscalOperations : EntityBase
    {
        public override string EntityName => "Dados Agenda";

        public string Code { get; set; }
        public string Name { get; set; }
        public long CRFCode { get; set; }
        public long CFOP { get; set; }

    }
}
