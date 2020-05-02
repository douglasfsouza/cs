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
using Varsis.Data.Model.Integration;

namespace Varsis.Data.Serviceb1
{
    public class LegacyFiscalOperations : IEntityService<Model.LegacyFiscalOperations>
    {
        const string SL_TABLE_NAME = "U_VSCATLGCYUSAGE";

        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public LegacyFiscalOperations(ServiceLayerConnector serviceLayerConnector)
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
            bool result = false;

            Table table = new Table(_serviceLayerConnector);

            table.name = "VSCATLGCYCFOP";
            table.description = "Cadastro de CFOP";
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

            return lista;
        }

        private List<TableColumn> createColumns()
        {
            List<TableColumn> lista = new List<TableColumn>();

            lista.Add(new TableColumn() { name = "CRFCode", description = "CRFCode", mandatory = true, size = 7, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "CFOP", description = "CFOP", mandatory = true, size = 7, dataType = "db_Numeric" });
           
            return lista;
        }

        public Task Delete(Model.LegacyFiscalOperations entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task<Model.LegacyFiscalOperations> Find(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task Insert(Model.LegacyFiscalOperations entity)
        {

            IBatchProducer batch = _serviceLayerConnector.CreateBatch();
            batch = _serviceLayerConnector.CreateBatch();
            string record = toJson(entity);

            batch.Post(HttpMethod.Post, "/U_VSCATLGCYCFOP", record);

            ServiceLayerResponse response = await _serviceLayerConnector.Post(batch);


            if (!response.success)
            {
                string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }

        }

        private string toJson(Model.LegacyFiscalOperations cfop)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.Code = cfop.RecId.ToString();
            record.Name = cfop.RecId.ToString();
            record.U_CFOP = cfop.CFOP;
            record.U_CRFCode = cfop.CRFCode;

            result = JsonConvert.SerializeObject(record);

            return result;
        }

        public Task Insert(List<Model.LegacyFiscalOperations> entities)
        {
            throw new NotImplementedException();
        }


        async public Task<Pagination> TotalLinhas(long? size, List<Criteria> criterias)
        {
            return new Pagination();
        }

        public Task Update(Model.LegacyFiscalOperations entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<Model.LegacyFiscalOperations> entities)
        {
            throw new NotImplementedException();
        }

        async public Task<List<Model.LegacyFiscalOperations>> List(List<Criteria> criterias, long page = -1, long size = -1)
        {
            var filter = Global.parseCriterias(criterias, _FieldMap, _FieldType);

            string query = Global.MakeODataQuery(SL_TABLE_NAME, null, filter.Length == 0 ? null : filter, null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<Model.LegacyFiscalOperations> result = new List<Model.LegacyFiscalOperations>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    Model.LegacyFiscalOperations record = new Model.LegacyFiscalOperations()
                    {
                        RecId = Guid.Parse(o.Code),
                        CRFCode = o.U_CRFCODE,
                        CFOP = o.U_CFOP
                    };

                    result.Add(record);
                }
            }

            return result;
        }

        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "Code");
            map.Add("crfcode", "U_CRFCODE");
            map.Add("cfop", "U_CFOP");

            return map;
        }

        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "T");
            map.Add("crfcode", "T");
            map.Add("cfop", "T");

            return map;
        }


    }
}
