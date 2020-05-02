using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model.Integration;
using System.Linq;
using Varsis.Data.Model;
using static Varsis.Data.Model.ProductTree;

namespace Varsis.Data.Serviceb1.Integration
{
    public class ProductTreeService : IEntityService<Model.ProductTree>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public ProductTreeService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = mountFieldMap();
            _FieldType = mountFieldType();
        }

        public Task<bool> Create()
        {
            throw new NotImplementedException();
        }

        public Task Delete(ProductTree entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task<ProductTree> Find(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task Insert(ProductTree entity)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();
            batch = _serviceLayerConnector.CreateBatch();
            string record = toJsonComponete(entity);

            batch.Post(HttpMethod.Post, "/ProductTrees", record);

            ServiceLayerResponse response = await _serviceLayerConnector.Post(batch);

            if (!response.success)
            {
                string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
            else
            {

                ProductTreeIntegrationStatus status = ProductTreeIntegrationStatus.Processed;

                if (response.internalResponses.Where( x=> x.errorCode == "-2035").ToList().Count == 0 )
                {
                    if (response.internalResponses.Count(m => !m.success) != 0)
                    {
                        status = ProductTreeIntegrationStatus.Error;
                    }
                }
                

                var prod = toJsonComponent(status);
                foreach (var item in entity.productTrees_Lines)
                {
                    string query = Global.BuildQuery($"U_VSITPRODUCT_COMP('{item.RecId}')");
                    var responseStatus = await _serviceLayerConnector.Patch(query, prod, true);

                    if (!responseStatus.success)
                    {
                        string message = $"Erro ao atualizar status de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                        Console.WriteLine(message);
                        throw new ApplicationException(message);
                    }
                }
            

            

            }
        }

        private string toJsonComponent(ProductTreeIntegrationStatus status)
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.U_STATUS = (int)status;
            result = JsonConvert.SerializeObject(record);
            return result;
        }

        public Task Insert(List<ProductTree> entities)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductTree>> List(List<Criteria> criterias, long page = -1, long size = -1)
        {
            throw new NotImplementedException();
        }

        public Task<Pagination> TotalLinhas(long? size, List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task Update(ProductTree entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<ProductTree> entities)
        {
            throw new NotImplementedException();
        }

        private string toJsonComponete(ProductTree productTree)
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.TreeCode = productTree.TreeCode;
            record.TreeType = productTree.TreeType;
            record.Warehouse = productTree.Warehouse;

            var q = from p in productTree.productTrees_Lines select new { p.ItemCode, p.Quantity, p.Warehouse };
            var lista = q.ToList();
            record.ProductTreeLines = lista;

            result = JsonConvert.SerializeObject(record);

            return result;
        }

        private DateTime parseDate(dynamic value)
        {
            DateTime result;

            DateTime.TryParse(value, out result);

            return result;
        }


        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("treecode", "U_TREECODE");
            map.Add("treetype", "U_TREETYPE");
            map.Add("warehouse", "U_WAREHOUSE");

            return map;
        }

        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("treecode", "T");
            map.Add("treetype", "T");
            map.Add("warehouse", "T");

            return map;
        }
    }
}
