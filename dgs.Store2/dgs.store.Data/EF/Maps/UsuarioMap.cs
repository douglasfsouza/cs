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
    public class UsuarioMap : EntityMap<Usuario>, IEntityTypeConfiguration<Usuario> 
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder
                .ToTable("Usuario") // table
                .HasKey(x => x.Id);//pk
            builder
                .Property(x => x.Nome)
                .HasColumnType("varchar(50)")
                .IsRequired();
            builder
               .Property(x => x.Email)
               .HasColumnType("varchar(50)")
               .IsRequired();
            builder
              .Property(x => x.Senha)
              .HasColumnType("char(88)")
              .IsRequired();
            builder
              .Property(x => x.Genero)
              .HasColumnType("int")
              .IsRequired();

            builder.HasOne(x => x.Usuario)
                   .WithMany(x => x.Usuarios)
                   .HasForeignKey(x => x.UsuarioId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Perfis)
                   .WithMany(x => x.Usuarios)
                   .UsingEntity<Dictionary<string, object>>(
                   "PerfilUsuario",
                   perfil => perfil.HasOne<Perfil>()
                                   .WithMany()
                                   .HasForeignKey("UsuarioId"),
                   usuario => usuario.HasOne<Usuario>()
                                     .WithMany()
                                     .HasForeignKey("PerfilId"));

            base.Setup(builder);

        }

        
    }
}
