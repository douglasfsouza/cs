using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Integration
{
  public  class ItemBarCodes : EntityBase
    {
        public override string EntityName => "Cadastro de Produtos";

        public long AbsEntry { get; set; }

        public string BarCode { get; set; }

        public string FreeText { get; set; }

        public long UoMEntry { get; set; }

        public string ItemNo { get; set; }

    }
}
