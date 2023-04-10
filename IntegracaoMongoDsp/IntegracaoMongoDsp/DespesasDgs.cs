using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegracaoMongoDsp
{
    public class DespesasDgs
    {
        public int Id { get; set; }

        public DateTime Data { get; set; }

        public double Valor { get; set; }

        public string Tipo { get; set; }

        public string Descricao { get; set; }
    }
}
