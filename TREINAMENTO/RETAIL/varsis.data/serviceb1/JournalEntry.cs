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

namespace Varsis.Data.Serviceb1.Integration
{
    public class JournalEntryService : IEntityService<Model.JournalEntry>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public JournalEntryService(ServiceLayerConnector serviceLayerConnector)
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

        async private Task<bool> createTable()
        {
            throw new NotImplementedException();
        }

        public Task Delete(JournalEntry entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task<JournalEntry> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"JournalEntrys('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            JournalEntry invoice = toRecord(record);

            // Recupera as linhas da nota iscal
            string[] filter = new string[]
            {
                $"CardCode eq '{recid}'"
            };

            query = Global.MakeODataQuery("JournalEntrys", null, filter);

            data = await _serviceLayerConnector.getQueryResult(query);

            return invoice;
        }

        async public Task Insert(JournalEntry entity)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();
            string record = toJson(entity); 
            batch.Post(HttpMethod.Post, "/JournalEntries", record);
            ServiceLayerResponse response = await _serviceLayerConnector.Post("JournalEntries", record, true, true);
            var resp1 = response.success;
            if (!response.success)
            {
                var idEntidade = entity.RecId;
                var cad = toJsonError();
                string query = Global.BuildQuery($"U_VSITENTIDADE('{idEntidade}')");
                response = await _serviceLayerConnector.Patch(query, cad, true);
            }

            if (resp1)
            {
                //Invoice savedInvoice = JsonConvert.DeserializeObject<JournalEntry>(response.data);
                var idInvoice = entity.RecId;
                var cad = toJsonSucess();
                string query = Global.BuildQuery($"U_VSITINVOICE('{idInvoice}')");
                response = await _serviceLayerConnector.Patch(query, cad, true);
            }
        }
        private string toJsonSucess()
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.U_STATUS = Model.Integration.Invoice.InvoiceIntegrationStatus.Journey;
            record.U_DATA_INTEGRACAO = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //record.U_DOCENTRY = docentry;
            result = JsonConvert.SerializeObject(record);
            return result;
        }
        private string toJsonManual(long docentry)
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.U_STATUS = Model.Integration.Invoice.InvoiceIntegrationStatus.Processed;
            record.U_DATA_INTEGRACAO = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            record.U_DOCENTRY = docentry;
            result = JsonConvert.SerializeObject(record);
            return result;
        }
        private string toJsonError()
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.U_STATUS = Model.Integration.Invoice.InvoiceIntegrationStatus.Error;
            result = JsonConvert.SerializeObject(record);
            return result;
        }
        public Task Insert(List<JournalEntry> entities)
        {
            throw new NotImplementedException();
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
                    string field = _FieldMap[c.Field];
                    string type = _FieldType[c.Field];

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
            string query = Global.MakeODataQuery("JournalEntrys/$count", null, filter.Count == 0 ? null : filter.ToArray(), null, 1, 0);
            string data = await _serviceLayerConnector.getQueryResult(query);
            page.Linhas = Convert.ToInt64(data);
            page.Paginas = (Convert.ToInt64(data) / size.Value) + 1;
            page.qtdPorPagina = size.Value == 0 ? Convert.ToInt64(data) : size.Value;
            return page;
        }
        async public Task<List<JournalEntry>> List(List<Criteria> criterias, long page, long size)
        {
            List<string> filter = new List<string>();

            if (criterias?.Count != 0)
            {
                foreach(var c in criterias)
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

            string query = Global.MakeODataQuery("JournalEntrys", null, filter.Count == 0 ? null : filter.ToArray(),null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<JournalEntry> result = new List<JournalEntry>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        async public Task Update(JournalEntry entity)
        {
            throw new NotImplementedException();
        }
        private string toUpdate(dynamic businesspartners)
        {
            dynamic record = new ExpandoObject();
            //fazer a magia 
            return record;
        }


        public Task Update(List<JournalEntry> entities)
        {
            throw new NotImplementedException();
        }

        private List<TableColumn> createColumns()
        {
            List<TableColumn> lista = new List<TableColumn>();
            
            return lista;
        }

        private List<TableIndexes> createIndexes()
        {
            throw new NotImplementedException();
        }



        
        private string toJson(JournalEntry invoice)
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.ReferenceDate = invoice.ReferenceDate;
            record.Memo = invoice.Memo;
            record.TaxDate = invoice.TaxDate;
            record.DueDate = invoice.DueDate;
            record.ECDPostingType = invoice.ECDPostingType;
            record.JournalEntryLines = invoice.JournalEntryLines;
            result = JsonConvert.SerializeObject(record);

            return result;
        }
       
        private JournalEntry toRecord(dynamic record)
        {
            JournalEntry invoice = new JournalEntry();
            

            return invoice;
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
            map.Add("CardCode", "CardCode");
            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            return map;
        }
    }
}
