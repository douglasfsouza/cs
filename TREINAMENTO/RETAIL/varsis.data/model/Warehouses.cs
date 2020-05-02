using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
    public class Warehouses : EntityBase
    {
        //public override string EntityName => "Cadastro de WareHouses";
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }      
    }
}
