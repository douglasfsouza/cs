using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace oDataOnly.infrastructure.attribute
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class ODataStringProperty : Attribute
    {
        readonly int maxLength;
        public ODataStringProperty(int maxLength)
        {
            this.maxLength = maxLength;
        }
        public int MaxLength { get => maxLength; }
    }
}