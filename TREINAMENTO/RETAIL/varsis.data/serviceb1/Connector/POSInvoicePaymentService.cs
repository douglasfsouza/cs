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
using Varsis.Data.Model.Connector;

namespace Varsis.Data.Serviceb1.Connector
{
    public class POSInvoicePaymentService : IEntityService<Model.Connector.POSInvoicePayment>
    {
        const string SL_TABLE_NAME = "IncomingPayments";
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;
        public POSInvoicePaymentService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = mountFieldMap();
            _FieldType = mountFieldType();

        }
        public Task<bool> Create()
        {
            throw new NotImplementedException();
        }

        public Task Delete(POSInvoicePayment entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task<POSInvoicePayment> Find(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task Insert(POSInvoicePayment entity)
        {

            ServiceLayerResponse response = await _serviceLayerConnector.Post(SL_TABLE_NAME, JsonConvert.SerializeObject(entity));

            if (!response.success)
            {
                string message = $"Erro ao enviar transação de '{entity.CardCode}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
        }

        public Task Insert(List<POSInvoicePayment> entities)
        {
            throw new NotImplementedException();
        }

        public Task<List<POSInvoicePayment>> List(List<Criteria> criterias, long page = -1, long size = -1)
        {
            throw new NotImplementedException();
        }

        public Task<Pagination> TotalLinhas(long? size, List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task Update(POSInvoicePayment entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<POSInvoicePayment> entities)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            /*    map.Add("recid", "Code");
                map.Add("branchid", "U_BPLID");
                map.Add("branchidlegacy", "U_BPLIDLGCY");
                */
            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            /*
            map.Add("recid", "T");
            map.Add("branchid", "N");
            map.Add("branchidlegacy", "T");
            */
            return map;
        }
    }
}
