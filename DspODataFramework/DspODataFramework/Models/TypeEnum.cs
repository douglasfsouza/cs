using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspODataFramework.Models
{
    public class TypeEnum : ISimpleList
    {
        private string _key;
        private string _description;

        public TypeEnum(string key, string description)
        {
            _key = key;
            _description= description;
        }


        [Key]
        public string Key { get => _key; set => _key = value; }
        public string Description { get => _description; set => _description = value; }

        public static List<TypeEnum> Values()
        {
            return new List<TypeEnum>()
            {
                {new TypeEnum("C","Crédito") },
                {new TypeEnum("D", "Débito") }
            };
        }
    }
}
 