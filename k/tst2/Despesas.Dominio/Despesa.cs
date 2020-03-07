using System;
using System.Collections.Generic;
using System.Text;

namespace Despesas.Dominio.Despesas
{
    public enum Status
    {
        Pendente = 0,
        Aprovada = 1,
        Paga = 2,
        NaoAprovada = 3,
        Cancelada = 4
    }

        
    public class Despesa : BaseEntity
    {
        public Projeto Projeto { get; set; }
        public Tipo TipoDespesa { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public Consultor Consultor{ get; set; }
        public Status StatusDespesa{ get; set; }
    }
}
