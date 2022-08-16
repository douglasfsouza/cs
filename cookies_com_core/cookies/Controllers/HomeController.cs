using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace cookies.Controllers
{
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            //Cookie c = new Cookie();
            //c.Expires = DateTime.Now.AddDays(1);
            //c.Name = "core";
            //c.Value = "api";
            //CookieContainer cc = new CookieContainer();
            //cc.Add(c);

            Response.Cookies.Append("core","API");

          
            return RedirectToAction();
        }
    }
}
