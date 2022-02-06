using dgs.store.Data.EF;
using dgs.Store.Domain.Entities;
using dgs.Store2.UI.ViewModels;
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
    public class ProdutosController : Controller
    {
        private readonly StoreDbContext _ctx;
        public ProdutosController(IConfiguration config)
        {
            _ctx = new StoreDbContext(config);
        }
        public IActionResult Index()
        {
            var prods = _ctx.Produtos.Include(x => x.Categoria)
            .ToList().Select(x => new ProdutoIndexVM() { 
                Id = x.Id, 
                Nome = x.Nome, 
                Preco = x.Preco, 
                Categoria = x.Categoria?.Nome, 
                DataCadastro = x.DataCadastro });

           
            return View(prods);
        }

        [HttpGet]
        public IActionResult Adicionar()
        {
            var categorias = _ctx.Categorias.ToList();
            // ViewBag.Categorias = categorias;
            var model = new ProdutoEditVM()
            {
                Categorias = categorias.Select(x => new SelectListItem(){ Value = x.Id.ToString(), Text = x.Nome})
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Adicionar(ProdutoEditVM model)
        {
            if (!ModelState.IsValid)
            {
                addCategoriasToModel(model);
                return View("Adicionar",model);
            }
            var prod = new Produto()
            {
                Id = model.Id,
                Nome = model.Nome,
                Preco = (decimal)model.Preco,
                CategoriaId = (int)model.CategoriaId
            };
            if (model.Id == 0)
                _ctx.Produtos.Add(prod);
            else
            {
                var ed = _ctx.Produtos.Find(model.Id);
                _ctx.Produtos.Update(ed);
            }
            _ctx.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Salvar(ProdutoEditVM model)
        {
            if (!ModelState.IsValid)
            {
                addCategoriasToModel(model);
                return View("Adicionar", model);
            }
            var prod = new Produto()
            {
                Id = model.Id,
                Nome = model.Nome,
                Preco = (decimal)model.Preco,
                CategoriaId = (int)model.CategoriaId
            };
            if (model.Id == 0)
                _ctx.Produtos.Add(prod);
            else
            {
                _ctx.Produtos.Update(prod);
            }
            _ctx.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Editar(int Id)
        {
            //var categorias = _ctx.Categorias.ToList();
            var prod = _ctx.Produtos.Find(Id);
            var vm = new ProdutoEditVM()
            {
                Id = Id,
                CategoriaId = prod.CategoriaId,
                // Categorias = categorias.Select(x => new ProdutoAddCategorias() { Nome = x.Nome, Id = x.Id }),
                Nome = prod.Nome,
                Preco = prod.Preco
            };
            addCategoriasToModel(vm);
            return View(vm);
        }

        public IActionResult ConfirmarDel(int Id)
        {
            var prod = _ctx.Produtos.Find(Id);
            var pro = new ProdutoIndexVM()
            {
                Id = Id,
                Nome = prod.Nome
            };
             
            return View(pro);
        }

        public IActionResult Excluir(int Id)
        {
            var pro = _ctx.Produtos.Find(Id);
            _ctx.Produtos.Remove(pro);
            _ctx.SaveChanges();
            return RedirectToAction("Index");
        }

        private void addCategoriasToModel(ProdutoEditVM model)
        {
            var categorias = _ctx.Categorias.ToList();
            model.Categorias = categorias.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Nome });

        }
    }
}
