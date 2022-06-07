using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chamados2.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string AvatarURL { get; set; }
    }
}
