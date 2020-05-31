using Microsoft.AspNet.OData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace OData2.Models
{
    [EnableQuery]
    public class OContexto : DbContext
    {
        public OContexto(DbContextOptions<OContexto> options) : base(options)
        {

        }
        
        public DbSet<Cidade> Cidades { get; set; }
    }
}
