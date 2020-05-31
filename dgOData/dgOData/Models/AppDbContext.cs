using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dgOData.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {

        }
        public DbSet<Escola> Escolas { get; set; }
        public DbSet<Aluno> Alunos { get; set; }
    }
}
