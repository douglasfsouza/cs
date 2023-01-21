using ODataOnlyCore.infrastructure.attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static ODataOnlyCore.infrastructure.attributes.ODataTableAttrib;

namespace ODataOnlyCore.Model
{
    [ODataTable("Country", LogicalName = "Pais")]
    public class Country
    {
        [Key]
        public string Code { get; set; }

        [ODataStringProperty(20, Label = "Country")]

        public string Name { get; set; }

        public string Capital { get; set; }

        public string Currency { get; set; }

        public string River { get; set; }
    }
}
