using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model.Integration;

namespace Varsis.Data.Serviceb1.Integration
{
    public class NcmCodesSetupService : IEntityService<Model.Integration.NcmCodesSetup>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public NcmCodesSetupService(ServiceLayerConnector serviceLayerConnector)
        {

            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = mountFieldMap();
            _FieldType = mountFieldType();
        }
        public Task<bool> Create()
        {
            throw new NotImplementedException();
        }

        public Task Delete(NcmCodesSetup entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task<NcmCodesSetup> Find(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task Insert(NcmCodesSetup entity)
        {
            throw new NotImplementedException();
        }

        public Task Insert(List<NcmCodesSetup> entities)
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
                    string field = _FieldMap[c.Field.ToLower()];
                    string type = _FieldType[c.Field.ToLower()];

                    if (type == "T")
                    {
                        if (c.Operator.ToLower() == "startswith")
                        {
                            filter.Add($"{c.Operator.ToLower()}({field},'{c.Value}')");
                        }
                        else
                        {
                            filter.Add($"{field} {c.Operator.ToLower()} '{c.Value}'");
                        }
                    }
                    else if (type == "N")
                    {
                        filter.Add($"{field} {c.Operator.ToLower()} {c.Value}");
                    }
                }
            }


            Varsis.Data.Infrastructure.Pagination page = new Varsis.Data.Infrastructure.Pagination();
            string query = Global.MakeODataQuery("U_VSITENTIDADE/$count", null, filter.Count == 0 ? null : filter.ToArray(), null, 1, 0);
            string data = await _serviceLayerConnector.getQueryResult(query);
            page.Linhas = Convert.ToInt64(data);
            page.Paginas = (Convert.ToInt64(data) / size.Value) + 1;
            page.qtdPorPagina = size.Value == 0 ? Convert.ToInt64(data) : size.Value;
            return page;
        }
        async public Task<List<NcmCodesSetup>> List(List<Criteria> criterias, long page, long size)
        {
            List<string> filter = new List<string>();

            int cont = 0;
            if (criterias?.Count != 0)
            {
                foreach (var c in criterias)
                {
                    cont++;
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

            string query = Global.MakeODataQuery("NCMCodesSetup");

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<NcmCodesSetup> result = new List<NcmCodesSetup>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        public NcmCodesSetup toRecord(dynamic record)
        {
            NcmCodesSetup ncmCode = new NcmCodesSetup();

            ncmCode.AbsEntry = record.AbsEntry;
            ncmCode.Description = record.Description;
            ncmCode.GroupCode = record.GroupCode;
            ncmCode.NCMCode = record.NCMCode;

            return ncmCode;
        }

        public Task Update(NcmCodesSetup entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<NcmCodesSetup> entities)
        {
            throw new NotImplementedException();
        }
        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("loj_cli", "U_LOJ_CLI");

            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("loj_cli", "T");

            return map;
        }
    }
}
