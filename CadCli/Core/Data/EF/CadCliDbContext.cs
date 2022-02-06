using CadCli.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadCli.Core.Data.EF
{
    public class CadCliDbContext : DbContext
    {
        private readonly IConfiguration _config;

        public DbSet<Cliente> Clientes { get; set; }
        public CadCliDbContext(IConfiguration config)
        {
            _config = config;
            base.Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseInMemoryDatabase("cadCli");
            optionsBuilder.UseSqlServer(_config.GetConnectionString("CadCliCon"));


            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>().HasData(
                new Cliente() { Id = 1, Nome = "Douglas", Idade = 45},
                new Cliente() { Id = 2, Nome = "Andreia", Idade = 43}
            );
        }



    }
}
