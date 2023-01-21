using Microsoft.AspNet.OData;
using oDataBasic.infrastructure.attributes;
using oDataBasic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace oDataBasic.Controller
{
    [ODataEntitySet("Countries", EntityType = typeof(Country))]
    [Route("api/[controller]")]
    public class CountriesController : ApiController
    {
        [EnableQuery]
        [Route("countries")]
        public IEnumerable<Country> Get()
        {
            List<Country> countries = new List<Country>()
            { new Country
                {
                    Code = "Brazil",
                    Name = "Brazil",
                    Currency = "Real",
                    Capital = "Brasilia",
                    River = "Amazonas"

                }
            };

            return countries.AsEnumerable();             
        }
    }
}