using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Varsis.OData.Attributes
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class SAPODataPropertyAttribute : Attribute
    {
        public enum AgregationRoleEnum
        {
            None,
            Dimension,
            Measure
        }

        readonly string labelString;

        // This is a positional argument
        public SAPODataPropertyAttribute(string label)
        {
            this.labelString = label;
        }

        public string Label
        {
            get { return labelString; }
        }

        public string Header { get; set; }

        public string Tooltip { get; set; }
        public bool Creatable { get; set; } = true;
        public bool Sortable { get; set; } = true;
        public bool Filterable { get; set; } = true;
        public string DisplayFormat { get; set; }
        public string FilterRestriction { get; set; }
        public string Semantics { get; set; }
        public AgregationRoleEnum AgregationRole { get; set; }
        public bool IgnoreOnBackend { get; set; }
        public string ValueList { get; set; }
    }
}