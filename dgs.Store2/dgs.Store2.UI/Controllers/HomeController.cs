﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dgs.Store2.UI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {             
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Produtos()
        {
            return View();
        }
    }
}
