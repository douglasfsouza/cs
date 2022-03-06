using dgs.Store.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.store.Data.EF.Repositories
{
    public class ProdutoRepositoryEF : RepositoryEF<Produto>, IProdutoRepository
    {
        public ProdutoRepositoryEF(StoreDbContext ctx) : base(ctx)
        {
                
        }

        public IEnumerable<Produto> GetAllWithCategorias()
        {
            return _dbSet.Include(x => x.Categoria).ToList();
        }

        public async Task<IEnumerable<Produto>> GetAllWithCategoriasAsync()
        {
            return await _dbSet.Include(x => x.Categoria).ToListAsync();
        }

        public Produto GetByNome(string Nome)
        {
            return _dbSet.FirstOrDefault(x => x.Nome == Nome);
        }

        public async Task<Produto> GetByNomeAsync(string Nome)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Nome == Nome);
        }
    }
}
