using System;
using System.Collections.Generic;
using System.Text;

namespace Varsis.Data.Serviceb1
{
    public class TableIndexes
    {
        public string name { get; set; }
        public bool isUnique { get; set; }
        public string[] keys { get; set; }
    }
}
