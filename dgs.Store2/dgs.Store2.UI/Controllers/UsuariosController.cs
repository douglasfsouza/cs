using dgs.store.Data.EF;
using dgs.store.Data.EF.Repositories;
using dgs.Store.Domain.Contracts.Data;
using dgs.Store.Domain.Entities;
using dgs.Store.Domain.Enums;
using dgs.Store.Domain.Helpers;
using dgs.Store2.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.Store2.UI.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPerfilRepository _perfilRepository;
        private readonly IUnityOfWork _uow;

        public UsuariosController(IUsuarioRepository usuarioRepository, IUnityOfWork uow, IPerfilRepository perfilRepository)
        {
            _usuarioRepository = usuarioRepository;
            _perfilRepository = perfilRepository;
            _uow = uow;
        }
        public async Task<IActionResult> Index()
        {

            var usrs = ( await _usuarioRepository.GetAsync()).Select(x => x.ToUsuarioIndexVM());
            //ou
            //var usrs = (await _usuarioRepository.GetWithPerfisAsync()).Select(x => x.ToUsuarioIndexVM());

            var uivs = await MontarIndexPerfis(usrs);
             
            return View(uivs);
        }

        private async Task<IEnumerable<UsuarioIndexVM>> MontarIndexPerfis(IEnumerable<UsuarioIndexVM> usrs)
        {
            List<UsuarioIndexVM> uivs = new List<UsuarioIndexVM>();
            
            var perfis = await _perfilRepository.GetAllWithUsuariosAsync();

            foreach (UsuarioIndexVM usr in usrs)
            {                
                var nomes = (from p in perfis
                            where p.UsuarioId == usr.Id
                            select p.Nome).Distinct().ToList();                 

                UsuarioIndexVM uiv = new UsuarioIndexVM()
                {
                    Id = usr.Id,
                    Nome = usr.Nome,
                    Email = usr.Email,
                    NomesPerfis = string.Join(",", nomes),
                    Genero = usr.Genero
                };
                uivs.Add(uiv);

            }
            return uivs;

        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int Id)
        {

            var model = new UsuarioAddEditVM();

            if (Id != 0)
            {
                var data = await _usuarioRepository.GetbyIdWithPerfisAsync(Id);
                model = data.ToUsuarioAddEditVM();

                model.PerfisId = data.Perfis?.Select(x => x.Id);
            }
            await GetPerfis(model);
            return View(model);
        }

        private async Task GetPerfis(UsuarioAddEditVM model)
        {
            model.Perfis = (await _perfilRepository.GetAsync()).Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Nome.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(int Id, UsuarioAddEditVM model)
        {
            if (!ModelState.IsValid)
            {
                await GetPerfis(model);
                return View(model);
            }
            var usr = model.ToData();

            usr.Perfis = await MontarPerfil(model.PerfisId);

            if (Id == 0)
            {
                usr.Senha = usr.Senha.Encrypt();
                _usuarioRepository.Add(usr);
            }
            else
            {
                //tratar para não mostrar a senha no browser                
                //usr.Id = Id;
                //usr.Senha = _usuarioRepository.Get(Id).Senha;
                //usr.DataAlteracao = DateTime.Now;
                // com model.toData não funcionou!!!
                var x = _usuarioRepository.Get(Id);
                x.Nome = model.Nome;
                x.Genero = (Genero)model.Genero;
                x.Email = model.Email;
                x.DataAlteracao = DateTime.Now;
                x.Perfis = usr.Perfis;

                //Usuario u = new Usuario()
                //{
                //    Id = Id,
                //    Nome = model.Nome,
                //    Genero = (Genero)model.Genero,
                //    Email = model.Email,
                //    DataAlteracao = DateTime.Now
                //};

                _usuarioRepository.Update(x);
            }
            await _uow.CommitAsync();
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<Perfil>> MontarPerfil(IEnumerable<int> perfisId)
        {
            return await _perfilRepository.GetByIdsAsync(perfisId);
        }

        public async Task<IActionResult> ConfirmarDel(int Id)
        {
            var prod = await _usuarioRepository.GetAsync(Id);
            var pro = new UsuarioIndexVM()
            {
                Id = Id,
                Nome = prod.Nome
            };

            return View(pro);
        }

        public async Task<IActionResult> Excluir(int Id)
        {
            var cat = await _usuarioRepository.GetAsync(Id);

            if (cat == null)
            {
                return BadRequest();
            }

            _usuarioRepository.Delete(cat);

            await _uow.CommitAsync();

            return NoContent();
        }

        [HttpGet]

        public async Task<IActionResult> MeuPerfil()
        {
            var userId = User.Claims?.FirstOrDefault(x => x.Type == "idUsuario").Value;
            Usuario usr = null;
            if (!string.IsNullOrEmpty(userId))
            {
                usr = await _usuarioRepository.GetAsync(Convert.ToInt32(userId));
            }
            return View(usr.ToMeuPerfilVM());

        }

        [HttpPost]
        public async Task<IActionResult> MeuPerfil(MeuPerfilVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usr = await _usuarioRepository.GetAsync(model.Id);

            if (usr.Senha != model.SenhaAtual.Encrypt())
            {
                ModelState.AddModelError("NovaSenha", "As senhas não confererem");
                return View(model);
            }

            usr.Senha = model.NovaSenha.Encrypt();
            await _uow.CommitAsync();

            TempData["msg"] = "Senha alterada com Sucesso!";

            return RedirectToAction("Index");

        }

    }
}
