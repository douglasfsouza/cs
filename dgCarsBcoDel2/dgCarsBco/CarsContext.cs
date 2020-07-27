using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace dgCarsBco
{
    public class CarsContext : DbContext
    {        
        public DbSet<Cars> cars { get;  set;}
    }
}
