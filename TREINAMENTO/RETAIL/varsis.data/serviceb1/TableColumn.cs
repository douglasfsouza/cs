using System;
using System.Collections.Generic;
using System.Text;

namespace Varsis.Data.Serviceb1
{
    public class TableColumn
    {
        public string name { get; set; }
        public string description { get; set; }
        public string dataType { get; set; }
        public string dataTypeSub { get; set; }
        public int size { get; set; }
        public bool mandatory { get; set; }
    }
}
