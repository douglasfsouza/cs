using dgs.Store.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.Store2.UI.ViewModels
{
    public class CategoriaIndexVM
    {
        public int Id { get; set; }
        public string Nome { get; set; }
       
    }

    public class CategoriaAddVM
    {
        public string Nome { get; set; }
        public Decimal Preco { get; set; }
        public int ProdutoId { get; set; }
        public IEnumerable<ProdutoAddCategorias> Produtos { get; set; }
    }

    public class CategoriaEditVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(50, ErrorMessage = "Tamanho excedido")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        

        
        public int? ProdutoId { get; set; }
        //public IEnumerable<ProdutoAddCategorias> Categorias { get; set; }
        public IEnumerable<SelectListItem> Produtos{ get; set; }
    }

    public class CategoriaAddEditVM
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        
        public int? ProdutoId { get; set; }
        public IEnumerable<SelectListItem> Produtos { get; set; }
    }


    public class CategoriaAddProdutos
    {
        public int Id { get; set; }
        public string Nome { get; set; }

    }

    public static class CategoriaVMExtensions
    {
        public static CategoriaIndexVM ToCategoriaIndexVM(this Categoria data)
        {
            return new CategoriaIndexVM()
            {
                Id = data.Id,
                Nome = data.Nome 
            };
        }
        public static CategoriaAddEditVM ToCategoriaAddEditVM(this Categoria data)
        {
            return new CategoriaAddEditVM()
            {
                Id = data.Id,
                Nome = data.Nome                
                 
            };
        }

        public static Categoria ToData(this CategoriaAddEditVM model)
        {
            return new Categoria()
            {
                Id = model.Id,
                Nome = model.Nome                 
            };
        }
    }
}
