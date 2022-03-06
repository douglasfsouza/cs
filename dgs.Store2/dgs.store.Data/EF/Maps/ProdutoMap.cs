using dgs.Store.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace dgs.store.Data.EF.Maps
{
    public class ProdutoMap : EntityMap<Produto>, IEntityTypeConfiguration<Produto> 
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder
                .ToTable("Produto") // table
                .HasKey(x => x.Id);//pk
            builder
                .Property(x => x.Nome)
                .HasColumnName("NomeCompleto")
                .HasColumnType("varchar(50)")
                //ou .HasMaxLength(50)
                .IsRequired();
            builder.Property(x => x.Preco)
                   .HasColumnType("money")
                   .IsRequired();
            builder.Property(x => x.CategoriaId)
                   .HasColumnType("int")
                   .IsRequired();

            builder.HasOne(x => x.Categoria)
                   .WithMany(x => x.Produtos)
                   .HasForeignKey(x => x.CategoriaId);

            builder.HasOne(x => x.Usuario)
                   .WithMany(x => x.Produtos)
                   .HasForeignKey(x => x.UsuarioId)
                   .OnDelete(DeleteBehavior.Restrict);

            base.Setup(builder);

        }

        
    }
}
