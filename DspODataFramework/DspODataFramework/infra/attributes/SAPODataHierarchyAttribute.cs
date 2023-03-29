using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspODataFramework.infra.attributes
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class SAPODataHierarchyAttribute : Attribute
    {
        public bool IsKey { get; set; }

        public string IsParentFor { get; set; }

        public string IsDrillDownFor { get; set; }

        public string IsLevelIndicatorFor { get; set; }
    }
}
