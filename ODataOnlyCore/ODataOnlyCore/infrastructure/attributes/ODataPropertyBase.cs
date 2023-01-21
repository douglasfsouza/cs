using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ODataOnlyCore.infrastructure.attributes
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ODataPropertyBase : Attribute
    {
        public string Label { get; set; }

        public string PhysicalName { get; set; }

        public bool Mandatory { get; set; }

        public string linkedTable { get; set; }

        public string linkedUDO { get; set; }

        public string linkedSystemObject { get; set; }

        public string defaultValue { get; set; }

        public string Tooltip { get; set; }

        public bool IsCustom { get; set; } = false;

        public bool OnlyDisplay { get; set; }

        public bool IgnoreOnBackend { get; set; }
    }
}
