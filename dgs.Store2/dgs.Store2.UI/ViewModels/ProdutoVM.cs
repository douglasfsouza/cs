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

    public class ProdutoAddCategorias
    {
        public int Id { get; set; }
        public string Nome { get; set; }

    }
}
