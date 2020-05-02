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
using Varsis.Data.Model.Connector;

namespace Varsis.Data.Serviceb1.Connector
{
    public class POSMonitorService : IEntityService<Model.Connector.POSMonitor>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public POSMonitorService(ServiceLayerConnector serviceLayerConnector)
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

        async public Task Delete(Model.Connector.POSMonitor entity)
        {
            ServiceLayerResponse response = await _serviceLayerConnector.Delete($"U_VSPOSMONITOR('{entity.RecId}')", "");

            if (!response.success)
            {
                string message = $"Erro ao excluir transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async public Task Delete(List<Criteria> criterias)
        {
            List<Model.Connector.POSMonitor> entities = await this.List(criterias, 0, 0);

            IBatchProducer batch = _serviceLayerConnector.CreateBatch();

            entities.ForEach(e =>
            {
                batch.Post(HttpMethod.Delete, $"/U_VSPOSMONITOR('{e.RecId}')", "");
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

        async public Task<Model.Connector.POSMonitor> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"U_VSPOSMONITOR('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            Model.Connector.POSMonitor monitor = null;

            if (record != null)
            {
                monitor = toRecord(record);
            }

            return monitor;
        }

        async public Task Insert(Model.Connector.POSMonitor entity)
        {
            string record = toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Post("U_VSPOSMONITOR", record);

            if (!response.success)
            {
                string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async public Task Insert(List<Model.Connector.POSMonitor> entities)
        {
          
            foreach (var e in entities)
            {
                IBatchProducer batch = _serviceLayerConnector.CreateBatch();
                batch = _serviceLayerConnector.CreateBatch();
                string record = toJson(e);

                batch.Post(HttpMethod.Post, "/U_VSPOSMONITOR", record);

                ServiceLayerResponse response = await _serviceLayerConnector.Post(batch);

                if (response.internalResponses.Where(x => x.errorCode == "-2035").ToList().Count == 0)
                {
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
            }
        }
        async public Task<Varsis.Data.Infrastructure.Pagination> TotalLinhas(long? size, List<Criteria> criterias)
        {
            return new Varsis.Data.Infrastructure.Pagination();
        }
        async public Task<List<Model.Connector.POSMonitor>> List(List<Criteria> criterias, long page, long size)
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

            string query = Global.MakeODataQuery("U_VSPOSMONITOR", null, filter.Count == 0 ? null : filter.ToArray(), null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<Model.Connector.POSMonitor> result = new List<Model.Connector.POSMonitor>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        async public Task Update(Model.Connector.POSMonitor entity)
        {
            string record = toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Put($"U_VSPOSMONITOR('{entity.RecId}')", record);

            if (!response.success)
            {
                string message = $"Erro ao atualizar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async public Task Update(List<Model.Connector.POSMonitor> entities)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();

            entities.ForEach(e =>
            {
                string record = toJson(e);
                batch.Post(HttpMethod.Put, $"/U_VSPOSMONITOR('{e.RecId}')", record);
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

        async public Task InitMonitoring(InitMonitorArgs args)
        {

            DateTime startDate = args.startDate == null ? DateTime.Now : args.startDate.Value;

            DateTime curDate = startDate.Date;

            POSBranchConfigService pOSBranchConfigService = new POSBranchConfigService(_serviceLayerConnector);

            List<Criteria> criteria = new List<Criteria>();
            var posBranchConfig = await pOSBranchConfigService.ListSummary(criteria, 1, 0);

            List<Model.Connector.POSMonitor> lista = new List<Model.Connector.POSMonitor>();
            foreach (var b in posBranchConfig)
            {
                var date = posBranchConfig.Where(x => x.BranchId == b.BranchId && x.OpeningDate != null).ToList();
                TimeSpan diff = new TimeSpan();

                if (date.Count > 0)
                {
                    if (startDate > date.FirstOrDefault().OpeningDate.Value)
                        diff = startDate - date.FirstOrDefault().OpeningDate.Value;

                    for (int i = 0; i <= diff.Days; i++)
                    {
                        //add a diferenca
                        POSMonitor pOSMonitor = new POSMonitor();
                        pOSMonitor.BranchId = (int)b.BranchId; // devemos encontrar a filial correta a partir do branchIdLegacy
                        pOSMonitor.BranchIdLegacy = b.BranchIdLegacy;
                        pOSMonitor.TransactionDate = date.FirstOrDefault().OpeningDate.Value.AddDays(i);
                        pOSMonitor.Status = Model.Connector.IntegrationStatus.Pending;

                        lista.Add(pOSMonitor);
                    }
                }
            }

            if (lista.Count > 0)
            {
                await this.Insert(lista);
            }
        }

        async private Task<List<dynamic>> listJoinCountOfErrors(List<Criteria> criterias)
        {
            var filter = this.parseCriterias(criterias);
            string filterUrl = String.Join(" and ", filter);

            if (!string.IsNullOrEmpty(filterUrl))
            {
                filterUrl = filterUrl + " and ";
            }

            // Lista quantidade registros com erro
            string[] queryArgs = new string[]
            {
                "$crossjoin(U_VSPOSMONITOR,U_VSPOSMONITORDET)?",
                "$apply=",
                $"filter({filterUrl}U_VSPOSMONITORDET/U_MONITOR eq U_VSPOSMONITOR/Code ",
                "and U_VSPOSMONITORDET/U_STATUS eq 99)",
                "/groupby((U_VSPOSMONITOR/Code),",
                "aggregate(U_VSPOSMONITORDET($count as U_COUNT)))"
            };
            var query = string.Join("", queryArgs);
            query = Uri.EscapeUriString(query);

            var data = await _serviceLayerConnector.getQueryResult(query);
            var details = Global.parseQueryToCollection(data);
            List<dynamic> result = new List<dynamic>();
            details?.ForEach(d => result.Add(d));

            return result;
        }
        async private Task<List<dynamic>> listMonitorJoinBusinessPlaces(List<Criteria> criterias)
        {
            var filter = this.parseCriterias(criterias);
            string filterUrl = String.Join(" and ", filter);

            if (!string.IsNullOrEmpty(filterUrl))
            {
                filterUrl = filterUrl + " and ";
            }

            // Lista quantidade registros com erro
            string[] queryArgs = new string[]
            {
                "$crossjoin(U_VSPOSMONITOR,BusinessPlaces)?",
                "$expand=",
                "U_VSPOSMONITOR($select=Code,U_TRANSDATE,U_STATUS,U_BRANCHIDLGCY),",
                "BusinessPlaces($select=BPLID,BPLName)",
                $"&$filter={filterUrl}U_VSPOSMONITOR/U_BPLID eq BusinessPlaces/BPLID"
            };
            var query = string.Join("", queryArgs);
            query = Uri.EscapeUriString(query);

            var data = await _serviceLayerConnector.getQueryResult(query);
            var details = Global.parseQueryToCollection(data);
            List<dynamic> result = new List<dynamic>();
            details?.ForEach(d => result.Add(d));

            return result;
        }

        async private Task<List<dynamic>> listJoinDetailsTotals(List<Criteria> criterias)
        {
            var filter = this.parseCriterias(criterias);
            string filterUrl = String.Join(" and ", filter);

            if (!string.IsNullOrEmpty(filterUrl))
            {
                filterUrl = filterUrl + " and ";
            }

            // Lista quantidade registros com erro
            string[] queryArgs = new string[]
            {
                "$crossjoin(U_VSPOSMONITOR,U_VSPOSMONITORDET)?",
                "$apply=",
                $"filter({filterUrl}U_VSPOSMONITORDET/U_MONITOR eq U_VSPOSMONITOR/Code) ",
                "/groupby(",
                "(U_VSPOSMONITOR/Code),",
                "aggregate(",
                "U_VSPOSMONITORDET(U_TOTALAMT with sum as U_TOTALAMTSUM),",
                "U_VSPOSMONITORDET($count as U_DETAILSCOUNT)",
                "))"
            };
            var query = string.Join("", queryArgs);
            query = Uri.EscapeUriString(query);

            var data = await _serviceLayerConnector.getQueryResult(query);
            var details = Global.parseQueryToCollection(data);
            List<dynamic> result = new List<dynamic>();
            details?.ForEach(d => result.Add(d));

            return result;
        }

        async public Task<List<Model.Connector.POSMonitorSummary>> ListSummary(List<Criteria> criterias, long page = -1, long size = -1)
        {
            List<Model.Connector.POSMonitorSummary> result = null;

            List<dynamic> branchConfig = new List<dynamic>();

            var branchData = await _serviceLayerConnector.getQueryResult("U_VSPOSBRANCHCFG");
            var branchCollection = Global.parseQueryToCollection(branchData);
            branchCollection.ForEach(d => branchConfig.Add(d));

            var monitorList = await this.listMonitorJoinBusinessPlaces(criterias);
            var detailsTotals = await this.listJoinDetailsTotals(criterias);
            var countOfErrorsList = await this.listJoinCountOfErrors(criterias);

            result = new List<Model.Connector.POSMonitorSummary>();
            foreach (dynamic l in monitorList)
            {
                var branch = branchConfig.FirstOrDefault(m => m.U_BPLID == l.BusinessPlaces.BPLID);
                var detail = detailsTotals.FirstOrDefault(m => m.U_VSPOSMONITOR.Code == l.U_VSPOSMONITOR.Code);
                var errors = countOfErrorsList.FirstOrDefault(m => m.U_VSPOSMONITOR.Code == l.U_VSPOSMONITOR.Code);

                var item = new Model.Connector.POSMonitorSummary()
                {
                    RecId = l.U_VSPOSMONITOR.Code,
                    BranchId = Convert.ToInt64(l.BusinessPlaces.BPLID),
                    BranchName = l.BusinessPlaces.BPLName,
                    BranchIdLegacy = branch == null ? null : branch.U_BPLIDLGCY,
                    TransactionDate = parseDate(l.U_VSPOSMONITOR.U_TRANSDATE),
                    Status = (Model.Connector.IntegrationStatus)l.U_VSPOSMONITOR.U_STATUS,
                    CountTickets = detail == null ? 0 : Convert.ToInt64(detail.U_VSPOSMONITORDET.U_DETAILSCOUNT),
                    SumTickets = detail == null ? 0 : Convert.ToDouble(detail.U_VSPOSMONITORDET.U_TOTALAMTSUM),
                    CountErrors = errors == null ? 0 : Convert.ToInt64(errors.U_VSPOSMONITORDET.U_COUNT)
                };

                result.Add(item);
            }

            return result;
        }

        async private Task<bool> createTable()
        {
            bool result = false;

            Table table = new Table(_serviceLayerConnector);

            table.name = "VSPOSMONITOR";
            table.description = "Monitor de Vendas POS";
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
                name = "TRANSDATE",
                dataType = "db_Date",
                mandatory = true,
                description = "Data da transação"
            });

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
                name = "BRANCHIDLGCY",
                dataType = "db_Alpha",
                size = 10,
                mandatory = true,
                description = "Código da filial - legado"
            });

            lista.Add(new TableColumn()
            {
                name = "STATUS",
                dataType = "db_Numeric",
                size = 2,
                mandatory = true,
                description = "Status da integração"
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
                keys = new string[] { "TRANSDATE", "BPLID" }
            });

            lista.Add(new TableIndexes()
            {
                name = "STATUS",
                isUnique = false,
                keys = new string[] { "STATUS" }
            });

            return lista;
        }

        private string toJson(Model.Connector.POSMonitor monitor)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.Code = monitor.RecId;
            record.Name = monitor.RecId;
            record.U_TRANSDATE = monitor.TransactionDate;
            record.U_BPLID = monitor.BranchId;
            record.U_BRANCHIDLGCY = monitor.BranchIdLegacy;
            record.U_STATUS = monitor.Status;
            result = JsonConvert.SerializeObject(record);

            return result;
        }

        private Model.Connector.POSMonitor toRecord(dynamic record)
        {
            Model.Connector.POSMonitor monitor = new Model.Connector.POSMonitor();

            monitor.RecId = Guid.Parse(record.Code);
            monitor.TransactionDate = parseDate(record.U_TRANSDATE);
            monitor.BranchId = Convert.ToInt32(record.U_BPLID);
            monitor.BranchIdLegacy = record.U_BRANCHIDLGCY;
            monitor.Status = (Model.Connector.IntegrationStatus)Convert.ToInt32(record.U_STATUS);

            return monitor;
        }

        private DateTime parseDate(dynamic value)
        {
            DateTime result;

            DateTime.TryParse(value, out result);

            return result;
        }

        private string[] parseCriterias(List<Criteria> criterias)
        {
            List<string> filter = new List<string>();
            if (criterias?.Count != 0)
            {
                foreach (var c in criterias)
                {
                    if (_FieldMap.ContainsKey(c.Field.ToLower()))
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
                    else
                    {
                        filter.Add($"{c.Field} {c.Operator.ToLower()} {c.Value}");
                    }
                }
            }

            return filter.ToArray();
        }

        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "Code");
            map.Add("transactiondate", "U_TRANSDATE");
            map.Add("branchid", "U_BPLID");
            map.Add("branchidlegacy", "U_BRANCHIDLGCY");
            map.Add("status", "U_STATUS");

            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "T");
            map.Add("transactiondate", "T");
            map.Add("branchid", "N");
            map.Add("branchidlegacy", "T");
            map.Add("status", "N");

            return map;
        }
    }
}
