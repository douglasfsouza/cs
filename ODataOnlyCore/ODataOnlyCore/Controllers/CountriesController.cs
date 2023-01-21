using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using ODataOnlyCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ODataOnlyCore.infrastructure.attributes.ODataEntitySetAttribute;

namespace ODataOnlyCore.Controllers
{
    [ODataEntitySet("Countries", EntityType = typeof(Country))]
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController :  Controller
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
