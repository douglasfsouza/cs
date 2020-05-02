using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Model
{
    public class BusinessPartners : EntityBase
    {
        public string BuildingFloorRoom { get; set; }
        public string StreetNo { get; set; }
        public override string EntityName => "Cadastro de Parceiros de Negocio";
        public string AcceptsEndorsedChecks { get; set; }
        //public string AccountRecivablePayables { get; set; }
        public string AccrualCriteria { get; set; }
        public string AdditionalID { get; set; }
        public string Address { get; set; }
        public List<BPAddresses> BPAddresses { get; set; }
        public string Affiliate { get; set; }
        public string AgentCode { get; set; }
        public string AliasName { get; set; }
        public long? AttachmentEntry { get; set; }
        public string AutomaticPosting { get; set; }
        public long? AvarageLate { get; set; }
        public string BackOrder { get; set; }
        public long? BankChargesAllocationCode { get; set; }
        public string BankCountry { get; set; }
        public string BillofExchangeonCollection { get; set; }
        public string BillToBuildingFloorRoom { get; set; }
        public string BilltoDefault { get; set; }
        public string BillToState { get; set; }
        public string Block { get; set; }
        public string BlockDunning { get; set; }
        public string BlockSendingMarketingContent { get; set; }
        public string BookkeepingCertified { get; set; }
        public string Box1099 { get; set; }
        public string BusinessType { get; set; }
        public long? CampaignNumber { get; set; }
        public string CardCode { get; set; }
        public string CardForeignName { get; set; }
        public string CardName { get; set; }
        public string CardType { get; set; }
        public string Cellular { get; set; }
        public string CertificateNumber { get; set; }
        public string ChannelBP { get; set; }
        public string City { get; set; }
        public long? ClosingDateProcedureNumber { get; set; }
        public string CollectionAuthorization { get; set; }
        public long? CommissionGroupCode { get; set; }
        public double? CommissionPercent { get; set; }
        //public string CompanyPrivate { get; set; }
        public string CompanyRegistrationNumber { get; set; }     
        public List<ContactEmployees> ContactEmployees { get; set; }
        public List<BPBankAccounts> BPBankAccounts { get; set; }
        public string ContactPerson { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateTime { get; set; }
        public long? CreditCardCode { get; set; }
        public DateTime? CreditCardExpiration { get; set; }
        public string CreditCardNum { get; set; }
        public double? CreditLimit { get; set; }
        public string Currency { get; set; }
        public double? CurrentAccountBalance { get; set; }
        public string CustomerBillofExchangDisc { get; set; }
        public string CustomerBillofExchangPres { get; set; }
        public string DatevAccount { get; set; }
        public string DatevFirstDataEntry { get; set; }
        public string DebitorAccount { get; set; }
        public string DeductibleAtSource { get; set; }
        public string DeductionOffice { get; set; }
        public double? DeductionPercent { get; set; }
        public DateTime? DeductionValidUntil { get; set; }
        public string DefaultAccount { get; set; }
        public string DefaultBankCode { get; set; }
        public long? DefaultBlanketAgreementNumber { get; set; }
        public string DefaultBranch { get; set; }
        public long? DefaultTechnician { get; set; }
        public long? DefaultTransporterEntry { get; set; }
        public long? DefaultTransporterLineNumber { get; set; }
        public string DeferredTax { get; set; }
        //public string DiscountBaseObject { get; set; }
        //public string DiscountGroups { get; set; }
        public double? DiscountPercent { get; set; }
        //public string DiscountRelations { get; set; }
        public string DME { get; set; }
        public string DownPaymentClearAct { get; set; }
        public string DownPaymentInterimAccount { get; set; }
        public DateTime? DunningDate { get; set; }
        public long? DunningLevel { get; set; }
        public string DunningTerm { get; set; }
        public string ECommerceMerchantID { get; set; }
        public string EDIRecipientID { get; set; }
        public string EDISenderID { get; set; }
        public long? EDocBuildingNumber { get; set; }
        public string EDocCity { get; set; }
        public string EDocCountry { get; set; }
        public string EDocDistrict { get; set; }
        //public string EDocGenerationType { get; set; }
        public string EDocPECAddress { get; set; }
        public string EDocRepresentativeAdditionalId { get; set; }
        public string EDocRepresentativeCompany { get; set; }
        public string EDocRepresentativeFirstName { get; set; }
        public string EDocRepresentativeFiscalCode { get; set; }
        public string EDocRepresentativeSurname { get; set; }
        public string EDocStreet { get; set; }
        public string EDocStreetNumber { get; set; }
        public string EDocZipCode { get; set; }
        //public string EffectiveDiscount { get; set; }
        //public string EffectivePrice { get; set; }
        public string EmailAddress { get; set; }
        public string EndorsableChecksFromBP { get; set; }
        public string Equalization { get; set; }
        public long? ETaxWebSite { get; set; }
        //public string ExemptionMaxAmountValidationType { get; set; }
        public DateTime? ExemptionValidityDateFrom { get; set; }
        public DateTime? ExemptionValidityDateTo { get; set; }
        public string ExemptNum { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string ExportCode { get; set; }
        public string FatherCard { get; set; }
        //public string FatherType { get; set; }
        public string Fax { get; set; }
        //public string FCERelevant { get; set; }
        public string FederalTaxID { get; set; }
        public string FeeAccount { get; set; }       
        public List<BPFiscalTaxIDCollection> BPFiscalTaxIDCollection { get; set; }
        public long? FormCode1099 { get; set; }
        public string FreeText { get; set; }
        public string Frozen { get; set; }
        public DateTime? FrozenFrom { get; set; }
        public string FrozenRemarks { get; set; }
        public DateTime? FrozenTo { get; set; }
        public string GlobalLocationNumber { get; set; }
        public long? GroupCode { get; set; }
        public string GTSBankAccountNo { get; set; }
        public string GTSBillingAddrTel { get; set; }
        public string GTSRegNo { get; set; }
        public string HierarchicalDeduction { get; set; }
        public string HouseBank { get; set; }
        public string HouseBankAccount { get; set; }
        public string HouseBankBranch { get; set; }
        public string HouseBankCountry { get; set; }
        public string HouseBankIBAN { get; set; }
        public string IBAN { get; set; }
        public string Indicator { get; set; }
        public long? Industry { get; set; }
        public string IndustryType { get; set; }
        public string InstructionKey { get; set; }
        public string InsuranceOperation347 { get; set; }
        public string InterestAccount { get; set; }
        //public string IntrastatExtension { get; set; }
        public double? IntrestRatePercent { get; set; }
        public string IPACodeForPA { get; set; }
        public string ISRBillerID { get; set; }
        public long? LanguageCode { get; set; }
        public long? LastMultiReconciliationNum { get; set; }
        public string LinkedBusinessPartner { get; set; }
        public string MailAddress { get; set; }
        public string MailCity { get; set; }
        public string MailCountry { get; set; }
        public string MailZipCode { get; set; }
        public double? MaxAmountOfExemption { get; set; }
        public double? MaxCommitment { get; set; }
        public double? MinIntrest { get; set; }
        public string NationalInsuranceNum { get; set; }
        public string NoDiscounts { get; set; }
        public string Notes { get; set; }
        public double? OpenDeliveryNotesBalance { get; set; }
        public long? OpenOpportunities { get; set; }
        public double? OpenOrdersBalance { get; set; }
        //public string OperationCode347 { get; set; }
        public string OtherReceivablePayable { get; set; }
        public long? OwnerCode { get; set; }
        public string OwnerIDNumber { get; set; }
        public string Pager { get; set; }
        public string PartialDelivery { get; set; }
        public string Password { get; set; }
        public string PaymentBlock { get; set; }
        public long? PaymentBlockDescription { get; set; }
        public long? PayTermsGrpCode { get; set; }
        public string PeymentMethodCode { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Picture { get; set; }
        public string PlanningGroup { get; set; }
        public long? PriceListNum { get; set; }
        //public string PriceMode { get; set; }
        public long? Priority { get; set; }
        public string Profession { get; set; }
        public string ProjectCode { get; set; }
        //public string Properties { get; set; }
        public string RateDiffAccount { get; set; }
        public string ReferenceDetails { get; set; }
        public string RelationshipCode { get; set; }
        public string RelationshipDateFrom { get; set; }
        public string RelationshipDateTill { get; set; }
        public string RepresentativeName { get; set; }
        //public string ResidenNumber { get; set; }
        public long? SalesPersonCode { get; set; }
        public long? Series { get; set; }
        //public string ShaamGroup { get; set; }
        public long? ShippingType { get; set; }
        public string ShipToBuildingFloorRoom { get; set; }
        public string ShipToDefault { get; set; }
        public string SinglePayment { get; set; }
        public string SubjectToWithholdingTax { get; set; }
        public string SurchargeOverlook { get; set; }
        public string TaxExemptionLetterNum { get; set; }
        //public string TaxRoundingRule { get; set; }
        public long? Territory { get; set; }
        public string ThresholdOverlook { get; set; }
        //public string TypeOfOperation { get; set; }
        //public string TypeReport { get; set; }
        public string UnifiedFederalTaxID { get; set; }
        public string UnpaidBillofExchange { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string UseBillToAddrToDetermineTax { get; set; }
        //public string UserFields { get; set; }
        public string UseShippedGoodsAccount { get; set; }
        public string Valid { get; set; }
        public DateTime? ValidFrom { get; set; }
        public string ValidRemarks { get; set; }
        public DateTime? ValidTo { get; set; }
        public string VatGroup { get; set; }
        public string VatGroupLatinAmerica { get; set; }
        public string VatIDNum { get; set; }
        //public string VatLiable { get; set; }
        public string VATRegistrationNumber { get; set; }
        public string VerificationNumber { get; set; }
        public string Website { get; set; }
        public string WithholdingTaxCertified { get; set; }
        public long? WithholdingTaxDeductionGroup { get; set; }
        public string WTCode { get; set; }
        public string ZipCode { get; set; }

        //campos para verificação
        public string NATUREZA { get; set; }
        public long CODIGO { get; set; }
    }
    public class CardTypes
    {
        public string CardName { get; set; }
        public string CardType { get; set; }
    }

    public class BPAddresses
    {
        public string AddressName { get; set; }
        public string BuildingFloorRoom { get; set; }
        //public string AddressType { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Block { get; set; }   
        public string County { get; set; }
        public string State { get; set; }

        public string StreetNo { get; set; }
    }
    public class BPFiscalTaxIDCollection
    {
        public string TaxId0 { get; set; }
        public string TaxId1 { get; set; }
        public string TaxId2 { get; set; }
        public string TaxId3 { get; set; }

    }
    public class ContactEmployees
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string Position { get; set; }
        public string E_Mail { get; set; }
        public string MobilePhone { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
    }
    public class BPBankAccounts
    {
        public string BankCode { get; set; }
        public string AccountNo { get; set; }
        public string Branch { get; set; }
        public string ControlKey { get; set; }
    }
}
