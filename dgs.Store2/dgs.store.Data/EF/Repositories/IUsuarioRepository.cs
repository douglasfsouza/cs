using dgs.Store.Domain.Contracts.Repositories;
using dgs.Store.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.store.Data.EF.Repositories
{
    public interface IUsuarioRepository  : IRepository<Usuario>
    {
        Usuario GetbyIdWithPerfis(int id);
        Task<Usuario> GetbyIdWithPerfisAsync(int id);
        IEnumerable<Usuario> GetAllWithPerfis();
        Task<IEnumerable<Usuario>> GetAllWithPerfisAsync();

        Task<Usuario> SighInAsync(string Email, string Senha);
       
    }
}
