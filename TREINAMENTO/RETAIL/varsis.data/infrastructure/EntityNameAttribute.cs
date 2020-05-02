using System;
using System.Collections.Generic;
using System.Text;

namespace Varsis.Data.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EntityNameAttribute : System.Attribute
    {
        private string _entityName;

        public EntityNameAttribute(string entityName)
        {
            _entityName = entityName;
        }

        public string EntityName
        {
            get
            {
                return _entityName;
            }
        }
    }
}
