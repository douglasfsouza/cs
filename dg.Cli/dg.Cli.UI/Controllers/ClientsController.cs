using dg.Cli.UI.DAL;
using dg.Cli.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dg.Cli.UI.Controllers
{
    public class ClientsController : Controller
    {
        ClientsDbContext ctx = new ClientsDbContext();
        public IActionResult Add()
        {
            return View();
        }

        

        public IActionResult Update(int id)
        {
            var cli = ctx.Clients.Find(id);

            return View(cli);

        }


        public IActionResult Delete(int id)
        {
            var cli = ctx.Clients.Find(id);
            ctx.Remove(cli);
            ctx.SaveChanges();
            return View(cli);
            //return RedirectToAction("index");
        }


        public IActionResult Save(Client cli)
        {
             
            if (cli.Id == 0)
            {
                ctx.Add(cli);
            }
            else
            {
                ctx.Update(cli);
            }
            ctx.SaveChanges();
            return View();
           // return RedirectToAction("Index");
        }

    }
}
