using Chamados2.Context;
using Chamados2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Chamados2.Services
{
    public class UserService
    {  
        private readonly Chamados2DbContext _ctx;
        private readonly User _user;
        public UserService(Chamados2DbContext ctx, User user)
        {
            _ctx = ctx;
            _user = user;
        } 
      

        public async Task<(User user, string erro)> CreateUserWithEmailAndPassword(string Nome, string Email, string Password)
        {
            string erro = null;
            User user = null;

            try
            {
                user = await _ctx.Users.FirstOrDefaultAsync(a => a.Email == Email);

                if (user == null)
                {
                    user = new User
                    {
                        Nome = Nome,
                        Email = Email,
                        Senha = Password
                    };
                    _ctx.Users.Add(user);
                    await _ctx.SaveChangesAsync();
                }              
                
            }
            catch (Exception ex)
            {
                erro =  $"Erro ao incluir usuário: Erro: {ex.Message.ToString()}";                  
            }          
             
            return (user, erro);
        }

        public async Task<List<User>> GetUsers()
        {
            List<User> users = _ctx.Users.Select(x => new User{Id = x.Id, Nome = x.Nome, Email = x.Email, Senha = x.Senha }).ToList();
            return users;
        }

        public async Task<User> GetUser(string Email)
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Email == Email);
            return user;
        }

        public async Task<(User user, string error)> UpdateUser(User pUser)
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Email == pUser.Email);
            if (user == null)
            {
                return (pUser, "Usuário não cadastrado");
            }
            else
            {
                try
                {
                    user.Nome = pUser.Nome;
                    user.AvatarURL = pUser.AvatarURL;
                    //user.Senha = pUser.Senha;
                    _ctx.Users.Update(user);
                    _ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    return (pUser, ex.Message); 
                }
                
                return (user, null);
            }

        }
    }
}
