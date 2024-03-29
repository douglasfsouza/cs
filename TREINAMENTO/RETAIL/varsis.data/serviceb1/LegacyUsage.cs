﻿using Newtonsoft.Json;
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
    public class LegacyUsage : IEntityService<Model.LegacyUsage>
    {
        const string SL_TABLE_NAME = "U_VSCATLGCYUSAGE";

        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public LegacyUsage(ServiceLayerConnector serviceLayerConnector)
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

            table.name = "VSCATLGCYUSAGE";
            table.description = "Cadastro de Agências";
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

            return lista;
        }


        public Task Delete(Model.LegacyUsage entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task<Model.LegacyUsage> Find(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task Insert(Model.LegacyUsage entity)
        {

            IBatchProducer batch = _serviceLayerConnector.CreateBatch();
            batch = _serviceLayerConnector.CreateBatch();
            string record = toJson(entity);

            batch.Post(HttpMethod.Post, "/U_VSCATLGCYUSAGE", record);


            ServiceLayerResponse response = await _serviceLayerConnector.Post(batch);


            if (!response.success)
            {
                string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }


        }

        private string toJson(Model.LegacyUsage agency)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.Name = agency.Name;
            record.Code = agency.Code;

            result = JsonConvert.SerializeObject(record);

            return result;
        }

        public Task Insert(List<Model.LegacyUsage> entities)
        {
            throw new NotImplementedException();
        }


        public Task<Pagination> TotalLinhas(long? size, List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task Update(Model.LegacyUsage entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<Model.LegacyUsage> entities)
        {
            throw new NotImplementedException();
        }
        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "Code");
            map.Add("code", "Code");
            map.Add("name", "Name");

            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "T");
            map.Add("code", "T");
            map.Add("name", "T");

            return map;
        }

        async public Task<List<Model.LegacyUsage>> List(List<Criteria> criterias, long page = -1, long size = -1)
        {
            var filter = Global.parseCriterias(criterias, _FieldMap, _FieldType);

            string query = Global.MakeODataQuery(SL_TABLE_NAME, null, filter.Length == 0 ? null : filter, null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<Model.LegacyUsage> result = new List<Model.LegacyUsage>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    Model.LegacyUsage record = new Model.LegacyUsage()
                    {
                        Code = o.Code,
                        Name = o.Name
                    };

                    result.Add(record);
                }
            }

            return result;
        }
    }
}
