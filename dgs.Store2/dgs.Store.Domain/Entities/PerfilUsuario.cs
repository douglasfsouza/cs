using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.Store.Domain.Entities
{
    public class PerfilUsuario : Entity
    {
        public int Id { get; set; }
        public IEnumerable<int> UsuariosId { get; set; }
        public IEnumerable<int> PerfisId { get; set; }


    }
}
