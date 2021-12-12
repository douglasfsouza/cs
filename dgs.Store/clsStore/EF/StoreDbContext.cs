using dgs.Store.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.Store.Data.EF
{
    public class StoreDbContext : DbContext
    {
        private readonly string _conString;

        public StoreDbContext(IConfiguration config)
        {
            _conString = config.GetConnectionString("StoreDbConn");
            Database.EnsureCreated();

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer();
            optionsBuilder.LogTo(Console.WriteLine);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nome = "Higiene" },
                new Categoria { Id = 2, Nome = "Alimento" }
                );

            modelBuilder.Entity<Produto>().HasData(
                new Produto { Id = 1, Nome = "Arroz", CategoriaId = 2, Preco = 10 },
                new Produto { Id = 2, Nome = "Sabonete", CategoriaId = 1, Preco = 2 }

                );
        }

    }
}
