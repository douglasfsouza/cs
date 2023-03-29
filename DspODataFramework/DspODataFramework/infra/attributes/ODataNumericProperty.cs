using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspODataFramework.infra.attributes
{
    public sealed class ODataNumericProperty : ODataPropertyBase
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

        public bool IsPercentage { get; set; }

        public int Scale { get; set; }
    }
}
