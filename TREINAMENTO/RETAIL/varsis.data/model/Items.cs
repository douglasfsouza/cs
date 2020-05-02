using System;
using System.Collections.Generic;
using System.Text;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model.Integration;

namespace Varsis.Data.Model
{
    public class Items : EntityBase
    {
        public override string EntityName => "Cadastro de Produtos";
        public Items()
        {
            Items_PreferredVendors = new List<Items_PreferredVendors>();
            producttrees = new List<ProductTree>();

            BusinessPartners = new List<BusinessPartners>();

            ItemBarCodes = new List<ItemBarCodes>();
        }

        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public long? MaterialGroup { get; set; }
        public string SWW { get; set; }
        public long? NCMCode { get; set; }
        public long? ProductSource { get; set; } //ProductSrc
        public string SupplierCatalogNo { get; set; } //Substitute
        public double? PurchaseQtyPerPackUnit { get; set; } //NumInBuy
        public double? SalesQtyPerPackUnit { get; set; } //SalPackUn  
        public double? PurchaseHeightUnit { get; set; } //BHeight1
        public double? PurchaseUnitWidth { get; set; } //BWidth1
        public double? PurchaseUnitWeight { get; set; } //BWeight1
        public double? SalesUnitHeight { get; set; } //SHeight1
        public double? SalesUnitWidth { get; set; }//SWidth1
        public long? SalesLengthUnit { get; set; } //SLength1
     
        public string SalesUnit { get; set; } // SalUnitMsr
        public string ForeignName { get; set; } //FrgnName
        public string ManageBatchNumbers { get; set; } // ManBtchNum

        public string PurchasePackagingUnit { get; set; } //tpo_emb_for

        public string SalesPackagingUnit { get; set; } // SalPackMsr
        public string InventoryUOM { get; set; } //InvntryUom

        public DateTime Validto { get; set; } //frozenFor
        public DateTime FrozenFrom { get; set; } 

        public DateTime ValidFrom { get; set; }

        public string PurchaseItem { get; set; }

        public string SalesItem { get; set; }

        public string InventoryItem { get; set; }

        public string ItemClass { get; set; }

        public string IssueMethod { get; set; }
        

        public List<Items_PreferredVendors> Items_PreferredVendors { get; set; } //VendorCode

        public List<ProductTree> producttrees { get; set; }

        public List<ItemBarCodes> ItemBarCodes { get; set; }

        public List<BusinessPartners> BusinessPartners { get; set; }

        public string BarCode { get; set; }
        public string ItemType { get; set; }

        public string Valid { get; set; }

        public string Frozen { get; set; }

        public string PurchaseUnit { get; set; }

        public string MaterialType { get; set; }

        public double InventoryWeight { get; set; }

        public double? PurchaseLengthUnit { get; set; }

        public double? PurchaseUnitLength { get; set; }
        public double? PurchaseUnitHeight { get; set; }

        public double PurchaseItemsPerUnit { get; set; }

        public string Mainsupplier { get; set; }

    }

}
