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
    public class ProdutosController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IUnityOfWork _uow;

        public ProdutosController(IProdutoRepository produtoRepository, ICategoriaRepository categoriaRepository, IUnityOfWork uow )
        {
            _produtoRepository = produtoRepository;
            _categoriaRepository = categoriaRepository;
            _uow = uow;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {  
            var prods = (await _produtoRepository.GetAllWithCategoriasAsync()).Select(x => x.ToProdutoIndexVM());

            return View(prods);
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int Id)
        {
             // ViewBag.Categorias = categorias;
            var model = new ProdutoAddEditVM();
            
            if(Id != 0)
            {
                var data = await _produtoRepository.GetAsync(Id);
                model = data.ToProdutoAddEditVM();       
            }

            await addCategoriasToModel(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(int Id, ProdutoAddEditVM model)
        {
            if (!ModelState.IsValid)
            {
                await addCategoriasToModel(model);
                return View(model);
            }
            var prod = model.ToData();

            if (Id == 0)
                _produtoRepository.Add(prod);                
            else
            {
                prod.Id = Id;
                prod.DataAlteracao = DateTime.Now;
                _produtoRepository.Update(prod);                
            }

            await _uow.CommitAsync(); ;


            return RedirectToAction("Index");
        }       

        public async Task<IActionResult> ConfirmarDel(int Id)
        {
            var prod = await _produtoRepository.GetAsync(Id);
            var pro = new ProdutoIndexVM()
            {
                Id = Id,
                Nome = prod.Nome
            };
             
            return View(pro);
        }

        public async Task<IActionResult> Excluir(int Id)
        {
            var pro = await _produtoRepository.GetAsync(Id);

            if (pro == null)
            {
                return BadRequest();
            }

            _produtoRepository.Delete(pro);
            await _uow.CommitAsync();

            return NoContent();
        }

        private async Task addCategoriasToModel(ProdutoAddEditVM model)
        {
            var categorias = await _categoriaRepository.GetAsync();
            model.Categorias = categorias.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Nome });

        }
    }
}
