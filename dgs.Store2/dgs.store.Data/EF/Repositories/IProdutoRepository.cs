using dgs.Store.Domain.Contracts.Repositories;
using dgs.Store.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.store.Data.EF.Repositories
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        Produto GetByNome(string Nome);
        Task<Produto> GetByNomeAsync(string Nome);
        IEnumerable<Produto> GetAllWithCategorias();
        Task<IEnumerable<Produto>> GetAllWithCategoriasAsync();
    }
    
}
