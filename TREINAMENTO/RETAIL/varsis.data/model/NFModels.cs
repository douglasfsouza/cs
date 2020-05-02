using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
    public class NFModels : EntityBase
    {
        public override string EntityName => "";
        public string AbsEntry { get; set; }
        public string NFMCode { get; set; }
        public string NFMDescription { get; set; }
        public string NFMName { get; set; }

    }
}
