using DspODataFramework.infra.attributes;
using DspODataFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;

namespace DspODataFramework.Controllers
{
     
    public class ValuesController : ODataController
    {
        // GET api/values
        [EnableQuery]

        public IEnumerable<string> Get()
        {
            List<string> values = new List<string>();
            values.Add("Douglas");
            return values.AsEnumerable();
            //return new string[] { "value1", "value2" };
        }



        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
