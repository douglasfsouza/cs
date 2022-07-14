using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTIAspMVC.Models
{
    public class GTICliente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string RG { get; set; }
        public DateTime? DataExpedicao { get; set; }
        public string  OrgaoExpedicao { get; set; }
        public string  UF { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Sexo { get; set; }
        public string EstadoCivil { get; set; }
        public string EnderecoCEP { get; set; }
        public string EnderecoLogradouro { get; set; }
        public string EnderecoNumero { get; set; }
        public string EnderecoComplemento { get; set; }
        public string EnderecoBairro { get; set; }
        public string EnderecoCidade { get; set; }
        public string EnderecoUF { get; set; }
    }

    public class GTIClienteVM : GTICliente
    {
        public IEnumerable<SelectListItem> UFs { get; set; }
        public IEnumerable<SelectListItem> SexoList { get; set; }
        public IEnumerable<SelectListItem> EstadoCivilList { get; set; }
    }

    public static class GTIClienteExtensions
    {
        public static GTIClienteVM ToGTIClienteVM(this GTICliente cliente)
        {
            return new GTIClienteVM
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                DataNascimento = cliente.DataNascimento,
                DataExpedicao = cliente.DataExpedicao,
                OrgaoExpedicao = cliente.OrgaoExpedicao,
                CPF = cliente.CPF,
                RG = cliente.RG,
                Sexo = cliente.Sexo,
                UF = cliente.UF,
                EstadoCivil = cliente.EstadoCivil,
                EnderecoUF = cliente.EnderecoUF,
                EnderecoCEP = cliente.EnderecoCEP,
                EnderecoCidade = cliente.EnderecoCidade,
                EnderecoBairro = cliente.EnderecoBairro,
                EnderecoLogradouro = cliente.EnderecoLogradouro,
                EnderecoNumero = cliente.EnderecoNumero,
                EnderecoComplemento = cliente.EnderecoComplemento
            };
        }
    }
}
