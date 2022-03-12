using FullApiCadCli.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullApiCadCli.Controllers
{
    public class Index : Controller
    {
        [HttpGet("getCliente")]
        public IActionResult GetCliente()
        {
            Cliente cli = 
             new Cliente
            {
                Id = 1,
                Nome = "Douglas Ferreira"
            };
            return Ok(cli);

            //return BadRequest("Cliente não encontrado");

        }
        
    }
}
