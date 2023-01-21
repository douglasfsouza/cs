using dgOData.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static dgOData.infrastructure.attributes.ODataEntitySetAttribute;

namespace dgOData.Controllers
{
    [ODataEntitySet("Countries", EntityType = typeof(Country))]
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        [HttpGet]
        [EnableQuery]

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
