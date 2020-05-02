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
    public class StoreService : IEntityService<Model.Integration.Store>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public StoreService(ServiceLayerConnector serviceLayerConnector)
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
            bool result = false;

            Table table = new Table(_serviceLayerConnector);

            table.name = "VSISSTORE";
            table.description = "Cadstro de Lojas";
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

        public Task Delete(Store entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task<Store> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"U_VSISSTORE('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            Store store = toRecord(record);

            // Recupera as linhas da nota iscal
            string[] filter = new string[]
            {
                $"U_PRODUCTCODE eq '{recid}'"
            };

            query = Global.MakeODataQuery("U_VSISPRODUCT", null, filter);

            data = await _serviceLayerConnector.getQueryResult(query);

            return store;
        }

        async public Task Insert(Store entity)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();
            entity.status = Data.Model.Integration.Store.StoreIntegrationStatus.Importing;
            string record = toJson(entity);
            batch.Post(HttpMethod.Post, "/U_VSISSTORE", record);

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
                entity.status = Store.StoreIntegrationStatus.Created;
                record = toJson(entity);
                response = await _serviceLayerConnector.Patch($"U_VSISSTORE('{entity.RecId}')", record, true);

                if (!response.success)
                {
                    string message = $"Erro ao inserir dados em '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";

                    Console.WriteLine(message);
                    throw new ApplicationException(message);
                }
            }
            else
            {
                // Erro no cabeçalho da nota fiscal
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

        public Task Insert(List<Store> entities)
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
        async public Task<List<Store>> List(List<Criteria> criterias, long page, long size)
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

            string query = Global.MakeODataQuery("U_VSISSTORE", null, filter.Count == 0 ? null : filter.ToArray(), null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<Store> result = new List<Store>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        public Task Update(Store entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<Store> entities)
        {
            throw new NotImplementedException();
        }

        private List<TableColumn> createColumns()
        {
            List<TableColumn> lista = new List<TableColumn>();
            lista.Add(new TableColumn() { name = "CODIGO", description = "identificação", mandatory = true, size = 7, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DIGITO", description = "identificação", mandatory = true, size = 1, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "RAZAO_SOCIAL", description = "identificação", mandatory = false, size = 30, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "NOME_FANTASIA", description = "identificação", mandatory = false, size = 30, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "ENDERECO", description = "identificação", mandatory = false, size = 35, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "BAIRRO", description = "identificação", mandatory = false, size = 20, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "CIDADE", description = "identificação", mandatory = false, size = 20, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "ESTADO", description = "identificação", mandatory = false, size = 2, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "CEP", description = "identificação", mandatory = false, size = 9, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "LOJ_CLI", description = "identificação", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "NATUREZA", description = "identificação", mandatory = false, size = 2, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "DATA_CAD", description = "identificação", mandatory = false, size = 6, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "CLI_EMP_PRINC", description = "identificação", mandatory = false, size = 8, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "CXPOSTAL", description = "identificação", mandatory = false, size = 9, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FAX_DDD", description = "identificação", mandatory = false, size = 9, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FAX_NUM", description = "identificação", mandatory = false, size = 9, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FONE_DDD", description = "identificação", mandatory = false, size = 9, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FONE_NUM", description = "identificação", mandatory = false, size = 9, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TELEX_DDD", description = "identificação", mandatory = false, size = 9, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "TELEX_NUM", description = "identificação", mandatory = false, size = 9, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "RENPAR_DDD", description = "identificação", mandatory = false, size = 9, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "RENPAR_NUM", description = "identificação", mandatory = false, size = 9, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "STM400", description = "identificação", mandatory = false, size = 20, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "COD_EDI", description = "identificação", mandatory = false, size = 15, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "COD_X25", description = "identificação", mandatory = false, size = 15, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "FIS_JUR", description = "identificação", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "CGC_CPF", description = "identificação", mandatory = false, size = 11, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "INSC_EST_IDENT", description = "identificação", mandatory = false, size = 15, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "INSC_MUN", description = "identificação", mandatory = false, size = 15, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "REGIAO", description = "identificação", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DIVISAO", description = "identificação", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DISTRITO", description = "identificação", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "EMPRESA", description = "identificação", mandatory = false, size = 3, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "LINHA_TABELA", description = "identificação", mandatory = false, size = 2, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "TEM_OBSER", description = "identificação", mandatory = false, size = 1, dataType = "db_Alpha" });
            lista.Add(new TableColumn() { name = "COD_VAN", description = "identificação", mandatory = false, size = 7, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "DIG_VAN", description = "identificação", mandatory = false, size = 1, dataType = "db_Numeric" });
            lista.Add(new TableColumn() { name = "FILLER", description = "identificação", mandatory = false, size = 7, dataType = "db_Numeric" });

            return lista;
        }

        private List<TableIndexes> createIndexes()
        {
            List<TableIndexes> lista = new List<TableIndexes>();

            //lista.Add(new TableIndexes()
            //{
            //    name = "PK",
            //    isUnique = true,
            //    keys = new string[] { "INVOICEDIRECTION", "INVOICEID", "INVOICESERIES", "INVOICEDATE", "ISSUERID" }
            //});

            //lista.Add(new TableIndexes()
            //{
            //    name = "STATUS",
            //    isUnique = false,
            //    keys = new string[] { "STATUS" }
            //});

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

        private string toJson(Store store)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.Code = store.RecId.ToString();
            record.Name = store.RecId.ToString();
            record.U_CODIGO = store.codigo;
            record.U_DIGITO = store.digito;
            record.U_RAZAO_SOCIAL = store.razao_social;
            record.U_NOME_FANTASIA = store.nome_fantasia;
            record.U_ENDERECO = store.endereco;
            record.U_BAIRRO = store.bairro;
            record.U_CIDADE = store.cidade;
            record.U_ESTADO = store.estado;
            record.U_CEP = store.cep;
            record.U_LOJ_CLI = store.loj_cli;
            record.U_NATUREZA = store.natureza;
            record.U_DATA_CAD = store.data_cad;
            record.U_CLI_EMP_PRINC = store.cli_emp_princ;
            record.U_CXPOSTAL = store.cxpostal;
            record.U_FAX_DDD = store.fax_ddd;
            record.U_FAX_NUM = store.fax_num;
            record.U_FONE_DDD = store.fone_ddd;
            record.U_FONE_NUM = store.fone_num;
            record.U_TELEX_DDD = store.telex_ddd;
            record.U_TELEX_NUM = store.telex_num;
            record.U_RENPAR_DDD = store.renpar_ddd;
            record.U_RENPAR_NUM = store.renpar_num;
            record.U_STM400 = store.stm400;
            record.U_COD_EDI = store.cod_edi;
            record.U_COD_X25 = store.cod_x25;
            record.U_FIS_JUR = store.fis_jur;
            record.U_CGC_CPF = store.cgc_cpf;
            record.U_INSC_EST_IDENT = store.insc_est_ident;
            record.U_INSC_MUN = store.insc_mun;
            record.U_REGIAO = store.regiao;
            record.U_DIVISAO = store.divisao;
            record.U_DISTRITO = store.distrito;
            record.U_EMPRESA = store.empresa;
            record.U_LINHA_TABELA = store.linha_tabela;
            record.U_TEM_OBSER = store.tem_obser;
            record.U_COD_VAN = store.cod_van;
            record.U_DIG_VAN = store.dig_van;
            record.U_FILLER = store.filler;

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

        private Store toRecord(dynamic record)
        {
            Store store = new Store();
            store.RecId = Guid.Parse(record.Code);
            store.codigo = record.U_CODIGO;
            store.digito = record.U_DIGITO;
            store.razao_social = record.U_RAZAO_SOCIAL;
            store.nome_fantasia = record.U_NOME_FANTASIA;
            store.endereco = record.U_ENDERECO;
            store.bairro = record.U_BAIRRO;
            store.cidade = record.U_CIDADE;
            store.estado = record.U_ESTADO;
            store.cep = record.U_CEP;
            store.loj_cli = record.U_LOJ_CLI;
            store.natureza = record.U_NATUREZA;
            store.data_cad = record.U_DATA_CAD;
            store.cli_emp_princ = record.U_CLI_EMP_PRINC;
            store.cxpostal = record.U_CXPOSTAL;
            store.fax_ddd = record.U_FAX_DDD;
            store.fax_num = record.U_FAX_NUM;
            store.fone_ddd = record.U_FONE_DDD;
            store.fone_num = record.U_FONE_NUM;
            store.telex_ddd = record.U_TELEX_DDD;
            store.telex_num = record.U_TELEX_NUM;
            store.renpar_ddd = record.U_RENPAR_DDD;
            store.renpar_num = record.U_RENPAR_NUM;
            store.stm400 = record.U_STM400;
            store.cod_edi = record.U_COD_EDI;
            store.cod_x25 = record.U_COD_X25;
            store.fis_jur = record.U_FIS_JUR;
            store.cgc_cpf = record.U_CGC_CPF;
            store.insc_est_ident = record.U_INSC_EST_IDENT;
            store.insc_mun = record.U_INSC_MUN;
            store.regiao = record.U_REGIAO;
            store.divisao = record.U_DIVISAO;
            store.distrito = record.U_DISTRITO;
            store.empresa = record.U_EMPRESA;
            store.linha_tabela = record.U_LINHA_TABELA;
            store.tem_obser = record.U_TEM_OBSER;
            store.cod_van = record.U_COD_VAN;
            store.dig_van = record.U_DIG_VAN;
            store.filler = record.U_FILLER;


            return store;
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

            //map.Add("invoicedirection", "U_INVOICEDIRECTION");
            //map.Add("invoiceid", "U_INVOICEID");
            //map.Add("invoiceseries", "U_INVOICESERIES");
            //map.Add("invoicedate", "U_INVOICEDATE");
            //map.Add("issuedate", "U_ISSUEDATE");
            //map.Add("recipientid", "U_RECIPIENTID");
            //map.Add("taxcodedetermination", "U_DETERMINATION");
            //map.Add("cfop", "U_CFOP");
            //map.Add("lastupdate", "U_LASTUPDATE");
            //map.Add("status", "U_STATUS");

            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            //map.Add("invoicedirection", "N");
            //map.Add("invoiceid", "T");
            //map.Add("invoiceseries", "T");
            //map.Add("invoicedate", "T");
            //map.Add("issuedate", "T");
            //map.Add("recipientid", "T");
            //map.Add("taxcodedetermination", "T");
            //map.Add("cfop", "N");
            //map.Add("lastupdate", "T");
            //map.Add("status", "N");

            return map;
        }
    }
}
