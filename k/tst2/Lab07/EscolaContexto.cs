using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab07
{
    public class EscolaContexto : DbContext
    {
        public EscolaContexto(string stringName) : base(stringName)
        {

        }
        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Desempenho> Desempenho { get; set; }

    }
}
