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

namespace DspODataFramework.Controllers
{
    [ODataEntitySet("TypeList", EntityType = typeof(TypeEnum))]
    public class TypeListController : ODataController
    {
        [EnableQuery]
        public IEnumerable<TypeEnum> Get()
        {
            return TypeEnum.Values().AsEnumerable();
        }

        [HttpGet]
        [ActionName("$count")]
        public IHttpActionResult GetCount([FromUri(Name = "$filter")] string filter = "")
        {
            HttpResponseMessage response = new HttpResponseMessage();

            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent(TypeEnum.Values().Count.ToString(), System.Text.Encoding.UTF8, "text/plain");
            return ResponseMessage(response);
        }
    }
}
