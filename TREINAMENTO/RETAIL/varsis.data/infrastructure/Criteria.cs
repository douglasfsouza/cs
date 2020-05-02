using System;
using System.Collections.Generic;
using System.Text;

namespace Varsis.Data.Infrastructure
{
    public class Criteria
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public static Criteria Create(string field, string @operator, string value)
        {
            return new Criteria()
            {
                Field = field,
                Operator = @operator,
                Value = value
            };
        }
    }
}
