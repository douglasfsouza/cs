using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.Store.Domain.Entities
{
    //para criar no banco com este nome
    [Table("Produto")]
    public class Produto : Entity
    {
        public int Id { get; set; }
        //para criar no banco sem aceitar nulo
        [Required]
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
       

    }
}
