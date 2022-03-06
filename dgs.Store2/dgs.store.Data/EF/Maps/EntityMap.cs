using dgs.Store.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dgs.store.Data.EF.Maps
{
    public abstract class EntityMap<TEntity> where TEntity: Entity
    {
        protected void Setup(EntityTypeBuilder<TEntity> builder) 
        {
            builder.Property(x => x.DataCadastro)
                   .HasColumnType("datetime2")
                   .IsRequired();
            builder.Property(x => x.DataAlteracao)
                   .HasColumnType("datetime2")
                   .IsRequired();
            builder.Property(x => x.UsuarioId)
                   .HasColumnType("int")
                   .IsRequired();
            //builder.HasOne(x => x.Usuario)
            //       .WithMany(NamTEntity)
        }
    }
}
