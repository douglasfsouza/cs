using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chamados2.Models
{
    public class IssueClient
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public string Cliente { get; set; }
        public string Assunto { get; set; }
        public string Status { get; set; }
        public DateTime Abertura { get; set; }
        public string Complemento { get; set; }
        public int UserId { get; set; }
    }
}
