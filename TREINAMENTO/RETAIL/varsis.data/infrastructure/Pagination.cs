using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Infrastructure
{
    public class Pagination 
    {
        public long Linhas { get; set; }
        public long Paginas { get; set; }
        public long qtdPorPagina { get; set; }
    }
}
