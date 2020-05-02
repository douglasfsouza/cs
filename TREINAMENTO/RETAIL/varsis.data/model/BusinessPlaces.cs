using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
    public class BusinessPlaces : EntityBase
    {
        public override string EntityName => "Cadastro de Lojas";
        public string AdditionalIdNumber { get; set; }
        public string Address { get; set; }
        public string Addressforeign { get; set; }
        public string AddressType { get; set; }
        public string AliasName { get; set; }
        public string Block { get; set; }
        public long? BPLID { get; set; }
        public string BPLName { get; set; }
        public string BPLNameForeign { get; set; }
        //public string Browser { get; set; }
        public string Building { get; set; }
        public string Business { get; set; }
        public string City { get; set; }
        public string CommercialRegister { get; set; }
        public long? CompanyQualificationCode { get; set; }
        public long? CooperativeAssociationTypeCode { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        public string CreditContributionOriginCode { get; set; }
        public DateTime? DateOfIncorporation { get; set; }
        public long? DeclarerTypeCode { get; set; }
        public string DefaultCustomerID { get; set; }
        public string DefaultResourceWarehouseID { get; set; }
        public string DefaultTaxCode { get; set; }
        public string DefaultVendorID { get; set; }
        public string DefaultWarehouseID { get; set; }
        public string Disabled { get; set; }
        public long? EconomicActivityTypeCode { get; set; }
        public long? EnvironmentType { get; set; }
        public string FederalTaxID { get; set; }
        public string FederalTaxID2 { get; set; }
        public string FederalTaxID3 { get; set; }
        public string GlobalLocationNumber { get; set; }
        //public string IENumbers { get; set; }
        public string Industry { get; set; }
        public string IPIPeriodCode { get; set; }
        //public string MainBPL { get; set; }
        public long? NatureOfCompanyCode { get; set; }
        //public string Opting4ICMS { get; set; }
        public string PaymentClearingAccount { get; set; }
        public string PreferredStateCode { get; set; }
        public long? ProfitTaxationCode { get; set; }
        public string RepName { get; set; }
        public string SPEDProfile { get; set; }
        public string State { get; set; }
        public string Street { get; set; }
        public string StreetNo { get; set; }
        public string TaxOffice { get; set; }
        public string TaxOfficeNo { get; set; }
        //public string TributaryInfos { get; set; }
        //public string UserFields { get; set; }
        public string VATRegNum { get; set; }
        public string ZipCode { get; set; }



        //campos para Business Partners
        public string BillToBuildingFloorRoom { get; set; }
        //public string RecId { get; set; }
        public string CardCode { get; set; }
        //public string AliasName { get; set; }
        public string RelationshipDateTill { get; set; }
        public List<BPAddresses> BPAddresses { get; set; }
        public string BPFiscalTaxIDCollection { get; set; }
        public string ContactEmployees { get; set; }
        public string BPBankAccounts { get; set; }
        public string CardType { get; set; }
        public string CardName { get; set; }
        public string FatherCard { get; set; }
        public string ContactPerson { get; set; }

        public string RelationshipDateFrom { get; set; }

    }
}
