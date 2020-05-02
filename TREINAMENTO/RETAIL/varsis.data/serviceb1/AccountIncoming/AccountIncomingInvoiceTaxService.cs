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
    public class AccountIncomingInvoiceTaxService : IEntityService<AccountIncomingInvoiceTax>
    {
        const string SL_SERVICE_NAME = "U_VSPAGIMPOSTOS";

        readonly ServiceLayerConnector _serviceLayerConnector;

        readonly Dictionary<string, string> _FieldMap;
        readonly Dictionary<string, string> _FieldType;

        public AccountIncomingInvoiceTaxService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = this.mountFieldMap();
            _FieldType = this.mountFieldType();
        }

        public Task<bool> Create()
        {
            throw new NotSupportedException();
        }

        public Task Delete(AccountIncomingInvoiceTax entity)
        {
            throw new NotSupportedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotSupportedException();
        }

        async public Task<AccountIncomingInvoiceTax> Find(List<Criteria> criterias)
        {
            AccountIncomingInvoiceTax result = null;

            try
            {
                List<AccountIncomingInvoiceTax> lista = await this.List(criterias, -1, -1);

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

        public Task Insert(AccountIncomingInvoiceTax entity)
        {
            throw new NotSupportedException();
        }

        public Task Insert(List<AccountIncomingInvoiceTax> entities)
        {
            throw new NotSupportedException();
        }

        async public Task<List<AccountIncomingInvoiceTax>> List(List<Criteria> criterias, long page, long size)
        {
            var filter = Global.parseCriterias(criterias, _FieldMap, _FieldMap).ToArray();
            var query = Global.MakeODataQuery(SL_SERVICE_NAME, null, filter.Length == 0 ? null : filter);
            var data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<AccountIncomingInvoiceTax> result = new List<AccountIncomingInvoiceTax>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        private string[] parseCriteria(List<Criteria> criterias)
        {
            var sliceCriteria = criterias.FirstOrDefault(m => m.Field.ToLower() == "slice");

            if (sliceCriteria != null)
            {
                criterias.Remove(sliceCriteria);
            }

            var filter = Global.parseCriterias(criterias, _FieldMap, _FieldType).ToList();

            if (sliceCriteria != null)
            {
                switch (sliceCriteria.Value.ToLower())
                {
                    case "open":
                        filter.Add($"(U_Dt_Pagto eq null and U_Vencto ge '{DateTime.Now.ToString("yyyy-MM-dd")}')");
                        break;
                    case "overdue":
                        filter.Add($"(U_Dt_Pagto eq null and U_Vencto lt '{DateTime.Now.ToString("yyyy-MM-dd")}')");
                        break;
                    case "closed":
                        filter.Add($"(U_Status eq 'L' or U_Status eq 'C' or U_Dt_Pagto ne null)");
                        break;
                    case "all":
                        break;
                    default:
                        break;
                }
            }

            return filter.ToArray();
        }

        public Task Update(AccountIncomingInvoiceTax entity)
        {
            throw new NotSupportedException();
        }

        public Task Update(List<AccountIncomingInvoiceTax> entities)
        {
            throw new NotSupportedException();
        }

        private string toJson(AccountIncomingInvoiceTax entity)
        {
            dynamic record = new ExpandoObject();

            record.Code = entity.Code;
            record.Name = entity.Name;
            record.U_Code_titulo = entity.Titulo;
            record.U_Fato = entity.Fato;
            record.U_Data = entity.DataTransacao;
            record.U_Cod_imposto = entity.CodigoImposto;
            record.U_Valbase = entity.ValorBase;
            record.U_Aliquota = entity.Aliquota;
            record.U_Valimp = entity.ValorImposto;
            record.U_Valret = entity.ValorRetencao;

            var result = JsonConvert.SerializeObject(record);

            return result;
        }

        private AccountIncomingInvoiceTax toRecord(dynamic record)
        {
            AccountIncomingInvoiceTax entity = new AccountIncomingInvoiceTax();

            //entity.RecId = Guid.Parse(record.Code);

            entity.Code = Convert.ToString(record.Code);
            entity.Code = Convert.ToString(record.Code);
            entity.Name = Convert.ToString(record.Name);
            entity.Titulo = Convert.ToString(record.U_Code_titulo);
            entity.Fato = Convert.ToString(record.U_Fato);
            entity.DataTransacao = ((string)record.U_Data).toDate().Value;
            entity.CodigoImposto = Convert.ToString(record.U_Cod_imposto);
            entity.ValorBase = Convert.ToDouble(record.U_Valbase);
            entity.Aliquota = Convert.ToDouble(record.U_Aliquota);
            entity.ValorImposto = Convert.ToDouble(record.U_Valimp);
            entity.ValorRetencao = Convert.ToDouble(record.U_Valret);

            return entity;
        }

        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "Code");
            map.Add("code", "Code");
            map.Add("name", "Name");
            map.Add("titulo", "U_Code_titulo");
            map.Add("fato", "U_Fato");
            map.Add("datatransacao", "U_Data");
            map.Add("codigoimposto", "U_Cod_imposto");

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
            map.Add("name", "T");
            map.Add("titulo", "T");
            map.Add("fato", "T");
            map.Add("datatransacao", "T");
            map.Add("codigoimposto", "T");

            return map;
        }
        
    }
}
