using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model.Connector;
using System.Linq;

namespace Varsis.Data.Serviceb1.Connector
{
    public class BusinessPartnersService : IEntityService<BusinessPartners>
    {
        const string SL_TABLE_NAME = "BusinessPartners";

        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        readonly CountiesService countiesService;

        public BusinessPartnersService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = mountFieldMap();
            _FieldType = mountFieldType();

            countiesService = new CountiesService(serviceLayerConnector);
        }

        public Task<bool> Create()
        {
            throw new NotSupportedException();
        }

        public Task Delete(BusinessPartners entity)
        {
            throw new NotSupportedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotSupportedException();
        }

        async public Task<BusinessPartners> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"{SL_TABLE_NAME}('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            BusinessPartners entity = null;

            if (record != null)
            {
                entity = toRecord(record);
            }

            return entity;
        }

        async public Task Insert(BusinessPartners entity)
        {
            string record = toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Post(SL_TABLE_NAME, record);

            if (!response.success)
            {
                string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                throw new ApplicationException(message);
            }
        }

        public Task Insert(List<BusinessPartners> entities)
        {
            throw new NotSupportedException();
        }

        async public Task<Varsis.Data.Infrastructure.Pagination> TotalLinhas(long? size, List<Criteria> criterias)
        {
            return new Varsis.Data.Infrastructure.Pagination();
        }

        async public Task<List<BusinessPartners>> List(List<Criteria> criterias, long page, long size)
        {
            var filter = parseCriterias(criterias);

            string query = Global.MakeODataQuery(SL_TABLE_NAME, null, filter.Length == 0 ? null : filter, null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<BusinessPartners> result = new List<BusinessPartners>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        async public Task Update(BusinessPartners entity)
        {
            string record = toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Put($"{SL_TABLE_NAME}('{entity.RecId}')", record);

            if (!response.success)
            {
                string message = $"Erro ao atualizar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                throw new ApplicationException(message);
            }
        }

        public Task Update(List<BusinessPartners> entities)
        {
            throw new NotSupportedException();
        }

        private string toJson(BusinessPartners entity)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            if (!"FC".Contains(entity.Tipo[0]))
            {
                throw new ArgumentOutOfRangeException($"Parâmetro inválido [Tipo] = {entity.Tipo}");
            }

            record.CardCode = entity.Codigo;
            record.CardType = entity.Tipo == "C" ? "cCustomer" : "cSupplier";
            record.CardName = entity.RazaoSocial;
            record.CardForeignName = entity.NomeFantasia;
            record.GroupCode = entity.CodigoGrupo;
            record.DebitorAccount = entity.ContaFinanceira;
            record.DownPaymentClearAct = entity.ContaAdiantamento;
            //record.U_VAR_Grp_Retido = entity.CodigoGrupoImpostoRetido;
            record.U_VAR_IntMmais = "Y";
            record.Phone1 = entity.FoneFixo;
            record.Phone2 = entity.DDDFoneFixo;
            record.EmailAddress = entity.Email;

            record.BPAddresses = makeAddress(entity);
            record.BPFiscalTaxIDCollection = makeFiscalTax(entity);
            record.BPAccountReceivablePaybleCollection = makeAccountRP(entity);

            result = JsonConvert.SerializeObject(record);

            return result;
        }

        private List<dynamic> makeAddress(BusinessPartners entity)
        {
            List<dynamic> result = new List<dynamic>();

            dynamic address = new ExpandoObject();


            List<Criteria> countiesCriterias = new List<Criteria>()
            {
                {new Criteria() { Field = "name", Operator = "eq", Value=entity.Municipio } }
            };

            Model.Counties county = countiesService.Find(countiesCriterias).Result;

            address.AddressName = "PAGAR";
            address.AddressType = "bo_BillTo";
            address.TypeOfAddress = entity.TipoLogradouro;
            address.Street = entity.Logradouro;
            address.StreetNo = entity.NumeroLogradouro;
            address.BuildingFloorRoom = entity.ComplementoLogradouro;
            address.ZipCode = entity.CEP;
            address.Block = entity.Bairro;
            address.City = entity.Municipio;
            address.State = entity.Estado;
            address.Country = entity.Pais;

            if (county?.AbsId != null)
            {
                address.County = county.AbsId.ToString();
            }

            result.Add(address);

            return result;
        }

        private List<dynamic> makeFiscalTax(BusinessPartners entity)
        {
            List<dynamic> result = new List<dynamic>();

            dynamic fiscalTax = new ExpandoObject();

            //fiscalTax.Address = "PAGAR";
            //fiscalTax.AddrType = "bo_BillTo";
            fiscalTax.TaxId0 = entity.CNPJ;
            fiscalTax.TaxId1 = entity.InscricaoEstadual;
            fiscalTax.TaxId3 = entity.InscricaoMunicipal;
            fiscalTax.TaxId4 = entity.CPF;
            fiscalTax.TaxId5 = entity.IdEstrangeiro;

            result.Add(fiscalTax);

            return result;
        }

        private List<dynamic> makeAccountRP(BusinessPartners entity)
        {
            List<dynamic> result = new List<dynamic>();

            dynamic accountRP = new ExpandoObject();

            if (!string.IsNullOrEmpty(entity.ContaBoleto))
            {
                accountRP = new ExpandoObject();
                accountRP.AccountType = entity.Tipo == "C" ? "bpat_Receivable" : "bpat_Payable";
                accountRP.AccountCode = entity.ContaBoleto;
                result.Add(accountRP);
            }

            if (!string.IsNullOrEmpty(entity.ContaEmAberto))
            {
                accountRP = new ExpandoObject();
                accountRP.AccountType = "bpat_OpenDebts";
                accountRP.AccountCode = entity.ContaEmAberto;
                result.Add(accountRP);
            }

            return result;
        }

        private BusinessPartners toRecord(dynamic record)
        {
            BusinessPartners entity = new BusinessPartners();

            entity.Codigo = record.CardCode;

            entity.Tipo = record.CardType == "cCustomer" ? "C" : "F";
            entity.RazaoSocial = record.CardName;
            entity.NomeFantasia = record.CardForeignName;
            entity.CodigoGrupo = Convert.ToInt64(record.GroupCode);
            entity.ContaFinanceira = record.DebitorAccount;
            entity.ContaAdiantamento = record.DownPaymentClearAct;
            //entity.CodigoGrupoImpostoRetido = record.U_VAR_Grp_Retido;
            entity.FoneFixo = record.Phone1;
            entity.DDDFoneFixo = record.Phone2;
            entity.Email = record.EmailAddress;

            if (record.BPAddresses != null)
            {
                foreach (var address in record.BPAddresses)
                {
                    if (address.AddressName == "PAGAR")
                    {
                        entity.TipoLogradouro = address.TypeOfAddress;
                        entity.Logradouro = address.Street;
                        entity.NumeroLogradouro = address.StreetNo;
                        entity.ComplementoLogradouro = address.BuildingFloorRoom;
                        entity.CEP = address.ZipCode;
                        entity.Bairro = address.Block;
                        entity.Municipio = address.City;
                        entity.Estado = address.State;
                        entity.Pais = address.Country;
                        break;
                    }
                }
            }

            if (record.BPFiscalTaxIDCollection != null)
            {
                foreach (var fiscalTax in record.BPFiscalTaxIDCollection)
                {
                    if (fiscalTax.Address == "PAGAR")
                    {
                        entity.CNPJ = fiscalTax.TaxId0;
                        entity.InscricaoEstadual = fiscalTax.TaxId1;
                        entity.InscricaoMunicipal = fiscalTax.TaxId3;
                        entity.CPF = fiscalTax.TaxId4;
                        entity.IdEstrangeiro = fiscalTax.TaxId5;

                        break;
                    }
                }
            }

            if (record.BPAccountReceivablePaybleCollection != null)
            {
                foreach (var accountRP in record.BPAccountReceivablePaybleCollection)
                {
                    switch (accountRP.AccountType ?? "")
                    {
                        case "bpat_Payable":
                        case "bpat_Receivable":
                            entity.ContaBoleto = accountRP.AccountCode;
                            break;

                        case "bpat_OpenDebts":
                            entity.ContaEmAberto = accountRP.AccountCode;
                            break;
                    }
                }
            }

            return entity;
        }

        private string[] parseCriterias(List<Criteria> criterias)
        {
            List<string> filter = new List<string>();
            if (criterias != null)
            {
                if (criterias?.Count != 0)
                {
                    foreach (var c in criterias)
                    {
                        string type = string.Empty;
                        string field = string.Empty;

                        if (_FieldMap.ContainsKey(c.Field.ToLower()))
                        {
                            field = _FieldMap[c.Field.ToLower()];
                            type = _FieldType[c.Field.ToLower()];
                        }
                        else
                        {
                            field = c.Field;
                            type = "T";
                        }

                        string value = string.Empty;

                        if (type == "T")
                        {
                            value = $"'{c.Value}'";
                        }
                        else if (type == "N")
                        {
                            value = $"{c.Value}";
                        }

                        switch (c.Operator.ToLower())
                        {
                            case "startswith":
                                filter.Add($"startswith({field},{value})");
                                break;

                            default:
                                filter.Add($"{field} {c.Operator.ToLower()} {value}");
                                break;
                        }
                    }
                }
            }

            return filter.ToArray();
        }

        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "CardCode");
            map.Add("code", "CardCode");
            map.Add("cardcode", "CardCode");

            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "T");
            map.Add("code", "T");
            map.Add("cardcode", "T");

            return map;
        }
    }
}
