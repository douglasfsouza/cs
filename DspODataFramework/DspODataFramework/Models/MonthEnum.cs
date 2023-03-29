using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DspODataFramework.Models
{
    public class MonthEnum : ISimpleList
    {
        private string _key;
        private string _description;
        public MonthEnum(string key, string description)
        {
            _key = key;
            _description = description;
        }

        [Key]
        public string Key { get =>_key; set => _key = value; }
        public string Description { get => _description; set => _description = value; }

        public static List<MonthEnum> Values()
        {
            return new List<MonthEnum>
            {
                {new MonthEnum("1", "January") },
                {new MonthEnum("2", "February") },
                {new MonthEnum("3", "March") },
                {new MonthEnum("4", "April") },
                {new MonthEnum("5", "May") },
                {new MonthEnum("6", "June") },
                {new MonthEnum("7", "July") },
                {new MonthEnum("8", "August") },
                {new MonthEnum("9", "September") },
                {new MonthEnum("10", "October") },
                {new MonthEnum("11", "November") },
                {new MonthEnum("12", "December") }
            };
        }
         
    }
}
