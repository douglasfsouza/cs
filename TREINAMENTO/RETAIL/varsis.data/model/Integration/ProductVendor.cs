using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;


namespace Varsis.Data.Model.Integration
{
    public class ProductVendor : EntityBase
    {
        public override string EntityName => "Dados do forncedor cadastro produto";

        public long cod_item { get; set; }
        public long cod_forn { get; set; }
        public long cod_forn_alt { get; set; }
        public long cod_item_alt { get; set; }
        public long? dig_item { get; set; }
        public long? dig_forn { get; set; }
        public string? referencia { get; set; }
        public string? descricao { get; set; }
        public string?  fatur_unid { get; set; }
        public long?  uf_unid { get; set; }
        public string? uf_fator { get; set; }
        public double? uf_fator_conv { get; set; }
        public string? emb_xml { get; set; }

        public DateTime lastupdate { get; set; }
    }
}
