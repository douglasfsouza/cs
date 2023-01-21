using System.ComponentModel.DataAnnotations;
using oDataOnly.infrastructure.attribute;
using System;

namespace oDataOnly.Models
{
    public class Country
    {
        [Key]
        [SAPODataProperty("Code")]

        [ODataStringProperty(50)]
        public string Code { get; set; }

        [ODataStringProperty(50)]
        public string Name { get; set; }

        [ODataStringProperty(50)]
        public string Capital { get; set; }

        [ODataStringProperty(50)]
        public string Currency { get; set; }

        [ODataStringProperty(50)]
        public string River { get; set; }
    }
}