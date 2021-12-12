using dgs.Store.Data.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.Store.UI.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly StoreDbContext _ctx;

        public ProdutosController(IConfiguration config)
        {
            _ctx = new StoreDbContext(config);

        }
        public IActionResult Index()
        {
            return View();
        }



    }
}
