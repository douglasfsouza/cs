using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model;

namespace Varsis.Data.Serviceb1
{
    public class ProductService : IEntityService<Model.Product>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public ProductService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = this.mountFieldMap();
            _FieldType = this.mountFieldType();
        }
        async public Task<bool> Create()
        {
            bool result = false;

            Table table = new Table(_serviceLayerConnector);

            table.name = "VSIS_PRODUCT";
            table.description = "Produtos (varsis)";
            table.tableType = "bott_NoObject";

            table.columns = new List<TableColumn>();

            table.columns.Add(new TableColumn()
            {
                name = "VSIS_ID",
                description = "identificação",
                mandatory = true,
                size = 40,
                dataType = "db_Alpha"
            });


            table.columns.Add(new TableColumn()
            {
                name = "VSIS_FULLNAME",
                description = "Descrição do produto",
                mandatory = true,
                size = 100,
                dataType = "db_Alpha"
            });

            table.columns.Add(new TableColumn()
            {
                name = "VSIS_SHORTNAME",
                description = "Descrição curta do produto",
                size = 60,
                dataType = "db_Alpha"
            });

            table.columns.Add(new TableColumn()
            {
                name = "VSIS_PRICE",
                description = "Preço unitário",
                size = 15,
                dataType = "db_Float",
                dataTypeSub = "st_Price"
            });

            table.columns.Add(new TableColumn()
            {
                name = "VSIS_QUANTITY",
                description = "Quantidade",
                dataType = "db_Float",
                dataTypeSub = "st_Quantity"

            });

            table.columns.Add(new TableColumn()
            {
                name = "VSIS_PRICELASTUPD",
                description = "Última atualização do preço",
                dataType = "db_Date"
            });

            table.indexes = new List<TableIndexes>();
            table.indexes.Add(new TableIndexes()
            {
                name = "PK",
                isUnique = true,
                keys = new string[] { "VSIS_ID" }
            });

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

        async public Task Delete(Product entity)
        {
            string record = toJson(entity);
            string query = Uri.EscapeUriString($"U_VSIS_PRODUCT('{entity.id}')");

            ServiceLayerResponse response = await _serviceLayerConnector.Delete(query, record);

            if (!response.success)
            {
                string message = $"Erro ao excluir o registro '{entity.id}' em '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";

                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task<Product> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"U_VSIS_PRODUCT('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            Product product = toProduct(record);

            return product;
        }

        async public Task Insert(Product entity)
        {
            string record = toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Post("U_VSIS_PRODUCT", record);

            if (!response.success)
            {
                string message = $"Erro ao inserir dados em '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";

                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async public Task Insert(List<Product> entities)
        {
            List<string> records = new List<string>();

            foreach (var e in entities)
            {
                records.Add(toJson(e));
            }

            ServiceLayerResponse response = await _serviceLayerConnector.PostMany("U_VSIS_PRODUCT", records);

            if (!response.success)
            {
                string message = $"Erro ao inserir dados em '{entities[0].EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
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
                        filter.Add($"{field} {c.Operator.ToLower()} '{c.Value}'");
                    }
                    else if (type == "N")
                    {
                        filter.Add($"{field} {c.Operator.ToLower()} {c.Value}");
                    }
                }
            }


            Varsis.Data.Infrastructure.Pagination page = new Varsis.Data.Infrastructure.Pagination();
            string query = Global.MakeODataQuery("U_VSITENTIDADECONT/$count", null, filter.Count == 0 ? null : filter.ToArray(), null, 1, 0);
            string data = await _serviceLayerConnector.getQueryResult(query);
            page.Linhas = Convert.ToInt64(data);
            page.Paginas = (Convert.ToInt64(data) / size.Value) + 1;
            page.qtdPorPagina = size.Value == 0 ? Convert.ToInt64(data) : size.Value;
            return page;
        }
        async public Task<List<Product>> List(List<Criteria> criterias, long page, long size)
        {
            List<string> filter = new List<string>();

            if (criterias?.Count != 0)
            {
                foreach (var c in criterias)
                {
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

            string query = Global.MakeODataQuery("U_VSIS_PRODUCT", null, filter.Count == 0 ? null : filter.ToArray(), null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<Product> result = new List<Product>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toProduct(o));
                }
            }

            return result;
        }

        async public Task Update(Product entity)
        {
            string record = toJson(entity);
            string query = Uri.EscapeUriString($"U_VSIS_PRODUCT('{entity.id}')");

            ServiceLayerResponse response = await _serviceLayerConnector.Patch(query, record);

            if (!response.success)
            {
                string message = $"Erro ao atualizar o registro '{entity.id}' em '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";

                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        public Task Update(List<Product> entities)
        {
            throw new NotImplementedException();
        }

        private string toJson(Product product)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.Code = product.id;
            record.Name = product.fullName;
            record.U_VSIS_ID = product.id;
            record.U_VSIS_FULLNAME = product.fullName;
            record.U_VSIS_SHORTNAME = product.shortName;
            record.U_VSIS_PRICE = product.price;
            record.U_VSIS_QUANTITY = product.quantity;
            record.U_VSIS_PRICELASTUPD = product.priceLastUpdate;

            result = JsonConvert.SerializeObject(record);

            return result;
        }

        private Product toProduct(dynamic record)
        {
            Product product = new Product();

            product.id = record.U_VSIS_ID;
            product.fullName = record.U_VSIS_FULLNAME;
            product.shortName = record.U_VSIS_SHORTNAME;
            product.price = record.U_VSIS_PRICE;
            product.quantity = (long)record.U_VSIS_QUANTITY;

            DateTime parseDate;

            DateTime.TryParse(record.U_VSIS_PRICELASTUPD, out parseDate);

            product.priceLastUpdate = parseDate;

            return product;
        }

        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("id", "U_VSIS_ID");
            map.Add("fullname", "U_VSIS_FULLNAME");
            map.Add("shortname", "U_VSIS_SHORTNAME");
            map.Add("price", "U_VSIS_PRICE");
            map.Add("pricelastupdate", "U_VSIS_PRICELASTUPD");
            map.Add("quantity", "U_VSIS_QUANTITY");

            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("id", "T");
            map.Add("fullname", "T");
            map.Add("shortname", "T");
            map.Add("price", "N");
            map.Add("pricelastupdate", "T");
            map.Add("quantity", "N");

            return map;
        }


    }
}
