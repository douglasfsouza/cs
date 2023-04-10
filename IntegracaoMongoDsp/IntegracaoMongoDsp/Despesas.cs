using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace IntegracaoMongoDsp
{
    public class Despesas
    {
        [Key]
        public string Code { get; set; }

        public int U_Id { get; set; }

        public DateTime U_Data { get; set; }

        public decimal U_Valor { get; set; }

        public string U_Tipo { get; set; }

        public string U_Tipo_Desc { get; set; }

        public string U_Descricao { get; set; }

        public int U_Ano { get; set; }

        public int U_IdMes { get; set; }

        public string U_Mes { get; set; }
    }
}
