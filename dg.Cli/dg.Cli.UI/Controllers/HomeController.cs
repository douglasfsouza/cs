using dg.Cli.UI.DAL;
using dg.Cli.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dg.Cli.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly bool _logActive;
        ClientsDbContext ctx = new ClientsDbContext();

        public HomeController(IConfiguration config)
        {
            _logActive = Convert.ToBoolean(config["LogActive"]);
        }
        public IActionResult Index()
        {
            return View();
            
        }

        public IActionResult About()
        {
            ViewBag.logActive = string.Format("O log está {0}",_logActive ? "Ativo" : "Desativado");
            return View();
        }
        public IActionResult Clients()
        {           

            return View(ctx.Clients.ToList());
        }
    }
}
