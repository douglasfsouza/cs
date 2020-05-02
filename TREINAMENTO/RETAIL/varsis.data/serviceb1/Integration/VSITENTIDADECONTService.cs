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
    public class VSITENTIDADECONTService : IEntityService<Model.Integration.VSITENTIDADECONT>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public VSITENTIDADECONTService(ServiceLayerConnector serviceLayerConnector)
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

            table.name = "VSITENTIDADECONT";
            table.description = "Cadastro de VSITENTIDADECONT";
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

        public Task Delete(VSITENTIDADECONT entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task<VSITENTIDADECONT> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"U_VSITENTIDADECONT('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            VSITENTIDADECONT entidade = toRecord(record);

            // Recupera as linhas da nota iscal
            string[] filter = new string[]
            {
                $"Code eq '{recid}'"
            };

            query = Global.MakeODataQuery("U_VSITENTIDADECONT", null, filter);

            data = await _serviceLayerConnector.getQueryResult(query);

            return entidade;
        }

        async public Task Insert(VSITENTIDADECONT entity)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();

            entity.status = Data.Model.Integration.VSITENTIDADECONT.VSITENTIDADECONTIntegrationStatus.Importing;
            string record = toJson(entity);
            batch.Post(HttpMethod.Post, "/U_VSITENTIDADECONT", record);

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
        }

        public Task Insert(List<VSITENTIDADECONT> entities)
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
        async public Task<List<VSITENTIDADECONT>> List(List<Criteria> criterias, long page, long size)
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

            string query = Global.MakeODataQuery("U_VSITENTIDADECONT", null, filter.Count == 0 ? null : filter.ToArray(),null, page, size);



            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<VSITENTIDADECONT> result = new List<VSITENTIDADECONT>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        public Task Update(VSITENTIDADECONT entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<VSITENTIDADECONT> entities)
        {
            throw new NotImplementedException();
        }

        private List<TableColumn> createColumns()
        {
            List<TableColumn> lista = new List<TableColumn>();

            lista.Add(new TableColumn() { name = "STATUS", dataType = "db_Numeric", mandatory = false, size = 1, description = "STATUS" });
            lista.Add(new TableColumn() { name = "LAST_UPDATE", dataType = "db_Date", mandatory = false, description = "LAST_UPDATE" });
            lista.Add(new TableColumn() { name = "NOME", dataType = "db_Alpha", mandatory = false, size = 40, description = "NOME" });
            lista.Add(new TableColumn() { name = "CODIGO", dataType = "db_Numeric", mandatory = true, size = 11, description = "CODIGO" });
            lista.Add(new TableColumn() { name = "SEQ", dataType = "db_Numeric", mandatory = true, size = 11, description = "SEQ" });
            lista.Add(new TableColumn() { name = "CARGO", dataType = "db_Alpha", mandatory = false, size = 100, description = "CARGO" });
            lista.Add(new TableColumn() { name = "EMAIL_1", dataType = "db_Alpha", mandatory = false, size = 100, description = "EMAIL_1" });
            lista.Add(new TableColumn() { name = "EMAIL_2", dataType = "db_Alpha", mandatory = false, size = 100, description = "EMAIL_2" });
            lista.Add(new TableColumn() { name = "EMAIL_1_FLAGS", dataType = "db_Alpha", mandatory = false, size = 10, description = "EMAIL_1_FLAGS" });
            lista.Add(new TableColumn() { name = "EMAIL_2_FLAGS", dataType = "db_Alpha", mandatory = false, size = 10, description = "EMAIL_2_FLAGS" });
            lista.Add(new TableColumn() { name = "SITE", dataType = "db_Alpha", mandatory = false, size = 100, description = "SITE" });
            lista.Add(new TableColumn() { name = "TIPO_TEL_1", dataType = "db_Numeric", mandatory = false, size = 11, description = "TIPO_TEL_1" });
            lista.Add(new TableColumn() { name = "TIPO_TEL_2", dataType = "db_Numeric", mandatory = false, size = 11, description = "TIPO_TEL_2" });
            lista.Add(new TableColumn() { name = "TIPO_TEL_3", dataType = "db_Numeric", mandatory = false, size = 11, description = "TIPO_TEL_3" });
            lista.Add(new TableColumn() { name = "TIPO_TEL_4", dataType = "db_Numeric", mandatory = false, size = 11, description = "TIPO_TEL_4" });
            lista.Add(new TableColumn() { name = "TIPO_TEL_5", dataType = "db_Numeric", mandatory = false, size = 11, description = "TIPO_TEL_5" });
            lista.Add(new TableColumn() { name = "DDD_1", dataType = "db_Numeric", mandatory = false, size = 11, description = "DDD_1" });
            lista.Add(new TableColumn() { name = "DDD_2", dataType = "db_Numeric", mandatory = false, size = 11, description = "DDD_2" });
            lista.Add(new TableColumn() { name = "DDD_3", dataType = "db_Numeric", mandatory = false, size = 11, description = "DDD_3" });
            lista.Add(new TableColumn() { name = "DDD_4", dataType = "db_Numeric", mandatory = false, size = 11, description = "DDD_4" });
            lista.Add(new TableColumn() { name = "DDD_5", dataType = "db_Numeric", mandatory = false, size = 11, description = "DDD_5" });
            lista.Add(new TableColumn() { name = "FONE_1", dataType = "db_Numeric", mandatory = false, size = 11, description = "FONE_1" });
            lista.Add(new TableColumn() { name = "FONE_2", dataType = "db_Numeric", mandatory = false, size = 11, description = "FONE_2" });
            lista.Add(new TableColumn() { name = "FONE_3", dataType = "db_Numeric", mandatory = false, size = 11, description = "FONE_3" });
            lista.Add(new TableColumn() { name = "FONE_4", dataType = "db_Numeric", mandatory = false, size = 11, description = "FONE_4" });
            lista.Add(new TableColumn() { name = "FONE_5", dataType = "db_Numeric", mandatory = false, size = 11, description = "FONE_5" });
            lista.Add(new TableColumn() { name = "OBS", dataType = "db_Alpha", mandatory = false, size = 100, description = "OBS" });
            lista.Add(new TableColumn() { name = "CRM", dataType = "db_Alpha", mandatory = false, size = 1, description = "CRM" });
            lista.Add(new TableColumn() { name = "CNPJ_AUTO_XML", dataType = "db_Alpha", mandatory = false, size = 14, description = "CNPJ_AUTO_XML" });
            lista.Add(new TableColumn() { name = "CPF_AUTO_XML", dataType = "db_Numeric", mandatory = false, size = 11, description = "CPF_AUTO_XML" });

            return lista;
        }

        private List<TableIndexes> createIndexes()
        {
            List<TableIndexes> lista = new List<TableIndexes>();
            return lista;
        }

        private List<TableIndexes> createIndexesItem()
        {
            List<TableIndexes> lista = new List<TableIndexes>();
            return lista;
        }

        private string toJson(VSITENTIDADECONT entidade)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.Code = entidade.RecId.ToString();
            record.Name = entidade.RecId.ToString();
            record.U_STATUS = (VSITENTIDADECONT.VSITENTIDADECONTIntegrationStatus.Created);
            record.U_LAST_UPDATE = DateTime.Now;
            record.U_NOME = entidade.nome;
            record.U_CODIGO = entidade.codigo;
            record.U_SEQ = entidade.seq;
            record.U_CARGO = entidade.cargo;
            record.U_EMAIL_1 = entidade.email_1;
            record.U_EMAIL_2 = entidade.email_2;
            record.U_EMAIL_1_FLAGS = entidade.email_1_flags;
            record.U_EMAIL_2_FLAGS = entidade.email_2_flags;
            record.U_SITE = entidade.site;
            record.U_TIPO_TEL_1 = entidade.tipo_tel_1;
            record.U_TIPO_TEL_2 = entidade.tipo_tel_2;
            record.U_TIPO_TEL_3 = entidade.tipo_tel_3;
            record.U_TIPO_TEL_4 = entidade.tipo_tel_4;
            record.U_TIPO_TEL_5 = entidade.tipo_tel_5;
            record.U_DDD_1 = entidade.ddd_1;
            record.U_DDD_2 = entidade.ddd_2;
            record.U_DDD_3 = entidade.ddd_3;
            record.U_DDD_4 = entidade.ddd_4;
            record.U_DDD_5 = entidade.ddd_5;
            record.U_FONE_1 = entidade.fone_1;
            record.U_FONE_2 = entidade.fone_2;
            record.U_FONE_3 = entidade.fone_3;
            record.U_FONE_4 = entidade.fone_4;
            record.U_FONE_5 = entidade.fone_5;
            record.U_OBS = entidade.obs;
            record.U_CRM = entidade.crm;
            record.U_CNPJ_AUTO_XML = entidade.cnpj_auto_xml;
            record.U_CPF_AUTO_XML = entidade.cpf_auto_xml;
            result = JsonConvert.SerializeObject(record);
            return result;
        }

        private VSITENTIDADECONT toRecord(dynamic record)
        {
            VSITENTIDADECONT entidade = new VSITENTIDADECONT();

            entidade.RecId = Guid.Parse(record.Code);
            entidade.codigo = record.U_CODIGO;
            entidade.status = (VSITENTIDADECONT.VSITENTIDADECONTIntegrationStatus)record.U_STATUS;
            //entidade.lastupdate = parseDate(record.U_LASTUPDATE);
            entidade.nome = record.U_NOME;
            entidade.codigo = record.U_CODIGO;
            entidade.seq = record.U_SEQ;
            entidade.cargo = record.U_CARGO;
            entidade.email_1 = record.U_EMAIL_1;
            entidade.email_2 = record.U_EMAIL_2;
            entidade.email_1_flags = record.U_EMAIL_1_FLAGS;
            entidade.email_2_flags = record.U_EMAIL_2_FLAGS;
            entidade.site = record.U_SITE;
            entidade.tipo_tel_1 = record.U_TIPO_TEL_1;
            entidade.tipo_tel_2 = record.U_TIPO_TEL_2;
            entidade.tipo_tel_3 = record.U_TIPO_TEL_3;
            entidade.tipo_tel_4 = record.U_TIPO_TEL_4;
            entidade.tipo_tel_5 = record.U_TIPO_TEL_5;
            entidade.ddd_1 = record.U_DDD_1;
            entidade.ddd_2 = record.U_DDD_2;
            entidade.ddd_3 = record.U_DDD_3;
            entidade.ddd_4 = record.U_DDD_4;
            entidade.ddd_5 = record.U_DDD_5;
            entidade.fone_1 = record.U_FONE_1;
            entidade.fone_2 = record.U_FONE_2;
            entidade.fone_3 = record.U_FONE_3;
            entidade.fone_4 = record.U_FONE_4;
            entidade.fone_5 = record.U_FONE_5;
            entidade.obs = record.U_OBS;
            entidade.crm = record.U_CRM;
            entidade.cnpj_auto_xml = record.U_CNPJ_AUTO_XML;
            entidade.cpf_auto_xml = record.U_CPF_AUTO_XML;
            return entidade;
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

            map.Add("status", "U_STATUS");
            map.Add("nome", "U_NOME");
            map.Add("codigo", "U_CODIGO");
            map.Add("seq", "U_SEQ");
            map.Add("cargo", "U_CARGO");
            map.Add("email_1", "U_EMAIL_1");
            map.Add("email_2", "U_EMAIL_2");
            map.Add("email_1_flags", "U_EMAIL_1_FLAGS");
            map.Add("email_2_flags", "U_EMAIL_2_FLAGS");
            map.Add("site", "U_SITE");
            map.Add("tipo_tel_1", "U_TIPO_TEL_1");
            map.Add("tipo_tel_2", "U_TIPO_TEL_2");
            map.Add("tipo_tel_3", "U_TIPO_TEL_3");
            map.Add("tipo_tel_4", "U_TIPO_TEL_4");
            map.Add("tipo_tel_5", "U_TIPO_TEL_5");
            map.Add("ddd_1", "U_DDD_1");
            map.Add("ddd_2", "U_DDD_2");
            map.Add("ddd_3", "U_DDD_3");
            map.Add("ddd_4", "U_DDD_4");
            map.Add("ddd_5", "U_DDD_5");
            map.Add("fone_1", "U_FONE_1");
            map.Add("fone_2", "U_FONE_2");
            map.Add("fone_3", "U_FONE_3");
            map.Add("fone_4", "U_FONE_4");
            map.Add("fone_5", "U_FONE_5");
            map.Add("obs", "U_OBS");
            map.Add("crm", "U_CRM");
            map.Add("cnpj_auto_xml", "U_CNPJ_AUTO_XML");
            map.Add("cpf_auto_xml", "U_CPF_AUTO_XML");
            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("status", "N");
            map.Add("nome", "N");
            map.Add("codigo", "N");
            map.Add("seq", "T");
            map.Add("cargo", "T");
            map.Add("email_1", "N");
            map.Add("email_2", "N");
            map.Add("email_1_flags", "N");
            map.Add("email_2_flags", "N");
            map.Add("site", "N");
            map.Add("tipo_tel_1", "T");
            map.Add("tipo_tel_2", "T");
            map.Add("tipo_tel_3", "T");
            map.Add("tipo_tel_4", "T");
            map.Add("tipo_tel_5", "T");
            map.Add("ddd_1", "T");
            map.Add("ddd_2", "T");
            map.Add("ddd_3", "T");
            map.Add("ddd_4", "T");
            map.Add("ddd_5", "T");
            map.Add("fone_1", "T");
            map.Add("fone_2", "T");
            map.Add("fone_3", "T");
            map.Add("fone_4", "T");
            map.Add("fone_5", "T");
            map.Add("obs", "N");
            map.Add("crm", "N");
            map.Add("cnpj_auto_xml", "T");
            map.Add("cpf_auto_xml", "T");
            return map;
        }
    }
}
