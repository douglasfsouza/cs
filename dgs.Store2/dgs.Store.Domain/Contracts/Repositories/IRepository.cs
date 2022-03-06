using dgs.Store.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.Store.Domain.Contracts.Repositories
{
    public interface IRepository<TEntity> where TEntity:Entity
    {
        //Crud
        IEnumerable<TEntity> Get( );
        Task<IEnumerable<TEntity>> GetAsync();

        TEntity Get(int Id);
        Task<TEntity> GetAsync(int Id);

        void Add(TEntity entity);


        void Update(TEntity entity);
 
        void Delete(TEntity entity);
 
    }
}
