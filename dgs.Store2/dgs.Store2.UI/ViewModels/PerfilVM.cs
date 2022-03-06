using dgs.Store.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.Store2.UI.ViewModels
{
    public class PerfilIndexVM
    {
        public int Id { get; set; }
        public string Nome { get; set; }       
    }

    public class PerfilAddVM
    {
        public string Nome { get; set; }
        public int UsuarioId { get; set; }
        public IEnumerable<PerfilAddUsuarios> Usuarios { get; set; }
    }

    public class PerfilEditVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(50, ErrorMessage = "Tamanho excedido")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        

        
        public int? UsuarioId { get; set; }
         public IEnumerable<SelectListItem> Usuarios{ get; set; }
    }

    public class PerfilAddEditVM
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        
        public int? UsuarioId { get; set; }
        public IEnumerable<SelectListItem> Usuarios { get; set; }
    }


    public class PerfilAddUsuarios
    {
        public int Id { get; set; }
        public string Nome { get; set; }

    }

    public static class PerfilVMExtensions
    {
        public static PerfilIndexVM ToPerfilIndexVM(this Perfil data)
        {
            return new PerfilIndexVM()
            {
                Id = data.Id,
                Nome = data.Nome 
            };
        }
        public static PerfilAddEditVM ToPerfilAddEditVM(this Perfil data)
        {
            return new PerfilAddEditVM()
            {
                Id = data.Id,
                Nome = data.Nome                
            };
        }

        public static Perfil ToData(this PerfilAddEditVM model)
        {
            return new Perfil()
            {
                Id = model.Id,
                Nome = model.Nome                 
            };
        }
    }
}
