using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Varsis.OData.Attributes
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class SAPODataEntityTypeAttribute : Attribute
    {
        public enum SemanticsEnum
        {
            None,
            VCard,
            VEvent,
            VTodo,
            Parameters,
            Aggregate,
            Variant
        }
        readonly string labelString;

        // This is a positional argument
        public SAPODataEntityTypeAttribute(string label)
        {
            this.labelString = label;
        }

        public string Label
        {
            get { return labelString; }
        }

        public SemanticsEnum Semantics { get; set; }
    }
}