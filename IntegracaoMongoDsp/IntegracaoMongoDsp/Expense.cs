using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace IntegracaoMongoDsp
{
    public class Expense
    {
        [Key]
        public string Code { get; set; }

        [BsonId]
        public ObjectId Id { get; set; }

        public DateTime Date { get; set; }

        public long Year { get; set; }

        public long Month { get; set; }

        public string MonthDescription { get; set; }

        //[BsonElement("type")]
        public string Type { get; set; }

        public string TypeDescription { get; set; }

        public decimal Value { get; set; }

        public string Description { get; set; }
    }
}
