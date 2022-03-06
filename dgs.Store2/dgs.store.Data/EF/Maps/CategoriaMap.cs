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
    public class CategoriaMap : EntityMap<Categoria>, IEntityTypeConfiguration<Categoria> 
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder
                .ToTable("Categoria") // table
                .HasKey(x => x.Id);//pk
            builder
                .Property(x => x.Nome)
                .HasColumnType("varchar(50)")
                 .IsRequired();
             
            

            builder.HasOne(x => x.Usuario)
                   .WithMany(x => x.Categorias)
                   .HasForeignKey(x => x.UsuarioId)
                   .OnDelete(DeleteBehavior.Restrict);
            
            base.Setup(builder);

        }

        
    }
}
