using dgs.store.Data.EF.Repositories;
using dgs.Store2.UI.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace dgs.Store2.UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }
        [HttpGet]
        public IActionResult Login() => View();


        [HttpPost]
        public async Task<IActionResult> Login(string returnURL, AuthVM model)
        {
             
            if (ModelState.IsValid)
            {
                var usr = await _usuarioRepository.SighInAsync(model.Email, model.Senha);
                if (usr is null)
                {
                    ModelState.AddModelError("","Email ou Senha invalidos");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new Claim("nomeUsuario",usr.Nome),
                    new Claim("email",usr.Email),
                    new Claim("idUsuario", usr.Id.ToString())
                };

                var identity = new ClaimsIdentity(claims,
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        "nomeUsuario",
                        string.Join(", ", usr.Perfis.Select(x => x.Nome)));
                await HttpContext.SignInAsync(
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties()
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    }
                    );

            }

            if (string.IsNullOrEmpty(returnURL))
            {
                return RedirectToAction("index", "home");
            }
            else
            {
                if (Url.IsLocalUrl(returnURL))
                {
                    return Redirect(returnURL);
                }
                else
                {
                    return RedirectToAction("index", "home");
                }
                
            }
            
        }
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        } 
    }
}
