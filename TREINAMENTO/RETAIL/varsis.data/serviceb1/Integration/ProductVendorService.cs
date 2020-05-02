using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model.Integration;
using System.Linq;


namespace Varsis.Data.Serviceb1.Integration
{
  public  class ProductVendorService : IEntityService<Model.Integration.ProductVendor>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public ProductVendorService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = mountFieldMap();
            _FieldType = mountFieldType();
        }

        async public Task<bool> Create()
        {
            bool result = false;

            result = await createTable();

            return result;
        }
        async public Task<Varsis.Data.Infrastructure.Pagination> TotalLinhas(long? size, List<Criteria> criterias)
        {
            List<string> filter = new List<string>();
            int cont = 0;
            if (criterias?.Count != 0)
            {
                foreach (var c in criterias)
                {
                    cont++;
                    string field = _FieldMap[c.Field.ToLower()];
                    string type = _FieldType[c.Field.ToLower()];

                    if (type == "T")
                    {
                        if (c.Operator.ToLower() == "startswith")
                        {
                            filter.Add($"{c.Operator.ToLower()}({field},'{c.Value}')");
                        }
                        else
                        {
                            filter.Add($"{field} {c.Operator.ToLower()} '{c.Value}'");
                        }
                    }
                    else if (type == "N")
                    {
                        filter.Add($"{field} {c.Operator.ToLower()} {c.Value}");
                    }
                }
            }


            Varsis.Data.Infrastructure.Pagination page = new Varsis.Data.Infrastructure.Pagination();
            string query = Global.MakeODataQuery("U_VSITENTIDADE/$count", null, filter.Count == 0 ? null : filter.ToArray(), null, 1, 0);
            string data = await _serviceLayerConnector.getQueryResult(query);
            page.Linhas = Convert.ToInt64(data);
            page.Paginas = (Convert.ToInt64(data) / size.Value) + 1;
            page.qtdPorPagina = size.Value == 0 ? Convert.ToInt64(data) : size.Value;
            return page;
        }
        public Task Delete(ProductVendor entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task<ProductVendor> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"U_VSITPRODUCT_VENDORS('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            ProductVendor product = toRecord(record);

          
            return product;
        }

        private ProductVendor toJson(dynamic record)
        {
            ProductVendor vendorItem = new ProductVendor();

            vendorItem.RecId = Guid.Parse(record.Code);
            vendorItem.cod_item = record.U_COD_ITEM;
            vendorItem.cod_forn = record.U_COD_FORN;
            vendorItem.cod_forn_alt = record.U_COD_FORN_ALT;
            vendorItem.cod_item_alt = record.U_COD_ITEM_ALT;
            vendorItem.dig_item = record.U_DIG_ITEM;
            vendorItem.dig_forn = record.U_DIG_FORN;
            vendorItem.referencia = record.U_REFERENCIA;
            vendorItem.descricao = record.U_DESCRICAO;
            vendorItem.fatur_unid = record.U_FATUR_UNID;
            vendorItem.uf_unid = record.U_UF_UNID;
            vendorItem.uf_fator = record.U_UF_FATOR;
            vendorItem.uf_fator_conv = record.U_UF_FATOR_CONV;
            vendorItem.emb_xml = record.U_EMB_XML;
            vendorItem.lastupdate = DateTime.Now;

            return vendorItem;
        }

        async public Task Insert(ProductVendor entity)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();

           // entity.status = Data.Model.Integration.Product.ProductIntegrationStatus.Importing;
            string record = toJson(entity);
            batch.Post(HttpMethod.Post, "/U_VSITPRODUCT_VENDORS", record);

            ServiceLayerResponse response = await _serviceLayerConnector.Post(batch);

            //
            // Erro no protocolo http ou na estrutura do arquivo
            //
            if (!response.success)
            {
                string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }

            //
            // Verificar erros no lote
            //
            if (response.internalResponses.Count(m => m.success == false) == 0)
            {
                //
                // O registro só será alterado se não houver erros
                //
               // entity.status = Product.ProductIntegrationStatus.Created;
                record = toJson(entity);
                response = await _serviceLayerConnector.Patch($"U_VSITPRODUCT('{entity.RecId}')", record, true);

                if (!response.success)
                {
                    string message = $"Erro ao inserir dados em '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";

                    Console.WriteLine(message);
                    throw new ApplicationException(message);
                }
            }
            else
            {
                // Erro no cabeçalho do produto
                if (response.internalResponses.Count == 1)
                {
                    string message = $"Erro ao inserir dados em '{entity.EntityName}': {response.internalResponses[0].errorCode}-{response.internalResponses[0].errorMessage}";
                    Console.WriteLine(message);
                    throw new ApplicationException(message);
                }
                else
                {
                    int position = response.internalResponses.Count - 1;
                    //int item = position - 1;

                    //string message = $"Erro ao inserir dados em '{entity.items[0].EntityName}' item {entity.items[item].itemId} : {response.internalResponses[position].errorCode}-{response.internalResponses[position].errorMessage}";
                    //Console.WriteLine(message);
                    throw new ApplicationException("Vazio");
                }
            }
        }

        public Task Insert(List<ProductVendor> entities)
        {
            throw new NotImplementedException();
        }

        async public Task<List<ProductVendor>> List(List<Criteria> criterias, long page, long size)
        {
            List<string> filter = new List<string>();

            int cont = 0;
            if (criterias?.Count != 0)
            {
                foreach (var c in criterias)
                {
                    cont++;
                    string field = _FieldMap[c.Field.ToLower()];
                    string type = _FieldType[c.Field.ToLower()];

                    if (type == "T")
                    {
                        filter.Add($"{field} {c.Operator.ToLower()} '{c.Value}'");
                    }
                    else if (type == "N")
                    {
                        filter.Add($"{field} {c.Operator.ToLower()} {c.Value}");
                    }
                }
            }

            string query = Global.MakeODataQuery("U_VSITPRODUCT_VENDORS", null, filter.Count == 0 ? null : filter.ToArray(), null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<ProductVendor> result = new List<ProductVendor>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        public Task Update(ProductVendor entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<ProductVendor> entities)
        {
            throw new NotImplementedException();
        }

        private List<TableIndexes> createIndexesItem()
        {
            List<TableIndexes> lista = new List<TableIndexes>();

            lista.Add(new TableIndexes()
            {
                name = "VENDORS_COD_ITEM_COD_FORN",
                isUnique = true,
                keys = new string[] { "cod_item", "cod_forn" }
            });

            return lista;
        }

        async private Task<bool> createTable()
        {
            bool result = false;

            Table table = new Table(_serviceLayerConnector);

            table.name = "VSITPRODUCT_VENDORS";
            table.description = "Vendors";
            table.tableType = "bott_NoObject";
            table.columns = createColumns();
            table.indexes = createIndexes();

            try
            {
                await table.create();
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }



        private List<TableIndexes> createIndexes()
        {
            List<TableIndexes> lista = new List<TableIndexes>();

            lista.Add(new TableIndexes()
            {
                name = "VENDORS_COD_ITEM_COD_FORN",
                isUnique = true,
                keys = new string[] { "cod_item", "cod_forn" }
            });

            return lista;
        }
 
        private List<TableColumn> createColumns()
        {
            List<TableColumn> lista = new List<TableColumn>();

            lista.Add(new TableColumn() { name = "COD_ITEM", description = "COD_ITEM", mandatory = true, size = 7, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COD_FORN", description = "COD_FORN", mandatory = true, size = 7, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COD_FORN_ALT", description = "COD_FORN_ALT", mandatory = true, size = 7, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COD_ITEM_ALT", description = "COD_ITEM_ALT", mandatory = true, size = 7, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DIG_ITEM", description = "DIG_ITEM", mandatory = false, size = 1, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DIG_FORN", description = "DIG_FORN", mandatory = false, size = 1, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "REFERENCIA", description = "REFERENCIA", mandatory = false, size = 25, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "DESCRICAO", description = "DESCRICAO", mandatory = false, size = 60, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "FATUR_UNID", description = "FATUR_UNID", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "UF_UNID", description = "UF_UNID", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "UF_FATOR", description = "UF_FATOR", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "UF_FATOR_CONV", description = "UF_FATOR_CONV", mandatory = false, size = 14, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "EMB_XML", description = "EMB_XML", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "LASTUPDATE", description = "LASTUPDATE", mandatory = false, dataType = "db_Date" });

            return lista;
        }

      

        private string toJson(ProductVendor vendor)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.Code = vendor.RecId.ToString();
            record.Name = vendor.RecId.ToString();
            record.U_COD_ITEM = vendor.cod_item;
            record.U_COD_FORN = vendor.cod_forn;
            record.U_COD_FORN_ALT = vendor.cod_forn_alt;
            record.U_COD_ITEM_ALT = vendor.cod_item_alt;
            record.U_DIG_ITEM = vendor.dig_item;
            record.U_DIG_FORN = vendor.dig_forn;
            record.U_REFERENCIA = vendor.referencia;
            record.U_DESCRICAO = vendor.descricao;
            record.U_FATUR_UNID = vendor.fatur_unid;
            record.U_UF_UNID = vendor.uf_unid;
            record.U_UF_FATOR = vendor.uf_fator;
            record.U_UF_FATOR_CONV = vendor.uf_fator_conv;
            record.U_EMB_XML = vendor.emb_xml;
            record.U_LASTUPDATE = DateTime.Now;




            result = JsonConvert.SerializeObject(record);

            return result;
        }

 
        public ProductVendor toRecord(dynamic record)
        {
            ProductVendor productVendor = new ProductVendor();
            productVendor.RecId = Guid.Parse(record.Code);
            productVendor.cod_item = record.U_COD_ITEM;
            productVendor.cod_forn = record.U_COD_FORN;
            productVendor.cod_forn_alt = record.U_COD_FORN_ALT;
            productVendor.cod_item_alt = record.U_COD_ITEM_ALT;
            productVendor.dig_item = record.U_DIG_ITEM;
            productVendor.dig_forn = record.U_DIG_FORN;
            productVendor.referencia = record.U_REFERENCIA;
            productVendor.descricao = record.U_DESCRICAO;
            productVendor.fatur_unid = record.U_FATUR_UNID;
            productVendor.uf_unid = record.U_UF_UNID;
            productVendor.uf_fator = record.U_UF_FATOR;
            productVendor.uf_fator_conv = record.U_UF_FATOR_CONV;
            productVendor.emb_xml = record.U_EMB_XML;
            productVendor.lastupdate = parseDate(record.U_LASTUPDATE);

            return productVendor;

        }

        private DateTime parseDate(dynamic value)
        {
            DateTime result;

            DateTime.TryParse(value, out result);

            return result;
        }

        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("cod_item", "U_COD_ITEM");
            map.Add("cod_forn", "U_COD_FOR");
            map.Add("cod_forn_alt", "U_COD_FORN_ALT");
            map.Add("cod_item_alt", "U_ITEM_ALT");
            map.Add("dig_item", "U_DIG_ITEM");
            map.Add("dig_forn", "U_DIG_FORN");
            map.Add("referencia", "U_REFERENCIA");
            map.Add("descricao", "U_DESCRICAO");
            map.Add("uf_unid", "U_UNID");
            map.Add("uf_fator", "U_FATOR");
            map.Add("uf_fator_conv", "U_FATOR_CONV");
            map.Add("emb_xml", "U_EMB_XML");
            map.Add("fatur_unid", "U_FATUR_UNID");
            map.Add("lastupdate", "U_LASTUPDATE");
        
            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("cod_item", "N");
            map.Add("cod_forn", "N");
            map.Add("cod_forn_alt", "N");
            map.Add("cod_item_alt", "N");
            map.Add("dig_item", "N");
            map.Add("dig_forn", "N");
            map.Add("referencia", "T");
            map.Add("descricao", "T");
            map.Add("uf_unid", "N");
            map.Add("uf_fator", "T");
            map.Add("uf_fator_conv", "N");
            map.Add("emb_xml", "T");
            map.Add("fatur_unid", "T");
            map.Add("lastupdate", "T");

            return map;
        }
    }
}
