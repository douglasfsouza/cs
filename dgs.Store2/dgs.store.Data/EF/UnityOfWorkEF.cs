using dgs.Store.Domain.Contracts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.store.Data.EF
{
    public class UnityOfWorkEF : IUnityOfWork
    {
        private readonly StoreDbContext _ctx;

        public UnityOfWorkEF(StoreDbContext ctx)
        {
            _ctx = ctx;
        }
        public void Commit()
        {
            _ctx.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await _ctx.SaveChangesAsync();
        }

        public void Rollback()
        {
            return;
        }
    }
}
