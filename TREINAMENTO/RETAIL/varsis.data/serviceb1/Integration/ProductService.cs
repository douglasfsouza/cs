using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model.Integration;
using System.Linq;

namespace Varsis.Data.Serviceb1.Integration
{
    public class ProductService : IEntityService<Model.Integration.Product>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public ProductService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = mountFieldMap();
            _FieldType = mountFieldType();
        }

        async public Task<bool> Create()
        {
            bool result = false;

            result = await createTable();

            if (result)
            {
                result = await createComponentsTable();
            }

            if (result)
            {
                result = await createVendorTable();
            }
            return result;
        }

        async private Task<bool> createTable()
        {
            bool result = false;

            Table table = new Table(_serviceLayerConnector);

            table.name = "VSITPRODUCT";
            table.description = "Cadastro de Produtos";
            table.tableType = "bott_NoObject";
            table.columns = createColumns();
            table.indexes = createIndexes();

            try
            {
                await table.create();
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public Task Delete(Product entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task<Product> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"U_VSITPRODUCT('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            Product product = toRecord(record);

            List<string> filter;
            try
            {
                filter = new List<string>();
                //Componentes 


                filter.Add("U_PRODUTO_AK eq " + product.cod_item);
                string queryComponentes = Global.MakeODataQuery("U_VSITPRODUCT_COMP", null, filter.Count == 0 ? null : filter.ToArray(), null, 1, 0);

                string dataComp = await _serviceLayerConnector.getQueryResult(queryComponentes);

                List<ExpandoObject> recordComp = Global.parseQueryToCollection(dataComp);

                if (recordComp.Count() != 0)
                {

                    product.components = new List<ProductComponent>();
                    foreach (var comp in recordComp)
                    {
                        ProductComponent componet = new ProductComponent();
                        componet = ToRecordComponent(comp);
                        product.components.Add(componet);
                    }
                }

                //Vendors (Fornecedores) 
                filter = new List<string>();
                filter.Add("U_COD_ITEM eq " + product.cod_item.ToString());
                string queryVendors = Global.MakeODataQuery("U_VSITPRODUCT_VENDORS", null, filter.Count == 0 ? null : filter.ToArray(), null, 1, 0);

                string dataVendors = await _serviceLayerConnector.getQueryResult(queryVendors);

                List<ExpandoObject> recordVendors = Global.parseQueryToCollection(dataVendors);

                if (recordVendors.Count() != 0)
                {

                    product.vendors = new List<ProductVendor>();
                    foreach (var v in recordVendors)
                    {
                        ProductVendor vendor = new ProductVendor();
                        vendor = ToRecordVendor(v);
                        product.vendors.Add(vendor);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }



            return product;
        }

        private ProductVendor toVendorItem(dynamic record)
        {
            ProductVendor vendorItem = new ProductVendor();

            vendorItem.RecId = Guid.Parse(record.Code);
            vendorItem.cod_item = record.U_COD_ITEM;
            vendorItem.cod_forn = record.U_COD_FORN;
            vendorItem.cod_forn_alt = record.U_COD_FORN_ALT;
            vendorItem.cod_item_alt = record.U_COD_ITEM_ALT;
            vendorItem.dig_item = record.U_DIG_ITEM;
            vendorItem.dig_forn = record.U_DIG_FORN;
            vendorItem.referencia = record.U_REFERENCIA;
            vendorItem.descricao = record.U_DESCRICAO;
            vendorItem.fatur_unid = record.U_FATUR_UNID;
            vendorItem.uf_unid = record.U_UF_UNID;
            vendorItem.uf_fator = record.U_UF_FATOR;
            vendorItem.uf_fator_conv = record.U_UF_FATOR_CONV;
            vendorItem.emb_xml = record.U_EMB_XML;
            vendorItem.lastupdate = DateTime.Now;

            return vendorItem;
        }


        private ProductComponent toComponentItem(dynamic record)
        {
            ProductComponent componentsItem = new ProductComponent();

            componentsItem.RecId = Guid.Parse(record.Code);
            componentsItem.produto = record.U_PRODUTO;
            componentsItem.componente = record.U_COMPONENTE;
            componentsItem.componente_ak = record.U_COMPONENTE_AK;
            componentsItem.produto_ak = record.U_PRODUTO_AK;
            componentsItem.quantidade = record.U_QUANTIDADE;
            componentsItem.sec_sim_1 = record.U_SEC_SIM_1;
            componentsItem.sec_sim_2 = record.U_SEC_SIM_2;
            componentsItem.sec_sim_3 = record.U_SEC_SIM_3;
            componentsItem.grp_sim_1 = record.U_GRP_SIM_1;
            componentsItem.grp_sim_2 = record.U_GRP_SIM_2;
            componentsItem.grp_sim_3 = record.U_GRP_SIM_3;
            componentsItem.sbg_sim_1 = record.U_SBG_SIM_1;
            componentsItem.sbg_sim_2 = record.U_SBG_SIM_2;
            componentsItem.sbg_sim_3 = record.U_SBG_SIM_3;
            componentsItem.ctg_sim_1 = record.U_CTG_SIM_1;
            componentsItem.ctg_sim_2 = record.U_CTG_SIM_2;
            componentsItem.ctg_sim_3 = record.U_CTG_SIM_3;
            componentsItem.filler = record.U_FILLER;
            record.U_LASTUPDATE = DateTime.Now;

            return componentsItem;
        }
        
        async public Task<bool> ProductExists(Product entity)
        {
            bool result = false;

            string[] filter = new string[]
            {
                $"U_COD_ITEM eq {entity.cod_item}",
                $"U_STATUS ne {((int)IntegrationStatus.Processed)}"
            };
            
            string[] fields = new string[]
            {
                "U_COD_ITEM"
            };

            string query = Global.MakeODataQuery("U_VSITPRODUCT", fields, filter, null);

            string data = await _serviceLayerConnector.getQueryResult(query);
            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            result = lista == null ? false :
                     lista.Count == 0 ? false : true;

            return result;
        }

        async public Task Insert(Product entity)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();

            //
            // Verifica se já existe um registro pendente de processamento
            //
            bool exists = await ProductExists(entity);
            if (exists)
            {
                return;
            }

            entity.status = Data.Model.Integration.Product.ProductIntegrationStatus.Importing;
            string record = toJson(entity);
            batch.Post(HttpMethod.Post, "/U_VSITPRODUCT", record);

            foreach (var i in entity.components)
            {
                record = toJsonComponents(i);
                batch.Post(HttpMethod.Post, "/U_VSITPRODUCT_COMP", record);
            }

            foreach (var i in entity.vendors)
            {
                record = toJsonVendors(i);
                batch.Post(HttpMethod.Post, "/U_VSITPRODUCT_VENDORS", record);
            }

            ServiceLayerResponse response = await _serviceLayerConnector.Post(batch);

            //
            // Erro no protocolo http ou na estrutura do arquivo
            //
            if (!response.success)
            {
                string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                Console.WriteLine(message);
                throw new ApplicationException(message);
            }
            else
            {
                //
                // Verificar erros no lote
                //
                if (response.internalResponses.Count(m => m.success == false) == 0)
                {
                    entity.status = Product.ProductIntegrationStatus.Created;
                }
                else
                {
                    entity.status = Product.ProductIntegrationStatus.Error;
                }

                record = ToJsonStatus(entity);
                response = await _serviceLayerConnector.Patch($"U_VSITPRODUCT('{entity.RecId}')", record);

                if (!response.success)
                {
                    string message = $"Erro ao inserir dados em '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                    Console.WriteLine(message);
                    throw new ApplicationException(message);
                }
            }
        }

        public Task Insert(List<Product> entities)
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
        async public Task<List<Product>> List(List<Criteria> criterias, long page, long size)
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

                if (cont.Equals(criterias.Count))
                {
                    var status = criterias.Where(c => c.Field == "status").ToList();
                    if (status.Count == 0)
                    {
                        filter.Add($"{"U_STATUS"} {"ne"} 2");
                    }
                }
            }

            if (filter.Count == 0)
            {
                filter.Add($"{"U_STATUS"} {"ne"} 2");
            }

            string query = Global.MakeODataQuery("U_VSITPRODUCT", null, filter.Count == 0 ? null : filter.ToArray(), null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<Product> result = new List<Product>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        public Task Update(Product entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<Product> entities)
        {
            throw new NotImplementedException();
        }

        private List<TableColumn> createColumns()
        {
            List<TableColumn> lista = new List<TableColumn>();

            lista.Add(new TableColumn() { name = "COD_FOR", description = "COD_FOR", mandatory = true, size = 7, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COD_ITEM", description = "COD_ITEM", mandatory = true, size = 7, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "CODIGO_EAN13", description = "CODIGO_EAN13", mandatory = false, size = 13, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "DESCRICAO", description = "DESCRICAO", mandatory = false, size = 40, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "DESC_REDUZ", description = "DESC_REDUZ", mandatory = false, size = 22, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "DESC_COML", description = "DESC_COML", mandatory = false, size = 30, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "REFERENCIA", description = "REFERENCIA", mandatory = false, size = 20, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "EMB_FOR", description = "EMB_FOR", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TPO_EMB_FOR", description = "TPO_EMB_FOR", mandatory = false, size = 2, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "EMB_VENDA", description = "EMB_VENDA", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TPO_EMB_VENDA", description = "TPO_EMB_VENDA", mandatory = false, size = 2, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "COMPR_EMB_VND", description = "COMPR_EMB_VND", mandatory = false, size = 7, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "LARGURA_EMB_VND", description = "LARGURA_EMB_VND", mandatory = false, size = 7, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ALTURA_EMB_VND", description = "ALTURA_EMB_VND", mandatory = false, size = 7, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "TIPO_PLU", description = "TIPO_PLU", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DAT_SAI_LIN", description = "DAT_SAI_LIN", mandatory = false, size = 10, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "CLASS_FIS", description = "CLASS_FIS", mandatory = false, size = 10, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "PROCEDENCIA", description = "PROCEDENCIA", mandatory = false, size = 1, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "LASTUPDATE", description = "LASTUPDATE", mandatory = false, dataType = "db_Date" });
            lista.Add(new TableColumn() { name = "STATUS", description = "STATUS", mandatory = false, size = 1, dataType = "db_Numeric" });

            lista.Add(new TableColumn() { name = "COMPRIMENTO_EMB", description = "COMPRIMENTO_EMB", mandatory = false, size = 7, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ALTURA_EMB", description = "ALTURA_EMB", mandatory = false, size = 7, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "LARGURA_EMB", description = "LARGURA_EMB", mandatory = false, size = 7, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PESO", description = "PESO", mandatory = false, size = 8, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "TIPO_PRO", description = "TIPO_PRO", mandatory = false, size = 2, dataType = "db_Numeric" });

            //ADICIONAR PROPRIEDADE
            lista.Add(new TableColumn() { name = "DAT_ENT_LIN", description = "DAT_ENT_LIN", mandatory = false, size = 10, dataType = "db_Alpha" });

            //adiconar o campo tipo_PRO

            //Campos da tabela AA1DITEM
            lista.Add(new TableColumn() { name = "CEST", description = "CEST", mandatory = false, size = 7, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "TIP_PRO_SPED", description = "TIP_PRO_SPED", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "CONTROLE_LOTE_SERIE", description = "CONTROLE_LOTE_SERIE", mandatory = false, size = 1, dataType = "db_Alpha" });
            #region CAMPOS
            /*
            lista.Add(new TableColumn() { name = "DEPTO", description = "DEPTO", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SECAO", description = "SECAO", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "GRUPO", description = "GRUPO", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SUBGRUPO", description = "SUBGRUPO", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "CATEGORIA", description = "CATEGORIA", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DIGITO", description = "DIGITO", mandatory = true, size = 1, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "CODIGO_PAI", description = "CODIGO_PAI", mandatory = false, size = 8, dataType = "db_Numeric" });       
            lista.Add(new TableColumn() { name = "EAN_EMB_FOR", description = "EAN_EMB_FOR", mandatory = false, size = 11, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "EMB_TRANSF", description = "EMB_TRANSF", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TPO_EMB_TRANSF", description = "TPO_EMB_TRANSF", mandatory = false, size = 2, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "EAN_EMB_TRANSF", description = "EAN_EMB_TRANSF", mandatory = false, size = 11, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "EAN_EMB_VENDA", description = "EAN_EMB_VENDA", mandatory = false, size = 11, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TIPO_PALLET", description = "TIPO_PALLET", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "BASE_PALLET", description = "BASE_PALLET", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "ALTURA_PALLET", description = "ALTURA_PALLET", mandatory = false, size = 2, dataType = "db_Numeric" });
            
            lista.Add(new TableColumn() { name = "COMPR_EMB_TRF", description = "COMPR_EMB_TRF", mandatory = false, size = 7, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "LARGURA_EMB", description = "LARGURA_EMB", mandatory = false, size = 7, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "LARGURA_EMB_TRF", description = "LARGURA_EMB_TRF", mandatory = false, size = 7, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ALTURA_EMB", description = "ALTURA_EMB", mandatory = false, size = 7, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ALTURA_EMB_TRF", description = "ALTURA_EMB_TRF", mandatory = false, size = 7, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PESO", description = "PESO", mandatory = false, size = 8, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PESO_TRF", description = "PESO_TRF", mandatory = false, size = 8, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PESO_VND", description = "PESO_VND", mandatory = false, size = 8, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DEPOSITO", description = "DEPOSITO", mandatory = false, size = 8, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "LINHA", description = "LINHA", mandatory = false, size = 2, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "CLASSE", description = "CLASSE", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "POLIT_PRE", description = "POLIT_PRE", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "SIS_ABAST", description = "SIS_ABAST", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "PRZ_ENTRG", description = "PRZ_ENTRG", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DIA_VISIT", description = "DIA_VISIT", mandatory = false, size = 1, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FRQ_VISIT", description = "FRQ_VISIT", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TIPO_ETQ", description = "TIPO_ETQ", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TIPO_PRO", description = "TIPO_PRO", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COMPRADOR", description = "COMPRADOR", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COND_PGTO", description = "COND_PGTO", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COND_PGTO_ANT", description = "COND_PGTO_ANT", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COND_PGTO_MAN", description = "COND_PGTO_MAN", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "CDPG_VDA", description = "CDPG_VDA", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TIPO_ETQ_GON", description = "TIPO_ETQ_GON", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COR", description = "COR", mandatory = false, size = 3, dataType = "db_Alpha" });
           
            lista.Add(new TableColumn() { name = "NAT_FISCAL", description = "NAT_FISCAL", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "ESTADO", description = "ESTADO", mandatory = false, size = 2, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "COD_PAUTA", description = "COD_PAUTA", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "PERC_IPI", description = "PERC_IPI", mandatory = false, size = 7, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "QTDE_ETQ_GON", description = "QTDE_ETQ_GON", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "PERC_BONIF", description = "PERC_BONIF", mandatory = false, size = 8, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PERC_BONIF_ANT", description = "PERC_BONIF_ANT", mandatory = false, size = 8, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PERC_BONIF_MAN", description = "PERC_BONIF_MAN", mandatory = false, size = 8, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ENTREGA", description = "ENTREGA", mandatory = false, size = 1, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FRETE", description = "FRETE", mandatory = false, size = 4, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CUS_FOR", description = "CUS_FOR", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CUSF_ANT", description = "CUSF_ANT", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CUSF_MAN", description = "CUSF_MAN", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DAT_CUS_FOR", description = "DAT_CUS_FOR", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DAT_CUSF_ANT", description = "DAT_CUSF_ANT", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DAT_CUSF_MAN", description = "DAT_CUSF_MAN", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DESP_ACES", description = "DESP_ACES", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DESP_ACES_ANT", description = "DESP_ACES_ANT", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DESP_ACES_MAN", description = "DESP_ACES_MAN", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CUS_REP", description = "CUS_REP", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CUSR_ANT", description = "CUSR_ANT", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CUSR_MAN", description = "CUSR_MAN", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DAT_CUS_REP", description = "DAT_CUS_REP", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DAT_CUSR_ANT", description = "DAT_CUSR_ANT", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DAT_CUSR_MAN", description = "DAT_CUSR_MAN", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "CUS_MED", description = "CUS_MED", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CUSM_ANT", description = "CUSM_ANT", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CUSM_MAN", description = "CUSM_MAN", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DAT_CUS_MED", description = "DAT_CUS_MED", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DAT_CUSM_ANT", description = "DAT_CUSM_ANT", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DAT_CUSM_MAN", description = "DAT_CUSM_MAN", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "CUS_MED_C", description = "CUS_MED_C", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DAT_CUS_MED_C", description = "DAT_CUS_MED_C", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "PRC_VEN_1", description = "PRC_VEN_1", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PRCV_ANT_1", description = "PRCV_ANT_1", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PRCV_MAN_1", description = "PRCV_MAN_1", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "MRG_LUCRO_1", description = "MRG_LUCRO_1", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DSC_MAX_1", description = "DSC_MAX_1", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COMISSAO_1", description = "COMISSAO_1", mandatory = false, size = 4, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DAT_PRC_VEN_1", description = "DAT_PRC_VEN_1", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DAT_PRCV_ANT_1", description = "DAT_PRCV_ANT_1", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DAT_PRCV_MAN_1", description = "DAT_PRCV_MAN_1", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "PRC_VEN_2", description = "PRC_VEN_2", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PRCV_ANT_2", description = "PRCV_ANT_2", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PRCV_MAN_2", description = "PRCV_MAN_2", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "MRG_LUCRO_2", description = "MRG_LUCRO_2", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DSC_MAX_2", description = "DSC_MAX_2", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COMISSAO_2", description = "COMISSAO_2", mandatory = false, size = 4, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DAT_PRC_VEN_2", description = "DAT_PRC_VEN_2", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DAT_PRCV_ANT_2", description = "DAT_PRCV_ANT_2", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DAT_PRCV_MAN_2", description = "DAT_PRCV_MAN_2", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "PRC_VEN_3", description = "PRC_VEN_3", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PRCV_ANT_3", description = "PRCV_ANT_3", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PRCV_MAN_3", description = "PRCV_MAN_3", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "MRG_LUCRO_3", description = "MRG_LUCRO_3", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DSC_MAX_3", description = "DSC_MAX_3", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COMISSAO_3", description = "COMISSAO_3", mandatory = false, size = 4, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DAT_PRC_VEN_3", description = "DAT_PRC_VEN_3", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DAT_PRCV_ANT_3", description = "DAT_PRCV_ANT_3", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DAT_PRCV_MAN_3", description = "DAT_PRCV_MAN_3", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "PRC_VEN_4", description = "PRC_VEN_4", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PRCV_ANT_4", description = "PRCV_ANT_4", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PRCV_MAN_4", description = "PRCV_MAN_4", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "MRG_LUCRO_4", description = "MRG_LUCRO_4", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DSC_MAX_4", description = "DSC_MAX_4", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COMISSAO_4", description = "COMISSAO_4", mandatory = false, size = 4, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DAT_PRC_VEN_4", description = "DAT_PRC_VEN_4", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DAT_PRCV_ANT_4", description = "DAT_PRCV_ANT_4", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DAT_PRCV_MAN_4", description = "DAT_PRCV_MAN_4", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "PRC_VEN_5", description = "PRC_VEN_5", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PRCV_ANT_5", description = "PRCV_ANT_5", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PRCV_MAN_5", description = "PRCV_MAN_5", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "MRG_LUCRO_5", description = "MRG_LUCRO_5", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DSC_MAX_5", description = "DSC_MAX_5", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COMISSAO_5", description = "COMISSAO_5", mandatory = false, size = 4, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DAT_PRC_VEN_5", description = "DAT_PRC_VEN_5", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DAT_PRCV_ANT_5", description = "DAT_PRCV_ANT_5", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DAT_PRCV_MAN_5", description = "DAT_PRCV_MAN_5", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TIP_OFT_1", description = "TIP_OFT_1", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TIP_OFT_ANT_1", description = "TIP_OFT_ANT_1", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "INI_OFT_1", description = "INI_OFT_1", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "INI_OFT_ANT_1", description = "INI_OFT_ANT_1", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FIM_OFT_1", description = "FIM_OFT_1", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FIM_OFT_ANT_1", description = "FIM_OFT_ANT_1", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "PRC_OFT_1", description = "PRC_OFT_1", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PRC_OFT_ANT_1", description = "PRC_OFT_ANT_1", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "LIM_OFT_1", description = "LIM_OFT_1", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "LIM_OFT_ANT_1", description = "LIM_OFT_ANT_1", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SAL_OFT_1", description = "SAL_OFT_1", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SAL_OFT_ANT_1", description = "SAL_OFT_ANT_1", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TIP_OFT_2", description = "TIP_OFT_2", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TIP_OFT_ANT_2", description = "TIP_OFT_ANT_2", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "INI_OFT_2", description = "INI_OFT_2", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "INI_OFT_ANT_2", description = "INI_OFT_ANT_2", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FIM_OFT_2", description = "FIM_OFT_2", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FIM_OFT_ANT_2", description = "FIM_OFT_ANT_2", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "PRC_OFT_2", description = "PRC_OFT_2", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PRC_OFT_ANT_2", description = "PRC_OFT_ANT_2", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "LIM_OFT_2", description = "LIM_OFT_2", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "LIM_OFT_ANT_2", description = "LIM_OFT_ANT_2", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SAL_OFT_2", description = "SAL_OFT_2", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SAL_OFT_ANT_2", description = "SAL_OFT_ANT_2", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TIP_OFT_3", description = "TIP_OFT_3", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TIP_OFT_ANT_3", description = "TIP_OFT_ANT_3", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "INI_OFT_3", description = "INI_OFT_3", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "INI_OFT_ANT_3", description = "INI_OFT_ANT_3", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FIM_OFT_3", description = "FIM_OFT_3", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FIM_OFT_ANT_3", description = "FIM_OFT_ANT_3", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "PRC_OFT_3", description = "PRC_OFT_3", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PRC_OFT_ANT_3", description = "PRC_OFT_ANT_3", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "LIM_OFT_3", description = "LIM_OFT_3", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "LIM_OFT_ANT_3", description = "LIM_OFT_ANT_3", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SAL_OFT_3", description = "SAL_OFT_3", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SAL_OFT_ANT_3", description = "SAL_OFT_ANT_3", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TIP_OFT_4", description = "TIP_OFT_4", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TIP_OFT_ANT_4", description = "TIP_OFT_ANT_4", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "INI_OFT_4", description = "INI_OFT_4", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "INI_OFT_ANT_4", description = "INI_OFT_ANT_4", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FIM_OFT_4", description = "FIM_OFT_4", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FIM_OFT_ANT_4", description = "FIM_OFT_ANT_4", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "PRC_OFT_4", description = "PRC_OFT_4", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PRC_OFT_ANT_4", description = "PRC_OFT_ANT_4", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "LIM_OFT_4", description = "LIM_OFT_4", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "LIM_OFT_ANT_4", description = "LIM_OFT_ANT_4", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SAL_OFT_4", description = "SAL_OFT_4", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SAL_OFT_ANT_4", description = "SAL_OFT_ANT_4", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TIP_OFT_5", description = "TIP_OFT_5", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TIP_OFT_ANT_5", description = "TIP_OFT_ANT_5", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "INI_OFT_5", description = "INI_OFT_5", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "INI_OFT_ANT_5", description = "INI_OFT_ANT_5", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FIM_OFT_5", description = "FIM_OFT_5", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FIM_OFT_ANT_5", description = "FIM_OFT_ANT_5", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "PRC_OFT_5", description = "PRC_OFT_5", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PRC_OFT_ANT_5", description = "PRC_OFT_ANT_5", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "LIM_OFT_5", description = "LIM_OFT_5", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "LIM_OFT_ANT_5", description = "LIM_OFT_ANT_5", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SAL_OFT_5", description = "SAL_OFT_5", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SAL_OFT_ANT_5", description = "SAL_OFT_ANT_5", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "INI_BONUS", description = "INI_BONUS", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "INI_BONUS_ANT", description = "INI_BONUS_ANT", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FIM_BONUS", description = "FIM_BONUS", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "PRES_ENT", description = "PRES_ENT", mandatory = false, size = 9, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PRC_BONUS", description = "PRC_BONUS", mandatory = false, size = 9, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PRES_REP", description = "PRES_REP", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ESTQ_ATUAL", description = "ESTQ_ATUAL", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ESTQ_DP", description = "ESTQ_DP", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ESTQ_LJ", description = "ESTQ_LJ", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "QDE_PEND", description = "QDE_PEND", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CUS_INV", description = "CUS_INV", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ESTQ_PADRAO", description = "ESTQ_PADRAO", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "SAI_MED_CAL", description = "SAI_MED_CAL", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TAMANHO", description = "TAMANHO", mandatory = false, size = 4, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "SAI_ACM_UN", description = "SAI_ACM_UN", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "SAI_ACM_CUS", description = "SAI_ACM_CUS", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "SAI_ACM_VEN", description = "SAI_ACM_VEN", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CUS_ULT_ENT_BRU", description = "CUS_ULT_ENT_BRU", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ENT_ACM_UN", description = "ENT_ACM_UN", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ENT_ACM_CUS", description = "ENT_ACM_CUS", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DAT_ULT_FAT", description = "DAT_ULT_FAT", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "ULT_QDE_FAT", description = "ULT_QDE_FAT", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ULT_QDE_ENT", description = "ULT_QDE_ENT", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CUS_ULT_ENT", description = "CUS_ULT_ENT", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DAT_ULT_ENT", description = "DAT_ULT_ENT", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "ABC_F", description = "ABC_F", mandatory = false, size = 9, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ABC_S", description = "ABC_S", mandatory = false, size = 9, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ABC_T", description = "ABC_T", mandatory = false, size = 8, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PERECIVEL", description = "PERECIVEL", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "PRZ_VALIDADE", description = "PRZ_VALIDADE", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TOT_PEDIDO", description = "TOT_PEDIDO", mandatory = false, size = 7, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TOT_FALTA", description = "TOT_FALTA", mandatory = false, size = 7, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COD_VAS", description = "COD_VAS", mandatory = false, size = 7, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DIG_VAS", description = "DIG_VAS", mandatory = false, size = 1, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COD_ENG", description = "COD_ENG", mandatory = false, size = 7, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DIG_ENG", description = "DIG_ENG", mandatory = false, size = 1, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "VALOR_IPI", description = "VALOR_IPI", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "VALOR_IPI_ANT", description = "VALOR_IPI_ANT", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "VALOR_IPI_MAN", description = "VALOR_IPI_MAN", mandatory = false, size = 24, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "QTD_AUT_PDV", description = "QTD_AUT_PDV", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "MOEDA_VDA", description = "MOEDA_VDA", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "BONI_MERC", description = "BONI_MERC", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "BONI_MERC_ANT", description = "BONI_MERC_ANT", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "BONI_MERC_MAN", description = "BONI_MERC_MAN", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "GRADE", description = "GRADE", mandatory = false, size = 2, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "MENS_AUX", description = "MENS_AUX", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "CATEGORIA_ANT", description = "CATEGORIA_ANT", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "ESTRATEGIA_REP", description = "ESTRATEGIA_REP", mandatory = false, size = 2, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DESP_ACES_ISEN", description = "DESP_ACES_ISEN", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DESP_ACES_ISEN_ANT", description = "DESP_ACES_ISEN_ANT", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DESP_ACES_ISEN_MAN", description = "DESP_ACES_ISEN_MAN", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FRETE_VALOR", description = "FRETE_VALOR", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FRETE_VALOR_ANT", description = "FRETE_VALOR_ANT", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FRETE_VALOR_MAN", description = "FRETE_VALOR_MAN", mandatory = false, size = 15, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "BONIF_PER", description = "BONIF_PER", mandatory = false, size = 7, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "BONIF_PER_ANT", description = "BONIF_PER_ANT", mandatory = false, size = 7, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "BONIF_PER_MAN", description = "BONIF_PER_MAN", mandatory = false, size = 7, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "VASILHAME", description = "VASILHAME", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "PERMITE_DESC", description = "PERMITE_DESC", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "QTD_OBRIG", description = "QTD_OBRIG", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "ENVIA_PDV", description = "ENVIA_PDV", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "ENVIA_BALANCA", description = "ENVIA_BALANCA", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "PESADO_PDV", description = "PESADO_PDV", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "JPMA_FLAG1", description = "JPMA_FLAG1", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "JPMA_FLAG2", description = "JPMA_FLAG2", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "JPMA_FLAG3", description = "JPMA_FLAG3", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "LINHA_ANTERIOR", description = "LINHA_ANTERIOR", mandatory = false, size = 2, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "LINHA_VALIDA", description = "LINHA_VALIDA", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "QUANT_EAN", description = "QUANT_EAN", mandatory = false, size = 4, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TOT_ESTOCADO", description = "TOT_ESTOCADO", mandatory = false, size = 7, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "EMPIL_MAX", description = "EMPIL_MAX", mandatory = false, size = 1, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TIPO_END", description = "TIPO_END", mandatory = false, size = 1, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SAZONAL", description = "SAZONAL", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "MARCA_PROP", description = "MARCA_PROP", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "FILLER", description = "FILLER", mandatory = false, size = 11, dataType = "db_Numeric" });*/
            #endregion
            return lista;
        }

        private List<TableIndexes> createIndexes()
        {
            List<TableIndexes> lista = new List<TableIndexes>();

            lista.Add(new TableIndexes()
            {
                name = "COD_ITEM",
                isUnique = false,
                keys = new string[] { "COD_ITEM", "STATUS" }
            });

            return lista;
        }

        async private Task<bool> createVendorTable()
        {
            bool result = false;

            Table table = new Table(_serviceLayerConnector);

            table.name = "VSITPRODUCT_VENDORS";
            table.description = "Vendors";
            table.tableType = "bott_NoObject";
            table.columns = createColumnsVendors();
            table.indexes = createIndexesVendors();

            try
            {
                await table.create();
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        async private Task<bool> createComponentsTable()
        {
            bool result = false;

            Table table = new Table(_serviceLayerConnector);

            table.name = "VSITPRODUCT_COMP";
            table.description = "Componentes";
            table.tableType = "bott_NoObject";
            table.columns = createColumnsComponents();
            table.indexes = createIndexesComponents();

            try
            {
                await table.create();
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        private List<TableIndexes> createIndexesVendors()
        {
            List<TableIndexes> lista = new List<TableIndexes>();

            lista.Add(new TableIndexes()
            {
                name = "PRODUCT",
                isUnique = false,
                keys = new string[] { "COD_ITEM"}
            });

            lista.Add(new TableIndexes()
            {
                name = "VENDOR",
                isUnique = false,
                keys = new string[] { "COD_FORN" }
            });

            return lista;
        }

        private List<TableIndexes> createIndexesComponents()
        {
            List<TableIndexes> lista = new List<TableIndexes>();

            lista.Add(new TableIndexes()
            {
                name = "COMPONENT",
                isUnique = false,
                keys = new string[] { "COMPONENTE", "PRODUTO" }
            });

            lista.Add(new TableIndexes()
            {
                name = "PRODUCT",
                isUnique = false,
                keys = new string[] { "PRODUTO", "COMPONENTE" }
            });

            return lista;
        }
        private List<TableColumn> createColumnsVendors()
        {
            List<TableColumn> lista = new List<TableColumn>();

            lista.Add(new TableColumn() { name = "COD_ITEM", description = "COD_ITEM", mandatory = true, size = 7, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COD_FORN", description = "COD_FORN", mandatory = true, size = 7, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COD_FORN_ALT", description = "COD_FORN_ALT", mandatory = true, size = 7, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COD_ITEM_ALT", description = "COD_ITEM_ALT", mandatory = true, size = 7, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DIG_ITEM", description = "DIG_ITEM", mandatory = false, size = 1, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DIG_FORN", description = "DIG_FORN", mandatory = false, size = 1, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "REFERENCIA", description = "REFERENCIA", mandatory = false, size = 25, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "DESCRICAO", description = "DESCRICAO", mandatory = false, size = 60, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "FATUR_UNID", description = "FATUR_UNID", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "UF_UNID", description = "UF_UNID", mandatory = false, size = 5, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "UF_FATOR", description = "UF_FATOR", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "UF_FATOR_CONV", description = "UF_FATOR_CONV", mandatory = false, size = 14, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "EMB_XML", description = "EMB_XML", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "LASTUPDATE", description = "LASTUPDATE", mandatory = false, dataType = "db_Date" });

            return lista;
        }

        private List<TableColumn> createColumnsComponents()
        {
            List<TableColumn> lista = new List<TableColumn>();

            lista.Add(new TableColumn() { name = "COMPONENTE_AK", description = "COMPONENTE_AK", mandatory = true, size = 8, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "PRODUTO_AK", description = "PRODUTO_AK", mandatory = true, size = 8, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "PRODUTO", description = "PRODUTO", mandatory = false, size = 8, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "COMPONENTE", description = "COMPONENTE", mandatory = false, size = 8, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "QUANTIDADE", description = "QUANTIDADE", mandatory = false, size = 18, dataType = "db_Float", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "SEC_SIM_1", description = "SEC_SIM_1", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SEC_SIM_2", description = "SEC_SIM_2", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SEC_SIM_3", description = "SEC_SIM_3", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "GRP_SIM_1", description = "GRP_SIM_1", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "GRP_SIM_2", description = "GRP_SIM_2", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "GRP_SIM_3", description = "GRP_SIM_3", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SBG_SIM_1", description = "SBG_SIM_1", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SBG_SIM_2", description = "SBG_SIM_2", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "SBG_SIM_3", description = "SBG_SIM_3", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "CTG_SIM_1", description = "CTG_SIM_1", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "CTG_SIM_2", description = "CTG_SIM_2", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "CTG_SIM_3", description = "CTG_SIM_3", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FILLER", description = "FILLER", mandatory = false, size = 11, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "LASTUPDATE", description = "LASTUPDATE", mandatory = false, dataType = "db_Date" });
            lista.Add(new TableColumn() { name = "STATUS", description = "STATUS", mandatory = false, size = 2, dataType = "db_Numeric" });

            return lista;
        }

        private string toJsonVendors(ProductVendor vendor)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.Code = vendor.RecId.ToString();
            record.Name = vendor.RecId.ToString();
            record.U_COD_ITEM = vendor.cod_item;
            record.U_COD_FORN = vendor.cod_forn;
            record.U_COD_FORN_ALT = vendor.cod_forn_alt;
            record.U_COD_ITEM_ALT = vendor.cod_item_alt;
            record.U_DIG_ITEM = vendor.dig_item;
            record.U_DIG_FORN = vendor.dig_forn;
            record.U_REFERENCIA = vendor.referencia;
            record.U_DESCRICAO = vendor.descricao;
            record.U_FATUR_UNID = vendor.fatur_unid;
            record.U_UF_UNID = vendor.uf_unid;
            record.U_UF_FATOR = vendor.uf_fator;
            record.U_UF_FATOR_CONV = vendor.uf_fator_conv;
            record.U_EMB_XML = vendor.emb_xml;
            record.U_LASTUPDATE = DateTime.Now;




            result = JsonConvert.SerializeObject(record);

            return result;
        }

        private string toJsonComponents(ProductComponent component)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.Code = component.RecId.ToString();
            record.Name = component.RecId.ToString();
            record.U_PRODUTO = component.produto;
            record.U_COMPONENTE = component.componente;
            record.U_COMPONENTE_AK = component.componente_ak;
            record.U_PRODUTO_AK = component.produto_ak;
            record.U_QUANTIDADE = component.quantidade;
            record.U_SEC_SIM_1 = component.sec_sim_1;
            record.U_SEC_SIM_2 = component.sec_sim_2;
            record.U_SEC_SIM_3 = component.sec_sim_3;
            record.U_GRP_SIM_1 = component.grp_sim_1;
            record.U_GRP_SIM_2 = component.grp_sim_2;
            record.U_GRP_SIM_3 = component.grp_sim_3;
            record.U_SBG_SIM_1 = component.sbg_sim_1;
            record.U_SBG_SIM_2 = component.sbg_sim_2;
            record.U_SBG_SIM_3 = component.sbg_sim_3;
            record.U_CTG_SIM_1 = component.ctg_sim_1;
            record.U_CTG_SIM_2 = component.ctg_sim_2;
            record.U_CTG_SIM_3 = component.ctg_sim_3;
            record.U_FILLER = component.filler;
            record.U_LASTUPDATE = DateTime.Now;

            result = JsonConvert.SerializeObject(record);

            return result;
        }

        private string ToJsonStatus(Product product)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            //record.Code = product.RecId.ToString();
            //record.Name = product.RecId.ToString();
            record.U_STATUS = product.status;
            record.U_LASTUPDATE = DateTime.Now;
            result = JsonConvert.SerializeObject(record);
            return result;

        }

        private string toJson(Product product)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.Code = product.RecId.ToString();
            record.Name = product.RecId.ToString();

            record.U_CLASS_FIS = product.class_fis;
            record.U_COD_FOR = product.cod_for;
            record.U_COD_ITEM = product.cod_item;
            record.U_DESCRICAO = product.descricao;
            record.U_DESC_REDUZ = product.desc_reduz;
            record.U_DESC_COML = product.desc_coml;
            record.U_REFERENCIA = product.referencia;
            record.U_EMB_FOR = product.emb_for;
            record.U_TPO_EMB_FOR = product.tpo_emb_for;
            record.U_EMB_VENDA = product.emb_venda;
            record.U_TPO_EMB_VENDA = product.tpo_emb_venda;
            record.U_COMPR_EMB_VND = product.compr_emb_vnd;
            record.U_LARGURA_EMB_VND = product.largura_emb_vnd;
            record.U_CODIGO_EAN13 = product.codigo_ean13;
            record.U_ALTURA_EMB_VND = product.altura_emb_vnd;
            record.U_TIPO_PLU = product.tipo_plu;
            record.U_PROCEDENCIA = product.procedencia;
            record.U_DAT_SAI_LIN = product.dat_sai_lin;
            record.U_DAT_ENT_LIN = product.dat_ent_lin;
            record.U_STATUS = (Product.ProductIntegrationStatus.Importing);
            record.U_LASTUPDATE = DateTime.Now;
            record.U_ALTURA_EMB = product.altura_emb;
            record.U_LARGURA_EMB = product.largura_emb;
            record.U_PESO = product.peso;
            record.U_TIPO_PRO = product.tipo_pro;
            record.U_LARGURA_EMB = product.largura_emb;
            record.U_COMPRIMENTO_EMB = product.comprimento_emb;

            //campos tabela AA1DITEM
            record.U_CEST = product.cest;
            record.U_CONTROLE_LOTE_SERIE = product.controle_lote_serie;
            record.U_TIP_PRO_SPED = product.tip_pro_sped;

            #region

            /*
            record.U_DEPTO = product.DEPTO;
            record.U_SECAO = product.SECAO;
            record.U_GRUPO = product.GRUPO;
            record.U_SUBGRUPO = product.SUBGRUPO;
            record.U_CATEGORIA = product.CATEGORIA;     
            record.U_DIGITO = product.DIGITO;
            record.U_CODIGO_PAI = product.CODIGO_PAI;
            record.U_EAN_EMB_FOR = product.EAN_EMB_FOR;
            record.U_EMB_TRANSF = product.EMB_TRANSF;
            record.U_TPO_EMB_TRANSF = product.TPO_EMB_TRANSF;
            record.U_EAN_EMB_TRANSF = product.EAN_EMB_TRANSF;
            record.U_EAN_EMB_VENDA = product.EAN_EMB_VENDA;
            record.U_TIPO_PALLET = product.TIPO_PALLET;
            record.U_BASE_PALLET = product.BASE_PALLET;
            record.U_ALTURA_PALLET = product.ALTURA_PALLET;
            record.U_COMPRIMENTO_EMB = product.COMPRIMENTO_EMB;
            record.U_COMPR_EMB_TRF = product.COMPR_EMB_TRF;
            record.U_LARGURA_EMB = product.LARGURA_EMB;
            record.U_LARGURA_EMB_TRF = product.LARGURA_EMB_TRF;
            record.U_ALTURA_EMB = product.ALTURA_EMB;
            record.U_ALTURA_EMB_TRF = product.ALTURA_EMB_TRF;
            record.U_PESO = product.PESO;
            record.U_PESO_TRF = product.PESO_TRF;
            record.U_PESO_VND = product.PESO_VND;
            record.U_DEPOSITO = product.DEPOSITO;
            record.U_LINHA = product.LINHA;
            record.U_CLASSE = product.CLASSE;
            record.U_POLIT_PRE = product.POLIT_PRE;
            record.U_SIS_ABAST = product.SIS_ABAST;
            record.U_PRZ_ENTRG = product.PRZ_ENTRG;
            record.U_DIA_VISIT = product.DIA_VISIT;
            record.U_FRQ_VISIT = product.FRQ_VISIT;
            record.U_TIPO_ETQ = product.TIPO_ETQ;
            record.U_TIPO_PRO = product.TIPO_PRO;
            record.U_COMPRADOR = product.COMPRADOR;
            record.U_COND_PGTO = product.COND_PGTO;
            record.U_COND_PGTO_ANT = product.COND_PGTO_ANT;
            record.U_COND_PGTO_MAN = product.COND_PGTO_MAN;
            record.U_CDPG_VDA = product.CDPG_VDA;
            record.U_TIPO_ETQ_GON = product.TIPO_ETQ_GON;
            record.U_COR = product.COR;
            record.U_DAT_ENT_LIN = product.DAT_ENT_LIN;
            record.U_DAT_SAI_LIN = product.DAT_SAI_LIN;
            record.U_NAT_FISCAL = product.NAT_FISCAL;
            record.U_ESTADO = product.ESTADO;
            record.U_COD_PAUTA = product.COD_PAUTA;
            record.U_PERC_IPI = product.PERC_IPI;
            record.U_QTDE_ETQ_GON = product.QTDE_ETQ_GON;
            record.U_PERC_BONIF = product.PERC_BONIF;
            record.U_PERC_BONIF_ANT = product.PERC_BONIF_ANT;
            record.U_PERC_BONIF_MAN = product.PERC_BONIF_MAN;
            record.U_ENTREGA = product.ENTREGA;
            record.U_FRETE = product.FRETE;
            record.U_CUS_FOR = product.CUS_FOR;
            record.U_CUSF_ANT = product.CUSF_ANT;
            record.U_CUSF_MAN = product.CUSF_MAN;
            record.U_DAT_CUS_FOR = product.DAT_CUS_FOR;
            record.U_DAT_CUSF_ANT = product.DAT_CUSF_ANT;
            record.U_DAT_CUSF_MAN = product.DAT_CUSF_MAN;
            record.U_DESP_ACES = product.DESP_ACES;
            record.U_DESP_ACES_ANT = product.DESP_ACES_ANT;
            record.U_DESP_ACES_MAN = product.DESP_ACES_MAN;
            record.U_CUS_REP = product.CUS_REP;
            record.U_CUSR_ANT = product.CUSR_ANT;
            record.U_CUSR_MAN = product.CUSR_MAN;
            record.U_DAT_CUS_REP = product.DAT_CUS_REP;
            record.U_DAT_CUSR_ANT = product.DAT_CUSR_ANT;
            record.U_DAT_CUSR_MAN = product.DAT_CUSR_MAN;
            record.U_CUS_MED = product.CUS_MED;
            record.U_CUSM_ANT = product.CUSM_ANT;
            record.U_CUSM_MAN = product.CUSM_MAN;
            record.U_DAT_CUS_MED = product.DAT_CUS_MED;
            record.U_DAT_CUSM_ANT = product.DAT_CUSM_ANT;
            record.U_DAT_CUSM_MAN = product.DAT_CUSM_MAN;
            record.U_CUS_MED_C = product.CUS_MED_C;
            record.U_DAT_CUS_MED_C = product.DAT_CUS_MED_C;
            record.U_PRC_VEN_1 = product.PRC_VEN_1;
            record.U_PRCV_ANT_1 = product.PRCV_ANT_1;
            record.U_PRCV_MAN_1 = product.PRCV_MAN_1;
            record.U_MRG_LUCRO_1 = product.MRG_LUCRO_1;
            record.U_DSC_MAX_1 = product.DSC_MAX_1;
            record.U_COMISSAO_1 = product.COMISSAO_1;
            record.U_DAT_PRC_VEN_1 = product.DAT_PRC_VEN_1;
            record.U_DAT_PRCV_ANT_1 = product.DAT_PRCV_ANT_1;
            record.U_DAT_PRCV_MAN_1 = product.DAT_PRCV_MAN_1;
            record.U_PRC_VEN_2 = product.PRC_VEN_2;
            record.U_PRCV_ANT_2 = product.PRCV_ANT_2;
            record.U_PRCV_MAN_2 = product.PRCV_MAN_2;
            record.U_MRG_LUCRO_2 = product.MRG_LUCRO_2;
            record.U_DSC_MAX_2 = product.DSC_MAX_2;
            record.U_COMISSAO_2 = product.COMISSAO_2;
            record.U_DAT_PRC_VEN_2 = product.DAT_PRC_VEN_2;
            record.U_DAT_PRCV_ANT_2 = product.DAT_PRCV_ANT_2;
            record.U_DAT_PRCV_MAN_2 = product.DAT_PRCV_MAN_2;
            record.U_PRC_VEN_3 = product.PRC_VEN_3;
            record.U_PRCV_ANT_3 = product.PRCV_ANT_3;
            record.U_PRCV_MAN_3 = product.PRCV_MAN_3;
            record.U_MRG_LUCRO_3 = product.MRG_LUCRO_3;
            record.U_DSC_MAX_3 = product.DSC_MAX_3;
            record.U_COMISSAO_3 = product.COMISSAO_3;
            record.U_DAT_PRC_VEN_3 = product.DAT_PRC_VEN_3;
            record.U_DAT_PRCV_ANT_3 = product.DAT_PRCV_ANT_3;
            record.U_DAT_PRCV_MAN_3 = product.DAT_PRCV_MAN_3;
            record.U_PRC_VEN_4 = product.PRC_VEN_4;
            record.U_PRCV_ANT_4 = product.PRCV_ANT_4;
            record.U_PRCV_MAN_4 = product.PRCV_MAN_4;
            record.U_MRG_LUCRO_4 = product.MRG_LUCRO_4;
            record.U_DSC_MAX_4 = product.DSC_MAX_4;
            record.U_COMISSAO_4 = product.COMISSAO_4;
            record.U_DAT_PRC_VEN_4 = product.DAT_PRC_VEN_4;
            record.U_DAT_PRCV_ANT_4 = product.DAT_PRCV_ANT_4;
            record.U_DAT_PRCV_MAN_4 = product.DAT_PRCV_MAN_4;
            record.U_PRC_VEN_5 = product.PRC_VEN_5;
            record.U_PRCV_ANT_5 = product.PRCV_ANT_5;
            record.U_PRCV_MAN_5 = product.PRCV_MAN_5;
            record.U_MRG_LUCRO_5 = product.MRG_LUCRO_5;
            record.U_DSC_MAX_5 = product.DSC_MAX_5;
            record.U_COMISSAO_5 = product.COMISSAO_5;
            record.U_DAT_PRC_VEN_5 = product.DAT_PRC_VEN_5;
            record.U_DAT_PRCV_ANT_5 = product.DAT_PRCV_ANT_5;
            record.U_DAT_PRCV_MAN_5 = product.DAT_PRCV_MAN_5;
            record.U_TIP_OFT_1 = product.TIP_OFT_1;
            record.U_TIP_OFT_ANT_1 = product.TIP_OFT_ANT_1;
            record.U_INI_OFT_1 = product.INI_OFT_1;
            record.U_INI_OFT_ANT_1 = product.INI_OFT_ANT_1;
            record.U_FIM_OFT_1 = product.FIM_OFT_1;
            record.U_FIM_OFT_ANT_1 = product.FIM_OFT_ANT_1;
            record.U_PRC_OFT_1 = product.PRC_OFT_1;
            record.U_PRC_OFT_ANT_1 = product.PRC_OFT_ANT_1;
            record.U_LIM_OFT_1 = product.LIM_OFT_1;
            record.U_LIM_OFT_ANT_1 = product.LIM_OFT_ANT_1;
            record.U_SAL_OFT_1 = product.SAL_OFT_1;
            record.U_SAL_OFT_ANT_1 = product.SAL_OFT_ANT_1;
            record.U_TIP_OFT_2 = product.TIP_OFT_2;
            record.U_TIP_OFT_ANT_2 = product.TIP_OFT_ANT_2;
            record.U_INI_OFT_2 = product.INI_OFT_2;
            record.U_INI_OFT_ANT_2 = product.INI_OFT_ANT_2;
            record.U_FIM_OFT_2 = product.FIM_OFT_2;
            record.U_FIM_OFT_ANT_2 = product.FIM_OFT_ANT_2;
            record.U_PRC_OFT_2 = product.PRC_OFT_2;
            record.U_PRC_OFT_ANT_2 = product.PRC_OFT_ANT_2;
            record.U_LIM_OFT_2 = product.LIM_OFT_2;
            record.U_LIM_OFT_ANT_2 = product.LIM_OFT_ANT_2;
            record.U_SAL_OFT_2 = product.SAL_OFT_2;
            record.U_SAL_OFT_ANT_2 = product.SAL_OFT_ANT_2;
            record.U_TIP_OFT_3 = product.TIP_OFT_3;
            record.U_TIP_OFT_ANT_3 = product.TIP_OFT_ANT_3;
            record.U_INI_OFT_3 = product.INI_OFT_3;
            record.U_INI_OFT_ANT_3 = product.INI_OFT_ANT_3;
            record.U_FIM_OFT_3 = product.FIM_OFT_3;
            record.U_FIM_OFT_ANT_3 = product.FIM_OFT_ANT_3;
            record.U_PRC_OFT_3 = product.PRC_OFT_3;
            record.U_PRC_OFT_ANT_3 = product.PRC_OFT_ANT_3;
            record.U_LIM_OFT_3 = product.LIM_OFT_3;
            record.U_LIM_OFT_ANT_3 = product.LIM_OFT_ANT_3;
            record.U_SAL_OFT_3 = product.SAL_OFT_3;
            record.U_SAL_OFT_ANT_3 = product.SAL_OFT_ANT_3;
            record.U_TIP_OFT_4 = product.TIP_OFT_4;
            record.U_TIP_OFT_ANT_4 = product.TIP_OFT_ANT_4;
            record.U_INI_OFT_4 = product.INI_OFT_4;
            record.U_INI_OFT_ANT_4 = product.INI_OFT_ANT_4;
            record.U_FIM_OFT_4 = product.FIM_OFT_4;
            record.U_FIM_OFT_ANT_4 = product.FIM_OFT_ANT_4;
            record.U_PRC_OFT_4 = product.PRC_OFT_4;
            record.U_PRC_OFT_ANT_4 = product.PRC_OFT_ANT_4;
            record.U_LIM_OFT_4 = product.LIM_OFT_4;
            record.U_LIM_OFT_ANT_4 = product.LIM_OFT_ANT_4;
            record.U_SAL_OFT_4 = product.SAL_OFT_4;
            record.U_SAL_OFT_ANT_4 = product.SAL_OFT_ANT_4;
            record.U_TIP_OFT_5 = product.TIP_OFT_5;
            record.U_TIP_OFT_ANT_5 = product.TIP_OFT_ANT_5;
            record.U_INI_OFT_5 = product.INI_OFT_5;
            record.U_INI_OFT_ANT_5 = product.INI_OFT_ANT_5;
            record.U_FIM_OFT_5 = product.FIM_OFT_5;
            record.U_FIM_OFT_ANT_5 = product.FIM_OFT_ANT_5;
            record.U_PRC_OFT_5 = product.PRC_OFT_5;
            record.U_PRC_OFT_ANT_5 = product.PRC_OFT_ANT_5;
            record.U_LIM_OFT_5 = product.LIM_OFT_5;
            record.U_LIM_OFT_ANT_5 = product.LIM_OFT_ANT_5;
            record.U_SAL_OFT_5 = product.SAL_OFT_5;
            record.U_SAL_OFT_ANT_5 = product.SAL_OFT_ANT_5;
            record.U_INI_BONUS = product.INI_BONUS;
            record.U_INI_BONUS_ANT = product.INI_BONUS_ANT;
            record.U_FIM_BONUS = product.FIM_BONUS;
            record.U_PRES_ENT = product.PRES_ENT;
            record.U_PRC_BONUS = product.PRC_BONUS;
            record.U_PRES_REP = product.PRES_REP;
            record.U_ESTQ_ATUAL = product.ESTQ_ATUAL;
            record.U_ESTQ_DP = product.ESTQ_DP;
            record.U_ESTQ_LJ = product.ESTQ_LJ;
            record.U_QDE_PEND = product.QDE_PEND;
            record.U_CUS_INV = product.CUS_INV;
            record.U_ESTQ_PADRAO = product.ESTQ_PADRAO;
            record.U_SAI_MED_CAL = product.SAI_MED_CAL;
            record.U_TAMANHO = product.TAMANHO;
            record.U_SAI_ACM_UN = product.SAI_ACM_UN;
            record.U_SAI_ACM_CUS = product.SAI_ACM_CUS;
            record.U_SAI_ACM_VEN = product.SAI_ACM_VEN;
            record.U_CUS_ULT_ENT_BRU = product.CUS_ULT_ENT_BRU;
            record.U_ENT_ACM_UN = product.ENT_ACM_UN;
            record.U_ENT_ACM_CUS = product.ENT_ACM_CUS;
            record.U_DAT_ULT_FAT = product.DAT_ULT_FAT;
            record.U_ULT_QDE_FAT = product.ULT_QDE_FAT;
            record.U_ULT_QDE_ENT = product.ULT_QDE_ENT;
            record.U_CUS_ULT_ENT = product.CUS_ULT_ENT;
            record.U_DAT_ULT_ENT = product.DAT_ULT_ENT;
            record.U_ABC_F = product.ABC_F;
            record.U_ABC_S = product.ABC_S;
            record.U_ABC_T = product.ABC_T;
            record.U_PERECIVEL = product.PERECIVEL;
            record.U_PRZ_VALIDADE = product.PRZ_VALIDADE;
            record.U_TOT_PEDIDO = product.TOT_PEDIDO;
            record.U_TOT_FALTA = product.TOT_FALTA;
            record.U_COD_VAS = product.COD_VAS;
            record.U_DIG_VAS = product.DIG_VAS;
            record.U_COD_ENG = product.COD_ENG;
            record.U_DIG_ENG = product.DIG_ENG;
            record.U_VALOR_IPI = product.VALOR_IPI;
            record.U_VALOR_IPI_ANT = product.VALOR_IPI_ANT;
            record.U_VALOR_IPI_MAN = product.VALOR_IPI_MAN;
            record.U_QTD_AUT_PDV = product.QTD_AUT_PDV;
            record.U_MOEDA_VDA = product.MOEDA_VDA;
            record.U_BONI_MERC = product.BONI_MERC;
            record.U_BONI_MERC_ANT = product.BONI_MERC_ANT;
            record.U_BONI_MERC_MAN = product.BONI_MERC_MAN;
            record.U_GRADE = product.GRADE;
            record.U_MENS_AUX = product.MENS_AUX;
            record.U_CATEGORIA_ANT = product.CATEGORIA_ANT;
            record.U_ESTRATEGIA_REP = product.ESTRATEGIA_REP;
            record.U_DESP_ACES_ISEN = product.DESP_ACES_ISEN;
            record.U_DESP_ACES_ISEN_ANT = product.DESP_ACES_ISEN_ANT;
            record.U_DESP_ACES_ISEN_MAN = product.DESP_ACES_ISEN_MAN;
            record.U_FRETE_VALOR = product.FRETE_VALOR;
            record.U_FRETE_VALOR_ANT = product.FRETE_VALOR_ANT;
            record.U_FRETE_VALOR_MAN = product.FRETE_VALOR_MAN;
            record.U_BONIF_PER = product.BONIF_PER;
            record.U_BONIF_PER_ANT = product.BONIF_PER_ANT;
            record.U_BONIF_PER_MAN = product.BONIF_PER_MAN;
            record.U_VASILHAME = product.VASILHAME;
            record.U_PERMITE_DESC = product.PERMITE_DESC;
            record.U_QTD_OBRIG = product.QTD_OBRIG;
            record.U_ENVIA_PDV = product.ENVIA_PDV;
            record.U_ENVIA_BALANCA = product.ENVIA_BALANCA;
            record.U_PESADO_PDV = product.PESADO_PDV;
            record.U_JPMA_FLAG1 = product.JPMA_FLAG1;
            record.U_JPMA_FLAG2 = product.JPMA_FLAG2;
            record.U_JPMA_FLAG3 = product.JPMA_FLAG3;
            record.U_LINHA_ANTERIOR = product.LINHA_ANTERIOR;
            record.U_LINHA_VALIDA = product.LINHA_VALIDA;
            record.U_QUANT_EAN = product.QUANT_EAN;
            record.U_TOT_ESTOCADO = product.TOT_ESTOCADO;
            record.U_EMPIL_MAX = product.EMPIL_MAX;
            record.U_TIPO_END = product.TIPO_END;
            record.U_SAZONAL = product.SAZONAL;
            record.U_MARCA_PROP = product.MARCA_PROP;
            record.U_FILLER = product.FILLER;*/
            #endregion


            result = JsonConvert.SerializeObject(record);

            //result = JsonConvert.SerializeObject(cadEntidade);
            return result;
        }

        //private string toJson(CadEntidade cadEntidade)
        //{
        //    string result = string.Empty;

        //    dynamic record = new ExpandoObject();

        //    //record.Code = invoiceItem.RecId.ToString();
        //    //record.Name = invoiceItem.RecId.ToString();
        //    //record.U_INVOICECODE = invoice.RecId.ToString();
        //    //record.U_ITEMID = invoiceItem.itemId;
        //    //record.U_QUANTITY = invoiceItem.quantity;
        //    //record.U_UNITPRICE = invoiceItem.unitPrice;
        //    //record.U_DETERMINATION = invoiceItem.taxCodeDetermination;
        //    //record.U_CSTPIS = invoiceItem.cstPIS;
        //    //record.U_CSTCOFINS = invoiceItem.cstCOFINS;

        //    result = JsonConvert.SerializeObject(record);

        //    return result;
        //}

        private Product toRecord(dynamic record)
        {
            Product product = new Product();
            product.RecId = Guid.Parse(record.Code);
            product.cod_for = record.U_COD_FOR;
            product.cod_item = record.U_COD_ITEM;
            product.codigo_ean13 = record.U_CODIGO_EAN13;
            product.descricao = record.U_DESCRICAO;
            product.desc_reduz = record.U_DESC_REDUZ;
            product.desc_coml = record.U_DESC_COML;
            product.referencia = record.U_REFERENCIA;
            product.emb_for = record.U_EMB_FOR;
            product.tpo_emb_for = record.U_TPO_EMB_FOR;
            product.emb_venda = record.U_EMB_VENDA;
            product.tpo_emb_venda = record.U_TPO_EMB_VENDA;
            product.compr_emb_vnd = record.U_COMPR_EMB_VND;
            product.altura_emb_vnd = record.U_ALTURA_EMB_VND;
            product.tipo_plu = record.U_TIPO_PLU;
            product.largura_emb_vnd = record.U_LARGURA_EMB_VND;
            product.class_fis = record.U_CLASS_FIS;
            product.procedencia = record.U_PROCEDENCIA;
            product.dat_sai_lin = long.Parse(record.U_DAT_SAI_LIN);
            product.dat_ent_lin = long.Parse(record.U_DAT_ENT_LIN);
            product.lastupdate = parseDate(record.U_LASTUPDATE);
            product.status = (Product.ProductIntegrationStatus)record.U_STATUS;
            try
            {
                product.altura_emb = record.U_ALTURA_EMB;
                product.largura_emb = record.U_LARGURA_EMB;
                product.comprimento_emb = record.U_COMPRIMENTO_EMB;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            product.peso = record.U_PESO;
            product.tipo_pro = record.U_TIPO_PRO;

            //campos tabela AA1DITEM
            product.cest = record.U_CEST;
            product.controle_lote_serie = record.U_CONTROLE_LOTE_SERIE;
            product.tip_pro_sped = record.U_TIP_PRO_SPED;



            #region CAMPOS
            /*
            product.DEPTO = record.U_DEPTO;
            product.SECAO = record.U_SECAO;
            product.GRUPO = record.U_GRUPO;
            product.SUBGRUPO = record.U_SUBGRUPO;
            product.CATEGORIA = record.U_CATEGORIA;
            product.DIGITO = record.U_DIGITO;
            product.CODIGO_PAI = record.U_CODIGO_PAI;
            product.EAN_EMB_FOR = record.U_EAN_EMB_FOR;
            product.EMB_TRANSF = record.U_EMB_TRANSF;
            product.TPO_EMB_TRANSF = record.U_TPO_EMB_TRANSF;
            product.EAN_EMB_TRANSF = record.U_EAN_EMB_TRANSF;
            product.EAN_EMB_VENDA = record.U_EAN_EMB_VENDA;
            product.TIPO_PALLET = record.U_TIPO_PALLET;
            product.BASE_PALLET = record.U_BASE_PALLET;
            product.ALTURA_PALLET = record.U_ALTURA_PALLET;
            product.COMPRIMENTO_EMB = record.U_COMPRIMENTO_EMB;
            product.COMPR_EMB_TRF = record.U_COMPR_EMB_TRF;
            product.LARGURA_EMB = record.U_LARGURA_EMB;
            product.LARGURA_EMB_TRF = record.U_LARGURA_EMB_TRF;
            product.ALTURA_EMB = record.U_ALTURA_EMB;
            product.ALTURA_EMB_TRF = record.U_ALTURA_EMB_TRF;
            product.PESO = record.U_PESO;
            product.PESO_TRF = record.U_PESO_TRF;
            product.PESO_VND = record.U_PESO_VND;
            product.DEPOSITO = record.U_DEPOSITO;
            product.LINHA = record.U_LINHA;
            product.CLASSE = record.U_CLASSE;
            product.POLIT_PRE = record.U_POLIT_PRE;
            product.SIS_ABAST = record.U_SIS_ABAST;
            product.PRZ_ENTRG = record.U_PRZ_ENTRG;
            product.DIA_VISIT = record.U_DIA_VISIT;
            product.FRQ_VISIT = record.U_FRQ_VISIT;
            product.TIPO_ETQ = record.U_TIPO_ETQ;
            product.TIPO_PRO = record.U_TIPO_PRO;
            product.COMPRADOR = record.U_COMPRADOR;
            product.COND_PGTO = record.U_COND_PGTO;
            product.COND_PGTO_ANT = record.U_COND_PGTO_ANT;
            product.COND_PGTO_MAN = record.U_COND_PGTO_MAN;
            product.CDPG_VDA = record.U_CDPG_VDA;
            product.TIPO_ETQ_GON = record.U_TIPO_ETQ_GON;
            product.COR = record.U_COR;
            product.DAT_ENT_LIN = record.U_DAT_ENT_LIN;
            
            product.NAT_FISCAL = record.U_NAT_FISCAL;
            product.ESTADO = record.U_ESTADO;
            product.COD_PAUTA = record.U_COD_PAUTA;
            product.PERC_IPI = record.U_PERC_IPI;
            product.QTDE_ETQ_GON = record.U_QTDE_ETQ_GON;
            product.PERC_BONIF = record.U_PERC_BONIF;
            product.PERC_BONIF_ANT = record.U_PERC_BONIF_ANT;
            product.PERC_BONIF_MAN = record.U_PERC_BONIF_MAN;
            product.ENTREGA = record.U_ENTREGA;
            product.FRETE = record.U_FRETE;
            product.CUS_FOR = record.U_CUS_FOR;
            product.CUSF_ANT = record.U_CUSF_ANT;
            product.CUSF_MAN = record.U_CUSF_MAN;
            product.DAT_CUS_FOR = record.U_DAT_CUS_FOR;
            product.DAT_CUSF_ANT = record.U_DAT_CUSF_ANT;
            product.DAT_CUSF_MAN = record.U_DAT_CUSF_MAN;
            product.DESP_ACES = record.U_DESP_ACES;
            product.DESP_ACES_ANT = record.U_DESP_ACES_ANT;
            product.DESP_ACES_MAN = record.U_DESP_ACES_MAN;
            product.CUS_REP = record.U_CUS_REP;
            product.CUSR_ANT = record.U_CUSR_ANT;
            product.CUSR_MAN = record.U_CUSR_MAN;
            product.DAT_CUS_REP = record.U_DAT_CUS_REP;
            product.DAT_CUSR_ANT = record.U_DAT_CUSR_ANT;
            product.DAT_CUSR_MAN = record.U_DAT_CUSR_MAN;
            product.CUS_MED = record.U_CUS_MED;
            product.CUSM_ANT = record.U_CUSM_ANT;
            product.CUSM_MAN = record.U_CUSM_MAN;
            product.DAT_CUS_MED = record.U_DAT_CUS_MED;
            product.DAT_CUSM_ANT = record.U_DAT_CUSM_ANT;
            product.DAT_CUSM_MAN = record.U_DAT_CUSM_MAN;
            product.CUS_MED_C = record.U_CUS_MED_C;
            product.DAT_CUS_MED_C = record.U_DAT_CUS_MED_C;
            product.PRC_VEN_1 = record.U_PRC_VEN_1;
            product.PRCV_ANT_1 = record.U_PRCV_ANT_1;
            product.PRCV_MAN_1 = record.U_PRCV_MAN_1;
            product.MRG_LUCRO_1 = record.U_MRG_LUCRO_1;
            product.DSC_MAX_1 = record.U_DSC_MAX_1;
            product.COMISSAO_1 = record.U_COMISSAO_1;
            product.DAT_PRC_VEN_1 = record.U_DAT_PRC_VEN_1;
            product.DAT_PRCV_ANT_1 = record.U_DAT_PRCV_ANT_1;
            product.DAT_PRCV_MAN_1 = record.U_DAT_PRCV_MAN_1;
            product.PRC_VEN_2 = record.U_PRC_VEN_2;
            product.PRCV_ANT_2 = record.U_PRCV_ANT_2;
            product.PRCV_MAN_2 = record.U_PRCV_MAN_2;
            product.MRG_LUCRO_2 = record.U_MRG_LUCRO_2;
            product.DSC_MAX_2 = record.U_DSC_MAX_2;
            product.COMISSAO_2 = record.U_COMISSAO_2;
            product.DAT_PRC_VEN_2 = record.U_DAT_PRC_VEN_2;
            product.DAT_PRCV_ANT_2 = record.U_DAT_PRCV_ANT_2;
            product.DAT_PRCV_MAN_2 = record.U_DAT_PRCV_MAN_2;
            product.PRC_VEN_3 = record.U_PRC_VEN_3;
            product.PRCV_ANT_3 = record.U_PRCV_ANT_3;
            product.PRCV_MAN_3 = record.U_PRCV_MAN_3;
            product.MRG_LUCRO_3 = record.U_MRG_LUCRO_3;
            product.DSC_MAX_3 = record.U_DSC_MAX_3;
            product.COMISSAO_3 = record.U_COMISSAO_3;
            product.DAT_PRC_VEN_3 = record.U_DAT_PRC_VEN_3;
            product.DAT_PRCV_ANT_3 = record.U_DAT_PRCV_ANT_3;
            product.DAT_PRCV_MAN_3 = record.U_DAT_PRCV_MAN_3;
            product.PRC_VEN_4 = record.U_PRC_VEN_4;
            product.PRCV_ANT_4 = record.U_PRCV_ANT_4;
            product.PRCV_MAN_4 = record.U_PRCV_MAN_4;
            product.MRG_LUCRO_4 = record.U_MRG_LUCRO_4;
            product.DSC_MAX_4 = record.U_DSC_MAX_4;
            product.COMISSAO_4 = record.U_COMISSAO_4;
            product.DAT_PRC_VEN_4 = record.U_DAT_PRC_VEN_4;
            product.DAT_PRCV_ANT_4 = record.U_DAT_PRCV_ANT_4;
            product.DAT_PRCV_MAN_4 = record.U_DAT_PRCV_MAN_4;
            product.PRC_VEN_5 = record.U_PRC_VEN_5;
            product.PRCV_ANT_5 = record.U_PRCV_ANT_5;
            product.PRCV_MAN_5 = record.U_PRCV_MAN_5;
            product.MRG_LUCRO_5 = record.U_MRG_LUCRO_5;
            product.DSC_MAX_5 = record.U_DSC_MAX_5;
            product.COMISSAO_5 = record.U_COMISSAO_5;
            product.DAT_PRC_VEN_5 = record.U_DAT_PRC_VEN_5;
            product.DAT_PRCV_ANT_5 = record.U_DAT_PRCV_ANT_5;
            product.DAT_PRCV_MAN_5 = record.U_DAT_PRCV_MAN_5;
            product.TIP_OFT_1 = record.U_TIP_OFT_1;
            product.TIP_OFT_ANT_1 = record.U_TIP_OFT_ANT_1;
            product.INI_OFT_1 = record.U_INI_OFT_1;
            product.INI_OFT_ANT_1 = record.U_INI_OFT_ANT_1;
            product.FIM_OFT_1 = record.U_FIM_OFT_1;
            product.FIM_OFT_ANT_1 = record.U_FIM_OFT_ANT_1;
            product.PRC_OFT_1 = record.U_PRC_OFT_1;
            product.PRC_OFT_ANT_1 = record.U_PRC_OFT_ANT_1;
            product.LIM_OFT_1 = record.U_LIM_OFT_1;
            product.LIM_OFT_ANT_1 = record.U_LIM_OFT_ANT_1;
            product.SAL_OFT_1 = record.U_SAL_OFT_1;
            product.SAL_OFT_ANT_1 = record.U_SAL_OFT_ANT_1;
            product.TIP_OFT_2 = record.U_TIP_OFT_2;
            product.TIP_OFT_ANT_2 = record.U_TIP_OFT_ANT_2;
            product.INI_OFT_2 = record.U_INI_OFT_2;
            product.INI_OFT_ANT_2 = record.U_INI_OFT_ANT_2;
            product.FIM_OFT_2 = record.U_FIM_OFT_2;
            product.FIM_OFT_ANT_2 = record.U_FIM_OFT_ANT_2;
            product.PRC_OFT_2 = record.U_PRC_OFT_2;
            product.PRC_OFT_ANT_2 = record.U_PRC_OFT_ANT_2;
            product.LIM_OFT_2 = record.U_LIM_OFT_2;
            product.LIM_OFT_ANT_2 = record.U_LIM_OFT_ANT_2;
            product.SAL_OFT_2 = record.U_SAL_OFT_2;
            product.SAL_OFT_ANT_2 = record.U_SAL_OFT_ANT_2;
            product.TIP_OFT_3 = record.U_TIP_OFT_3;
            product.TIP_OFT_ANT_3 = record.U_TIP_OFT_ANT_3;
            product.INI_OFT_3 = record.U_INI_OFT_3;
            product.INI_OFT_ANT_3 = record.U_INI_OFT_ANT_3;
            product.FIM_OFT_3 = record.U_FIM_OFT_3;
            product.FIM_OFT_ANT_3 = record.U_FIM_OFT_ANT_3;
            product.PRC_OFT_3 = record.U_PRC_OFT_3;
            product.PRC_OFT_ANT_3 = record.U_PRC_OFT_ANT_3;
            product.LIM_OFT_3 = record.U_LIM_OFT_3;
            product.LIM_OFT_ANT_3 = record.U_LIM_OFT_ANT_3;
            product.SAL_OFT_3 = record.U_SAL_OFT_3;
            product.SAL_OFT_ANT_3 = record.U_SAL_OFT_ANT_3;
            product.TIP_OFT_4 = record.U_TIP_OFT_4;
            product.TIP_OFT_ANT_4 = record.U_TIP_OFT_ANT_4;
            product.INI_OFT_4 = record.U_INI_OFT_4;
            product.INI_OFT_ANT_4 = record.U_INI_OFT_ANT_4;
            product.FIM_OFT_4 = record.U_FIM_OFT_4;
            product.FIM_OFT_ANT_4 = record.U_FIM_OFT_ANT_4;
            product.PRC_OFT_4 = record.U_PRC_OFT_4;
            product.PRC_OFT_ANT_4 = record.U_PRC_OFT_ANT_4;
            product.LIM_OFT_4 = record.U_LIM_OFT_4;
            product.LIM_OFT_ANT_4 = record.U_LIM_OFT_ANT_4;
            product.SAL_OFT_4 = record.U_SAL_OFT_4;
            product.SAL_OFT_ANT_4 = record.U_SAL_OFT_ANT_4;
            product.TIP_OFT_5 = record.U_TIP_OFT_5;
            product.TIP_OFT_ANT_5 = record.U_TIP_OFT_ANT_5;
            product.INI_OFT_5 = record.U_INI_OFT_5;
            product.INI_OFT_ANT_5 = record.U_INI_OFT_ANT_5;
            product.FIM_OFT_5 = record.U_FIM_OFT_5;
            product.FIM_OFT_ANT_5 = record.U_FIM_OFT_ANT_5;
            product.PRC_OFT_5 = record.U_PRC_OFT_5;
            product.PRC_OFT_ANT_5 = record.U_PRC_OFT_ANT_5;
            product.LIM_OFT_5 = record.U_LIM_OFT_5;
            product.LIM_OFT_ANT_5 = record.U_LIM_OFT_ANT_5;
            product.SAL_OFT_5 = record.U_SAL_OFT_5;
            product.SAL_OFT_ANT_5 = record.U_SAL_OFT_ANT_5;
            product.INI_BONUS = record.U_INI_BONUS;
            product.INI_BONUS_ANT = record.U_INI_BONUS_ANT;
            product.FIM_BONUS = record.U_FIM_BONUS;
            product.PRES_ENT = record.U_PRES_ENT;
            product.PRC_BONUS = record.U_PRC_BONUS;
            product.PRES_REP = record.U_PRES_REP;
            product.ESTQ_ATUAL = record.U_ESTQ_ATUAL;
            product.ESTQ_DP = record.U_ESTQ_DP;
            product.ESTQ_LJ = record.U_ESTQ_LJ;
            product.QDE_PEND = record.U_QDE_PEND;
            product.CUS_INV = record.U_CUS_INV;
            product.ESTQ_PADRAO = record.U_ESTQ_PADRAO;
            product.SAI_MED_CAL = record.U_SAI_MED_CAL;
            product.TAMANHO = record.U_TAMANHO;
            product.SAI_ACM_UN = record.U_SAI_ACM_UN;
            product.SAI_ACM_CUS = record.U_SAI_ACM_CUS;
            product.SAI_ACM_VEN = record.U_SAI_ACM_VEN;
            product.CUS_ULT_ENT_BRU = record.U_CUS_ULT_ENT_BRU;
            product.ENT_ACM_UN = record.U_ENT_ACM_UN;
            product.ENT_ACM_CUS = record.U_ENT_ACM_CUS;
            product.DAT_ULT_FAT = record.U_DAT_ULT_FAT;
            product.ULT_QDE_FAT = record.U_ULT_QDE_FAT;
            product.ULT_QDE_ENT = record.U_ULT_QDE_ENT;
            product.CUS_ULT_ENT = record.U_CUS_ULT_ENT;
            product.DAT_ULT_ENT = record.U_DAT_ULT_ENT;
            product.ABC_F = record.U_ABC_F;
            product.ABC_S = record.U_ABC_S;
            product.ABC_T = record.U_ABC_T;
            product.PERECIVEL = record.U_PERECIVEL;
            product.PRZ_VALIDADE = record.U_PRZ_VALIDADE;
            product.TOT_PEDIDO = record.U_TOT_PEDIDO;
            product.TOT_FALTA = record.U_TOT_FALTA;
            product.COD_VAS = record.U_COD_VAS;
            product.DIG_VAS = record.U_DIG_VAS;
            product.COD_ENG = record.U_COD_ENG;
            product.DIG_ENG = record.U_DIG_ENG;
            product.VALOR_IPI = record.U_VALOR_IPI;
            product.VALOR_IPI_ANT = record.U_VALOR_IPI_ANT;
            product.VALOR_IPI_MAN = record.U_VALOR_IPI_MAN;
            product.QTD_AUT_PDV = record.U_QTD_AUT_PDV;
            product.MOEDA_VDA = record.U_MOEDA_VDA;
            product.BONI_MERC = record.U_BONI_MERC;
            product.BONI_MERC_ANT = record.U_BONI_MERC_ANT;
            product.BONI_MERC_MAN = record.U_BONI_MERC_MAN;
            product.GRADE = record.U_GRADE;
            product.MENS_AUX = record.U_MENS_AUX;
            product.CATEGORIA_ANT = record.U_CATEGORIA_ANT;
            product.ESTRATEGIA_REP = record.U_ESTRATEGIA_REP;
            product.DESP_ACES_ISEN = record.U_DESP_ACES_ISEN;
            product.DESP_ACES_ISEN_ANT = record.U_DESP_ACES_ISEN_ANT;
            product.DESP_ACES_ISEN_MAN = record.U_DESP_ACES_ISEN_MAN;
            product.FRETE_VALOR = record.U_FRETE_VALOR;
            product.FRETE_VALOR_ANT = record.U_FRETE_VALOR_ANT;
            product.FRETE_VALOR_MAN = record.U_FRETE_VALOR_MAN;
            product.BONIF_PER = record.U_BONIF_PER;
            product.BONIF_PER_ANT = record.U_BONIF_PER_ANT;
            product.BONIF_PER_MAN = record.U_BONIF_PER_MAN;
            product.VASILHAME = record.U_VASILHAME;
            product.PERMITE_DESC = record.U_PERMITE_DESC;
            product.QTD_OBRIG = record.U_QTD_OBRIG;
            product.ENVIA_PDV = record.U_ENVIA_PDV;
            product.ENVIA_BALANCA = record.U_ENVIA_BALANCA;
            product.PESADO_PDV = record.U_PESADO_PDV;
            product.JPMA_FLAG1 = record.U_JPMA_FLAG1;
            product.JPMA_FLAG2 = record.U_JPMA_FLAG2;
            product.JPMA_FLAG3 = record.U_JPMA_FLAG3;
            product.LINHA_ANTERIOR = record.U_LINHA_ANTERIOR;
            product.LINHA_VALIDA = record.U_LINHA_VALIDA;
            product.QUANT_EAN = record.U_QUANT_EAN;
            product.TOT_ESTOCADO = record.U_TOT_ESTOCADO;
            product.EMPIL_MAX = record.U_EMPIL_MAX;
            product.TIPO_END = record.U_TIPO_END;
            product.SAZONAL = record.U_SAZONAL;
            product.MARCA_PROP = record.U_MARCA_PROP;
            product.FILLER = record.U_FILLER;*/
            #endregion

            return product;
        }


        public ProductVendor ToRecordVendor(dynamic record)
        {
            ProductVendor productVendor = new ProductVendor();
            productVendor.RecId = Guid.Parse(record.Code);
            productVendor.cod_item = record.U_COD_ITEM;
            productVendor.cod_forn = record.U_COD_FORN;
            productVendor.cod_forn_alt = record.U_COD_FORN_ALT;
            productVendor.cod_item_alt = record.U_COD_ITEM_ALT;
            productVendor.dig_item = record.U_DIG_ITEM;
            productVendor.dig_forn = record.U_DIG_FORN;
            productVendor.referencia = record.U_REFERENCIA;
            productVendor.descricao = record.U_DESCRICAO;
            productVendor.fatur_unid = record.U_FATUR_UNID;
            productVendor.uf_unid = record.U_UF_UNID;
            productVendor.uf_fator = record.U_UF_FATOR;
            productVendor.uf_fator_conv = record.U_UF_FATOR_CONV;
            productVendor.emb_xml = record.U_EMB_XML;
            productVendor.lastupdate = parseDate(record.U_LASTUPDATE);

            return productVendor;

        }


        public ProductComponent ToRecordComponent(dynamic record)
        {
            ProductComponent productComponent = new ProductComponent();

            productComponent.produto = record.U_PRODUTO;
            productComponent.componente = record.U_COMPONENTE;
            productComponent.componente_ak = record.U_COMPONENTE_AK;
            productComponent.produto_ak = record.U_PRODUTO_AK;
            productComponent.quantidade = record.U_QUANTIDADE;
            productComponent.sec_sim_1 = record.U_SEC_SIM_1;
            productComponent.sec_sim_2 = record.U_SEC_SIM_2;
            productComponent.sec_sim_3 = record.U_SEC_SIM_3;
            productComponent.grp_sim_1 = record.U_GRP_SIM_1;
            productComponent.grp_sim_2 = record.U_GRP_SIM_2;
            productComponent.grp_sim_3 = record.U_GRP_SIM_3;
            productComponent.sbg_sim_1 = record.U_SBG_SIM_1;
            productComponent.sbg_sim_2 = record.U_SBG_SIM_2;
            productComponent.sbg_sim_3 = record.U_SBG_SIM_3;
            productComponent.ctg_sim_1 = record.U_CTG_SIM_1;
            productComponent.ctg_sim_2 = record.U_CTG_SIM_2;
            productComponent.ctg_sim_3 = record.U_CTG_SIM_3;
            productComponent.filler = record.U_FILLER;
            productComponent.lastupdate = parseDate(record.U_LASTUPDATE);

            return productComponent;

        }


        private DateTime parseDate(dynamic value)
        {
            DateTime result;

            DateTime.TryParse(value, out result);

            return result;
        }

        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("cod_for", "U_COD_FOR");
            map.Add("cod_item", "U_COD_ITEM");
            map.Add("codigo_ean13", "U_CODIGO_EAN13");
            map.Add("descricao", "U_DESCRICAO");
            map.Add("desc_reduz", "U_DESC_REDUZ");
            map.Add("desc_coml", "U_DESC_COML");
            map.Add("referencia", "U_REFERENCIA");
            map.Add("emb_for", "U_EMB_FOR");
            map.Add("tpo_emb_for", "U_TPO_EMB_FOR");
            map.Add("emb_venda", "U_EMB_VENDA");
            map.Add("tpo_emb_venda", "U_TPO_EMB_VENDA");
            map.Add("compr_emb_vnd", "U_COMPR_EMB_VND");
            map.Add("altura_emb_vnd", "U_ALTURA_EMB_VND");
            map.Add("tipo_plu", "U_TIPO_PLU");
            map.Add("largura_emb_vnd", "U_LARGURA_EMB_VND");
            map.Add("class_fis", "U_CLASS_FIS");
            map.Add("procedencia", "U_PROCEDENCIA");
            map.Add("dat_sai_lin", "U_DAT_SAI_LIN");
            map.Add("dat_ent_lin", "U_DAT_ENT_LIN");
            map.Add("cest", "U_CEST");
            map.Add("controle_lote_serie", "U_CONTROLE_LOTE_SERIE");
            map.Add("tip_pro_sped", "U_TIP_PRO_SPED");
            map.Add("lastupdate", "U_LASTUPDATE");
            map.Add("status", "U_STATUS");
            map.Add("altura_emb", "U_ALTURA_EMB");
            map.Add("largura_emb", "U_LARGURA_EMB");
            map.Add("comprimento_emb", "U_COMPRIMENTO_EMB");
            map.Add("peso", "U_PESO");
            map.Add("tipo_pro", "U_TIPO_PRO");


            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("cod_for", "N");
            map.Add("cod_item", "N");
            map.Add("codigo_ean13", "N");
            map.Add("descricao", "T");
            map.Add("desc_reduz", "T");
            map.Add("desc_coml", "T");
            map.Add("referencia", "T");
            map.Add("emb_for", "N");
            map.Add("tpo_emb_for", "T");
            map.Add("emb_venda", "N");
            map.Add("tpo_emb_venda", "T");
            map.Add("compr_emb_vnd", "N");
            map.Add("altura_emb_vnd", "N");
            map.Add("tipo_plu", "N");
            map.Add("largura_emb_vnd", "N");
            map.Add("class_fis", "N");
            map.Add("procedencia", "N");
            map.Add("dat_sai_lin", "T");
            map.Add("dat_ent_lin", "T");
            map.Add("cest", "T");
            map.Add("controle_lote_serie", "T");
            map.Add("tip_pro_sped", "N");
            map.Add("lastupdate", "T");
            map.Add("status", "N");
            map.Add("altura_emb", "N");
            map.Add("largura_emb", "N");
            map.Add("comprimento_emb", "N");
            map.Add("peso", "N");
            map.Add("tipo_pro", "N");

            return map;
        }
    }
}
