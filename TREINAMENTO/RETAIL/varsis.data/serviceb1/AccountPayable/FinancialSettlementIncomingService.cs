using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model.AccountPayable;

namespace Varsis.Data.Serviceb1.Integration.AccountPayable
{
    public class FinancialSettlementIncomingService : IEntityService<FinancialSettlement>
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

        public Task Delete(FinancialSettlement entity)
        {
            throw new NotSupportedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotSupportedException();
        }

        async public Task<FinancialSettlement> Find(List<Criteria> criterias)
        {
            FinancialSettlement result = null;

            try
            {
                List<FinancialSettlement> lista = await this.List(criterias,-1,-1);

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

        public Task Insert(FinancialSettlement entity)
        {
            throw new NotSupportedException();
        }

        public Task Insert(List<FinancialSettlement> entities)
        {
            throw new NotSupportedException();
        }

        async public Task<List<FinancialSettlement>> List(List<Criteria> criterias, long page, long size)
        {
            var filter = Global.parseCriterias(criterias, _FieldMap, _FieldType).ToArray();
            var query = Global.MakeODataQuery(SL_SERVICE_NAME, null, filter.Length == 0 ? null : filter);
            var data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<FinancialSettlement> result = new List<FinancialSettlement>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        public Task Update(FinancialSettlement entity)
        {
            throw new NotSupportedException();
        }

        public Task Update(List<FinancialSettlement> entities)
        {
            throw new NotSupportedException();
        }

        private string toJson(FinancialSettlement entity)
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

        private FinancialSettlement toRecord(dynamic record)
        {
            FinancialSettlement entity = new FinancialSettlement();

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

        /*
        private Dictionary<string, FieldMapItem> mountFieldMap()
        {
            Dictionary<string, FieldMapItem> map = new Dictionary<string, FieldMapItem>();

            map.Add("recid", new FieldMapItem() { name = "Code", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("code", new FieldMapItem() { name = "Code", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("codigofornecedor", new FieldMapItem() { name = "U_Codfor", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("titulopagar", new FieldMapItem() { name = "U_Code_pag", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("tituloreceber", new FieldMapItem() { name = "U_Code_rec", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("datatransacao", new FieldMapItem() { name = "U_Data_aba", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("usuariotransacao", new FieldMapItem() { name = "U_Usuario", type = FieldMapItem.FieldTypeEnum.Alpha });

            return map;
        }*/
    }
}
