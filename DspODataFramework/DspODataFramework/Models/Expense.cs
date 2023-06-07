using DspODataFramework.infra;
using DspODataFramework.infra.attributes;
using Microsoft.SqlServer.Server;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData;

namespace DspODataFramework.Models
{
    [SAPODataEntityType("Expense", Semantics = SAPODataEntityTypeAttribute.SemanticsEnum.Aggregate)]
   // [XODataTableAttribute("Expense", IsCustom = true, LogicalName = "Expense", Semantics = XODataTableAttribute.SemanticsEnum.Aggregate)]
    //[DgDataTableAttribute("Expense", LogicalName = "Expense", Semantics = DgDataTableAttribute.SemanticsEnum.Aggregate, IsCustom = true)]
    public class Expense
    {
        [Key]
        [ODataStringProperty(50, Label ="#Code", Tooltip = "Code")]
        [SAPODataProperty("#Code", Tooltip = "Code", AgregationRole = SAPODataPropertyAttribute.AgregationRoleEnum.Dimension)]
        public string Code { get; set; }

        //[BsonId]
        //public ObjectId Id { get; set; }
        public string Id { get; set; }


        [SAPODataProperty("Date", Tooltip = "Date", FilterRestriction = "interval", DisplayFormat = "Date", AgregationRole = SAPODataPropertyAttribute.AgregationRoleEnum.Dimension,AggregationRole = "dimension")]
        [ODataDateProperty("Date", Label = "Date", Tooltip = "Date")]
       // [ODataFormatting["sap:aggregation","dimension"]
        //[Property("sap:aggregation-role")]
       // [MetadataAttribute("sap:aggregation-role","Dimension")]
       // [Attribute("sap:aggregation-role")]
       // [MetadataAttribute([Attribute("","")]
        //[SapAggregation(dimension)]
        //[DgAttribute(sap = "sap:aggregation-role")]
       
        //[Dg("dimension")]
        public DateTime Date { get; set; }

        [SAPODataProperty("Year", Tooltip = "YEAR", AgregationRole = SAPODataPropertyAttribute.AgregationRoleEnum.Dimension)]
        [ODataNumericProperty(4, Label = "Year", Tooltip = "Year")]
        public long Year { get; set; }

        [ODataStringProperty(15, Label = "Id Month", Tooltip = "Id Month")]
        [SAPODataProperty("Id Month",  Tooltip = "Id Month", AgregationRole = SAPODataPropertyAttribute.AgregationRoleEnum.Dimension)]
        public long Month { get; set; }

        [ODataStringProperty(15,Label = "Month", Tooltip = "Month")]
        [SAPODataProperty("Month", Tooltip = "Month", AgregationRole = SAPODataPropertyAttribute.AgregationRoleEnum.Dimension)]
        public string MonthDescription { get; set; }

        [ODataStringProperty(1,Label = "Id Type", Tooltip = "Id Type")]
        [SAPODataProperty("Id Type", Tooltip = "Id Type", AgregationRole = SAPODataPropertyAttribute.AgregationRoleEnum.Dimension)]
        //[BsonElement("type")]
        public string Type { get; set; }

        [ODataStringProperty(10, Label = "Type", Tooltip = "Type")]
        [SAPODataProperty("Type", Tooltip = "Type", AgregationRole = SAPODataPropertyAttribute.AgregationRoleEnum.Dimension)]
        public string TypeDescription { get; set; }

        [ODataNumericProperty(11, Scale = 2, Label = "Value", Tooltip = "Value")]
        [SAPODataProperty("Value", Tooltip = "Value", AgregationRole = SAPODataPropertyAttribute.AgregationRoleEnum.Measure)]
        public decimal Value { get; set; }

        [ODataStringProperty(50, Label = "Description", Tooltip = "Description")]
        [SAPODataProperty("Description", Tooltip = "Description", AgregationRole = SAPODataPropertyAttribute.AgregationRoleEnum.Dimension)]
        public string Description { get; set; }
    }
}
