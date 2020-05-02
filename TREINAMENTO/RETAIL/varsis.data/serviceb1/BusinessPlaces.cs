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

namespace Varsis.Data.Serviceb1.Integration
{
    public class BusinessPlacesService : IEntityService<Model.BusinessPlaces>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public BusinessPlacesService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = mountFieldMap();
            _FieldType = mountFieldType();
        }

        async public Task<bool> Create()
        {
            bool result = false;

            result = await createTable();
            return result;
        }

        async private Task<bool> createTable()
        {
            throw new NotImplementedException();
        }

        public Task Delete(BusinessPlaces entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task<BusinessPlaces> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"BusinessPlaces('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            BusinessPlaces businessplaces = toRecord(record);

            // Recupera as linhas da nota iscal
            string[] filter = new string[]
            {
                $"Code eq '{recid}'"
            };

            query = Global.MakeODataQuery("BusinessPlaces", null, filter);

            data = await _serviceLayerConnector.getQueryResult(query);

            return businessplaces;
        }

        async public Task Insert(BusinessPlaces entity)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();
            string codigoWH = criaCodigoWareHouses(entity.BPLName, entity.BPLID.ToString(), "01");
            entity.DefaultWarehouseID = codigoWH;
            entity.DefaultResourceWarehouseID = codigoWH;
            string recordWareHouses = toJsonwarehouses(entity.BPLName, codigoWH);
            batch.Post(HttpMethod.Post, "/Warehouses", recordWareHouses);
            ServiceLayerResponse response = await _serviceLayerConnector.Post("Warehouses", recordWareHouses);

            codigoWH = criaCodigoWareHouses(entity.BPLName, entity.BPLID.ToString(), "99");
            //entity.DefaultWarehouseID = codigoWH;
            //entity.DefaultResourceWarehouseID = codigoWH;
            recordWareHouses = toJsonwarehouses(entity.BPLName, codigoWH);
            batch.Post(HttpMethod.Post, "/Warehouses", recordWareHouses);
            response = await _serviceLayerConnector.Post("Warehouses", recordWareHouses);

            //businessPlaces
            batch = _serviceLayerConnector.CreateBatch();
            entity.BPLNameForeign = entity.BPLName;
            string record = toJson(entity);
            batch.Post(HttpMethod.Post, "/BusinessPlaces", record);
            response = await _serviceLayerConnector.Post("BusinessPlaces", record);

            if (entity.CardCode.Contains("D"))
            {

            }
            //businesspartners F
            entity.CardType = "cSupplier";
            record = toJsonPartners(entity, "F");
            batch.Post(HttpMethod.Post, "/BusinessPartners", record);
            response = await _serviceLayerConnector.Post("BusinessPartners", record);

            //businesspartners C
            entity.CardType = "cCustomer";
            record = toJsonPartners(entity, "C");
            batch.Post(HttpMethod.Post, "/BusinessPartners", record);
            response = await _serviceLayerConnector.Post("BusinessPartners", record);

            //descomentar daqui pra baixo
            //IBatchProducer batch = _serviceLayerConnector.CreateBatch();
            //string codigoWH = criaCodigoWareHouses(entity.BPLName, entity.BPLID.ToString(), "01");
            //entity.DefaultWarehouseID = codigoWH;
            //entity.DefaultResourceWarehouseID = codigoWH;
            //string recordFiliais = toJson(entity);
            //string recordWareHouses = toJsonwarehouses(entity.BPLName, codigoWH);
            //batch.Post(HttpMethod.Post, "/Warehouses", recordWareHouses);
            //batch.Post(HttpMethod.Post, "/BusinessPlaces", recordFiliais);
            //ServiceLayerResponse response = await _serviceLayerConnector.Post(batch);


            if (!response.success && !(response.errorMessage.Contains("1320000140")))
            {
                string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
            else
            {
                var idEntidade = entity.RecId;
                var cad = toJsonEntidades();
                string query = Global.BuildQuery($"U_VSITENTIDADE('{idEntidade}')");
                response = await _serviceLayerConnector.Patch(query, cad, true);
            }
        }

        private string toJsonPartners(BusinessPlaces businesspartners, string LOJ_CLI)
        {
            
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.ContactEmployees = businesspartners.ContactEmployees;
            record.BillToBuildingFloorRoom = businesspartners.BillToBuildingFloorRoom;
            record.AliasName = businesspartners.AliasName;
            record.CardCode = businesspartners.CardCode.Replace("-", LOJ_CLI);
            record.CardName = businesspartners.CardName;
            record.CardType = businesspartners.CardType;
            if (businesspartners.FatherCard != null & businesspartners.FatherCard != "" && !businesspartners.FatherCard.Contains("000000"))
            {
                record.FatherCard = businesspartners.FatherCard;
            }
            record.RelationshipDateFrom = businesspartners.RelationshipDateFrom;
            record.RelationshipDateTill = businesspartners.RelationshipDateTill;
            record.BPAddresses = businesspartners.BPAddresses;
            record.BPFiscalTaxIDCollection = businesspartners.BPFiscalTaxIDCollection;
            record.BPBankAccounts = businesspartners.BPBankAccounts;
            record.Address = businesspartners.Address;
            result = JsonConvert.SerializeObject(record);
            return result;
        }

        public Task Insert(List<BusinessPlaces> entities)
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
                        filter.Add($"{field} {c.Operator.ToLower()} '{c.Value}'");
                    }
                    else if (type == "N")
                    {
                        filter.Add($"{field} {c.Operator.ToLower()} {c.Value}");
                    }
                }
            }


            Varsis.Data.Infrastructure.Pagination page = new Varsis.Data.Infrastructure.Pagination();
            string query = Global.MakeODataQuery("U_VSITENTIDADECONT/$count", null, filter.Count == 0 ? null : filter.ToArray(), null, 1, 0);
            string data = await _serviceLayerConnector.getQueryResult(query);
            page.Linhas = Convert.ToInt64(data);
            page.Paginas = (Convert.ToInt64(data) / size.Value) + 1;
            page.qtdPorPagina = size.Value == 0 ? Convert.ToInt64(data) : size.Value;
            return page;
        }
        async public Task<List<BusinessPlaces>> List(List<Criteria> criterias, long page, long size)
        {
            List<string> filter = new List<string>();

            if (criterias?.Count != 0)
            {
                foreach(var c in criterias)
                {
                    string field = _FieldMap[c.Field.ToLower()];
                    string type = _FieldType[c.Field.ToLower()];

                    if (type == "T")
                    {
                        filter.Add($"{field} {c.Operator.ToLower()} '{c.Value}'");
                    }
                    else if(type == "N")
                    {
                        filter.Add($"{field} {c.Operator.ToLower()} {c.Value}");
                    }
                }
            }

            string query = Global.MakeODataQuery("BusinessPlaces", null, filter.Count == 0 ? null : filter.ToArray(),null, page -1, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<BusinessPlaces> result = new List<BusinessPlaces>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        public Task Update(BusinessPlaces entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<BusinessPlaces> entities)
        {
            throw new NotImplementedException();
        }

        private List<TableColumn> createColumns()
        {
            List<TableColumn> lista = new List<TableColumn>();
            
            return lista;
        }

        private List<TableIndexes> createIndexes()
        {
            List<TableIndexes> lista = new List<TableIndexes>();

            lista.Add(new TableIndexes()
            {
                name = "PK",
                isUnique = true,
                keys = new string[] { "INVOICEDIRECTION", "INVOICEID", "INVOICESERIES", "INVOICEDATE", "ISSUERID" }
            });

            lista.Add(new TableIndexes()
            {
                name = "STATUS",
                isUnique = false,
                keys = new string[] { "STATUS" }
            });

            return lista;
        }

        private List<TableIndexes> createIndexesItem()
        {
            List<TableIndexes> lista = new List<TableIndexes>();

            lista.Add(new TableIndexes()
            {
                name = "ITEMID",
                isUnique = true,
                keys = new string[] { "INVOICECODE", "ITEMID" }
            });

            return lista;
        }
        private string toJsonwarehouses(string nomeLoja, string codigoLoja)
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.WarehouseCode = codigoLoja;
            record.WarehouseName = nomeLoja;
            result = JsonConvert.SerializeObject(record);
            return result;
        }
        private string toJsonEntidades()
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.U_STATUS = Varsis.Data.Model.Integration.VSITENTIDADE.VSITENTIDADEIntegrationStatus.Processed;
            record.U_DATA_INTEGRACAO = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            result = JsonConvert.SerializeObject(record);
            return result;
        }
        private string criaCodigoWareHouses(string nomeLoja, string codigoLoja, string codigoWareHouse)
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            string codigo = codigoLoja;
            string zeros = "";

            if (codigo.Count() < 4)
            {
                int cont = codigo.Count();
                int faltam = 4 - codigo.Count();
                if (faltam < 4)
                {
                    for (var i = 0; i < faltam; i++)
                    {
                        zeros += "0";
                    }
                }
            }
            codigo = zeros + codigoLoja;
            return codigo + "_" + codigoWareHouse;
        }

        private string toJson(BusinessPlaces businessplaces)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            //record.Code = businessplaces.RecId.ToString();
            record.AdditionalIdNumber = businessplaces.AdditionalIdNumber;
            record.Address = businessplaces.Address;
            record.Addressforeign = businessplaces.Addressforeign;
            record.AddressType = businessplaces.AddressType;
            record.AliasName = businessplaces.AliasName;
            record.Block = businessplaces.Block;
            //record.BPLID = businessplaces.BPLID;
            record.BPLName = businessplaces.BPLName;
            record.BPLNameForeign = businessplaces.BPLNameForeign;
            record.Building = businessplaces.Building;
            record.Business = businessplaces.Business;
            record.City = businessplaces.City;
            record.CommercialRegister = businessplaces.CommercialRegister;
            record.CompanyQualificationCode = businessplaces.CompanyQualificationCode;
            record.CooperativeAssociationTypeCode = businessplaces.CooperativeAssociationTypeCode;
            record.Country = parseCountry(businessplaces.Country);
            record.County = "5344";
            record.CreditContributionOriginCode = businessplaces.CreditContributionOriginCode;
            record.DateOfIncorporation = businessplaces.DateOfIncorporation;
            record.DeclarerTypeCode = businessplaces.DeclarerTypeCode;
            record.DefaultCustomerID = businessplaces.DefaultCustomerID;
            record.DefaultResourceWarehouseID = businessplaces.DefaultResourceWarehouseID;
            record.DefaultTaxCode = businessplaces.DefaultTaxCode;
            record.DefaultVendorID = businessplaces.DefaultVendorID;

            record.DefaultWarehouseID = businessplaces.DefaultWarehouseID;
            record.Disabled = "tNO";
            record.EconomicActivityTypeCode = businessplaces.EconomicActivityTypeCode;
            record.EnvironmentType = businessplaces.EnvironmentType;
            record.FederalTaxID = businessplaces.FederalTaxID;
            record.FederalTaxID2 = businessplaces.FederalTaxID2;
            record.FederalTaxID3 = businessplaces.FederalTaxID3;
            record.GlobalLocationNumber = businessplaces.GlobalLocationNumber;    
            record.Industry = businessplaces.Industry;
            record.IPIPeriodCode = businessplaces.IPIPeriodCode; 
            record.NatureOfCompanyCode = businessplaces.NatureOfCompanyCode;
            record.PaymentClearingAccount = businessplaces.PaymentClearingAccount;
            record.PreferredStateCode = businessplaces.PreferredStateCode;
            record.ProfitTaxationCode = businessplaces.ProfitTaxationCode;
            record.RepName = businessplaces.RepName;
            record.SPEDProfile = businessplaces.SPEDProfile;
            record.State = businessplaces.State;
            record.Street = businessplaces.Street;
            record.StreetNo = businessplaces.StreetNo;
            record.TaxOffice = businessplaces.TaxOffice;
            record.TaxOfficeNo = businessplaces.TaxOfficeNo;

            record.VATRegNum = businessplaces.VATRegNum;
            record.ZipCode = businessplaces.ZipCode;

            //record.IENumbers = businessplaces.
            //record.MainBPL = businessplaces.
            //record.Opting4ICMS = businessplaces.
            //record.TributaryInfos = businessplaces.
            //record.UserFields = businessplaces.
            //record.Browser = businessplaces.
            result = JsonConvert.SerializeObject(record);

            return result;
        }
        private string toJsonTeste(BusinessPlaces businessplaces)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            //record.Code = businessplaces.RecId.ToString();
            record.AdditionalIdNumber = null;
            record.Address = "Avenue Paulista,13";
            record.Addressforeign = "Avenue Paulista,13";
            record.AddressType = "Avenida";
            record.AliasName = "OEC Computadores DEMONSTRAÇÃO";
            record.Block = "Vila Almeida";
            record.BPLID = 2142;
            record.BPLName = "MASTERCARD ON LINE";
            record.BPLNameForeign = "MASTERCARD ON LINE";
            record.Building = "cj 1284";
            record.Business = null; //
            record.City = "São Paulo";
            record.CommercialRegister = null;
            record.CompanyQualificationCode = 1;
            record.CooperativeAssociationTypeCode = -1;
            record.Country = "BR"; //
            record.County = "5344"; //
            record.CreditContributionOriginCode = 1;
            record.DateOfIncorporation = "2010-01-01";
            record.DeclarerTypeCode = 1;
            record.DefaultCustomerID = businessplaces.DefaultCustomerID;
            record.DefaultResourceWarehouseID = 1;
            record.DefaultTaxCode = 420;
            record.DefaultVendorID = null;
            record.DefaultWarehouseID = 1;
            //record.Disabled = businessplaces.Disabled;
            record.EconomicActivityTypeCode = 0;
            record.EnvironmentType = 1;
            record.FederalTaxID = businessplaces.FederalTaxID;
            record.FederalTaxID2 = businessplaces.FederalTaxID2;
            record.FederalTaxID3 = businessplaces.FederalTaxID3;
            record.GlobalLocationNumber = businessplaces.GlobalLocationNumber;
            record.Industry = businessplaces.Industry;
            record.IPIPeriodCode = 0; 
            record.NatureOfCompanyCode = 0;
            record.PaymentClearingAccount = businessplaces.PaymentClearingAccount;
            record.PreferredStateCode = "SP";
            record.ProfitTaxationCode = 3;
            record.RepName = businessplaces.RepName;
            record.SPEDProfile = "A";
            record.State = businessplaces.State;
            record.Street = "Paulista";
            record.StreetNo = "13.999";
            record.TaxOffice = businessplaces.TaxOffice;
            record.TaxOfficeNo = businessplaces.TaxOfficeNo;

            record.VATRegNum = businessplaces.VATRegNum;
            record.ZipCode = businessplaces.ZipCode;

            //record.IENumbers = businessplaces.
            //record.MainBPL = businessplaces.
            //record.Opting4ICMS = businessplaces.
            //record.TributaryInfos = businessplaces.
            //record.UserFields = businessplaces.
            //record.Browser = businessplaces.
            result = JsonConvert.SerializeObject(record);

            return result;
        }
        private BusinessPlaces toRecord(dynamic record)
        {
            BusinessPlaces businessplaces = new BusinessPlaces();
            //businessplaces.RecId = Guid.Parse(record.Code);
            businessplaces.AdditionalIdNumber = record.AdditionalIdNumber;
            businessplaces.Address = record.Address;
            businessplaces.Addressforeign = record.Addressforeign;
            businessplaces.AddressType = record.AddressType;
            businessplaces.AliasName = record.AliasName;
            businessplaces.Block = record.Block;
            businessplaces.BPLID = record.BPLID;
            businessplaces.BPLName = record.BPLName;
            businessplaces.BPLNameForeign = record.BPLNameForeign;
            businessplaces.Building = record.Building;
            businessplaces.Business = record.Business;
            businessplaces.City = record.City;
            businessplaces.CommercialRegister = record.CommercialRegister;
            businessplaces.CompanyQualificationCode = record.CompanyQualificationCode;
            businessplaces.CooperativeAssociationTypeCode = record.CooperativeAssociationTypeCode;
            businessplaces.Country = record.Country;
            businessplaces.County = record.County;
            businessplaces.CreditContributionOriginCode = record.CreditContributionOriginCode;
            businessplaces.DateOfIncorporation = parseDate(record.DateOfIncorporation);
            businessplaces.DeclarerTypeCode = record.DeclarerTypeCode;
            businessplaces.DefaultCustomerID = record.DefaultCustomerID;
            businessplaces.DefaultResourceWarehouseID = record.DefaultResourceWarehouseID;
            businessplaces.DefaultTaxCode = record.DefaultTaxCode;
            businessplaces.DefaultVendorID = record.DefaultVendorID;
            businessplaces.DefaultWarehouseID = record.DefaultWarehouseID;
            businessplaces.Disabled = record.Disabled;
            businessplaces.EconomicActivityTypeCode = record.EconomicActivityTypeCode;
            businessplaces.EnvironmentType = record.EnvironmentType;
            businessplaces.FederalTaxID = record.FederalTaxID;
            businessplaces.FederalTaxID2 = record.FederalTaxID2;
            businessplaces.FederalTaxID3 = record.FederalTaxID3;
            businessplaces.GlobalLocationNumber = record.GlobalLocationNumber;
            //record.IENumbers = record.
            businessplaces.Industry = record.Industry;
            businessplaces.IPIPeriodCode = record.IPIPeriodCode;
            //record.MainBPL = record.
            businessplaces.NatureOfCompanyCode = record.NatureOfCompanyCode;
            //record.Opting4ICMS = record.
            businessplaces.PaymentClearingAccount = record.PaymentClearingAccount;
            businessplaces.PreferredStateCode = record.PreferredStateCode;
            businessplaces.ProfitTaxationCode = record.ProfitTaxationCode;
            businessplaces.RepName = record.RepName;
            businessplaces.SPEDProfile = record.SPEDProfile;
            businessplaces.State = record.State;
            businessplaces.Street = record.Street;
            businessplaces.StreetNo = record.StreetNo;
            businessplaces.TaxOffice = record.TaxOffice;
            businessplaces.TaxOfficeNo = record.TaxOfficeNo;
            //record.TributaryInfos = record.
            //record.UserFields = record.
            businessplaces.VATRegNum = record.VATRegNum;
            businessplaces.ZipCode = record.ZipCode;

            return businessplaces;
        }


        private DateTime parseDate(dynamic value)
        {
            DateTime result;

            DateTime.TryParse(value, out result);

            return result;
        }
        private string parseCountry(dynamic value)
        {
            string origem = value;
            string result = null;
            origem = origem.Replace("0", "").Replace("4", "");
            if (origem.Count() > 2)
            {
                for (var i = 0; i < origem.Count() - 1; i ++)
                {
                    result += origem[i];
                }
            }
            return result;
        }

        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("loj_cli", "U_LOJ_CLI");
            map.Add("codigo", "U_CODIGO");
            map.Add("nome_fantasia", "U_NOME_FANTASIA");
            map.Add("razao_social", "U_RAZAO_SOCIAL");
            map.Add("cgc_cpf", "U_CGC_CPF");
            map.Add("insc_est_ident", "U_INSC_EST_IDENT");
            map.Add("insc_mun", "U_INSC_MUN");
            map.Add("cep", "U_CEP");
            map.Add("bairro", "U_BAIRRO");
            map.Add("cidade", "U_CIDADE");
            map.Add("estado", "U_ESTADO");
            map.Add("cod_x25", "U_COD_X25");
            map.Add("insc_est_subst", "U_INSC_EST_SUBST");
            map.Add("cod_municipio", "U_COD_MUNICIPIO");
            map.Add("nire", "U_NIRE");
            map.Add("suframa", "U_SUFRAMA");
            map.Add("localidade", "U_LOCALIDADE");
            map.Add("nr_interior", "U_NR_INTERIOR");
            map.Add("data_fecha", "U_DATA_FECHA");

            map.Add("for_contato", "U_FOR_CONTATO");
            map.Add("cli_contato", "U_CLI_CONTATO");
            map.Add("forpri", "U_FORPRI");
            map.Add("data_cad", "U_DATA_CAD");
            map.Add("dta_flinha", "U_DTA_FLINHA");
            map.Add("situacao", "U_SITUACAO");
            map.Add("nr_exterior", "U_NR_EXTERIOR");
            map.Add("banco", "U_BANCO");
            map.Add("conta", "U_CONTA");
            map.Add("agencia", "U_AGENCIA");
            map.Add("dig_conta", "U_DIG_CONTA");
            map.Add("dig_agen", "U_DIG_AGEN");
            map.Add("cargo", "U_CARGO");



            map.Add("lastupdate", "U_LASTUPDATE");
            map.Add("status", "U_STATUS");
            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("loj_cli", "T");
            map.Add("codigo", "N");
            map.Add("nome_fantasia", "T");
            map.Add("razao_social", "T");
            map.Add("cgc_cpf", "T");
            map.Add("insc_est_ident", "T");
            map.Add("insc_mun", "T");
            map.Add("cep", "N");
            map.Add("bairro", "T");
            map.Add("cidade", "T");
            map.Add("estado", "T");
            map.Add("cod_x25", "T");
            map.Add("insc_est_subst", "T");
            map.Add("cod_municipio", "N");
            map.Add("nire", "T");
            map.Add("suframa", "T");
            map.Add("localidade", "T");
            map.Add("nr_interior", "T");
            map.Add("data_fecha", "N");
            map.Add("lastupdate", "T");
            map.Add("status", "N");

            map.Add("for_contato", "T");
            map.Add("cli_contato", "T");
            map.Add("forpri", "N");
            map.Add("data_cad", "N");
            map.Add("dta_flinha", "N");
            map.Add("situacao", "T");
            map.Add("nr_exterior", "T");
            map.Add("banco", "N");
            map.Add("conta", "N");
            map.Add("agencia", "N");
            map.Add("dig_conta", "N");
            map.Add("dig_agen", "N");
            map.Add("cargo", "N");

            return map;
        }
    }
}
