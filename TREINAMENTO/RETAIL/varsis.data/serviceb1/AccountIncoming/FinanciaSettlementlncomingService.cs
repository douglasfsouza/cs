using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model.AccountIncoming;

namespace Varsis.Data.Serviceb1.Integration.AccountIncoming
{
    public class FinancialSettlementIncomingService : IEntityService<FinancialSettlementIncoming>
    {
        const string SL_SERVICE_NAME = "U_VSPAGABAT";

        readonly ServiceLayerConnector _serviceLayerConnector;

        readonly Dictionary<string, string> _FieldMap;
        readonly Dictionary<string, string> _FieldType;

        public FinancialSettlementIncomingService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = mountFieldMap();
            _FieldType = mountFieldType();
        }

        public Task<bool> Create()
        {
            throw new NotSupportedException();
        }

        public Task Delete(FinancialSettlementIncoming entity)
        {
            throw new NotSupportedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotSupportedException();
        }

        async public Task<FinancialSettlementIncoming> Find(List<Criteria> criterias)
        {
            FinancialSettlementIncoming result = null;

            try
            {
                List<FinancialSettlementIncoming> lista = await this.List(criterias, -1, -1);

                if (lista.Count > 0)
                {
                    result = lista[0];
                }
            }
            catch
            {
                throw;
            }

            return result;
        }

        public Task Insert(FinancialSettlementIncoming entity)
        {
            throw new NotSupportedException();
        }

        public Task Insert(List<FinancialSettlementIncoming> entities)
        {
            throw new NotSupportedException();
        }

        async public Task<List<FinancialSettlementIncoming>> List(List<Criteria> criterias, long page, long size)
        {
            var filter = Global.parseCriterias(criterias, _FieldMap, _FieldType).ToArray();
            var query = Global.MakeODataQuery(SL_SERVICE_NAME, null, filter.Length == 0 ? null : filter);
            var data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<FinancialSettlementIncoming> result = new List<FinancialSettlementIncoming>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        public Task Update(FinancialSettlementIncoming entity)
        {
            throw new NotSupportedException();
        }

        public Task Update(List<FinancialSettlementIncoming> entities)
        {
            throw new NotSupportedException();
        }

        private string toJson(FinancialSettlementIncoming entity)
        {
            dynamic record = new ExpandoObject();

            record.Code = entity.Code;
            record.Name = entity.Name;
            record.U_Codfor = entity.CodigoFornecedor;
            record.U_Code_pag = entity.TituloPagar;
            record.U_Code_rec = entity.TituloReceber;
            record.U_Valor_aba = entity.ValorAbatimento;
            record.U_Data_aba = entity.DataTransacao;
            record.U_Usuario = entity.UsuarioTransacao;

            var result = JsonConvert.SerializeObject(record);

            return result;
        }

        private FinancialSettlementIncoming toRecord(dynamic record)
        {
            FinancialSettlementIncoming entity = new FinancialSettlementIncoming();

            entity.Code = Convert.ToString(record.Code);
            entity.Name = Convert.ToString(record.Name);
            entity.CodigoFornecedor = Convert.ToString(record.U_Codfor);
            entity.TituloPagar = Convert.ToString(record.U_Code_pag);
            entity.TituloReceber = Convert.ToString(record.U_Code_rec);
            entity.ValorAbatimento = Convert.ToDouble(record.U_Valor_aba);
            entity.DataTransacao = ((string)record.U_Data_aba).toDate().Value;
            entity.UsuarioTransacao = Convert.ToString(record.U_Usuario);

            return entity;
        }
        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "Code");
            map.Add("code", "Code");
            map.Add("codigofornecedor", "U_Codfor");
            map.Add("titulopagar", "U_Code_pag");
            map.Add("tituloreceber", "U_Code_rec");
            map.Add("datatransacao", "U_Data_aba");
            map.Add("usuariotransacao", "U_Usuario");

            return map;
        }

        async public Task<Varsis.Data.Infrastructure.Pagination> TotalLinhas(long? size, List<Criteria> criterias)
        {
            return new Varsis.Data.Infrastructure.Pagination();
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "T");
            map.Add("code", "T");
            map.Add("codigofornecedor", "T");
            map.Add("titulopagar", "T");
            map.Add("tituloreceber", "T");
            map.Add("datatransacao", "T");
            map.Add("usuariotransacao", "T");

            return map;
        }
        
    }
}
