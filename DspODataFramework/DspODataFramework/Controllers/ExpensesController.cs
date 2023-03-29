using DspODataFramework.infra.attributes;
using DspODataFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using System.Xml.Linq;
using System.CodeDom;
using DspODataFramework.Service;

namespace DspODataFramework.Controllers
{
    [ODataEntitySet("Expenses", EntityType = typeof(Expense))]
    public class ExpensesController : ODataController
    {
        readonly ExpenseService _service;
        //public ExpensesController(ExpenseService service) : base(service)
        //{
        //    _service = service;
        //}

        [EnableQuery]
               
        public PageResult<Expense> Get(ODataQueryOptions<Expense> queryOptions, [FromODataUri] string key = null)
        {
            //if (key == null)
            //{
            var dsp = new Expense();
            dsp.Id = Guid.NewGuid().ToString();
            dsp.Value = 120;
            dsp.Date= DateTime.Now;
            dsp.Year = 2023;
            dsp.Month = 3;
            dsp.Type = "C";
            dsp.TypeDescription = "Crédito";
            dsp.Description = "Pizza";

            List<Expense> lista = new List<Expense> { dsp};
                                

            var enumerable = lista as IEnumerable<Expense>;
            long count = lista.Count();

            var result = new PageResult<Expense>(enumerable, null, count > 0 ? (long?)count : null);

            return result;
            //}
            //else
            //{
            //    var statusEnum = Expense.Values().ToArray();
            //    var lista = statusEnum.Where(x => x.Key == key).ToList();
            //    var enumerable = lista as IEnumerable<StatusEnum>;
            //    long count = lista.Count();

            //    var result = new PageResult<StatusEnum>(enumerable, null, count > 0 ? (long?)count : null);

            //    return result;
            //}

        }

        [HttpGet]
        [ActionName("$count")]
        public IHttpActionResult GetCount([FromUri(Name = "$filter")] string filter = "")
        {
            HttpResponseMessage response = new HttpResponseMessage();

            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent("1", System.Text.Encoding.UTF8, "text/plain");
            return ResponseMessage(response);
        }
    }
}
