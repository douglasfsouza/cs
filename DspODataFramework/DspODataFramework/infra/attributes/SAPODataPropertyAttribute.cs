using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspODataFramework.infra.attributes
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class SAPODataPropertyAttribute : Attribute
    {
        public enum AgregationRoleEnum
        {
            None,
            Dimension,
            Measure
        }
        public enum AgregationFunctionEnum
        {
            Sum,
            Max,
            Min,
            Avg
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

        public string AggregationRole { get; set; } //dgs

        public string Tooltip { get; set; }
        public bool Creatable { get; set; } = true;
        public bool Updatable { get; set; } = true;
        public bool Sortable { get; set; } = true;
        public bool Filterable { get; set; } = true;
        public string DisplayFormat { get; set; }
        public string FilterRestriction { get; set; }
        public string Semantics { get; set; }
        public AgregationRoleEnum AgregationRole { get; set; }

        public AgregationFunctionEnum AgregationFunction { get; set; } = AgregationFunctionEnum.Sum;
    }
}
