using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.Store.Domain.Entities
{
    public abstract class Entity
    {
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public DateTime DataAlteracao { get; set; }= DateTime.Now;
        public int UsuarioId { get; set; } = 1;
        public Usuario Usuario { get; set; }
    }
}
