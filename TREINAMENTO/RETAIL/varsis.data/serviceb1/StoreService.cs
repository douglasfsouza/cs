using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model;

namespace Varsis.Data.Serviceb1
{
    public class StoreService : IEntityService<Model.Store>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;

        public StoreService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
        }
        public Task<bool> Create()
        {
            throw new NotImplementedException();
        }

        public Task Delete(Store entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task<Store> Find(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task Insert(Store entity)
        {
            throw new NotImplementedException();
        }

        public Task Insert(List<Store> entities)
        {
            throw new NotImplementedException();
        }
        async public Task<Varsis.Data.Infrastructure.Pagination> TotalLinhas(long? size, List<Criteria> criterias)
        {
            


            Varsis.Data.Infrastructure.Pagination page = new Varsis.Data.Infrastructure.Pagination();
            //string query = Global.MakeODataQuery("U_VSITENTIDADECONT/$count", null, filter.Count == 0 ? null : filter.ToArray(), null, 1, 0);
            //string data = await _serviceLayerConnector.getQueryResult(query);
            //page.Linhas = Convert.ToInt64(data);
            //page.Paginas = (Convert.ToInt64(data) / size.Value) + 1;
            //page.qtdPorPagina = size.Value == 0 ? Convert.ToInt64(data) : size.Value;
            return page;
        }
        async public Task<List<Store>> List(List<Criteria> criterias, long page, long size)
        {
            string query = Global.BuildQuery("BusinessPlaces");

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<Store> result = new List<Store>();

            foreach (dynamic o in lista)
            {
                result.Add(new Store()
                {
                    code = o.BPLID,
                    name = o.BPLName
                });
            }

            return result;
        }

        public Task Update(Store entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<Store> entities)
        {
            throw new NotImplementedException();
        }
    }
}
