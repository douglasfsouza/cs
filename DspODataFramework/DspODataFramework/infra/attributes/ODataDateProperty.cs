using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspODataFramework.infra.attributes
{
    public sealed class ODataDateProperty : ODataPropertyBase
    {
        public ODataDateProperty(string Label)
        {
            this.Label = Label;
        }

        public bool OnlyTime { get; set; }
    }
}
