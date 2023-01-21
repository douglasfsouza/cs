using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace oDataBasic.Model
{
    public class Country
    {
        [Key]
        public string Code { get; set; }

        public string Name { get; set; }

        public string Capital { get; set; }

        public string Currency { get; set; }

        public string River { get; set; }


    }
}