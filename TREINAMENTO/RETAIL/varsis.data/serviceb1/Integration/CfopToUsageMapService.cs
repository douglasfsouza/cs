using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model.Integration;
using System.Linq;
using System.Dynamic;
using Newtonsoft.Json;

namespace Varsis.Data.Serviceb1.Integration
{
    public class CfopToUsageMapService : IEntityService<Model.Integration.CfopToUsageMap>
    {
        const string SL_TABLE_NAME = "U_VSITCFOPTOUSAGEMAP";

        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public CfopToUsageMapService(ServiceLayerConnector serviceLayerConnector)
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

        async public Task Delete(CfopToUsageMap entity)
        {
            ServiceLayerResponse response = await _serviceLayerConnector.Delete($"{SL_TABLE_NAME}('{entity.RecId}')", "");

            if (!response.success)
            {
                string message = $"Erro ao excluir registro de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async public Task Delete(List<Criteria> criterias)
        {
            List<CfopToUsageMap> entities = await this.List(criterias, 0, 0);

            IBatchProducer batch = _serviceLayerConnector.CreateBatch();

            entities.ForEach(e =>
            {
                batch.Post(HttpMethod.Delete, $"/{SL_TABLE_NAME}('{e.RecId}')", "");
            });

            ServiceLayerResponse response = await _serviceLayerConnector.Post(batch);

            if (!response.success)
            {
                string message = $"Erro ao excluir registros de '{entities[0].EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
            else if (response.internalResponses.Count(m => m.success == false) != 0)
            {
                var error = response.internalResponses[response.internalResponses.Count() - 1];
                string message = $"Erro ao excluir registros de '{entities[0].EntityName}': {error.errorCode}-{error.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async public Task<CfopToUsageMap> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"{SL_TABLE_NAME}('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            CfopToUsageMap result = null;

            if (record != null)
            {
                result = toRecord(record);
            }

            return result;
        }

        async public Task Insert(CfopToUsageMap entity)
        {
            string record = toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Post(SL_TABLE_NAME, record);

            if (!response.success)
            {
                string message = $"Erro ao enviar registro de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async public Task Insert(List<CfopToUsageMap> entities)
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
                string message = $"Erro ao enviar registros de '{entities[0].EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
            else if (response.internalResponses.Count(m => m.success == false) != 0)
            {
                var error = response.internalResponses[response.internalResponses.Count() - 1];
                string message = $"Erro ao enviar registros de '{entities[0].EntityName}': {error.errorCode}-{error.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async public Task<List<CfopToUsageMap>> List(List<Criteria> criterias, long page = -1, long size = -1)
        {
            var filter = Global.parseCriterias(criterias, _FieldMap, _FieldType);

            string query = Global.MakeODataQuery(SL_TABLE_NAME, null, filter.Length == 0 ? null : filter, null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<CfopToUsageMap> result = new List<CfopToUsageMap>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }
            
            return result;
        }

        async private Task<List<dynamic>> listJoinLegacyUsage(string[] filter)
        {
            for (int i = 0; i < filter.Length; i++)
            {
                filter[i] = $"U_VSITCFOPTOUSAGEMAP/{filter[i]}";
            }

            string[] queryArgs = new string[]
            {
                "$crossjoin(U_VSITCFOPTOUSAGEMAP,U_VSCATLGCYUSAGE)?$expand=",
                "U_VSITCFOPTOUSAGEMAP($select=Code),",
                "U_VSCATLGCYUSAGE($select=Name),",
                "&$filter=U_VSCATLGCYUSAGE/Code eq U_VSITCFOPTOUSAGEMAP/U_LGCYUSAGE",
                filter.Length == 0 ? string.Empty : " and ",
                string.Join(" and ", filter)
            };

            string query = String.Join("", queryArgs);

            string data = await _serviceLayerConnector.getQueryResult(query);
            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<dynamic> result = new List<dynamic>();

            lista?.ForEach((l =>
            {
                var i = (dynamic)l;
                dynamic newItem = new ExpandoObject();
                newItem.Code = i.U_VSITCFOPTOUSAGEMAP.Code;
                newItem.Name = i.U_VSCATLGCYUSAGE.Name;
                result.Add(newItem);

            }));

            return result;
        }
        async private Task<List<dynamic>> listJoinServiceItem(string[] filter)
        {
            for (int i = 0; i < filter.Length; i++)
            {
                filter[i] = $"U_VSITCFOPTOUSAGEMAP/{filter[i]}";
            }

            string[] queryArgs = new string[]
            {
                "$crossjoin(U_VSITCFOPTOUSAGEMAP,Items)?$expand=",
                "U_VSITCFOPTOUSAGEMAP($select=Code),",
                "Items($select=ItemName),",
                "&$filter=Items/ItemCode eq U_VSITCFOPTOUSAGEMAP/U_SERVICEITEM",
                filter.Length == 0 ? string.Empty : string.Join(" and ", filter)
            };

            string query = String.Join("", queryArgs);

            string data = await _serviceLayerConnector.getQueryResult(query);
            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<dynamic> result = new List<dynamic>();

            lista?.ForEach((l =>
            {
                var i = (dynamic)l;
                dynamic newItem = new ExpandoObject();
                newItem.Code = i.U_VSITCFOPTOUSAGEMAP.Code;
                newItem.Name = i.Items.ItemName;
                result.Add(newItem);
            }));

            return result;
        }

        async public Task<List<CfopToUsageMapSummary>> ListSummary(List<Criteria> criterias, long page = -1, long size = -1)
        {
            List<CfopToUsageMapSummary> result = null;

            var filter = Global.parseCriterias(criterias, _FieldMap, _FieldType);

            string query = Global.MakeODataQuery(SL_TABLE_NAME, null, filter.Length == 0 ? null : filter, null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            result = new List<CfopToUsageMapSummary>();

            foreach (dynamic l in lista)
            {
                var item = new CfopToUsageMapSummary()
                {
                    recId = l.Code,
                    cfop = Convert.ToInt64(l.U_CFOP),
                    usageLegacy = l.U_LGCYUSAGE,
                    usage = Convert.ToInt64(l.U_USAGE),
                    taxCode = l.U_TAXCODE,
                    documentType = Convert.ToInt64(l.U_DOCTYPE),
                    warehouse = Convert.ToInt64(l.U_WAREHOUSE),
                    serviceItem = l.U_SERVICEITEM
                };

                try
                {
                    item.taxCode = l.U_TAXCODE;
                }
                catch
                {
                }

                result.Add(item);
            }

            var usages = await listJoinLegacyUsage(filter);
            var serviceItems = await listJoinServiceItem(filter);

            usages?.ForEach(p =>
            {
                var record = result.Find(m => m.recId == p.Code);
                record.usageLegacyName = p.Name;
            });

            serviceItems?.ForEach(p =>
            {
                var record = result.Find(m => m.recId == p.Code);
                record.serviceItemName = p.Name;
            });

            return result;
        }

        public Task<Pagination> TotalLinhas(long? size, List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task Update(CfopToUsageMap entity)
        {
            string record = toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Put($"{SL_TABLE_NAME}('{entity.RecId}')", record);

            if (!response.success)
            {
                string message = $"Erro ao atualizar registro de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async public Task Update(List<CfopToUsageMap> entities)
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
                string message = $"Erro ao atualizar registro de '{entities[0].EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
            else if (response.internalResponses.Count(m => m.success == false) != 0)
            {
                var error = response.internalResponses[response.internalResponses.Count() - 1];
                string message = $"Erro ao atualizar registro de '{entities[0].EntityName}': {error.errorCode}-{error.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async private Task<bool> createTable()
        {
            bool result = false;

            Table table = new Table(_serviceLayerConnector);

            table.name = "VSITCFOPTOUSAGEMAP";
            table.description = "Agendas x Utilização";
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

            lista.Add(new TableColumn() { name = "LGCYUSAGE", description = "Utilização (legado)", mandatory = true, size = 10, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "CFOP", description = "CFOP", mandatory = true, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "USAGE", description = "Utilização", mandatory = true, size = 11, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TAXCODE", description = "Código de imposto", mandatory = false, size = 20, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "DOCTYPE", description = "Tipo de documento", mandatory = true, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SERVICEITEM", description = "Item de serviço", mandatory = false, size = 10, dataType = "db_Alpha" });

            lista.Add(new TableColumn() { name = "CONTAPN", description = "CONTAPN", mandatory = false, size = 10, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "CONTADEBITO", description = "CONTADEBITOo", mandatory = false, size = 100, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "CONTACREDITO", description = "CONTACREDITO", mandatory = false, size = 100, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "CONTACONTROLE", description = "CONTACONTROLE", mandatory = false, size = 100, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "CONTATAXA", description = "CONTATAXA", mandatory = false, size = 100, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "OBSERVACOES", description = "OBSERVACOES", mandatory = false, size = 100, dataType = "db_Alpha" });

            return lista;
        }

        private List<TableIndexes> createIndexes()
        {
            List<TableIndexes> lista = new List<TableIndexes>();

            lista.Add(new TableIndexes()
            {
                name = "PK",
                isUnique = true,
                keys = new string[] { "LGCYUSAGE", "CFOP"}
            });

            return lista;
        }

        private string toJson(CfopToUsageMap map)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.Code = map.RecId;
            record.Name = map.RecId;
            record.U_CFOP = map.Cfop;
            record.U_LGCYUSAGE = map.UsageLegacy;
            record.U_USAGE = map.Usage;
            record.U_TAXCODE = map.TaxCode;
            record.U_DOCTYPE = map.DocumentType;
            record.U_SERVICEITEM = map.ServiceItem;
            record.U_WAREHOUSE = map.Warehouse;
            record.U_CONTAPN = map.ContaPN;
            record.U_CONTADEBITO = map.ContaDebito;
            record.U_CONTACREDITO = map.ContaCredito;
            record.U_CONTACONTROLE = map.ContaControle;
            record.U_CONTATAXA = map.ContaTaxa;
            record.U_OBSERVACOES = map.Observacoes;
            result = JsonConvert.SerializeObject(record);

            return result;
        }

        private CfopToUsageMap toRecord(dynamic record)
        {
            CfopToUsageMap map = new CfopToUsageMap();

            map.RecId = Guid.Parse(record.Code);
            map.Cfop = Convert.ToInt64(record.U_CFOP);
            map.UsageLegacy = record.U_LGCYUSAGE;
            map.Usage = Convert.ToInt64(record.U_USAGE);
            map.TaxCode = record.U_TAXCODE;
            map.DocumentType = (CfopToUsageMap.DocumentTypeEnum)Convert.ToInt64(record.U_DOCTYPE);
            map.ServiceItem = record.U_SERVICEITEM;
            map.Warehouse = Convert.ToInt64(record.U_WAREHOUSE);

            map.ContaPN = record.U_CONTAPN;
            map.ContaDebito = record.U_CONTADEBITO;
            map.ContaCredito = record.U_CONTACREDITO;
            map.ContaControle = record.U_CONTACONTROLE;
            map.ContaTaxa = record.U_CONTATAXA;
            map.Observacoes = record.U_OBSERVACOES;
            return map;
        }

        private string[] parseCriterias(List<Criteria> criterias)
        {
            List<string> filter = new List<string>();
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

            return filter.ToArray();
        }

        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "Code");
            map.Add("cfop", "U_CFOP");
            map.Add("usagelegacy", "U_LGCYUSAGE");
            map.Add("usage", "U_USAGE");
            map.Add("taxcode", "U_TAXCODE");
            map.Add("documenttype", "U_DOCTYPE");
            map.Add("warehouse", "U_WAREHOUSE");
            map.Add("serviceitem", "U_SERVICEITEM");

            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "T");
            map.Add("cfop", "N");
            map.Add("usagelegacy", "T");
            map.Add("usage", "N");
            map.Add("taxcode", "T");
            map.Add("documenttype", "N");
            map.Add("warehouse", "N");
            map.Add("serviceitem", "T");

            return map;
        }

    }
}
