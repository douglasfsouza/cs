using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Varsis.OData.Attributes
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class ODataNumericProperty : Attribute
    {
        readonly int precision;

        public ODataNumericProperty(int precision)
        {
            this.precision = precision;
        }

        public int Precision
        {
            get => precision;
        }

        public int Scale { get; set; }
    }
}