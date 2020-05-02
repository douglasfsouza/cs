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
    public class POSMonitorDetailService : IEntityService<Model.Connector.POSMonitorDetail>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public POSMonitorDetailService(ServiceLayerConnector serviceLayerConnector)
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

        async public Task Delete(Model.Connector.POSMonitorDetail entity)
        {
            ServiceLayerResponse response = await _serviceLayerConnector.Delete($"U_VSPOSMONITORDET('{entity.RecId}')", "");

            if (!response.success)
            {
                string message = $"Erro ao excluir transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async public Task Delete(List<Criteria> criterias)
        {
            List<Model.Connector.POSMonitorDetail> entities = await this.List(criterias, 0, 0);

            IBatchProducer batch = _serviceLayerConnector.CreateBatch();

            entities.ForEach(e =>
            {
                batch.Post(HttpMethod.Delete, $"/U_VSPOSMONITORDET('{e.RecId}')", "");
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

        async public Task<Model.Connector.POSMonitorDetail> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"U_VSPOSMONITORDET('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            Model.Connector.POSMonitorDetail detail = null;

            if (record != null)
            {
                detail = toRecord(record);
            }

            return detail;
        }

        async public Task Insert(Model.Connector.POSMonitorDetail entity)
        {
            string record = toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Post("U_VSPOSMONITORDET", record);

            if (!response.success)
            {
                string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async public Task Insert(List<Model.Connector.POSMonitorDetail> entities)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();

            entities.ForEach(e =>
            {
                string record = toJson(e);
                batch.Post(HttpMethod.Post, "/U_VSPOSMONITORDET", record);
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
        async public Task<List<Model.Connector.POSMonitorDetail>> List(List<Criteria> criterias, long page, long size)
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

            string query = Global.MakeODataQuery("U_VSPOSMONITORDET", null, filter.Count == 0 ? null : filter.ToArray(), null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<Model.Connector.POSMonitorDetail> result = new List<Model.Connector.POSMonitorDetail>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        async public Task Update(Model.Connector.POSMonitorDetail entity)
        {
            string record = toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Put($"U_VSPOSMONITORDET('{entity.RecId}')", record);

            if (!response.success)
            {
                string message = $"Erro ao atualizar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async public Task Update(List<Model.Connector.POSMonitorDetail> entities)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();

            entities.ForEach(e =>
            {
                string record = toJson(e);
                batch.Post(HttpMethod.Put, $"/U_VSPOSMONITORDET('{e.RecId}')", record);
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

        async private Task<bool> createTable()
        {
            bool result = false;

            Table table = new Table(_serviceLayerConnector);

            table.name = "VSPOSMONITORDET";
            table.description = "Monitor POS - detalhe";
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
                name = "MONITOR",
                dataType = "db_Alpha",
                size = 50,
                mandatory = true,
                description = "Id do registro do monitor"
            });

            lista.Add(new TableColumn()
            {
                name = "POSID",
                dataType = "db_Alpha",
                size = 20,
                mandatory = true,
                description = "Id do POS"
            });

            lista.Add(new TableColumn()
            {
                name = "TRANSTIME",
                dataType = "db_Date",
                dataTypeSub = "st_Time",
                size = 7,
                mandatory = true,
                description = "Horário da transação"
            });

            lista.Add(new TableColumn()
            {
                name = "INVOICEID",
                dataType = "db_Alpha",
                size = 20,
                mandatory = true,
                description = "Número do cupom"
            });

            lista.Add(new TableColumn()
            {
                name = "TOTALAMT",
                dataType = "db_Float",
                dataTypeSub = "st_Measurement",
                mandatory = true,
                description = "Total do cupom"
            });

            lista.Add(new TableColumn()
            {
                name = "ITEMSCNT",
                dataType = "db_Numeric",
                size = 7,
                mandatory = true,
                description = "# de itens"
            });

            lista.Add(new TableColumn()
            {
                name = "DOCNUM",
                dataType = "db_Numeric",
                size = 11,
                mandatory = false,
                description = "Número do documento gerado"
            });

            lista.Add(new TableColumn()
            {
                name = "STATUS",
                dataType = "db_Numeric",
                size = 2,
                mandatory = false,
                description = "Status da integração"
            });

            lista.Add(new TableColumn()
            {
                name = "ERRORMESSAGE",
                dataType = "db_Alpha",
                size = 200,
                mandatory = false,
                description = "Mensagem de erro"
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
                keys = new string[] { "MONITOR", "POSID", "TRANSTIME", "INVOICEID" }
            });

            return lista;
        }

        private string toJson(Model.Connector.POSMonitorDetail detail)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.Code = detail.RecId.ToString();
            record.Name = detail.RecId.ToString();
            record.U_MONITOR = detail.POSMonitor;
            record.U_POSID = detail.POSId;
            record.U_TRANSTIME = detail.TransactionTime;
            record.U_INVOICEID = detail.InvoiceId;
            record.U_TOTALAMT = detail.totalAmount;
            record.U_ITEMSCNT = detail.itemsCount;
            record.U_STATUS = (int)detail.status;
            record.U_ERRORMESSAGE = detail.errorMessage;
            record.U_DOCNUM = detail.DocNum == null ? 0 : (long)detail.DocNum;

            result = JsonConvert.SerializeObject(record);

            return result;
        }

        private Model.Connector.POSMonitorDetail toRecord(dynamic record)
        {
            Model.Connector.POSMonitorDetail detail = new Model.Connector.POSMonitorDetail();

            detail.RecId = Guid.Parse(record.Code);
            detail.POSMonitor = Guid.Parse(record.U_MONITOR);
            detail.POSId = record.U_POSID;
            detail.TransactionTime = parseTime(record.U_TRANSTIME);
            detail.InvoiceId = record.U_INVOICEID;
            detail.totalAmount = Convert.ToDouble(record.U_TOTALAMT);
            detail.itemsCount = Convert.ToInt32(record.U_ITEMSCNT);
            detail.status = (Model.Connector.DetailIntegrationStatus)Convert.ToInt32(record.U_STATUS);
            detail.errorMessage = record.U_ERRORMESSAGE;
            detail.DocNum = record.U_DOCNUM == null ? 0 : (long)record.U_DOCNUM;

            return detail;
        }

        private DateTime parseTime(dynamic value)
        {
            DateTime result;

            DateTime.TryParse(value, out result);

            return result;
        }

        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "Code");
            map.Add("posmonitor", "U_MONITOR");
            map.Add("posid", "U_POSID");
            map.Add("transactiontime", "U_TRANSTIME");
            map.Add("invoiceid", "U_INVOICEID");
            map.Add("totalamount", "U_TOTALAMT");
            map.Add("itemscount", "U_ITEMSCNT");
            map.Add("status", "U_STATUS");
            map.Add("docnum", "U_DOCNUM");

            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "T");
            map.Add("posmonitor", "T");
            map.Add("posid", "T");
            map.Add("transactiontime", "T");
            map.Add("invoiceid", "T");
            map.Add("totalamount", "N");
            map.Add("itemscount", "T");
            map.Add("status", "N");
            map.Add("docnum", "N");

            return map;
        }
    }
}
