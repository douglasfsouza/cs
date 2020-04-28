using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgGirls
{
    public class Contexto : DbContext
    {
        public DbSet<Girl> Girls { get; set; }
    }
}
