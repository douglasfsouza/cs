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
        public string Usuario { get; set; } = "Douglas";
    }
}
