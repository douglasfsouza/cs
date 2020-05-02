using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Varsis.Data.Infrastructure
{
    public interface IEntityServiceWithReturn<T> where T : EntityBase
    {
        public Task<T> Insert(T entity);
        public Task<List<T>> Insert(List<T> entities);
    }

}
