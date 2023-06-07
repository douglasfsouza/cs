using DspODataFramework.infra.attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspODataFramework.infra
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DgAttribute : Attribute
    {
       
        private string _aggregationRole;

        

        public DgAttribute(string aggregationRole)
        {
            _aggregationRole = aggregationRole;
        }

        public string AggregationRole { get => _aggregationRole; set => _aggregationRole = value; }
    




    }
}
