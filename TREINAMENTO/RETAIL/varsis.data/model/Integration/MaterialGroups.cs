using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Integration
{
  public  class MaterialGroups : EntityBase
    {
        public override string EntityName => "Lista Grupos";

        public long AbsEntry { get; set; }
        public string MaterialGroupCode { get; set; }
        public string Description { get; set; }

    }
}
