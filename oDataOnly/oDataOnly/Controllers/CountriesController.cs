using oDataOnly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using static oDataOnly.infrastructure.attribute.ODataEntitySetAttribute;

namespace oDataOnly.Controllers
{
    [ODataEntitySet("Countries", EntityType = typeof(Country))]
    //[Route("odata/[controller]")]
    //[ApiController]
    public class CountriesController : ODataController
    {
        [HttpGet]
        [EnableQuery]      
        [Queryable]
        
        //public IEnumerable<Country> Get()
        //{
        //    List<Country> countries = new List<Country>()
        //    { new Country
        //        {
        //            Code = "Brazil",
        //            Name = "Brazil",
        //            Currency = "Real",
        //            Capital = "Brasilia",
        //            River = "Amazonas"

        //        },
        //        new Country
        //        {
        //            Code = "Argentina",
        //            Name = "Argentina",
        //            Currency = "Peso",
        //            Capital = "Buenos Aires",
        //            River = "Parana"

        //        }
        //    };

        //    return countries.AsEnumerable();
        //}

        async public Task<PageResult<Country>> Get(ODataQueryOptions<Country> queryOptions, [FromODataUriAttribute] string key = null)
        {
            List<Country> countries = new List<Country>()
                { new Country
                    {
                        Code = "Brazil",
                        Name = "Brazil",
                        Currency = "Real",
                        Capital = "Brasilia",
                        River = "Amazonas"

                    },
                    new Country
                    {
                        Code = "Argentina",
                        Name = "Argentina",
                        Currency = "Peso",
                        Capital = "Buenos Aires",
                        River = "Parana"

                    }
                };
            //var lista = countries.AsEnumerable();

            var enumerable = countries as IEnumerable<Country>;

            var result = new PageResult<Country>(enumerable, null, null);

            return result;

        }


        [HttpGet]
        [ActionName("$count")]
        public IHttpActionResult GetCount([FromUri(Name = "$filter")] string filter = "")
        {
            HttpResponseMessage response = new HttpResponseMessage();

            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent("2", System.Text.Encoding.UTF8, "text/plain");
            return ResponseMessage(response);
        }

    }
}
