using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chamados2.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string NomeFantasia { get; set; }
        public string CNPJ { get; set; }
        public string Endereco { get; set; }
    }
}
