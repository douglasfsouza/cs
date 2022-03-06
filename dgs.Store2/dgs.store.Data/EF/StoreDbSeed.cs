using dgs.Store.Domain.Entities;
using dgs.Store.Domain.Enums;
using dgs.Store.Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.store.Data.EF
{
    public static class StoreDbSeed
    {
        public static void Seed(this ModelBuilder modelBuilder)
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

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario()
                {
                    Id = 1,
                    Nome = "Doug",
                    Email = "douglas.fsouza2014@gmail.com",
                    DataCadastro = DateTime.Now,
                    Genero = Genero.Masculino,
                    Senha = "dg".Encrypt()
                },
                new Usuario()
                {
                    Id = 2,
                    Nome = "Andreia",
                    Email = "deya@gmail.com",
                    DataCadastro = DateTime.Now,
                    Genero = Genero.Feminino,
                    Senha = "and".Encrypt()
                });

            modelBuilder.Entity<Perfil>().HasData(
                new Perfil() { Id = 1, Nome = "Administrador" },
                new Perfil() { Id = 2, Nome = "Analista" },
                new Perfil() { Id = 3, Nome = "Assistente" }
            );

        }
    }
}
