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
    public class NFModelsService : IEntityService<Model.NFModels>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public NFModelsService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = mountFieldMap();
            _FieldType = mountFieldType();
        }

        public Task<bool> Create()
        {
            throw new NotImplementedException();
        }

        private Task<bool> createTable()
        {
            throw new NotImplementedException();
        }

        public Task Delete(NFModels entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }
        private string toJsonError()
        {
            throw new NotImplementedException();
        }
        public Task<NFModels> Find(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task Insert(NFModels entity)
        {
            throw new NotImplementedException();

        }

        public Task Insert(List<NFModels> entities)
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
            string query = Global.MakeODataQuery("NFModels/$count", null, filter.Count == 0 ? null : filter.ToArray(), null, 1, 0);
            string data = await _serviceLayerConnector.getQueryResult(query);
            page.Linhas = Convert.ToInt64(data);
            page.Paginas = (Convert.ToInt64(data) / size.Value) + 1;
            page.qtdPorPagina = size.Value == 0 ? Convert.ToInt64(data) : size.Value;
            return page;
        }
        async public Task<List<NFModels>> List(List<Criteria> criterias, long page, long size)
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

            string query = Global.MakeODataQuery("NFModels", null, filter.Count == 0 ? null : filter.ToArray(),null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<NFModels> result = new List<NFModels>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        async public Task Update(NFModels entity)
        {
            throw new NotImplementedException();
        }
        public Task Update(List<NFModels> entities)
        {
            throw new NotImplementedException();
        }

        private List<TableColumn> createColumns()
        {
            throw new NotImplementedException();
        }

        private List<TableIndexes> createIndexes()
        {
            throw new NotImplementedException();
        }

        private List<TableIndexes> createIndexesItem()
        {
            throw new NotImplementedException();
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
        private NFModels toRecord(dynamic record)
        {
            NFModels NFModels = new NFModels();
            NFModels.AbsEntry = record.AbsEntry;
            NFModels.NFMCode = record.NFMCode;
            NFModels.NFMDescription = record.NFMDescription;
            NFModels.NFMName = record.NFMName;
            return NFModels;
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

            map.Add("nfmcode", "NFMCode");
          
            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("nfmcode", "T");

            return map;
        }
    }
}
