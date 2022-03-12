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
    public class CategoriasController : Controller
    {
         private readonly ICategoriaRepository _categoriaRepository;
        private readonly IUnityOfWork _uow;

        public CategoriasController(ICategoriaRepository categoriaRepository,
                                    IUnityOfWork uow)
        {
             _categoriaRepository = categoriaRepository;
            _uow = uow;
        }
        public async Task<IActionResult> Index()
        {
            var cats = (await _categoriaRepository.GetAsync()).Select(x => x.ToCategoriaIndexVM());

            return View(cats);
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int Id)
        {              
            var model = new CategoriaAddEditVM();
            
            if(Id != 0)
            {
                var data = await _categoriaRepository.GetAsync(Id);
                model = data.ToCategoriaAddEditVM();       
            }           

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(int Id, CategoriaAddEditVM model)
        {
            if (!ModelState.IsValid)
            {
                 
                return View(model);
            }
            var cat = model.ToData();

            if (Id == 0)
                _categoriaRepository.Add(cat);                
            else
            {
                cat.Id = Id;
                cat.DataAlteracao = DateTime.Now;
                _categoriaRepository.Update(cat);                
            }

            await _uow.CommitAsync();

            return RedirectToAction("Index");
        }

        public IActionResult Salvar(CategoriaAddEditVM model)
        {
            if (!ModelState.IsValid)
            {
                 
                return View("Adicionar", model);
            }
            var prod = new Categoria()
            {
                Id = model.Id,
                Nome = model.Nome
            };
            if (model.Id == 0)
                _categoriaRepository.Add(prod);
            else
            {
                _categoriaRepository.Update(prod);
            }
             
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int Id)
        {
            var prod = await _categoriaRepository.GetAsync(Id);
            var vm = new CategoriaAddEditVM()
            {
                Id = Id,
                Nome = prod.Nome
             };
             return View(vm);
        }

        public IActionResult ConfirmarDel(int Id)
        {
            var prod = _categoriaRepository.Get(Id);
            var pro = new CategoriaIndexVM()
            {
                Id = Id,
                Nome = prod.Nome
            };
             
            return View(pro);
        }

        public async Task<IActionResult> Excluir(int Id)
        {
            var cat = await _categoriaRepository.GetAsync(Id);

            if (cat == null)
            {
                return BadRequest();
            }

            _categoriaRepository.Delete(cat);
            await _uow.CommitAsync();

            return NoContent();
        }
         
    }
}
