using Chamados2.Context;
using Chamados2.Models;
using Chamados2.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Chamados2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly Chamados2DbContext _ctx;
        private readonly User _user;

        public UserController(Chamados2DbContext ctx, User user)
        {
            _ctx = ctx;
            _user = user;
        }
        
        [HttpGet]
        [Route("getUsers")]
        public async Task<List<User>> GetUsers()
        {
            UserService service = new UserService(_ctx,_user);
            List<User> users = await service.GetUsers();
            return users;
        }

        [HttpPost]
        [Route("acessar")]
        public async Task<IActionResult> Acessar([FromBody] User user)
        {
            UserService service = new UserService(_ctx, _user);
            User rsUser = await service.GetUser(user.Email);
            if (rsUser == null)
            {
                return BadRequest(Newtonsoft.Json.JsonConvert.SerializeObject( "Usuário não cadastrado"));
            }

            if (rsUser.Senha != user.Senha)
            {
                return BadRequest(Newtonsoft.Json.JsonConvert.SerializeObject("Senha incorreta"));
            }

            return Ok(rsUser);

        }

        [HttpPost]
        [Route("createUserWithEmailAndPassword")]
        public async Task<IActionResult> CreateUserWithEmailAndPassword([FromBody] User user)
        {            
            User serviceResult = null;
            string error = null;
            UserService service = new UserService(_ctx,_user);

            try
            {
                (serviceResult, error) = await service.CreateUserWithEmailAndPassword(user.Nome, user.Email, user.Senha);
            }
            catch (Exception ex)
            {
                error = ex.Message;                 
            }             

            if (error == null)
                return Ok(serviceResult);
            else
                return BadRequest(error);            
        }
        [HttpPatch]
        [Route("update")]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            User resultUser = null;
            string error = null;
            UserService service = new UserService(_ctx, _user);
            try
            {
                (resultUser,error) = await service.UpdateUser(user);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            if (error == null)
            {
                return Ok(resultUser);
            }
            else
            {
                return BadRequest(error);
            }
        }
    }
}
