using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model.Integration;

namespace Varsis.Data.Model
{
  public  class ProductTree : EntityBase
    {
        public override string EntityName => "Cadastro dos Componentes";

        public enum ProductTreeIntegrationStatus
        {
            Importing = 0,
            Created = 1,
            Processed = 2,
            Error = 99
        }
        public ProductTree()
        {
            productTrees_Lines = new List<ProductTrees_Lines>();

        }

        public string TreeCode { get; set; }

        public string TreeType { get; set; }

        public string Warehouse { get; set; }

        public List<ProductTrees_Lines> productTrees_Lines { get; set; }
    }
}
