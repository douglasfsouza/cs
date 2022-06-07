using Chamados2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chamados2.Context
{    
    public class Chamados2DbContext : DbContext
    {
        private string _conn = string.Empty;
        public Chamados2DbContext()
        {
            _conn = "Data Source=(localdb)\\MSSqlLocalDB;Initial Catalog=Chamados;Integrated Security=True;Pooling=False";
            Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Issue> Issues { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_conn);
        }

    }
}
