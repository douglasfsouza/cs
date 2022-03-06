using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.Store2.UI.ViewModels
{
    public class AuthVM
    {
        [Required(ErrorMessage = "Campo obrigatorio")]
        [EmailAddress(ErrorMessage = "email invalido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Campo obrigatorio")]

        [DataType(DataType.Password)]
        public string Senha { get; set; }
    }
}
