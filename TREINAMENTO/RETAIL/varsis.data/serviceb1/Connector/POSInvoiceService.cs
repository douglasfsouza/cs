using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model.Connector;

namespace Varsis.Data.Serviceb1.Connector
{
    public class POSInvoiceService : IEntityService<Model.Connector.POSInvoice>, IEntityServiceWithReturn<Model.Connector.POSInvoice>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;

        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        const string SL_TABLE_NAME = "Invoices";

        public POSInvoiceService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = this.mountFieldMap();
            _FieldType = this.mountFieldType();
        }

        public Task<bool> Create()
        {
            throw new NotImplementedException();
        }

        public Task Delete(POSInvoice entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task<POSInvoice> Find(List<Criteria> criterias)
        {
            POSInvoice result = null;

            List<POSInvoice> lista = await this.List(criterias);

            if (lista != null && lista.Count != 0)
            {
                result = lista[0];
            };

            return result;
        }

        async public Task Insert(POSInvoice entity)
        {
            string record = toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Post(SL_TABLE_NAME, record, false, true);

            if (!response.success)
            {
                string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async public Task Insert(List<POSInvoice> entities)
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
                string message = $"Erro ao enviar lista de '{entities[0].EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
            else if (response.internalResponses.Count(m => m.success == false) != 0)
            {
                var error = response.internalResponses[response.internalResponses.Count() - 1];
                string message = $"Erro ao enviar lista de '{entities[0].EntityName}': {error.errorCode}-{error.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        async Task<POSInvoice> IEntityServiceWithReturn<POSInvoice>.Insert(POSInvoice entity)
        {
            POSInvoice result = null;

            string record = toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Post(SL_TABLE_NAME, record, false, true);

            if (!response.success)
            {
                string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
            else
            {
                ExpandoObject responseData = Global.parseQueryToObject(response.data);
                result = toRecord(responseData);
            }

            return result;
        }

        Task<List<POSInvoice>> IEntityServiceWithReturn<POSInvoice>.Insert(List<POSInvoice> entities)
        {
            throw new NotImplementedException();
        }

        async public Task<List<POSInvoice>> List(List<Criteria> criterias, long page = -1, long size = -1)
        {
            var filter = this.ParseCriterias(criterias);

            string query = Global.MakeODataQuery(SL_TABLE_NAME, null, filter.Count == 0 ? null : filter.ToArray(), null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<Model.Connector.POSInvoice> result = new List<Model.Connector.POSInvoice>();

            lista.ForEach(i =>
            {
                result.Add(toRecord(i));
            });

            return result;
        }

        public Task<Pagination> TotalLinhas(long? size, List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task Update(POSInvoice entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<POSInvoice> entities)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("documententry", "DocEntry");
            map.Add("documentNum", "DocNum");
            map.Add("branchid", "BPL_IDAssignedToInvoice");
            map.Add("documentdate", "DocDate");
            map.Add("invoiceid", "SequenceSerial");
            map.Add("invoicemodel", "SequenceModel");
            map.Add("invoiceseries", "SeriesString");
            map.Add("fiscalKey", "U_chaveacesso");

            return map;
        }

        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("documententry", "N");
            map.Add("documentNum", "N");
            map.Add("branchid", "N");
            map.Add("documentdate", "T");
            map.Add("invoiceid", "N");
            map.Add("invoicemodel", "T");
            map.Add("invoiceseries", "T");
            map.Add("fiscalKey", "T");

            return map;
        }

        private List<string> ParseCriterias(List<Criteria> criterias)
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

            return filter;
        }

        private string toJson(Model.Connector.POSInvoice invoice)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.DocType = invoice.DocumentType;
            record.DocDate = invoice.DocumentDate;
            record.DocDueDate = invoice.DueDate;
            record.DocTime = invoice.DocumentTime;
            record.SalesPersonCode = invoice.SalesPersonId;
            record.BPL_IDAssignedToInvoice = invoice.BranchId;
            record.CardCode = invoice.CustomerId;
            record.SequenceCode = -1;
            record.SequenceSerial = invoice.InvoiceId;
            record.SeriesString = invoice.InvoiceSeries;
            record.SequenceModel = invoice.InvoiceModel;
            record.U_chaveacesso = invoice.FiscalKey;
            record.Incoterms = "0";

            record.DocumentLines = new List<dynamic>();

            invoice.Items?.ForEach(i =>
            {
                dynamic item = new ExpandoObject();

                item.LineNum = i.LineSequence;
                item.ItemCode = i.ItemId;
                item.Quantity = i.Quantity;
                item.Price = i.Price;
                item.UnitPrice = i.Price;
                item.SalesPersonCode = -1;
                item.Usage = 10;

                record.DocumentLines.Add(item);
            });

            result = JsonConvert.SerializeObject(record);

            return result;
        }

        private Model.Connector.POSInvoice toRecord(dynamic record)
        {
            Model.Connector.POSInvoice invoice = new Model.Connector.POSInvoice();

            invoice.DocumentType = record.DocType;
            invoice.DocumentEntry = Convert.ToInt64(record.DocEntry);
            invoice.DocumentNum = Convert.ToInt64(record.DocNum);
            invoice.DocumentDate = DateTime.Parse(Convert.ToString(record.DocDate));
            invoice.DueDate = DateTime.Parse(Convert.ToString(record.DocDueDate));
            invoice.DocumentTime = DateTime.Parse(Convert.ToString(record.DocTime));
            invoice.SalesPersonId = Convert.ToInt64(record.SalesPersonCode);
            invoice.BranchId = Convert.ToInt64(record.BPL_IDAssignedToInvoice);
            invoice.CustomerId = record.CardCode;
            invoice.InvoiceId = Convert.ToInt64(record.SequenceSerial);
            invoice.InvoiceSeries = record.SeriesString;
            invoice.InvoiceModel = record.SequenceModel;
            invoice.FiscalKey = record.U_chaveacesso;
            invoice.Items = new List<POSInvoiceItem>();

            foreach (var item in record.DocumentLines)
            {
                invoice.Items.Add(new POSInvoiceItem()
                {
                    LineSequence = Convert.ToInt64(item.LineNum),
                    ItemId = item.ItemCode,
                    Quantity = Convert.ToDouble(item.Quantity),
                    Price = Convert.ToDouble(item.UnitPrice),
                    Usage = Convert.ToInt64(item.Usage)
                });
            }

            return invoice;
        }
    }
}
