using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Integration
{
   public class ProductTrees_Lines : EntityBase
    {
        public override string EntityName => "Cadastro de ProductTrees_Lines";

        public  string ItemCode { get; set; }
        public double Quantity { get; set; }

        public string Warehouse { get; set; }

        public long ChildNum { get; set; }

        public long VisualOrder { get; set; }
    }
}
