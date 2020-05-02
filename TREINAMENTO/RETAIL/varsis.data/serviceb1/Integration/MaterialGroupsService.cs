using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model.Integration;

namespace Varsis.Data.Serviceb1.Integration
{
    public class MaterialGroupsService : IEntityService<Model.Integration.MaterialGroups>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public MaterialGroupsService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = mountFieldMap();
            _FieldType = mountFieldType();

        }
        public Task<bool> Create()
        {
            throw new NotImplementedException();
        }

        public Task Delete(Model.Integration.MaterialGroups entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task<Model.Integration.MaterialGroups> Find(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task Insert(Model.Integration.MaterialGroups entity)
        {
            throw new NotImplementedException();
        }

        public Task Insert(List<Model.Integration.MaterialGroups> entities)
        {
            throw new NotImplementedException();
        }

  
      async  public Task<List<MaterialGroups>> List(List<Criteria> criterias, long page = -1, long size = -1)
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

            string query = Global.MakeODataQuery("MaterialGroups");

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<MaterialGroups> result = new List<MaterialGroups>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        public MaterialGroups toRecord(dynamic record)
        {
            MaterialGroups mt = new MaterialGroups();


            mt.AbsEntry = record.AbsEntry;
            mt.Description = record.Description;
            mt.MaterialGroupCode = record.MaterialGroupCode;
            

            return mt;
        }

        public Task<Pagination> TotalLinhas(long? size, List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task Update(Model.Integration.MaterialGroups entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<Model.Integration.MaterialGroups> entities)
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
