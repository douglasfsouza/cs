using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.Store.Domain.Entities
{
    public class Categoria : Entity
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public IEnumerable<Produto> Produtos { get; set; }
    }
}
