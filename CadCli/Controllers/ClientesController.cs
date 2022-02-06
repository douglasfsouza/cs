using CadCli.Core.Contracts;
 
using CadCli.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CadCli.Controllers
{
    public class ClientesController : Controller
    {
        IRepository _repository;
        public ClientesController(IRepository repository)
        {
            _repository = repository;
        }         
        

        public ViewResult Index()
        {
            var clientes = _repository.Get();
            return View(clientes);
        }  
        
        public IActionResult Adicionar()
        {
            return View();
        }
        
        public IActionResult Editar(int id)
        {
            Cliente cli = _repository.Get(id);
 
            return View(cli);
        }

        public IActionResult Gravar(Cliente cli)
        {
            if (cli.Id != 0)            
            {
                _repository.Update(cli);
            }
            else
            {
                _repository.Add(cli);
            }

                     

            return RedirectToAction("Index");
        }

        public IActionResult ConfExcluir(int id)
        {
            Cliente cli = _repository.Get(id) ;
            _repository.Delete(cli);

            return View(cli);
        }

        public IActionResult Excluir(int id)
        {
            Cliente cli = _repository.Get( id);
            _repository.Delete(cli);             
 
            return RedirectToAction("Index");
        }
    }
}
