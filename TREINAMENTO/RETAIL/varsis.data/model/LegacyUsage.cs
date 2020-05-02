using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
    public class LegacyUsage : EntityBase
    {
        public override string EntityName => "Dados Agenda";
        public string Code { get; set; }
        public string Name { get; set; }

    }
}
