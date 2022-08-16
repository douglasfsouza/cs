using dgsServiceLayer.Models;
using dgsServiceLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using Microsoft.Extensions.Logging;
using System.Dynamic;
using System.Net.Http.Headers;
using System.Web;
using RestSharp;

namespace dgsServiceLayer.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {       

        [HttpGet]
        public async Task<IActionResult> Get(string query, string token)
        {
            LoginService _service = new LoginService();
           // query = "https://10.101.222.27:50000/b1s/v1/BusinessPartners('F001292')?$select=CardCode,CardName";
            string result = await _service.getQueryResultDg(query, token);

            return Ok(result);
        }

        [HttpPost]
        async public Task<IActionResult> Post([FromBody] Credentials credentials)
        {
            IActionResult result = BadRequest();
            
            try
            {
                LoginService _service = new LoginService();

                string token = await _service.Login(credentials);

                Dictionary<string, object> response = new Dictionary<string, object>()
                {
                    { "success", true },
                    { "data", token },
                    { "message", null }
                };

                result = Ok(token);
            }
            catch (Exception ex)
            {
                Dictionary<string, object> response = new Dictionary<string, object>()
                {
                    { "success", false },
                    { "data", null },
                    { "message", ex.Message }
                };

                var json = JsonConvert.SerializeObject(response);

                var responseMessage = new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReasonPhrase = ex.Message,
                    Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
                };
                result = BadRequest(responseMessage);
            }

            return result;
        }

        

    }
}
