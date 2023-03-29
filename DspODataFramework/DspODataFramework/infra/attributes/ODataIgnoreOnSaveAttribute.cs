using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspODataFramework.infra.attributes
{
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class ODataIgnoreOnSaveAttribute : Attribute
    {
        // This is a positional argument
        public ODataIgnoreOnSaveAttribute()
        {
        }
    }
}
