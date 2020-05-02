using System;
using System.Collections.Generic;
using System.Text;

namespace Varsis.Data.Infrastructure
{
    public class EntityBase : IEntity
    {
        public EntityBase()
        {
            this.RecId = Guid.NewGuid();
        }

        public virtual string EntityName => throw new NotImplementedException();

        public virtual Guid RecId { get; set; }
    }
}
