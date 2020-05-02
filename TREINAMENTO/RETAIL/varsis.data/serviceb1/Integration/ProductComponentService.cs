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
    public class ProductComponentService : IEntityService<Model.Integration.ProductComponent>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public ProductComponentService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = mountFieldMap();
            _FieldType = mountFieldType();
        }

        async public Task<bool> Create()
        {
            bool result = false;
            return result;
        }

        public Task Delete(ProductComponent entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
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
                        if (c.Operator.ToLower() == "startswith")
                        {
                            filter.Add($"{c.Operator.ToLower()}({field},'{c.Value}')");
                        }
                        else
                        {
                            filter.Add($"{field} {c.Operator.ToLower()} '{c.Value}'");
                        }
                    }
                    else if (type == "N")
                    {
                        filter.Add($"{field} {c.Operator.ToLower()} {c.Value}");
                    }
                }
            }


            Varsis.Data.Infrastructure.Pagination page = new Varsis.Data.Infrastructure.Pagination();
            string query = Global.MakeODataQuery("U_VSITENTIDADE/$count", null, filter.Count == 0 ? null : filter.ToArray(), null, 1, 0);
            string data = await _serviceLayerConnector.getQueryResult(query);
            page.Linhas = Convert.ToInt64(data);
            page.Paginas = (Convert.ToInt64(data) / size.Value) + 1;
            page.qtdPorPagina = size.Value == 0 ? Convert.ToInt64(data) : size.Value;
            return page;
        }
        async public Task<ProductComponent> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"U_VSITPRODUCT_COMP('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            ProductComponent product = toRecord(record);


            return product;
        }

        private string toJson(ProductComponent productComponent)
        {

            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.Code = productComponent.RecId.ToString();
            record.Name = productComponent.RecId.ToString();


            record.U_PRODUTO = productComponent.produto;
            record.U_COMPONENTE = productComponent.componente;
            record.U_COMPONENTE_AK = productComponent.componente_ak;
            record.U_PRODUTO_AK = productComponent.produto_ak;
            record.U_QUANTIDADE = productComponent.quantidade;
            record.U_SEC_SIM_1 = productComponent.sec_sim_1;
            record.U_SEC_SIM_2 = productComponent.sec_sim_2;
            record.U_SEC_SIM_3 = productComponent.sec_sim_3;
            record.U_GRP_SIM_1 = productComponent.grp_sim_1;
            record.U_GRP_SIM_2 = productComponent.grp_sim_2;
            record.U_GRP_SIM_3 = productComponent.grp_sim_3;
            record.U_SBG_SIM_1 = productComponent.sbg_sim_1;
            record.U_SBG_SIM_2 = productComponent.sbg_sim_2;
            record.U_SBG_SIM_3 = productComponent.sbg_sim_3;
            record.U_CTG_SIM_1 = productComponent.ctg_sim_1;
            record.U_CTG_SIM_2 = productComponent.ctg_sim_2;
            record.U_CTG_SIM_3 = productComponent.ctg_sim_3;
            record.U_FILLER = productComponent.filler;
            record.U_LASTUPDATE = productComponent.lastupdate;
            record.U_STATUS = productComponent.status;

            result = JsonConvert.SerializeObject(record);

            return result;
        }

        async public Task Insert(ProductComponent entity)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();

            // entity.status = Data.Model.Integration.Product.ProductIntegrationStatus.Importing;
            string record = toJson(entity);
            batch.Post(HttpMethod.Post, "/U_VSITPRODUCT_COMP", record);

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

            //
            // Verificar erros no lote
            //
            if (response.internalResponses.Count(m => m.success == false) == 0)
            {
                //
                // O registro só será alterado se não houver erros
                //
                // entity.status = Product.ProductIntegrationStatus.Created;
                record = toJson(entity);
                response = await _serviceLayerConnector.Patch($"U_VSITPRODUCT_COMP('{entity.RecId}')", record, true);

                if (!response.success)
                {
                    string message = $"Erro ao inserir dados em '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";

                    Console.WriteLine(message);
                    throw new ApplicationException(message);
                }
            }
            else
            {
                // Erro no cabeçalho do produto
                if (response.internalResponses.Count == 1)
                {
                    string message = $"Erro ao inserir dados em '{entity.EntityName}': {response.internalResponses[0].errorCode}-{response.internalResponses[0].errorMessage}";
                    Console.WriteLine(message);
                    throw new ApplicationException(message);
                }
                else
                {
                    int position = response.internalResponses.Count - 1;
                    //int item = position - 1;

                    //string message = $"Erro ao inserir dados em '{entity.items[0].EntityName}' item {entity.items[item].itemId} : {response.internalResponses[position].errorCode}-{response.internalResponses[position].errorMessage}";
                    //Console.WriteLine(message);
                    throw new ApplicationException("Vazio");
                }
            }
        }

        public Task Insert(List<ProductComponent> entities)
        {
            throw new NotImplementedException();
        }

        async public Task<List<ProductComponent>> List(List<Criteria> criterias, long page, long size)
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

            string query = Global.MakeODataQuery("U_VSITPRODUCT_COMP", null, filter.Count == 0 ? null : filter.ToArray(), null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<ProductComponent> result = new List<ProductComponent>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        public Task Update(ProductComponent entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<ProductComponent> entities)
        {
            throw new NotImplementedException();
        }




        public ProductComponent toRecord(dynamic record)
        {
            ProductComponent productVendor = new ProductComponent();
            productVendor.RecId = Guid.Parse(record.Code);
            productVendor.produto = record.U_PRODUTO;
            productVendor.componente = record.U_COMPONENTE;
            productVendor.componente_ak = record.U_COMPONENTE_AK;
            productVendor.produto_ak = record.U_PRODUTO_AK;
            productVendor.quantidade = record.U_QUANTIDADE;
            productVendor.sec_sim_1 = record.U_SEC_SIM_1;
            productVendor.sec_sim_2 = record.U_SEC_SIM_2;
            productVendor.sec_sim_3 = record.U_SEC_SIM_3;
            productVendor.grp_sim_1 = record.U_GRP_SIM_1;
            productVendor.grp_sim_2 = record.U_GRP_SIM_2;
            productVendor.grp_sim_3 = record.U_GRP_SIM_3;
            productVendor.sbg_sim_1 = record.U_SBG_SIM_1;
            productVendor.sbg_sim_2 = record.U_SBG_SIM_2;
            productVendor.sbg_sim_3 = record.U_SBG_SIM_3;
            productVendor.ctg_sim_1 = record.U_CTG_SIM_1;
            productVendor.ctg_sim_2 = record.U_CTG_SIM_2;
            productVendor.ctg_sim_3 = record.U_CTG_SIM_3;
            productVendor.filler = record.U_FILLER;
            productVendor.lastupdate = parseDate(record.U_LASTUPDATE);
            productVendor.status = (int)record.U_STATUS;

            return productVendor;
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

            map.Add("produto", "U_PRODUTO");
            map.Add("componente", "U_COMPONENTE");
            map.Add("cod_fcomponente_ak", "U_COMPONENTE_AK");
            map.Add("produto_ak", "U_PRODUTO_AK");
            map.Add("quantidade", "U_QUANTIDADE");
            map.Add("sec_sim_1", "U_SEC_SIM_1");
            map.Add("sec_sim_2", "U_SEC_SIM_2");
            map.Add("sec_sim_3", "U_SEC_SIM_3");
            map.Add("grp_sim_1", "U_GRP_SIM_1");
            map.Add("grp_sim_2", "U_GRP_SIM_2");
            map.Add("grp_sim_3", "U_GRP_SIM_3");
            map.Add("sbg_sim_1", "U_SBG_SIM_1");
            map.Add("sbg_sim_2", "U_SBG_SIM_2");
            map.Add("sbg_sim_3", "U_SBG_SIM_3");
            map.Add("ctg_sim_1", "U_CTG_SIM_1");
            map.Add("ctg_sim_2", "U_CTG_SIM_2");
            map.Add("ctg_sim_3", "U_CTG_SIM_3");
            map.Add("filler", "U_FILLER");
            map.Add("lastupdate", "U_LASTUPDATE");
            map.Add("status", "U_STATUS");

            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("produto", "N");
            map.Add("componente", "N");
            map.Add("cod_fcomponente_ak", "N");
            map.Add("produto_ak", "N");
            map.Add("quantidade", "N");
            map.Add("sec_sim_1", "N");
            map.Add("sec_sim_2", "N");
            map.Add("sec_sim_3", "N");
            map.Add("grp_sim_1", "N");
            map.Add("grp_sim_2", "N");
            map.Add("grp_sim_3", "N");
            map.Add("sbg_sim_1", "N");
            map.Add("sbg_sim_2", "N");
            map.Add("sbg_sim_3", "N");
            map.Add("ctg_sim_1", "N");
            map.Add("ctg_sim_2", "N");
            map.Add("ctg_sim_3", "N");
            map.Add("filler", "N");
            map.Add("lastupdate", "T");
            map.Add("status", "N");

            return map;
        }
    }
}
