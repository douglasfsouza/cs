using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspODataFramework.infra.attributes
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
