using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
    public class Counties : EntityBase
    {
        public override string EntityName => "Cadastro de municípios";

        public long AbsId { get; set; }
        public string Code { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Name { get; set; }
        public string TaxZone { get; set; }
        public string IBGECode { get; set; }
        public string GIACode { get; set; }
    }
}
