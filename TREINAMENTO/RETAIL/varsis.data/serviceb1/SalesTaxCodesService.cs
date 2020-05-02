using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model;
using System.Linq;
using System.Dynamic;
using Newtonsoft.Json;

namespace Varsis.Data.Serviceb1
{
    public class SalesTaxCodesService : IEntityService<Model.SalesTaxCodes>
    {
        const string SL_TABLE_NAME = "SalesTaxCodes";

        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public SalesTaxCodesService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = mountFieldMap();
            _FieldType = mountFieldType();
        }

        public Task<bool> Create()
        {
            throw new NotImplementedException();
        }

        public Task Delete(SalesTaxCodes entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task<SalesTaxCodes> Find(List<Criteria> criterias)
        {
            string code = criterias[0].Value;
            string query = Global.BuildQuery($"{SL_TABLE_NAME}('{code}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            SalesTaxCodes result = null;

            if (record != null)
            {
                result = toRecord(record);
            }

            return result;
        }

        public Task Insert(SalesTaxCodes entity)
        {
            throw new NotImplementedException();
        }

        public Task Insert(List<SalesTaxCodes> entities)
        {
            throw new NotImplementedException();
        }

        async public Task<List<SalesTaxCodes>> List(List<Criteria> criterias, long page = -1, long size = -1)
        {
            var filter = Global.parseCriterias(criterias, _FieldMap, _FieldType);

            string query = Global.MakeODataQuery(SL_TABLE_NAME, null, filter.Length == 0 ? null : filter, null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<SalesTaxCodes> result = new List<SalesTaxCodes>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        public Task<Pagination> TotalLinhas(long? size, List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task Update(SalesTaxCodes entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<SalesTaxCodes> entities)
        {
            throw new NotImplementedException();
        }

        private string toJson(SalesTaxCodes map)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.Code = map.Code;
            record.Name = map.Name;

            result = JsonConvert.SerializeObject(record);

            return result;
        }

        private SalesTaxCodes toRecord(dynamic record)
        {
            SalesTaxCodes map = new SalesTaxCodes();

            map.Code = record.Code;
            map.Name = record.Name;

            return map;
        }

        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("code", "Code");
            map.Add("name", "Name");

            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("code", "T");
            map.Add("name", "T");

            return map;
        }

    }
}
