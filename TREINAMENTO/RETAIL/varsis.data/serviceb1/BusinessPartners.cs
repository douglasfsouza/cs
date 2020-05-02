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
    public class BusinessPartnersService : IEntityService<Model.BusinessPartners>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public BusinessPartnersService(ServiceLayerConnector serviceLayerConnector)
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

        public Task Delete(BusinessPartners entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }
        private string toJsonError()
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.U_STATUS = Model.Integration.VSITENTIDADE.VSITENTIDADEIntegrationStatus.Error;
            result = JsonConvert.SerializeObject(record);
            return result;
        }
        async public Task<BusinessPartners> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"BusinessPartners('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            BusinessPartners businesspartners = toRecord(record);

            // Recupera as linhas da nota iscal
            string[] filter = new string[]
            {
                $"CardCode eq '{recid}'"
            };

            query = Global.MakeODataQuery("BusinessPartners", null, filter);

            data = await _serviceLayerConnector.getQueryResult(query);

            return businesspartners;
        }

        async public Task Insert(BusinessPartners entity)
        {
            try
            {         
                var Cards = listCardCodes(entity.NATUREZA, entity.CODIGO);
                bool sucesso = true;

                foreach (var card in Cards)
                {
                    IBatchProducer batch = _serviceLayerConnector.CreateBatch();
                    entity.CardCode = card.CardName;
                    entity.CardType = card.CardType;
                    string record = toJson(entity);
                    batch.Post(HttpMethod.Post, "/BusinessPartners", record);
                    ServiceLayerResponse response = await _serviceLayerConnector.Post("BusinessPartners", record);

                    //códigos de erro para se o produto ja existe
                    if (!response.success)
                    {
                        sucesso = false;
                    }
                }

                if (sucesso)
                {
                    var idEntidade = entity.RecId;
                    var cad = toJsonEntidades();
                    string query = Global.BuildQuery($"U_VSITENTIDADE('{idEntidade}')");
                    var response = await _serviceLayerConnector.Patch(query, cad, true);
                }
                else
                {
                    var idEntidade = entity.RecId;
                    var cad = toJsonError();
                    string query = Global.BuildQuery($"U_VSITENTIDADE('{idEntidade}')");
                    var response = await _serviceLayerConnector.Patch(query, cad, true);
                }
            }
            catch (Exception e)
            {

            }
        }
        private List<CardTypes> listCardCodes(string natureza, long codigo)
        {
            List<CardTypes> result = new List<CardTypes>();
            //fornecedor e cliente
            if (natureza == "DC" || natureza == "LS" || natureza == "MA")
            {
                if (natureza == "DC")
                {
                    result.Add(new CardTypes
                    {
                        CardName = "FD" + codigo.ToString("00000"),
                        CardType = "cSupplier"
                    });

                    result.Add(new CardTypes
                    {
                        CardName = "CD" + codigo.ToString("00000"),
                        CardType = "cCustomer"
                    });
                    //result.Add("FD" + codigo.ToString("00000"));
                    //result.Add("CD" + codigo.ToString("00000"));
                }
                if (natureza == "LS" || natureza == "MA")
                {
                    result.Add(new CardTypes
                    {
                        CardName = "FL" + codigo.ToString("00000"),
                        CardType = "cSupplier"
                    });

                    result.Add(new CardTypes
                    {
                        CardName = "CL" + codigo.ToString("00000"),
                        CardType = "cCustomer"
                    });
                    //result.Add("FL" + codigo.ToString("00000"));
                    //result.Add("CL" + codigo.ToString("00000"));
                }
            }
            //só cliente
            else if (natureza == "CA" || natureza == "CC" || natureza == "CD" || natureza == "CL" || natureza == "CS" || natureza == "CV")
            {
                result.Add(new CardTypes
                {
                    CardName = "C" + codigo.ToString("000000"),
                    CardType = "cCustomer"
                });
                //result.Add("C" + codigo.ToString("000000"));
            }
            //só fornecedor
            else if (natureza == "AD" || natureza == "EM" || natureza == "ES" || natureza == "FB" || natureza == "FC" || natureza == "FD" || natureza == "FE" || natureza == "FF" || natureza == "FP" || natureza == "FS" || natureza == "FT" || natureza == "ME" || natureza == "OC" || natureza == "SD" || natureza == "SN" || natureza == "TE" || natureza == "VE" || natureza == "RC" || natureza == "CT")
            {
                result.Add(new CardTypes
                {
                    CardName = "F" + codigo.ToString("000000"),
                    CardType = "cSupplier"
                });
                //result.Add("F" + codigo.ToString("000000"));
            }

            return result;
        }


        public Task Insert(List<BusinessPartners> entities)
        {
            throw new NotImplementedException();
        }
        private string toJsonEntidades()
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.U_STATUS = Model.Integration.VSITENTIDADE.VSITENTIDADEIntegrationStatus.Processed;
            record.U_DATA_INTEGRACAO = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            result = JsonConvert.SerializeObject(record);
            return result;
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
                    string field = _FieldMap[c.Field];
                    string type = _FieldType[c.Field];

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
            string query = Global.MakeODataQuery("BusinessPartners/$count", null, filter.Count == 0 ? null : filter.ToArray(), null, 1, 0);
            string data = await _serviceLayerConnector.getQueryResult(query);
            page.Linhas = Convert.ToInt64(data);
            page.Paginas = (Convert.ToInt64(data) / size.Value) + 1;
            page.qtdPorPagina = size.Value == 0 ? Convert.ToInt64(data) : size.Value;
            return page;
        }
        async public Task<List<BusinessPartners>> List(List<Criteria> criterias, long page, long size)
        {
            List<string> filter = new List<string>();

            if (criterias?.Count != 0)
            {
                foreach(var c in criterias)
                {
                    if (_FieldMap.ContainsKey(c.Field.ToLower()))
                    {
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
                    else 
                    {
                        filter.Add($"{c.Field} {c.Operator.ToLower()} {c.Value}");
                    }
                }
            }

            string query = Global.MakeODataQuery("BusinessPartners", null, filter.Count == 0 ? null : filter.ToArray(),null, page, size);

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
            
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();
            batch = _serviceLayerConnector.CreateBatch();
            string record = toJson(entity);
            batch.Post(HttpMethod.Patch, "/BusinessPartners(CardCode='C20000')", record);
            ServiceLayerResponse response = await _serviceLayerConnector.Post("BusinessPartners", record);
            if (!response.success)
            {
                string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
            throw new NotImplementedException();
        }

        private string toUpdate(dynamic businesspartners)
        {
            dynamic record = new ExpandoObject();
            //fazer a magia 
            return record;
        }


        public Task Update(List<BusinessPartners> entities)
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

        private string criaCodigoWareHouses(string nomeLoja, string codigoLoja)
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
            return codigo + "_01";
        }

        private string toJson(BusinessPartners businesspartners)
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.ContactEmployees = businesspartners.ContactEmployees;
            record.BillToBuildingFloorRoom = businesspartners.BillToBuildingFloorRoom;
            record.AliasName = businesspartners.AliasName;
            record.CardCode = businesspartners.CardCode;
            record.CardName = businesspartners.CardName;
            record.CardType = businesspartners.CardType;
            record.County = businesspartners.County;
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
        private string toJsonTeste(BusinessPartners businesspartners)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            //record.Code = businessplaces.RecId.ToString();
            record.AdditionalIdNumber = null;
            
            result = JsonConvert.SerializeObject(record);

            return result;
        }
        private BusinessPartners toRecord(dynamic record)
        {
            BusinessPartners businesspartners = new BusinessPartners();
            businesspartners.AcceptsEndorsedChecks = record.AcceptsEndorsedChecks;
            businesspartners.AccrualCriteria = record.AccrualCriteria;
            businesspartners.AdditionalID = record.AdditionalID;
            businesspartners.Address = record.Address;
            //businesspartners.Addresses = record.Addresses;
            businesspartners.Affiliate = record.Affiliate;
            businesspartners.AgentCode = record.AgentCode;
            businesspartners.AliasName = record.AliasName;
            businesspartners.AttachmentEntry = record.AttachmentEntry;
            businesspartners.AutomaticPosting = record.AutomaticPosting;
            businesspartners.AvarageLate = record.AvarageLate;
            businesspartners.BackOrder = record.BackOrder;
            businesspartners.BankChargesAllocationCode = record.BankChargesAllocationCode;
            businesspartners.BankCountry = record.BankCountry;
            businesspartners.BillofExchangeonCollection = record.BillofExchangeonCollection;
            businesspartners.BillToBuildingFloorRoom = record.BillToBuildingFloorRoom;
            businesspartners.BilltoDefault = record.BilltoDefault;
            businesspartners.BillToState = record.BillToState;
            businesspartners.Block = record.Block;
            businesspartners.BlockDunning = record.BlockDunning;
            businesspartners.BlockSendingMarketingContent = record.BlockSendingMarketingContent;
            businesspartners.BookkeepingCertified = record.BookkeepingCertified;
            businesspartners.Box1099 = record.Box1099;
            businesspartners.BusinessType = record.BusinessType;
            businesspartners.CampaignNumber = record.CampaignNumber;
            businesspartners.CardCode = record.CardCode;
            businesspartners.CardForeignName = record.CardForeignName;
            businesspartners.CardName = record.CardName;
            businesspartners.CardType = record.CardType;
            businesspartners.Cellular = record.Cellular;
            businesspartners.CertificateNumber = record.CertificateNumber;
            businesspartners.ChannelBP = record.ChannelBP;
            businesspartners.City = record.City;
            businesspartners.ClosingDateProcedureNumber = record.ClosingDateProcedureNumber;
            businesspartners.CollectionAuthorization = record.CollectionAuthorization;
            businesspartners.CommissionGroupCode = record.CommissionGroupCode;
            businesspartners.CommissionPercent = record.CommissionPercent;
            businesspartners.CompanyRegistrationNumber = record.CompanyRegistrationNumber;
            businesspartners.ContactPerson = record.ContactPerson;
            businesspartners.Country = record.Country;
            businesspartners.County = record.County;
            businesspartners.CreateDate = parseDate(record.CreateDate);
            businesspartners.CreateTime = record.CreateTime;
            businesspartners.CreditCardCode = record.CreditCardCode;
            businesspartners.CreditCardExpiration = parseDate(record.CreditCardExpiration);
            businesspartners.CreditCardNum = record.CreditCardNum;
            businesspartners.CreditLimit = record.CreditLimit;
            businesspartners.Currency = record.Currency;
            businesspartners.CurrentAccountBalance = record.CurrentAccountBalance;
            businesspartners.CustomerBillofExchangDisc = record.CustomerBillofExchangDisc;
            businesspartners.CustomerBillofExchangPres = record.CustomerBillofExchangPres;
            businesspartners.DatevAccount = record.DatevAccount;
            businesspartners.DatevFirstDataEntry = record.DatevFirstDataEntry;
            businesspartners.DebitorAccount = record.DebitorAccount;
            businesspartners.DeductibleAtSource = record.DeductibleAtSource;
            businesspartners.DeductionOffice = record.DeductionOffice;
            businesspartners.DeductionPercent = record.DeductionPercent;
            businesspartners.DeductionValidUntil = record.DeductionValidUntil;
            businesspartners.DefaultAccount = record.DefaultAccount;
            businesspartners.DefaultBankCode = record.DefaultBankCode;
            businesspartners.DefaultBlanketAgreementNumber = record.DefaultBlanketAgreementNumber;
            businesspartners.DefaultBranch = record.DefaultBranch;
            businesspartners.DefaultTechnician = record.DefaultTechnician;
            businesspartners.DefaultTransporterEntry = record.DefaultTransporterEntry;
            businesspartners.DefaultTransporterLineNumber = record.DefaultTransporterLineNumber;
            businesspartners.DeferredTax = record.DeferredTax;
            businesspartners.DiscountPercent = record.DiscountPercent;
            businesspartners.DME = record.DME;
            businesspartners.DownPaymentClearAct = record.DownPaymentClearAct;
            businesspartners.DownPaymentInterimAccount = record.DownPaymentInterimAccount;
            businesspartners.DunningDate = record.DunningDate;
            businesspartners.DunningLevel = record.DunningLevel;
            businesspartners.DunningTerm = record.DunningTerm;
            businesspartners.ECommerceMerchantID = record.ECommerceMerchantID;
            businesspartners.EDIRecipientID = record.EDIRecipientID;
            businesspartners.EDISenderID = record.EDISenderID;
            businesspartners.EDocBuildingNumber = record.EDocBuildingNumber;
            businesspartners.EDocCity = record.EDocCity;
            businesspartners.EDocCountry = record.EDocCountry;
            businesspartners.EDocDistrict = record.EDocDistrict;
            businesspartners.EDocPECAddress = record.EDocPECAddress;
            businesspartners.EDocRepresentativeAdditionalId = record.EDocRepresentativeAdditionalId;
            businesspartners.EDocRepresentativeCompany = record.EDocRepresentativeCompany;
            businesspartners.EDocRepresentativeFirstName = record.EDocRepresentativeFirstName;
            businesspartners.EDocRepresentativeFiscalCode = record.EDocRepresentativeFiscalCode;
            businesspartners.EDocRepresentativeSurname = record.EDocRepresentativeSurname;
            businesspartners.EDocStreet = record.EDocStreet;
            businesspartners.EDocStreetNumber = record.EDocStreetNumber;
            businesspartners.EDocZipCode = record.EDocZipCode;
            businesspartners.EmailAddress = record.EmailAddress;
            businesspartners.EndorsableChecksFromBP = record.EndorsableChecksFromBP;
            businesspartners.Equalization = record.Equalization;
            businesspartners.ETaxWebSite = record.ETaxWebSite;
            businesspartners.ExemptionValidityDateFrom = record.ExemptionValidityDateFrom;
            businesspartners.ExemptionValidityDateTo = record.ExemptionValidityDateTo;
            businesspartners.ExemptNum = record.ExemptNum;
            businesspartners.ExpirationDate = record.ExpirationDate;
            businesspartners.ExportCode = record.ExportCode;
            businesspartners.FatherCard = record.FatherCard;
            businesspartners.Fax = record.Fax;
            //businesspartners.FCERelevant = record.FCERelevant;
            businesspartners.FederalTaxID = record.FederalTaxID;
            businesspartners.FeeAccount = record.FeeAccount;
            businesspartners.FormCode1099 = record.FormCode1099;
            businesspartners.FreeText = record.FreeText;
            businesspartners.Frozen = record.Frozen;
            businesspartners.FrozenFrom = record.FrozenFrom;
            businesspartners.FrozenRemarks = record.FrozenRemarks;
            businesspartners.FrozenTo = record.FrozenTo;
            businesspartners.GlobalLocationNumber = record.GlobalLocationNumber;
            businesspartners.GroupCode = record.GroupCode;
            businesspartners.GTSBankAccountNo = record.GTSBankAccountNo;
            businesspartners.GTSBillingAddrTel = record.GTSBillingAddrTel;
            businesspartners.GTSRegNo = record.GTSRegNo;
            businesspartners.HierarchicalDeduction = record.HierarchicalDeduction;
            businesspartners.HouseBank = record.HouseBank;
            businesspartners.HouseBankAccount = record.HouseBankAccount;
            businesspartners.HouseBankBranch = record.HouseBankBranch;
            businesspartners.HouseBankCountry = record.HouseBankCountry;
            businesspartners.HouseBankIBAN = record.HouseBankIBAN;
            businesspartners.IBAN = record.IBAN;
            businesspartners.Indicator = record.Indicator;
            businesspartners.Industry = record.Industry;
            businesspartners.IndustryType = record.IndustryType;
            businesspartners.InstructionKey = record.InstructionKey;
            businesspartners.InsuranceOperation347 = record.InsuranceOperation347;
            businesspartners.InterestAccount = record.InterestAccount;
            businesspartners.IntrestRatePercent = record.IntrestRatePercent;
            businesspartners.IPACodeForPA = record.IPACodeForPA;
            businesspartners.ISRBillerID = record.ISRBillerID;
            businesspartners.LanguageCode = record.LanguageCode;
            businesspartners.LastMultiReconciliationNum = record.LastMultiReconciliationNum;
            businesspartners.LinkedBusinessPartner = record.LinkedBusinessPartner;
            businesspartners.MailAddress = record.MailAddress;
            businesspartners.MailCity = record.MailCity;
            businesspartners.MailCountry = record.MailCountry;
            businesspartners.MailZipCode = record.MailZipCode;
            businesspartners.MaxAmountOfExemption = record.MaxAmountOfExemption;
            businesspartners.MaxCommitment = record.MaxCommitment;
            businesspartners.MinIntrest = record.MinIntrest;
            businesspartners.NationalInsuranceNum = record.NationalInsuranceNum;
            businesspartners.NoDiscounts = record.NoDiscounts;
            businesspartners.Notes = record.Notes;
            businesspartners.OpenDeliveryNotesBalance = record.OpenDeliveryNotesBalance;
            businesspartners.OpenOpportunities = record.OpenOpportunities;
            businesspartners.OpenOrdersBalance = record.OpenOrdersBalance;
            businesspartners.OtherReceivablePayable = record.OtherReceivablePayable;
            businesspartners.OwnerCode = record.OwnerCode;
            businesspartners.OwnerIDNumber = record.OwnerIDNumber;
            businesspartners.Pager = record.Pager;
            businesspartners.PartialDelivery = record.PartialDelivery;
            businesspartners.Password = record.Password;
            businesspartners.PaymentBlock = record.PaymentBlock;
            businesspartners.PaymentBlockDescription = record.PaymentBlockDescription;
            businesspartners.PayTermsGrpCode = record.PayTermsGrpCode;
            businesspartners.PeymentMethodCode = record.PeymentMethodCode;
            businesspartners.Phone1 = record.Phone1;
            businesspartners.Phone2 = record.Phone2;
            businesspartners.Picture = record.Picture;
            businesspartners.PlanningGroup = record.PlanningGroup;
            businesspartners.PriceListNum = record.PriceListNum;
            businesspartners.Priority = record.Priority;
            businesspartners.Profession = record.Profession;
            businesspartners.ProjectCode = record.ProjectCode;
            businesspartners.RateDiffAccount = record.RateDiffAccount;
            businesspartners.ReferenceDetails = record.ReferenceDetails;
            businesspartners.RelationshipCode = record.RelationshipCode;
            //businesspartners.RelationshipDateFrom = record.RelationshipDateFrom;
            //businesspartners.RelationshipDateTill = record.RelationshipDateTill;
            businesspartners.RepresentativeName = record.RepresentativeName;
            businesspartners.SalesPersonCode = record.SalesPersonCode;
            businesspartners.Series = record.Series;
            businesspartners.ShippingType = record.ShippingType;
            businesspartners.ShipToBuildingFloorRoom = record.ShipToBuildingFloorRoom;
            businesspartners.ShipToDefault = record.ShipToDefault;
            businesspartners.SinglePayment = record.SinglePayment;
            businesspartners.SubjectToWithholdingTax = record.SubjectToWithholdingTax;
            businesspartners.SurchargeOverlook = record.SurchargeOverlook;
            businesspartners.TaxExemptionLetterNum = record.TaxExemptionLetterNum;
            businesspartners.Territory = record.Territory;
            businesspartners.ThresholdOverlook = record.ThresholdOverlook;
            businesspartners.UnifiedFederalTaxID = record.UnifiedFederalTaxID;
            businesspartners.UnpaidBillofExchange = record.UnpaidBillofExchange;
            businesspartners.UpdateDate = parseDate(record.UpdateDate);
            businesspartners.UpdateTime = record.UpdateTime;
            businesspartners.UseBillToAddrToDetermineTax = record.UseBillToAddrToDetermineTax;
            businesspartners.UseShippedGoodsAccount = record.UseShippedGoodsAccount;
            businesspartners.Valid = record.Valid;
            businesspartners.ValidFrom = record.ValidFrom;
            businesspartners.ValidRemarks = record.ValidRemarks;
            businesspartners.ValidTo = record.ValidTo;
            businesspartners.VatGroup = record.VatGroup;
            businesspartners.VatGroupLatinAmerica = record.VatGroupLatinAmerica;
            businesspartners.VatIDNum = record.VatIDNum;
            businesspartners.VATRegistrationNumber = record.VATRegistrationNumber;
            businesspartners.VerificationNumber = record.VerificationNumber;
            businesspartners.Website = record.Website;
            businesspartners.WithholdingTaxCertified = record.WithholdingTaxCertified;
            businesspartners.WithholdingTaxDeductionGroup = record.WithholdingTaxDeductionGroup;
            businesspartners.WTCode = record.WTCode;
            businesspartners.ZipCode = record.ZipCode;

            return businesspartners;
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

            map.Add("CardCode", "CardCode");
           
            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("CardCode", "T");

            return map;
        }
    }
}
