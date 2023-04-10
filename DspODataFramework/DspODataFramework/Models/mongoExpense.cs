using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspODataFramework.Models
{
    public class mongoExpense
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("type")]
        public string type { get; set; }

        [BsonElement("description")]
        public string description { get; set; }

        [BsonElement("value")]
        public  Decimal value { get; set; }

        [BsonElement("date")]
        public DateTime date { get; set; }
    }
}
