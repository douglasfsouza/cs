using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Integration
{
  public  class NcmCodesSetup : EntityBase
    {
        public override string EntityName => "Cadastro de Produtos";

        public long AbsEntry { get; set; }

        public string Description { get; set; }

        public string GroupCode { get; set; }

        public string NCMCode { get; set; }
    }
}
