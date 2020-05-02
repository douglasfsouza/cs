using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
    public class Product : EntityBase
    {
        public override string EntityName => "Dados complementares do produto";
        public string id { get; set; }
        public string fullName { get; set; }
        public string shortName { get; set; }
        public double price { get; set; }
        public DateTime priceLastUpdate { get; set; }
        public long quantity { get; set; }
    }
}
