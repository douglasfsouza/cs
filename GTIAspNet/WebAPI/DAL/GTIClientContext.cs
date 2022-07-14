using GTIAPI.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTIAPI.DAL
{
    public class GTIClientContext : DbContext
    {
        private readonly IConfiguration _config;
        private readonly string _stringConn;

        public GTIClientContext(IConfiguration config, IHostEnvironment env)
        {
            _config = config;
            _stringConn = _config.GetConnectionString("GTIDbConn");
            if (env.IsDevelopment())
                base.Database.EnsureCreated();
        }
        public DbSet<GTICliente> clientes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_stringConn);
        }     
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GTICliente>().HasData(
                new GTICliente { Id = 1, Nome = "Douglas", CPF = "15457457836", DataExpedicao = DateTime.Today, DataNascimento = Convert.ToDateTime("1977-01-05"), EstadoCivil = "C", RG = "30032917-9", Sexo = "M", UF = "SP", OrgaoExpedicao = "SSP", EnderecoBairro = "Fatima", EnderecoCEP = "06624-030", EnderecoCidade = "Jandira", EnderecoLogradouro = "Rua Itambé", EnderecoNumero = "128", EnderecoUF = "SP" },
                new GTICliente { Id = 2, Nome = "Denise", CPF = "57457457825", DataExpedicao = DateTime.Today, DataNascimento = Convert.ToDateTime("1985-10-05"), EstadoCivil = "C", RG = "40032917-8", Sexo = "F", UF = "SP", OrgaoExpedicao = "SSP", EnderecoBairro = "Fatima", EnderecoCEP = "16624-060", EnderecoCidade = "Rio de Janeiro", EnderecoLogradouro = "Avenida Brasil", EnderecoNumero = "1028", EnderecoUF = "RJ" }
                );
        }
    }
}
