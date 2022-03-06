using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.Store.Domain.Contracts.Data
{
    public interface IUnityOfWork
    {
        void Commit();
        Task CommitAsync();
        void Rollback();
    }
}
