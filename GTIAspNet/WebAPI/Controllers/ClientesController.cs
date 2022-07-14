using GTIAPI.DAL;
using GTIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.GTIAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly GTIClientContext _ctx;

        public ClientesController(IConfiguration config, IHostEnvironment env)
        {
            _ctx = new GTIClientContext(config, env);
        }
        [HttpGet]
        public IActionResult Get()
        {
            var clientes =  _ctx.clientes.Select(x => x.ToGetVM());

            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var cliente = _ctx.clientes.Find(id);
            if (cliente == null)
            {
                return BadRequest("Cliente não encontrado");
            }
            else
            {
                return Ok(cliente.ToGetVM());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] GTICLienteAddEditVM cliente)
        {
            List<string> errors = new List<string>();
            GTICliente data = null;
            try
            {
                errors = ValidarDados(cliente);

                if (errors.Count > 0)
                {
                    return BadRequest(errors);
                }

                data = cliente.ToData();   

                _ctx.clientes.Add(data);
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao adicionar o cliente. Erro: {ex.Message} ");                
            }
            return Ok(data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int  id, [FromBody] GTICliente cliente)
        {
            List<string> errors = new List<string>();
            var data = _ctx.clientes.Find(id);
            if (data is null)
            {
                ModelState.AddModelError("Id","Cliente não encontrado");
                return BadRequest(ModelState);
            }
            errors = ValidarDados(cliente);
            if (errors.Count > 0)
            {
                return BadRequest(errors);
            }

            data.Nome = cliente.Nome == null ? data.Nome : cliente.Nome;
            data.CPF = cliente.CPF == null ? data.CPF : cliente.CPF;
            data.RG = cliente.RG == null ? data.RG : cliente.RG;
            data.DataExpedicao = cliente.DataExpedicao?.Year == 1 ? data.DataExpedicao : cliente.DataExpedicao;
            data.OrgaoExpedicao = cliente.OrgaoExpedicao == null ? data.OrgaoExpedicao : cliente.OrgaoExpedicao;
            data.UF = cliente.UF == null ? data.UF : cliente.UF;
            data.DataNascimento = cliente.DataNascimento == null || cliente.DataNascimento.Year == 1 ? data.DataNascimento : cliente.DataNascimento;
            data.Sexo = cliente.Sexo == null ? data.Sexo : cliente.Sexo;
            data.EstadoCivil = cliente.EstadoCivil == null ? data.EstadoCivil : cliente.EstadoCivil;
            data.EnderecoCEP = cliente.EnderecoCEP == null ? data.EnderecoCEP : cliente.EnderecoCEP;
            data.EnderecoLogradouro = cliente.EnderecoLogradouro == null ? data.EnderecoLogradouro : cliente.EnderecoLogradouro;
            data.EnderecoNumero = cliente.EnderecoNumero == null ? data.EnderecoNumero : cliente.EnderecoNumero;
            data.EnderecoComplemento = cliente.EnderecoComplemento == null ? data.EnderecoComplemento : cliente.EnderecoComplemento;
            data.EnderecoBairro = cliente.EnderecoBairro == null ? data.EnderecoBairro : cliente.EnderecoBairro;
            data.EnderecoCidade = cliente.EnderecoCidade == null ? data.EnderecoCidade : cliente.EnderecoCidade;
            data.EnderecoUF = cliente.EnderecoUF == null ? data.EnderecoUF : cliente.EnderecoUF;

            _ctx.clientes.Update(data);
            await _ctx.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            var data = _ctx.clientes.Find(id);
            if (data is null)
            {
                ModelState.AddModelError("Id", "Cliente não encontrado");
                return BadRequest(ModelState);
            }

            _ctx.clientes.Remove(data);
            await _ctx.SaveChangesAsync();

            return NoContent();
        }

        private List<string> ValidarDados(dynamic data)
        {
            List<string> errors = new List<string>();

            if (data.CPF != null)
            {
                if (!Helpers.Helpers.IsCpf(data.CPF.Replace(".", null).Replace("-", null)))
                {
                    errors.Add("CPF Inválido");
                }
            }            

            List<string> ufs = getUF();
            if (data.EnderecoUF != null)
            {
                string enderecoUF = data.EnderecoUF.Trim().ToUpper();
                if (!ufs.Contains(enderecoUF))
                    errors.Add("UF inválido");
                data.EnderecoUF = enderecoUF;
            }

            if (data.UF != null)
            {
                string uf = data.UF.Trim().ToUpper();
                if (!string.IsNullOrEmpty(uf))
                {
                    if (!ufs.Contains(uf))
                        errors.Add("UF de expedição inválido");
                }
                data.UF = uf;
            }      

            if (data.Sexo != null)
            {
                switch (data.Sexo.ToUpper().Trim())
                {
                    case "M":
                    case "MASCULINO":
                        data.Sexo = "Masculino";
                        break;
                    case "F":
                    case "FEMININO":
                        data.Sexo = "Feminino";
                        break;
                    default:
                        errors.Add("Sexo inválido, Informe M-Masculino ou F-Feminino");
                        break;
                }
            }

            if (data.EstadoCivil != null)
            {
                switch (data.EstadoCivil.ToUpper().Trim())
                {
                    case "C":
                    case "CASADO":
                        data.EstadoCivil = "Casado";
                        break;
                    case "S":
                    case "SOLTEIRO":
                        data.EstadoCivil = "Solteiro";
                        break;
                    default:
                        errors.Add("Estado civil inválido, Informe C-Casado ou S-Solteiro");
                        break;
                }
            }

            if (data.DataNascimento != null)
            {
                if (data.DataNascimento.Year != 1)
                { 
                    try { DateTime nas = data.DataNascimento; }
                    catch { errors.Add("Data de Nascimento inválida");}

                    if (data.DataNascimento > DateTime.Today)
                    {
                        errors.Add("Data de Nascimento inválida");
                    }
                }               
            }

            if (data.DataExpedicao != null)
            {
                if (data.DataExpedicao.Year != 1)
                {
                    try { DateTime dtExpedicao = data.DataExpedicao; }
                    catch { errors.Add("Data de Expedição inválida"); }

                    if (data.DataExpedicao > DateTime.Today)
                    {
                        errors.Add("Data de Expedição inválida");
                    }
                }                
            }

            return errors;
        }

        private List<string> getUF()
        {
            return new List<string>()
            {
                "AC", "AL", "AP", "AM", "BA",
                "CE", "DF", "ES", "GO", "MA",
                "MT", "MS", "MG", "PA", "PB",
                "PR", "PE", "PI", "RJ", "RN",
                "RS", "RO", "RR", "SC", "SP",
                "SE", "TO"
            };
        }
    }
}
