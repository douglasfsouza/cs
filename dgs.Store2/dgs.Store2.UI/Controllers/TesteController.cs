using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dgs.Store2.UI.Controllers
{
    public class TesteController : Controller
    {
        public ContentResult Index1()
        {
            return Content("Teste");
        }
        public ContentResult Index() => Content("Teste");
    }
}
