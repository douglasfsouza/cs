using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    using global::GTIAPI.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    namespace GTIAPI.Models
    {
        public class GTIClienteVM
        {
            public int Id { get; set; }
            public string Nome { get; set; }
            public string CPF { get; set; }
            public string RG { get; set; }
            public DateTime? DataExpedicao { get; set; }
            public string OrgaoExpedicao { get; set; }
            public string UF { get; set; }
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

        public class GTICLienteAddEditVM
        {
            [Required(ErrorMessage = "campo obrigatório")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "campo inválido")]
            public string Nome { get; set; }

            [Required(ErrorMessage = "campo obrigatório")]
            [StringLength(15, MinimumLength = 11, ErrorMessage = "campo inválido")]
            public string CPF { get; set; }
            public string RG { get; set; }

            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
            public DateTime? DataExpedicao { get; set; }
            public string OrgaoExpedicao { get; set; }  
            
            [DisplayName("UF Expedição")]
            [MaxLength(2)]
            public string UF { get; set; }

            [Required(ErrorMessage = "campo obrigatório")]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
            public DateTime DataNascimento { get; set; }

            [Required(ErrorMessage = "campo obrigatório")]
            public string Sexo { get; set; }

            [Required(ErrorMessage = "campo obrigatório")]
            public string EstadoCivil { get; set; }

            [Required(ErrorMessage = "campo obrigatório")]
            [StringLength(10, MinimumLength = 8, ErrorMessage = "campo inválido")]             
            public string EnderecoCEP { get; set; }

            [Required(ErrorMessage = "campo obrigatório")]
            public string EnderecoLogradouro { get; set; }

            [Required(ErrorMessage = "campo obrigatório")]
            public string EnderecoNumero { get; set; }
            public string EnderecoComplemento { get; set; }

            [Required(ErrorMessage = "campo obrigatório")]
            public string EnderecoBairro { get; set; }

            [Required(ErrorMessage = "campo obrigatório")]
            public string EnderecoCidade { get; set; }

            [Required(ErrorMessage = "campo obrigatório")]
            [StringLength(2, MinimumLength = 2, ErrorMessage = "campo inválido")]            
            public string EnderecoUF { get; set; }

        }

        public static class GTIClienteVMExtensions
        {
            public static GTICliente ToData(this GTICLienteAddEditVM cliente)
            {
                return new GTICliente
                {
                    Nome = cliente.Nome,
                    DataNascimento = cliente.DataNascimento,
                    DataExpedicao = cliente.DataExpedicao,
                    OrgaoExpedicao = cliente.OrgaoExpedicao,
                    CPF = cliente.CPF,
                    RG = cliente.RG,
                    Sexo = cliente.Sexo,
                    UF = cliente.UF.ToUpper().Trim(),
                    EstadoCivil = cliente.EstadoCivil,
                    EnderecoUF = cliente.EnderecoUF.ToUpper().Trim(),
                    EnderecoCEP = cliente.EnderecoCEP,
                    EnderecoCidade = cliente.EnderecoCidade,
                    EnderecoBairro = cliente.EnderecoBairro,
                    EnderecoLogradouro = cliente.EnderecoLogradouro,
                    EnderecoNumero = cliente.EnderecoNumero,
                    EnderecoComplemento = cliente.EnderecoComplemento
                };
            }
            public static GTIClienteVM ToGetVM(this GTICliente cliente)
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

}
