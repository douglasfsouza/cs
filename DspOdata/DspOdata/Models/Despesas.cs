using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.OData;
using Varsis.OData.Attributes;

namespace Dsp.Models
{
    [SAPODataEntityType("Despesas", Semantics = SAPODataEntityTypeAttribute.SemanticsEnum.Aggregate)]

    public class Despesas
    {
        [Key]
        [ODataNumericProperty(10)]
        [SAPODataProperty("Id", AgregationRole = SAPODataPropertyAttribute.AgregationRoleEnum.Dimension, Tooltip = "Id")]
        public int Id { get; set; }

        [SAPODataProperty("Data", AgregationRole = SAPODataPropertyAttribute.AgregationRoleEnum.Dimension, Tooltip = "Data")]
        public DateTime Data { get; set; }

        [ODataNumericProperty(10, Scale = 2)]
        [SAPODataProperty("Valor", AgregationRole = SAPODataPropertyAttribute.AgregationRoleEnum.Dimension, Tooltip = "Valor")]
        public double Valor { get; set; }

        [ODataStringProperty(10)]
        [SAPODataProperty("Tipo", AgregationRole = SAPODataPropertyAttribute.AgregationRoleEnum.Dimension, Tooltip = "Tipo")]
        public string Tipo { get; set; }

        [ODataStringProperty(50)]
        [SAPODataProperty("Descrição", AgregationRole = SAPODataPropertyAttribute.AgregationRoleEnum.Dimension, Tooltip = "Descrição")]
        public string Descricao { get; set; }
    }
}
