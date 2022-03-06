using dgs.Store.Domain.Contracts.Repositories;
using dgs.Store.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.store.Data.EF.Repositories
{  

    public class RepositoryEF<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        private readonly StoreDbContext _ctx;
        protected readonly DbSet<TEntity> _dbSet;

        public RepositoryEF(StoreDbContext ctx)
        {
            _ctx = ctx;
            _dbSet = _ctx.Set<TEntity>();
                
        }
        public IEnumerable<TEntity> Get( )
        {             
            return _dbSet.ToList();
        }
        public async Task<IEnumerable<TEntity>> GetAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public TEntity Get(int Id)
        {
            return _dbSet.Find(Id);
        }
        public async Task<TEntity> GetAsync(int Id)
        {
            return await _dbSet.FindAsync(Id);
        }
        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }
        

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        

        public void Update(TEntity entity)
        {
            //_ctx.Entry(entity).State = EntityState.Detached;


            //_context.Entry(local).State = EntityState.Detached;
            _dbSet.Update(entity);
            
        }        
    }
}
