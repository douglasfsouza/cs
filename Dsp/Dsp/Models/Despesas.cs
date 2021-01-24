using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dsp.Models
{
    public class Despesas
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public double Valor { get; set; }
        public string Tipo { get; set; }
        public string Descricao { get; set; }
    }
}
