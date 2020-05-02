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

namespace Varsis.Data.Serviceb1
{
    public class ChartOfAccounts : IEntityService<Model.ChartOfAccounts>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;
        public ChartOfAccounts(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = this.mountFieldMap();
            _FieldType = this.mountFieldType();
        }
        public Task<bool> Create()
        {
            throw new NotImplementedException();
        }

        public Task Delete(Model.ChartOfAccounts entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task<Model.ChartOfAccounts> Find(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task Insert(Model.ChartOfAccounts entity)
        {
            throw new NotImplementedException();
        }

        public Task Insert(List<Model.ChartOfAccounts> entities)
        {
            throw new NotImplementedException();
        }

       async public Task<List<Model.ChartOfAccounts>> List(List<Criteria> criterias, long page = -1, long size = -1)
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

            string query = Global.MakeODataQuery("ChartOfAccounts", null, filter.Count == 0 ? null : filter.ToArray(), null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data).ToList();

            List<Model.ChartOfAccounts> result = new List<Model.ChartOfAccounts>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toChartOfAccounts(o));
                }
            }

            return result;
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
            string query = Global.MakeODataQuery("ChartOfAccounts/$count", null, filter.Count == 0 ? null : filter.ToArray(), null, 1, 0);
            string data = await _serviceLayerConnector.getQueryResult(query);
            page.Linhas = Convert.ToInt64(data);
            page.Paginas = (Convert.ToInt64(data) / size.Value) + 1;
            page.qtdPorPagina = size.Value == 0 ? Convert.ToInt64(data) : size.Value;
            return page;
        }

        private Model.ChartOfAccounts toChartOfAccounts(dynamic record)
        {
            Model.ChartOfAccounts chartofaccounts = new Model.ChartOfAccounts();

            chartofaccounts.Name = record.Name;
            chartofaccounts.Code = record.Code;

            return chartofaccounts;
        }

        public Task Update(Model.ChartOfAccounts entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<Model.ChartOfAccounts> entities)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("name", "U_NAME");
            map.Add("code", "U_CODE");

            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("name", "T");
            map.Add("code", "T");

            return map;
        }

    }
}
