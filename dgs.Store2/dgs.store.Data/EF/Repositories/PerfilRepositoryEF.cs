using dgs.Store.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.store.Data.EF.Repositories
{

    public class PerfilRepositoryEF : RepositoryEF<Perfil>, IPerfilRepository
    {
        public PerfilRepositoryEF(StoreDbContext ctx) : base(ctx)
        {
        }

        public IEnumerable<Perfil> GetAllWithUsuarios()
        {
            return _dbSet.Include(X => X.Usuarios).ToList();
        }

        public async Task<IEnumerable<Perfil>> GetAllWithUsuariosAsync()
        {
            return await _dbSet.Include(X => X.Usuarios).ToListAsync();
        }

        public IEnumerable<Perfil> GetByIds(IEnumerable<int> Ids)
        {
            return _dbSet.Where(x => Ids.Contains((x.Id))).ToList();
        }

        public async Task<IEnumerable<Perfil>> GetByIdsAsync(IEnumerable<int> Ids)
        {
            return await _dbSet.Where(x => Ids.Contains((x.Id))).ToListAsync();
        }
    }
}
