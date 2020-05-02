using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Varsis.Data.Infrastructure
{
    public interface IEntityService<T> where T : EntityBase
    {
        public Task<bool> Create();
        public Task Insert(T entity);
        public Task Insert(List<T> entities);
        public Task Update(T entity);
        public Task Update(List<T> entities);
        public Task Delete(T entity);
        public Task Delete(List<Criteria> criterias);
        public Task<List<T>> List(List<Criteria> criterias, long page = -1, long size = -1);
        public Task<Varsis.Data.Infrastructure.Pagination> TotalLinhas(long? size, List<Criteria> criterias);
        public Task<T> Find(List<Criteria> criterias);
    }

}
