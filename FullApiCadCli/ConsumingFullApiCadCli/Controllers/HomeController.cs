using ConsumingFullApiCadCli.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsumingFullApiCadCli.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;

        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string url = _config["urlApi"];
            var req = new HttpClient();
            var resp = await req.GetAsync(url);
            Cliente cli = null;
            if (resp.IsSuccessStatusCode)
            {
                var responseBody = await resp.Content.ReadAsStringAsync();
                cli = JsonConvert.DeserializeObject<Cliente>(responseBody);
            }
            
            return View(cli);
        }
    }
}
