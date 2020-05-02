using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Varsis.Api.Core.Utilities
{
    public class ApiResponseBase
    {
        public bool success { get; set; }
        public object data { get; set; }
        public string message { get; set; }
        public Varsis.Data.Infrastructure.Pagination paginacao { get; set; }
    }
}
