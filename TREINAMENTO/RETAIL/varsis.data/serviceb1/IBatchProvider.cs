using System;
using System.Collections.Generic;
using System.Text;

namespace Varsis.Data.Serviceb1
{
    public interface IBatchProvider
    {
        public List<BatchItem> Items { get; }
    }
}
