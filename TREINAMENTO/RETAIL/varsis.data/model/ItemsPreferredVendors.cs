using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
  public  class Items_PreferredVendors : EntityBase
    {
        public override string EntityName => "Cadastro de Fornecedores";

        public string BPCode { get; set; } 


    }
}
