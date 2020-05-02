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
using Varsis.Data.Model.Integration;
using static Varsis.Data.Model.Integration.Product;

namespace Varsis.Data.Serviceb1.Integration
{
    public class ItemsService : IEntityService<Model.Items>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public ItemsService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = mountFieldMap();
            _FieldType = mountFieldType();
        }

        public Task<bool> Create()
        {
            throw new NotImplementedException();
        }

        public Task Delete(Items entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        public Task<Items> Find(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task Insert(Items entity)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();
            batch = _serviceLayerConnector.CreateBatch();
            string record = toJson(entity);

            batch.Post(HttpMethod.Post, "/Items", record);

            foreach (var i in entity.ItemBarCodes.Where(m => m.BarCode !=  entity.BarCode))
            {
                record = toJsonBarCode(i);
                batch.Post(HttpMethod.Post, "/BarCodes", record);
            }

            ServiceLayerResponse response = await _serviceLayerConnector.Post(batch);

            if (!response.success)
            {
                string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
            else
            {
                ProductIntegrationStatus status = ProductIntegrationStatus.Processed;

                if (response.internalResponses.Count(m => !m.success) != 0)
                {
                    status = ProductIntegrationStatus.Error;
                }

                var prod = toJsonProduct(status);
                string query = Global.BuildQuery($"U_VSITPRODUCT('{entity.RecId}')");
                var responseStatus = await _serviceLayerConnector.Patch(query, prod, true);

                if (!responseStatus.success)
                {
                    string message = $"Erro ao atualizar status de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                    Console.WriteLine(message);
                    throw new ApplicationException(message);
                }
            }
        }

        public Task Insert(List<Items> entities)
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
        async public Task<List<Items>> List(List<Criteria> criterias, long page, long size)
        {
            List<string> filter = new List<string>();

            if (criterias?.Count != 0)
            {
                foreach (var c in criterias)
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
            }

            string query = Global.MakeODataQuery("Items", null, filter.Count == 0 ? null : filter.ToArray(), null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<Items> result = new List<Items>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }
        async public Task<List<Items>> SelectList(List<Criteria> criterias, long page, long size)
        {
            List<string> filter = new List<string>();

            if (criterias?.Count != 0)
            {
                foreach (var c in criterias)
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
            }

            string[] fields = new string[]
            {
                "ItemCode",
                "ItemName"
            };

            string query = Global.MakeODataQuery("Items", fields, filter.Count == 0 ? null : filter.ToArray(), null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<Items> result = new List<Items>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    Items newItem = new Items()
                    {
                        ItemCode = o.ItemCode,
                        ItemName = o.ItemName
                    };

                    result.Add(newItem);
                }
            }

            return result;
        }

        public Task Update(Items entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<Items> entities)
        {
            throw new NotImplementedException();
        }
        private DateTime parseDate(dynamic value)
        {
            DateTime result;

            DateTime.TryParse(value, out result);

            return result;
        }

        private string toJson(Items items)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.ItemCode = items.ItemCode;
            record.ItemName = items.ItemName;
            record.ItemType = items.ItemType;
            record.MaterialGroup = items.MaterialGroup;
            record.SalesUnit = items.SalesUnit;
            record.SalesPackagingUnit = items.SalesPackagingUnit; //SalPackMsr
            record.SalesQtyPerPackUnit = items.SalesQtyPerPackUnit;  //SalPackUn  
            record.InventoryUOM = items.InventoryUOM;
            record.PurchaseUnitWeight = items.PurchaseUnitWeight;  //BWeight1
            record.PurchaseUnitWidth = items.PurchaseUnitWidth;  //BWidth1
            record.SalesUnitHeight = items.SalesUnitHeight; // SHeight1
            record.PurchaseQtyPerPackUnit = items.PurchaseQtyPerPackUnit; //NumInBuy
            record.SupplierCatalogNo = items.SupplierCatalogNo;//Substitute
            record.ForeignName = items.ForeignName;//FrgnName
            record.SWW = items.SWW;
            record.SalesUnitWidth = items.SalesUnitWidth; // SWidth1
            record.PurchasePackagingUnit = items.PurchasePackagingUnit;
          //  record.PurchaseHeightUnit = items.PurchaseHeightUnit;  //BHeight1
            record.SalesLengthUnit = items.SalesLengthUnit;//SLength1
            record.ManageBatchNumbers = items.ManageBatchNumbers; // ManBtchNum, 
            record.ProductSource = items.ProductSource;
            record.PurchaseUnit = items.PurchaseUnit;
            record.InventoryWeight = items.InventoryWeight;
            //     record.PurchaseLengthUnit = items.PurchaseLengthUnit;
            record.PurchaseUnitLength = items.PurchaseUnitLength;
            record.PurchaseUnitHeight = items.PurchaseUnitHeight;
            record.PurchaseItemsPerUnit = items.PurchaseItemsPerUnit;

            if (items.Validto != DateTime.MinValue)
            {
                record.Validto = items.Validto;  //frozenFor 
            }

            if (items.FrozenFrom != DateTime.MinValue)
            {
                record.FrozenFrom = items.FrozenFrom.ToString("yyyy-MM-dd").Substring(0, 10);
            }
            if (items.ValidFrom != DateTime.MinValue)
            {
                record.ValidFrom = items.ValidFrom.ToString("yyyy-MM-dd").Substring(0, 10);
            }

            record.Valid = items.Valid;
            record.Frozen = items.Frozen;
            record.PurchaseItem = items.PurchaseItem;
            record.SalesItem = items.SalesItem;
            record.InventoryItem = items.InventoryItem;
            record.BarCode = items.BarCode;
            record.MaterialType = items.MaterialType;

            if (items.NCMCode > 0)
            {
                record.NCMCode = items.NCMCode;
            }

            var q = from p in items.Items_PreferredVendors group p by p.BPCode into v select new { BPCode = v.Key };
            var lista = q.ToList();

            record.ItemPreferredVendors = lista;
            record.Mainsupplier = items.Mainsupplier; 

            result = JsonConvert.SerializeObject(record);

            return result;
        }

        private string toJsonBarCode(ItemBarCodes barcode)
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();

            record.Barcode = barcode.BarCode;
            record.FreeText = barcode.FreeText;
            record.UoMEntry = barcode.UoMEntry;
            record.ItemNo = barcode.ItemNo;

            result = JsonConvert.SerializeObject(record);

            return result;
        }


        private string toJsonBusinessPartners(BusinessPartners businessPartners)
        {

            string result = string.Empty;
            dynamic record = new ExpandoObject();

            record.CardCode = businessPartners.CardCode;
            record.CardType = businessPartners.CardType;
            record.CardName = businessPartners.CardName;


            result = JsonConvert.SerializeObject(record);

            return result;
        }


        private string toJsonComponete(ProductTree productTree)
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.TreeCode = productTree.TreeCode;

            if (productTree.TreeType == "A" || productTree.TreeType == "P")
            {
                record.TreeType = productTree.TreeType;
            }
            record.Warehouse = productTree.Warehouse;

            var q = from p in productTree.productTrees_Lines select new { p.ItemCode, p.Quantity, p.Warehouse };
            var lista = q.ToList();
            record.ProductTreeLines = lista;

            result = JsonConvert.SerializeObject(record);

            return result;
        }

        private Items toRecord(dynamic record)
        {
            Items items = new Items();


            items.ItemCode = record.ItemCode;
            items.ItemName = record.ItemName;
            items.ItemType = record.ItemType;
            items.MaterialGroup = record.MaterialGroup;
            items.SalesUnit = record.SalesUnit;
            items.SalesPackagingUnit = record.SalesPackagingUnit; //SalPackMsr
            items.SalesQtyPerPackUnit = record.SalesQtyPerPackUnit;  //SalPackUn  
            items.InventoryUOM = record.InventoryUOM;
            items.PurchaseUnitWeight = record.PurchaseUnitWeight;  // BWeight1
            items.PurchaseUnitWidth = record.PurchaseUnitWidth;  //BWidth1
            items.SalesUnitHeight = record.SalesUnitHeight; //SHeight1
            items.PurchaseQtyPerPackUnit = record.PurchaseQtyPerPackUnit; //NumInBuy
            items.SupplierCatalogNo = record.SupplierCatalogNo;//Substitute
            items.ForeignName = record.Foreignname;//FrgnName
            items.SWW = record.SWW;
            items.SalesUnitWidth = record.SalesUnitWidth; //SWidth1
            items.PurchasePackagingUnit = record.PurchasePackagingUnit;
          //  items.PurchaseHeightUnit = record.PurchaseHeightUnit;  //BHeight1
            items.SalesLengthUnit = record.SalesLengthUnit;//SLength1
            items.ManageBatchNumbers = record.Managebatchnumbers; // ManBtchNum
            items.ProductSource = record.ProductSource;
            items.Validto = record.Validto;  //frozenFor
            items.FrozenFrom = record.frozenfrom;
            items.NCMCode = record.NCMCode;
            items.MaterialType = record.MaterialType;
          //  items.PurchaseLengthUnit = record.PurchaseLengthUnit;
            items.ValidFrom = record.ValidFrom;
            // items.PurchaseUnitLength = record.PurchaseUnitLength;
            items.PurchaseUnitHeight = record.PurchaseUnitHeight;
            items.PurchaseItemsPerUnit = record.PurchaseItemsPerUnit;
            items.Mainsupplier = record.Mainsupplier;




            items.Items_PreferredVendors = record.Items_PreferredVendors;

            return items;
        }

        private string toJsonProduct(ProductIntegrationStatus status)
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.U_STATUS = (int)status;
            result = JsonConvert.SerializeObject(record);
            return result;
        }


        //Verificar se precisa dese metodo
        private string parseCountry(dynamic value)
        {
            string origem = value;
            string result = null;
            origem = origem.Replace("0", "").Replace("4", "");
            if (origem.Count() > 2)
            {
                for (var i = 0; i < origem.Count() - 1; i++)
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

            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("loj_cli", "T");

            return map;
        }
    }
}
