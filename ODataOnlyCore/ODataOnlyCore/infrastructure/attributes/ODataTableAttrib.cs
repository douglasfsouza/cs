using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ODataOnlyCore.infrastructure.attributes
{
    public class ODataTableAttrib
    {
        [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
        public sealed class ODataTableAttribute : Attribute
        {
            readonly string _physicalName;

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

            // This is a positional argument
            public ODataTableAttribute(string physicalName)
            {
                _physicalName = physicalName;
            }

            public string PhysicalName
            {
                get { return _physicalName; }
            }
            public string LogicalName { get; set; }

            public bool IsCustom { get; set; } = true;

            public string TableType { get; set; } = "bott_NoObject";

            public SemanticsEnum Semantics { get; set; }
        }
    }
}
