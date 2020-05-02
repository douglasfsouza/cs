using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
    public class Store : EntityBase
    {
        public long code { get; set; }
        public string name { get; set; }
    }
}
