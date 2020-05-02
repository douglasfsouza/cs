using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Varsis.Data.Infrastructure
{
    public interface IDataSource<T> where T : EntityBase
    {
        public Task<List<T>> Read();
        public Task<List<T>> ReadDefinitive();
    }
}
