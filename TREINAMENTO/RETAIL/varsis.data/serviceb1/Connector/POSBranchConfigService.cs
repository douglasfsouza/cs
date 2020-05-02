using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model;
using System.Linq;

namespace Varsis.Data.Serviceb1.Connector
{
    public class POSBranchConfigService : IEntityService<Model.Connector.POSBranchConfig>
    {
        const string SL_TABLE_NAME = "U_VSPOSBRANCHCFG";

        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public POSBranchConfigService(ServiceLayerConnector serviceLayerConnector)
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

        async public Task Delete(Model.Connector.POSBranchConfig entity)
        {
            ServiceLayerResponse response = await _serviceLayerConnector.Delete($"{SL_TABLE_NAME}('{entity.RecId}')", "");

            if (!response.success)
            {
                string message = $"Erro ao excluir transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async public Task Delete(List<Criteria> criterias)
        {
            List<Model.Connector.POSBranchConfig> entities = await this.List(criterias, 0, 0);

            IBatchProducer batch = _serviceLayerConnector.CreateBatch();

            entities.ForEach(e =>
            {
                batch.Post(HttpMethod.Delete, $"/{SL_TABLE_NAME}('{e.RecId}')", "");
            });

            ServiceLayerResponse response = await _serviceLayerConnector.Post(batch);

            if (!response.success)
            {
                string message = $"Erro ao excluir transações de '{entities[0].EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
            else if (response.internalResponses.Count(m => m.success == false) != 0)
            {
                var error = response.internalResponses[response.internalResponses.Count() - 1];
                string message = $"Erro ao excluir transações de '{entities[0].EntityName}': {error.errorCode}-{error.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async public Task<Model.Connector.POSBranchConfig> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"{SL_TABLE_NAME}('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            Model.Connector.POSBranchConfig config = null;

            if (record != null)
            {
                config = toRecord(record);
            }

            return config;
        }

        async public Task Insert(Model.Connector.POSBranchConfig entity)
        {
            string record = toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Post(SL_TABLE_NAME, record);

            if (!response.success)
            {
                string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async public Task Insert(List<Model.Connector.POSBranchConfig> entities)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();

            entities.ForEach(e =>
            {
                string record = toJson(e);
                batch.Post(HttpMethod.Post, $"/{SL_TABLE_NAME}", record);
            });

            ServiceLayerResponse response = await _serviceLayerConnector.Post(batch);

            if (!response.success)
            {
                string message = $"Erro ao enviar transação de '{entities[0].EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
            else if (response.internalResponses.Count(m => m.success == false) != 0)
            {
                var error = response.internalResponses[response.internalResponses.Count() - 1];
                string message = $"Erro ao enviar transação de '{entities[0].EntityName}': {error.errorCode}-{error.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }
        async public Task<Varsis.Data.Infrastructure.Pagination> TotalLinhas(long? size, List<Criteria> criterias)
        {
            return new Varsis.Data.Infrastructure.Pagination();
        }
        async public Task<List<Model.Connector.POSBranchConfig>> List(List<Criteria> criterias, long page, long size)
        {
            var filter = parseCriterias(criterias);

            string query = Global.MakeODataQuery(SL_TABLE_NAME, null, filter.Length == 0 ? null : filter, null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<Model.Connector.POSBranchConfig> result = new List<Model.Connector.POSBranchConfig>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        async public Task Update(Model.Connector.POSBranchConfig entity)
        {
            string record = toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Put($"{SL_TABLE_NAME}('{entity.RecId}')", record);

            if (!response.success)
            {
                string message = $"Erro ao atualizar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async public Task Update(List<Model.Connector.POSBranchConfig> entities)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();

            entities.ForEach(e =>
            {
                string record = toJson(e);
                batch.Post(HttpMethod.Put, $"/{SL_TABLE_NAME}('{e.RecId}')", record);
            });

            ServiceLayerResponse response = await _serviceLayerConnector.Post(batch);

            if (!response.success)
            {
                string message = $"Erro ao atualizar transação de '{entities[0].EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
            else if (response.internalResponses.Count(m => m.success == false) != 0)
            {
                var error = response.internalResponses[response.internalResponses.Count() - 1];
                string message = $"Erro ao atualizar transação de '{entities[0].EntityName}': {error.errorCode}-{error.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }


        async private Task<List<dynamic>> listJoinBusinesPlaces()
        {
            string[] queryArgs = new string[]
            {
                "$crossjoin(U_VSPOSBRANCHCFG,BusinessPlaces)?$expand=",
                "U_VSPOSBRANCHCFG($select=Code),",
                "BusinessPlaces($select=BPLName),",
                "&$filter=BusinessPlaces/BPLID eq U_VSPOSBRANCHCFG/U_BPLID"
            };

            string query = String.Join("", queryArgs);

            string data = await _serviceLayerConnector.getQueryResult(query);
            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<dynamic> result = new List<dynamic>();

            foreach (dynamic i in lista)
            {
                dynamic newItem = new ExpandoObject();
                newItem.Code = i.U_VSPOSBRANCHCFG.Code;
                newItem.Name = i.BusinessPlaces.BPLName;
                result.Add(newItem);
            }

            return result;
        }
        async private Task<List<dynamic>> listJoinBusinesPartners()
        {
            string[] queryArgs = new string[]
            {
                "$crossjoin(U_VSPOSBRANCHCFG,BusinessPartners)?$expand=",
                "U_VSPOSBRANCHCFG($select=Code),",
                "BusinessPartners($select=CardName)",
                "&$filter=BusinessPartners/CardCode eq U_VSPOSBRANCHCFG/U_CARDCODE"
            };

            string query = String.Join("", queryArgs);

            string data = await _serviceLayerConnector.getQueryResult(query);
            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<dynamic> result = new List<dynamic>();

            foreach (dynamic i in lista)
            {
                dynamic newItem = new ExpandoObject();
                newItem.Code = i.U_VSPOSBRANCHCFG.Code;
                newItem.Name = i.BusinessPartners.CardName;
                result.Add(newItem);
            }

            return result;
        }

        async public Task<List<Model.Connector.POSBranchConfigSummary>> ListSummary(List<Criteria> criterias, long page = -1, long size = -1)
        {
            List<Model.Connector.POSBranchConfigSummary> result = null;

            string data = await _serviceLayerConnector.getQueryResult(SL_TABLE_NAME);
            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            result = new List<Model.Connector.POSBranchConfigSummary>();

            foreach (dynamic l in lista)
            {
                Model.Connector.POSBranchConfigSummary item = new Model.Connector.POSBranchConfigSummary();

                item.RecId = l.Code;
                item.BranchId = Convert.ToInt64(l.U_BPLID);
                item.BranchIdLegacy = l.U_BPLIDLGCY;
                item.DefaultCustomer = l.U_CARDCODE;
                item.UsageCode = l.U_USAGE;

                if (l.U_OPENINGDATE != null)
                {
                    item.OpeningDate = parseTime(l.U_OPENINGDATE);
                }
                result.Add(item);
            }

            var places = await listJoinBusinesPlaces();
            var partners = await listJoinBusinesPartners();

            places.ForEach(p =>
            {
                var record = result.Find(m => m.RecId == p.Code);
                record.BranchName = p.Name;
            });

            partners.ForEach(p =>
            {
                var record = result.Find(m => m.RecId == p.Code);
                record.DefaultCustomerName = p.Name;
            });

            return result;
        }

        async private Task<bool> createTable()
        {
            bool result = false;

            Table table = new Table(_serviceLayerConnector);

            table.name = "VSPOSBRANCHCFG";
            table.description = "Configuração de loja - POS";
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

        private List<TableColumn> createColumns()
        {
            List<TableColumn> lista = new List<TableColumn>();

            lista.Add(new TableColumn()
            {
                name = "BPLID",
                dataType = "db_Numeric",
                size = 7,
                mandatory = true,
                description = "Código da filial"
            });

            lista.Add(new TableColumn()
            {
                name = "BPLIDLGCY",
                dataType = "db_Alpha",
                size = 10,
                mandatory = true,
                description = "Código da filial - legado"
            });

            lista.Add(new TableColumn()
            {
                name = "CARDCODE",
                dataType = "db_Alpha",
                size = 20,
                mandatory = true,
                description = "Cliente padrão"
            });

            lista.Add(new TableColumn()
            {
                name = "USAGE",
                dataType = "db_Numeric",
                size = 10,
                mandatory = true,
                description = "Utilização"
            });
            lista.Add(new TableColumn()
            {
                name = "OPENINGDATE",
                dataType = "db_Date",
                size = 7,
                mandatory = false,
                description = "Data Importação"
            });

            lista.Add(new TableColumn()
            {
                name = "CASHACCOUNT",
                dataType = "db_Alpha",
                size = 20,
                mandatory = false,
                description = "Conta em Dinheiro"
            });
            lista.Add(new TableColumn()
            {
                name = "CREDITCARDID",
                dataType = "db_Numeric",
                size = 7,
                mandatory = false,
                description = "Id Cartão de Crédito"
            });
            lista.Add(new TableColumn()
            {
                name = "DEBITCARDID",
                dataType = "db_Numeric",
                size = 7,
                mandatory = false,
                description = "Id Cartão de Débito"
            });

            return lista;
        }

        private List<TableIndexes> createIndexes()
        {
            List<TableIndexes> lista = new List<TableIndexes>();

            lista.Add(new TableIndexes()
            {
                name = "PK",
                isUnique = true,
                keys = new string[] { "BPLID" }
            });

            lista.Add(new TableIndexes()
            {
                name = "LEGACY",
                isUnique = false,
                keys = new string[] { "BPLIDLGCY" }
            });

            return lista;
        }

        private string toJson(Model.Connector.POSBranchConfig config)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.Code = config.RecId;
            record.Name = config.RecId;
            record.U_BPLID = config.BranchId;
            record.U_BPLIDLGCY = config.BranchIdLegacy;
            record.U_CARDCODE = config.DefaultCustomer;
            record.U_USAGE = config.UsageCode;
            record.U_OPENINGDATE = config.OpeningDate.ToString("yyyy-MM-dd");
            record.U_CASHACCOUNT = config.Code; 
            record.U_CREDITCARDID = config.CreditCardId;
            record.U_DEBITCARDID = config.DebitCardId;
            result = JsonConvert.SerializeObject(record);

            return result;
        }

        private Model.Connector.POSBranchConfig toRecord(dynamic record)
        {
            Model.Connector.POSBranchConfig config = new Model.Connector.POSBranchConfig();

            config.RecId = Guid.Parse(record.Code);
            config.BranchId = Convert.ToInt64(record.U_BPLID);
            config.BranchIdLegacy = record.U_BPLIDLGCY;
            config.DefaultCustomer = record.U_CARDCODE;
            config.UsageCode = Convert.ToInt64(record.U_USAGE);
            config.OpeningDate = parseTime(record.U_OPENINGDATE);
            config.Code = (string)record.U_CASHACCOUNT == null ? "0" : record.U_CASHACCOUNT;
            config.CreditCardId = record.U_CREDITCARDID == null ? 0 : record.U_CREDITCARDID;
            config.DebitCardId = record.U_DEBITCARDID == null ? 0 : record.U_DEBITCARDID;

            return config;
        }
        private DateTime parseTime(dynamic value)
        {
            DateTime result;

            DateTime.TryParse(value, out result);

            return result;
        }


        private string[] parseCriterias(List<Criteria> criterias)
        {
            List<string> filter = new List<string>();
            if (criterias != null)
            {
                if (criterias?.Count != 0)
                {
                    foreach (var c in criterias)
                    {
                        string type = string.Empty;
                        string field = string.Empty;

                        if (_FieldMap.ContainsKey(c.Field.ToLower()))
                        {
                            field = _FieldMap[c.Field.ToLower()];
                            type = _FieldType[c.Field.ToLower()];
                        }
                        else
                        {
                            field = c.Field;
                            type = "T";
                        }

                        string value = string.Empty;

                        if (type == "T")
                        {
                            value = $"'{c.Value}'";
                        }
                        else if (type == "N")
                        {
                            value = $"{c.Value}";
                        }

                        switch (c.Operator.ToLower())
                        {
                            case "startswith":
                                filter.Add($"startswith({field},{value})");
                                break;

                            default:
                                filter.Add($"{field} {c.Operator.ToLower()} {value}");
                                break;
                        }
                    }
                }
            }

            return filter.ToArray();
        }

        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "Code");
            map.Add("branchid", "U_BPLID");
            map.Add("branchidlegacy", "U_BPLIDLGCY");

            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "T");
            map.Add("branchid", "N");
            map.Add("branchidlegacy", "T");

            return map;
        }
    }
}
