using dgs.store.Data.EF;
using dgs.store.Data.EF.Repositories;
using dgs.Store.Domain.Contracts.Data;
using dgs.Store.Domain.Entities;
using dgs.Store2.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.Store2.UI.Controllers
{
    [Authorize]
    public class PerfisController : Controller
    {
         private readonly IPerfilRepository _perfilRepository;
        private readonly IUnityOfWork _uow;

        public PerfisController(IPerfilRepository perfilRepository,IUnityOfWork uow )
        {
             _perfilRepository = perfilRepository;
            _uow = uow;
        }
        public async Task<IActionResult> Index()
        {            

            var cats = (await _perfilRepository.GetAsync()).Select(x => x.ToPerfilIndexVM());


            return View(cats);
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int Id)
        {
              
            var model = new PerfilAddEditVM();
            
            if(Id != 0)
            {
                var data = await _perfilRepository.GetAsync(Id);
                model = data.ToPerfilAddEditVM();       
            }
                      

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(int Id, PerfilAddEditVM model)
        {
            if (!ModelState.IsValid)
            {
                 
                return View(model);
            }
            var cat = model.ToData();

            if (Id == 0)
                _perfilRepository.Add(cat);                
            else
            {
                cat.Id = Id;
                cat.DataAlteracao = DateTime.Now;
                _perfilRepository.Update(cat);                
            }

            await _uow.CommitAsync();


            return RedirectToAction("Index");
        }

        public IActionResult Salvar(PerfilAddEditVM model)
        {
            if (!ModelState.IsValid)
            {
                 
                return View("Adicionar", model);
            }
            var prod = new Perfil()
            {
                Id = model.Id,
                Nome = model.Nome
            };
            if (model.Id == 0)
                _perfilRepository.Add(prod);
            else
            {
                _perfilRepository.Update(prod);
            }
             
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int Id)
        {
            var prod = await _perfilRepository.GetAsync(Id);
            var vm = new PerfilAddEditVM()
            {
                Id = Id,
                Nome = prod.Nome
             };
             return View(vm);
        }

        public async Task<IActionResult> ConfirmarDel(int Id)
        {
            var prod = await _perfilRepository.GetAsync(Id);
            var pro = new PerfilIndexVM()
            {
                Id = Id,
                Nome = prod.Nome
            };
             
            return View(pro);
        }

        public async Task<IActionResult> Excluir(int Id)
        {
            var cat = await _perfilRepository.GetAsync(Id);

            if (cat == null)
            {
                return BadRequest();
            }

            _perfilRepository.Delete(cat);

            await _uow.CommitAsync();

            return NoContent();
        }
         
    }
}
