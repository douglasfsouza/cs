using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsBco2
{
    public class CarsContext : DbContext
    {
        public DbSet<Cars> Cars { get; set; }
    }
}
