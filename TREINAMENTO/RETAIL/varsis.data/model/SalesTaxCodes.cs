using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
    public class SalesTaxCodes : EntityBase
    {
        public override string EntityName => "Código de imposto";
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
