using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgCodeFirst
{
    public class dgContexto : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
    }
}
