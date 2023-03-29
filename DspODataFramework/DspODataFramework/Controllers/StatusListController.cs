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
using System.Xml.Linq;
using System.Web.Http.Results;
using System.Web.Http.OData.Query;
using System.Collections;

namespace DspODataFramework.Controllers
{
    [ODataEntitySet("StatusList", EntityType = typeof(StatusEnum))]
    public class StatusListController : ODataController
    {
        [EnableQuery]
        //public IEnumerable<StatusEnum> Get()
        //{
        //    return StatusEnum.Values().AsEnumerable();
        //}

        public PageResult<StatusEnum> Get(ODataQueryOptions<StatusEnum> queryOptions, [FromODataUri] string key = null)
        {
            if (key == null)
            {           

                var lista = StatusEnum.Values();

                var enumerable = lista as IEnumerable<StatusEnum>;
                long count = lista.Count();

                var result = new PageResult<StatusEnum>(enumerable, null, count > 0 ? (long?)count : null);

                return result;

            }
            else
            {
                var statusEnum = StatusEnum.Values().ToArray();
                var lista = statusEnum.Where(x => x.Key == key).ToList();
                var enumerable = lista as IEnumerable<StatusEnum>;
                long count = lista.Count();

                var result = new PageResult<StatusEnum>(enumerable, null, count > 0 ? (long?)count : null);

                return result;
            }
             
        }


        [HttpGet]
        [ActionName("$count")]
        public IHttpActionResult GetCount([FromUri(Name = "$filter")] string filter = "")
        {
            HttpResponseMessage response = new HttpResponseMessage();

            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent(StatusEnum.Values().Count.ToString(), System.Text.Encoding.UTF8, "text/plain");
            return ResponseMessage(response);             
        }
    }


    
}
