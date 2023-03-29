using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData.Query;

namespace DspODataFramework.infra
{
    public class ListWithCountArgs<T>
    {
        public object Key { get; set; }
        public ODataQueryOptions<T> queryOptions { get; set; }
        public Func<string, string> parseQuery { get; set; }
        public Func<System.Dynamic.ExpandoObject, T> parseRowFunction { get; set; }
    }
}
