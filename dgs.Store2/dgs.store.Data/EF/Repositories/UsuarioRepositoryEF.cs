using dgs.Store.Domain.Entities;
using dgs.Store.Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.store.Data.EF.Repositories
{

    public class UsuarioRepositoryEF : RepositoryEF<Usuario>, IUsuarioRepository
    {
        public UsuarioRepositoryEF(StoreDbContext ctx) : base(ctx)
        {
        }

        public IEnumerable<Usuario> GetAllWithPerfis()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Usuario>> GetAllWithPerfisAsync()
        {
            throw new NotImplementedException();
        }

        public Usuario GetbyIdWithPerfis(int id)
        {
            return _dbSet.Include(x => x.Perfis).FirstOrDefault(x => x.Id == id);
        }

        public async Task<Usuario> GetbyIdWithPerfisAsync(int id)
        {
            return await _dbSet.Include(x => x.Perfis).FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task<Usuario> SighInAsync(string Email, string Senha)
        {
            return await _dbSet.Include(x => x.Perfis).FirstOrDefaultAsync(x => x.Email == Email && x.Senha == Senha.Encrypt());
        }
    }
}
