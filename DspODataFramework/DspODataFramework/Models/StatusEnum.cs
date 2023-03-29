using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspODataFramework.Models
{
    public class StatusEnum : ISimpleList
    {
        private string _key;
        private string _description;

        public StatusEnum(string key, string description)
        {
            _key = key;
            _description = description;             
        }


        [Key]
        public string Key { get => _key; set => _key = value; }
        public string Description { get => _description; set => _description = value; }

        public static List<StatusEnum> Values()
        {
            return new List<StatusEnum>
            {
                {new StatusEnum("A","Ativo") },
                {new StatusEnum("I", "Inativo") }
            };
        }
    }
}
