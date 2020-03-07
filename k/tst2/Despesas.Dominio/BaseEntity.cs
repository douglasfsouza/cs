using System;

namespace Despesas.Dominio
{
    public abstract class BaseEntity
    {
        public int Id{ get; set; }
        public string UsuarioCriacao{ get; set; }
        public string UsuarioAlteracao{ get; set; }
        public DateTime DataCriacao{ get; set; }
        public DateTime DataAlteracao { get; set; }

    }
}