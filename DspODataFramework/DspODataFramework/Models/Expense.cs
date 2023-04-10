﻿using DspODataFramework.infra.attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DspODataFramework.Models
{ 
    public class Expense
    {
        [Key]
        [SAPODataProperty("#Code",AgregationRole = SAPODataPropertyAttribute.AgregationRoleEnum.Dimension)]
        public string Code { get; set; }

        [BsonId]
        public ObjectId Id { get; set; }

        [SAPODataProperty("Date", FilterRestriction = "interval", DisplayFormat = "Date")]
        [ODataDateProperty("Date")]
        public DateTime Date { get; set; }

        public long Year { get; set; }

        [ODataStringProperty(15, Label = "Id Month")]
        public long Month { get; set; }

        [ODataStringProperty(15,Label = "Month")]
        public string MonthDescription { get; set; }

        [ODataStringProperty(1,Label = "Id Type")]
        //[BsonElement("type")]
        public string Type { get; set; }

        [ODataStringProperty(10, Label = "Type")]
        public string TypeDescription { get; set; }

        [ODataNumericProperty(11, Scale = 2, Label = "Value")]
        public decimal Value { get; set; }

        [ODataStringProperty(50, Label = "Description")]
        public string Description { get; set; }
    }
}
