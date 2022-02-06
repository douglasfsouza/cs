using dgs.Store.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace dgs.store.Data.EF
{
    public class StoreDbContext : DbContext
    {
        private string _conn;

        public StoreDbContext(IConfiguration config)
        {
            _conn = config.GetConnectionString("StoreDbConn");
            Database.EnsureCreated();

        }


        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_conn);
            optionsBuilder.LogTo(Console.WriteLine);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria() { Id = 1, Nome = "Bebidas" },
                new Categoria() { Id = 2, Nome = "Eletrônicos" }
                );

            modelBuilder.Entity<Produto>().HasData(
                new Produto()
                {
                    Id = 1,
                    Nome = "Coca-cola",
                    Preco = 4.5M,
                    CategoriaId = 1
                },
                new Produto()
                {
                    Id = 2,
                    Nome = "Laptop",
                    Preco = 5000M,
                    CategoriaId = 2
                },
                new Produto()
                {
                    Id = 3,
                    Nome = "Celular Samsung A30",
                    Preco = 1600M,
                    CategoriaId = 2
                });
        }

    }
}
