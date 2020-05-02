using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model.Integration
{
  public  class ProductComponent : EntityBase
    {
        public override string EntityName => "Dados complementares cadastro produto";
        
        public long? produto { get; set; }
        public long? componente { get; set; }
        public long? componente_ak { get; set; }
        public long? produto_ak { get; set; }
        public double? quantidade { get; set; }
        public long?  sec_sim_1 { get; set; }
        public long? sec_sim_2 { get; set; }
        public long? sec_sim_3 { get; set; }
        public long? grp_sim_1 { get; set; }
        public long? grp_sim_2 { get; set; }
        public long? grp_sim_3 { get; set; }
        public long? sbg_sim_1 { get; set; }
        public long? sbg_sim_2 { get; set; }
        public long? sbg_sim_3 { get; set; }
        public long?  ctg_sim_1 { get; set; }
        public long? ctg_sim_2 { get; set; }
        public long? ctg_sim_3 { get; set; }
        public string filler { get; set; }

        public int status { get; set; }

        public DateTime lastupdate { get; set; }
    }
}
