using GTIAspMVC.Models;
using GTIAspMVC.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace GTIAspMVC.Controllers
{
    public class ClientesController : Controller
    {
        private readonly GTIClienteService _service;

        public ClientesController(GTIClienteService service)
        {
            _service = service;
        }
        
        public async Task<ViewResult> Index()
        {
            var clientes = await _service.GetClients();
            return View(clientes);
        }

        public async Task<ViewResult> AdicionarEditar(int Id)
        {             
            GTIClienteVM clienteVM = new GTIClienteVM
            {
                UFs = GetUFList(),
                SexoList = GetSexoList(),
                EstadoCivilList = GetEstadoCivilList(),
                DataExpedicao = DateTime.Today,
                DataNascimento = DateTime.Today
            };

            if (Id != 0)
            {
                GTICliente cliente = await _service.GetClientById(Id);
                clienteVM = cliente.ToGTIClienteVM();
                clienteVM.UFs = GetUFList();
                clienteVM.SexoList = GetSexoList();
                clienteVM.EstadoCivilList = GetEstadoCivilList();
            }    

            return View(clienteVM);
        }

        public async Task<IActionResult> Salvar(GTICliente cliente)
        {
            var result = new RestResponse();           

            if (!ModelState.IsValid) // c# 7 (.net 5)
            {
                return BadRequest(ModelState);
            }

            if (cliente.Id == 0)
            {
                result = await _service.Adicionar(cliente);
            }
            else
            {
                result = await _service.Atualizar(cliente);
            }

            if (result.IsSuccessful)
            {
                return RedirectToAction("index");
            }
            else
            {
                ModelState.AddModelError("CPF",JsonConvert.DeserializeObject(result.Content).ToString());
                return BadRequest(ModelState);
            }            
        }

        public async Task<ViewResult> ConfirmarExclusao(int Id)
        {
            var cliente = await _service.GetClientById(Id);
            if (cliente == null)
            {
                return null;
            }
            return View(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(GTICliente cliente)
        {            
            if (cliente != null)
            {
                await _service.Excluir(cliente.Id);                
            }           

            return RedirectToAction("index");
        }

        private List<SelectListItem> GetUFList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var item in getUF())
            {
                SelectListItem uf = new SelectListItem();
                uf.Text = item;
                list.Add(uf);
            }

            return list;
        }

        private List<SelectListItem> GetSexoList()
        {
            List<SelectListItem> list = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Masculino" },
                new SelectListItem() { Text = "Feminino" }
            };           

            return list;
        }

        private List<SelectListItem> GetEstadoCivilList()
        {
            List<SelectListItem> list = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Casado" },
                new SelectListItem() { Text = "Solteiro" }
            };

            return list;
        }

        private List<string> getUF()
        {
            return new List<string>() 
            {             
                "AC", "AL", "AP", "AM", "BA",
                "CE", "DF", "ES", "GO", "MA",
                "MT", "MS", "MG", "PA", "PB",
                "PR", "PE", "PI", "RJ", "RN",
                "RS", "RO", "RR", "SC", "SP",
                "SE", "TO"
            };
        }
       
    }

   
}
