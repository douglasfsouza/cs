using dg.Cli.UI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dg.Cli.UI.DAL
{
    public class ClientsDbContext : DbContext
    {
        public ClientsDbContext()
        {
            base.Database.EnsureCreated();
        }

        public DbSet<Client> Clients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("cadcli");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().HasData(
                new Client { Id = 1, Name = "Doug", Dob = new DateTime(1977, 1, 5) },
                new Client { Id = 2, Name = "Andreia", Dob = new DateTime(1979, 2, 13) });

        }

    }
}
