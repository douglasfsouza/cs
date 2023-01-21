using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ODataOnlyCore.infrastructure.attributes
{
    public sealed class ODataStringProperty : ODataPropertyBase
    {
        readonly int maxLength;
        public ODataStringProperty(int maxLength)
        {
            this.maxLength = maxLength;
        }
        public int MaxLength { get => maxLength; }
    }
}
