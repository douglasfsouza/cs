using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.OData;

namespace OData2.Models
{
    [EnableQuery]   
    public class Cidade
    {
        
        public int Id { get; set; }
        [Required]        
        
        public string Nome { get; set; }
        public string UF { get; set; }
       
    }
}
