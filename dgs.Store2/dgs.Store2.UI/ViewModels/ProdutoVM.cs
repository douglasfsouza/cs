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
    public class ProdutoIndexVM
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public Decimal Preco { get; set; }
        public string Categoria { get; set; }
        public DateTime DataCadastro { get; set; }
    }

    public class ProdutoAddVM
    {
        public string Nome { get; set; }
        public Decimal Preco { get; set; }
        public int CategoriaId { get; set; }
        public IEnumerable<ProdutoAddCategorias> Categorias { get; set; }
    }

    public class ProdutoEditVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        public Decimal? Preco { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        public int? CategoriaId { get; set; }
        //public IEnumerable<ProdutoAddCategorias> Categorias { get; set; }
        public IEnumerable<SelectListItem> Categorias{ get; set; }
    }

    public class ProdutoAddEditVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        public Decimal? Preco { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        public int? CategoriaId { get; set; }
        public IEnumerable<SelectListItem> Categorias { get; set; }
    }


    public class ProdutoAddCategorias
    {
        public int Id { get; set; }
        public string Nome { get; set; }

    }

    public static class ProdutoVMExtensions
    {
        public static ProdutoIndexVM ToProdutoIndexVM(this Produto data)
        {
            return new ProdutoIndexVM()
            {
                Id = data.Id,
                Nome = data.Nome,
                Preco = data.Preco,
                Categoria = data.Categoria?.Nome,
                DataCadastro = data.DataCadastro
            };
        }
        public static ProdutoAddEditVM ToProdutoAddEditVM(this Produto data)
        {
            return new ProdutoAddEditVM()
            {
                Id = data.Id,
                Nome = data.Nome,
                Preco = data.Preco,
                CategoriaId = data.CategoriaId
            };
        }

        public static Produto ToData(this ProdutoAddEditVM model)
        {
            return new Produto()
            {
                Id = model.Id,
                Nome = model.Nome,
                Preco = (decimal)model.Preco,
                CategoriaId = (int)model.CategoriaId
            };
        }
    }
}
