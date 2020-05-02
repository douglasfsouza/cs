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
    public class InvoiceService : IEntityService<Model.Invoice>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public InvoiceService(ServiceLayerConnector serviceLayerConnector)
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

        public Task Delete(Invoice entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task<Invoice> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"Invoice('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            Invoice invoice = toRecord(record);

            // Recupera as linhas da nota iscal
            string[] filter = new string[]
            {
                $"CardCode eq '{recid}'"
            };

            query = Global.MakeODataQuery("Invoice", null, filter);

            data = await _serviceLayerConnector.getQueryResult(query);

            return invoice;
        }

        async public Task Insert(Invoice entity)
        {
            try
            {
                IBatchProducer batch = _serviceLayerConnector.CreateBatch();
                string record = toJson(entity);
                batch.Post(HttpMethod.Post, "/Drafts", record);
                ServiceLayerResponse response = await _serviceLayerConnector.Post("Drafts", record, true, true);
                var resp1 = response.success;
                if (!response.success)
                {
                    var idEntidade = entity.RecId;
                    var cad = toJsonError();
                    string query = Global.BuildQuery($"U_VSITINVOICE('{idEntidade}')");
                    response = await _serviceLayerConnector.Patch(query, cad, true);
                }

                if (resp1)
                {
                    Invoice savedInvoice = JsonConvert.DeserializeObject<Invoice>(response.data);
                    var idInvoice = entity.RecId;
                    var cad = toJsonSucess(savedInvoice.DocEntry.Value, entity.FRT_VAL, entity.TRN_FAT);
                    string query = Global.BuildQuery($"U_VSITINVOICE('{idInvoice}')");
                    response = await _serviceLayerConnector.Patch(query, cad, true);
                }
            }
            catch(Exception e)
            {

            }  
        }
        private string toJsonSucess(long docentry,double? FRT_VAL, long? TRN_FAT)
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.U_STATUS = Model.Integration.Invoice.InvoiceIntegrationStatus.Processed;
            record.U_DATA_INTEGRACAO = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            record.U_DOCENTRY = docentry;

            //if (FRT_VAL != null && TRN_FAT != null && FRT_VAL > 0 && TRN_FAT > 0)
            //{
            //    record.U_LANDEDCOSTS = 1;
            //}
            //else
            //{
            //    record.U_LANDEDCOSTS = 0;
            //}

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
        public Task Insert(List<Invoice> entities)
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
            string query = Global.MakeODataQuery("BusinessPartners/$count", null, filter.Count == 0 ? null : filter.ToArray(), null, 1, 0);
            string data = await _serviceLayerConnector.getQueryResult(query);
            page.Linhas = Convert.ToInt64(data);
            page.Paginas = (Convert.ToInt64(data) / size.Value) + 1;
            page.qtdPorPagina = size.Value == 0 ? Convert.ToInt64(data) : size.Value;
            return page;
        }
        async public Task<List<Invoice>> List(List<Criteria> criterias, long page, long size)
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

            string query = Global.MakeODataQuery("Invoice", null, filter.Count == 0 ? null : filter.ToArray(),null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<Invoice> result = new List<Invoice>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        async public Task Update(Invoice entity)
        {
            
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();
            batch = _serviceLayerConnector.CreateBatch();
            string record = toJson(entity);
            batch.Post(HttpMethod.Patch, "/BusinessPartners(CardCode='C20000')", record);
            ServiceLayerResponse response = await _serviceLayerConnector.Post("BusinessPartners", record);
            if (!response.success)
            {
                string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
            throw new NotImplementedException();
        }
        private string toUpdate(dynamic businesspartners)
        {
            dynamic record = new ExpandoObject();
            //fazer a magia 
            return record;
        }


        public Task Update(List<Invoice> entities)
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
            List<TableIndexes> lista = new List<TableIndexes>();

            lista.Add(new TableIndexes()
            {
                name = "PK",
                isUnique = true,
                keys = new string[] { "INVOICEDIRECTION", "INVOICEID", "INVOICESERIES", "INVOICEDATE", "ISSUERID" }
            });

            lista.Add(new TableIndexes()
            {
                name = "STATUS",
                isUnique = false,
                keys = new string[] { "STATUS" }
            });

            return lista;
        }

        private List<TableIndexes> createIndexesItem()
        {
            List<TableIndexes> lista = new List<TableIndexes>();

            lista.Add(new TableIndexes()
            {
                name = "ITEMID",
                isUnique = true,
                keys = new string[] { "INVOICECODE", "ITEMID" }
            });

            return lista;
        }
        private string toJsonwarehouses(string nomeLoja, string codigoLoja)
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.WarehouseCode = codigoLoja;
            record.WarehouseName = nomeLoja;
            result = JsonConvert.SerializeObject(record);
            return result;
        }
        private string criaCodigoWareHouses(string nomeLoja, string codigoLoja)
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            string codigo = codigoLoja;
            string zeros = "";

            if (codigo.Count() < 4)
            {
                int cont = codigo.Count();
                int faltam = 4 - codigo.Count();
                if (faltam < 4)
                {
                    for (var i = 0; i < faltam; i++)
                    {
                        zeros += "0";
                    }
                }
            }
            codigo = zeros + codigoLoja;
            return codigo + "_01";
        }
        private string toJson(Invoice invoice)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();
            record.BPL_IDAssignedToInvoice = invoice.BPL_IDAssignedToInvoice;
            record.CardCode = invoice.CardCode;
            record.DocDate = invoice.DocDate;
            record.TaxDate = invoice.TaxDate;
            record.DocDueDate = invoice.DocDueDate;
            //record.DocNum = invoice.DocNum;
            record.DocumentLines = invoice.DocumentLines;
            record.DocumentInstallments = invoice.DocumentInstallments;
            record.DocumentReferences = invoice.DocumentReferences;
            record.DiscountPercent = invoice.DiscountPercent;
            record.SequenceModel = invoice.SequenceModel;
            record.SequenceSerial = invoice.SequenceSerial;
            record.SeriesString = invoice.SeriesString;
            record.SequenceSerial = invoice.SequenceSerial;
            record.SeriesString = invoice.SeriesString;
            record.DocType = invoice.DocType;
            record.DocObjectCode = invoice.DocObjectCode;
            record.SequenceCode = invoice.SequenceCode;
            record.U_SubUtilizacao = invoice.U_SUBUTILIZACAO;

            //record.TaxCode = invoice.TaxCode;
            //record.CfopCode = invoice.CfopCode;
            //record.DocObjectCode = invoice.DocObjectCode;

            result = JsonConvert.SerializeObject(record, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            });

            return result;
        }
        private string toJsonTeste(BusinessPartners businesspartners)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            //record.Code = businessplaces.RecId.ToString();
            record.AdditionalIdNumber = null;
            
            result = JsonConvert.SerializeObject(record);

            return result;
        }
        private Invoice toRecord(dynamic record)
        {
            Invoice invoice = new Invoice();
            

            return invoice;
        }


        private DateTime parseDate(dynamic value)
        {
            DateTime result;

            DateTime.TryParse(value, out result);

            return result;
        }
        private string parseCountry(dynamic value)
        {
            string origem = value;
            string result = null;
            origem = origem.Replace("0", "").Replace("4", "");
            if (origem.Count() > 2)
            {
                for (var i = 0; i < origem.Count() - 1; i ++)
                {
                    result += origem[i];
                }
            }
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
