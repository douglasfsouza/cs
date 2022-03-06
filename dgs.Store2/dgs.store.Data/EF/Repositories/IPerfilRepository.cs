using dgs.Store.Domain.Contracts.Repositories;
using dgs.Store.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.store.Data.EF.Repositories
{
    public interface IPerfilRepository  : IRepository<Perfil>
    {
        IEnumerable<Perfil> GetAllWithUsuarios();
        Task<IEnumerable<Perfil>> GetAllWithUsuariosAsync();
        IEnumerable<Perfil> GetByIds(IEnumerable<int> Ids);
        Task<IEnumerable<Perfil>> GetByIdsAsync(IEnumerable<int> Ids);
    }
      
}
