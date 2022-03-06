using dgs.Store.Domain.Entities;
using dgs.Store.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.Store2.UI.ViewModels
{
    public class MeuPerfilVM
    {
        public int Id { get; set; }
        public string Nome { get; set; }      
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha obrigatoria")]
        public string SenhaAtual { get; set; }


        [Required(ErrorMessage = "Senha obrigatoria")]
        public string NovaSenha { get; set; } 

        [Compare("NovaSenha", ErrorMessage = "As senhas não conferem")]
        [DataType(DataType.Password)]
        public string ConfSenha { get; set; }
    }
    public class UsuarioIndexVM
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Genero { get; set; }
        public string NomesPerfis { get; set; }
    }

    public class UsuarioAddEditVM
    {
        public UsuarioAddEditVM()
        {
            Generos = UsuarioVMExtensions.GetGeneros();
        }
       // public int Id { get; set; }


        [Required(ErrorMessage = "Campo obrigatorio")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo obrigatorio")]
        [EmailAddress(ErrorMessage = "Email invalido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Senha obrigatoria")]
        public string Senha { get; set; }

        [Compare("Senha",ErrorMessage = "As senhas não conferem")]
        [DataType(DataType.Password)]
        public string ConfSenha { get; set; }

        [Required(ErrorMessage =  "Campo obrigatorio")]

        public int? Genero { get; set; }
        public IEnumerable<SelectListItem> Generos { get; set; }

        public IEnumerable<int> PerfisId { get; set; }
        public IEnumerable<SelectListItem> Perfis { get; set; }
    }

    public static class UsuarioVMExtensions
    {
        public static MeuPerfilVM ToMeuPerfilVM(this Usuario data)
        {
            return new MeuPerfilVM
            {
                Id = data.Id,
                Email = data.Email,
                Nome = data.Nome,

            };
        }
        public static UsuarioIndexVM ToUsuarioIndexVM(this Usuario data)
        {
            return new UsuarioIndexVM()
            {
                Id = data.Id,
                Nome = data.Nome,
                Email = data.Email,
                Genero = Enum.GetName(data.Genero),
                NomesPerfis = null
            };
        }
        public static UsuarioAddEditVM ToUsuarioAddEditVM(this Usuario data)
        {
            var senha = "temp";
            return new UsuarioAddEditVM()
            {
                //Id = data.Id,
                Nome = data.Nome,
                Email = data.Email,
                Senha = senha,
                ConfSenha = senha,
                Genero = (int)data.Genero,
                Generos = GetGeneros() 
            };
        }

        public static IEnumerable<SelectListItem> GetGeneros()
        {
            return Enum.GetValues(typeof(Genero)).Cast<Genero>().ToList().Select(x => new SelectListItem { Text = Enum.GetName(x), Value = ((int)x).ToString() });
        }

        public static Usuario ToData(this UsuarioAddEditVM model)
        {
            return new Usuario()
            {
                Nome = model.Nome,
                Email = model.Email,
                Senha = model.Senha,
                Genero = (Genero) model.Genero
            };
        }
    }
}
