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
    public class VSITVENCService : IEntityService<Model.Integration.VSITVENC>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public VSITVENCService(ServiceLayerConnector serviceLayerConnector)
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

            table.name = "VSITINVOICEVENC";
            table.description = "Cadastro de VSITINVOICEVENC";
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

        public Task Delete(VSITVENC entity)
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
            string query = Global.MakeODataQuery("U_VSITINVOICEVENC/$count", null, filter.Count == 0 ? null : filter.ToArray(), null, 1, 0);
            string data = await _serviceLayerConnector.getQueryResult(query);
            page.Linhas = Convert.ToInt64(data);
            page.Paginas = (Convert.ToInt64(data) / size.Value) + 1;
            page.qtdPorPagina = size.Value == 0 ? Convert.ToInt64(data) : size.Value;
            return page;
        }
        async public Task<VSITVENC> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"U_VSITINVOICEVENC('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            VSITVENC entidade = toRecord(record);

            // Recupera as linhas da nota iscal
            string[] filter = new string[]
            {
                $"Code eq '{recid}'"
            };

            query = Global.MakeODataQuery("U_VSITINVOICEVENC", null, filter);

            data = await _serviceLayerConnector.getQueryResult(query);

            return entidade;
        }

        async public Task Insert(VSITVENC entity)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();

            entity.status = Data.Model.Integration.VSITVENC.VSITVENCIntegrationStatus.Importing;
            string record = toJson(entity);
            batch.Post(HttpMethod.Post, "/U_VSITINVOICEVENC", record);

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

        public Task Insert(List<VSITVENC> entities)
        {
            throw new NotImplementedException();
        }

        async public Task<List<VSITVENC>> List(List<Criteria> criterias, long page, long size)
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

            string query = Global.MakeODataQuery("U_VSITINVOICEVENC", null, filter.Count == 0 ? null : filter.ToArray());



            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<VSITVENC> result = new List<VSITVENC>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        public Task Update(VSITVENC entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<VSITVENC> entities)
        {
            throw new NotImplementedException();
        }

        private List<TableColumn> createColumns()
        {
            List<TableColumn> lista = new List<TableColumn>();

            lista.Add(new TableColumn() { name = "STATUS", dataType = "db_Numeric", mandatory = false, size = 1, description = "STATUS" });
            lista.Add(new TableColumn() { name = "LAST_UPDATE", dataType = "db_Alpha", mandatory = false, description = "LAST_UPDATE", size = 19 });
            lista.Add(new TableColumn() { name = "DATA_INCLUSAO", dataType = "db_Alpha", mandatory = false, description = "DATA_INCLUSAO", size = 19 });
            lista.Add(new TableColumn() { name = "DATA_INTEGRACAO", dataType = "db_Alpha", mandatory = false, description = "DATA_INTEGRACAO", size = 19 });
            lista.Add(new TableColumn() { name = "LOJ_ORG", dataType = "db_Numeric", mandatory = false, size = 11, description = "LOJ_ORG" });
            lista.Add(new TableColumn() { name = "DIG_ORG", dataType = "db_Numeric", mandatory = false, size = 11, description = "DIG_ORG" });
            lista.Add(new TableColumn() { name = "NRO_NOTA", dataType = "db_Numeric", mandatory = false, size = 11, description = "NRO_NOTA" });
            lista.Add(new TableColumn() { name = "SERIE", dataType = "db_Alpha", mandatory = false, size = 3, description = "SERIE" });
            lista.Add(new TableColumn() { name = "DTA_AGENDA", dataType = "db_Numeric", mandatory = false, size = 11, description = "DTA_AGENDA" });
            lista.Add(new TableColumn() { name = "OPER", dataType = "db_Numeric", mandatory = false, size = 11, description = "OPER" });
            lista.Add(new TableColumn() { name = "DTA_VENCTO", dataType = "db_Numeric", mandatory = false, size = 11, description = "DTA_VENCTO" });
            lista.Add(new TableColumn() { name = "VLR_PARCELA", dataType = "db_Float", mandatory = false, size = 15, description = "VLR_PARCELA", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "VLR_DESCONTO", dataType = "db_Float", mandatory = false, size = 15, description = "VLR_DESCONTO", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "VLR_ACR_FIN", dataType = "db_Float", mandatory = false, size = 15, description = "VLR_ACR_FIN", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FORA_PGTO", dataType = "db_Numeric", mandatory = false, size = 11, description = "FORA_PGTO" });
            lista.Add(new TableColumn() { name = "VLR_COMISSAO", dataType = "db_Float", mandatory = false, size = 15, description = "VLR_COMISSAO", dataTypeSub = "st_Measurement" });

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

        private string toJson(VSITVENC entidade)
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.Code = entidade.RecId.ToString();
            record.Name = entidade.RecId.ToString();
            record.U_STATUS = (VSITVENC.VSITVENCIntegrationStatus.Created);
            record.U_LAST_UPDATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            record.U_DATA_INCLUSAO = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            record.U_LOJ_ORG = entidade.loj_org;
            record.U_DIG_ORG = entidade.dig_org;
            record.U_NRO_NOTA = entidade.nro_nota;
            record.U_SERIE = entidade.serie;
            record.U_DTA_AGENDA = entidade.dta_agenda;
            record.U_OPER = entidade.oper;
            record.U_DTA_VENCTO = entidade.dta_vencto;
            record.U_VLR_PARCELA = entidade.vlr_parcela;
            record.U_VLR_DESCONTO = entidade.vlr_desconto;
            record.U_VLR_ACR_FIN = entidade.vlr_acr_fin;
            record.U_FORA_PGTO = entidade.fora_pgto;
            record.U_VLR_COMISSAO = entidade.vlr_comissao;
            result = JsonConvert.SerializeObject(record);
            return result;
        }

        private VSITVENC toRecord(dynamic record)
        {
            VSITVENC entidade = new VSITVENC();

            entidade.RecId = Guid.Parse(record.Code);
            entidade.status = (VSITVENC.VSITVENCIntegrationStatus)record.U_STATUS;
            entidade.lastupdate = record.U_LAST_UPDATE ;
            entidade.loj_org = record.U_LOJ_ORG;
            entidade.dig_org = record.U_DIG_ORG;
            entidade.nro_nota = record.U_NRO_NOTA;
            entidade.serie = record.U_SERIE;
            entidade.dta_agenda = record.U_DTA_AGENDA;
            entidade.oper = record.U_OPER;
            entidade.dta_vencto = record.U_DTA_VENCTO;
            entidade.vlr_parcela = record.U_VLR_PARCELA;
            entidade.vlr_desconto = record.U_VLR_DESCONTO;
            entidade.vlr_acr_fin = record.U_VLR_ACR_FIN;
            entidade.fora_pgto = record.U_FORA_PGTO;
            entidade.vlr_comissao = record.U_VLR_COMISSAO;
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
            map.Add("loj_org", "U_LOJ_ORG");
            map.Add("dig_org", "U_DIG_ORG");
            map.Add("nro_nota", "U_NRO_NOTA");
            map.Add("serie", "U_SERIE");
            map.Add("dta_agenda", "U_DTA_AGENDA");
            map.Add("oper", "U_OPER");
            map.Add("dta_vencto", "U_DTA_VENCTO");
            map.Add("vlr_parcela", "U_VLR_PARCELA");
            map.Add("vlr_desconto", "U_VLR_DESCONTO");
            map.Add("vlr_acr_fin", "U_VLR_ACR_FIN");
            map.Add("fora_pgto", "U_FORA_PGTO");
            map.Add("vlr_comissao", "U_VLR_COMISSAO");
            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("status", "N");
            map.Add("loj_org", "N");
            map.Add("dig_org", "T");
            map.Add("nro_nota", "N");
            map.Add("serie", "T");
            map.Add("dta_agenda", "N");
            map.Add("oper", "N");
            map.Add("dta_vencto", "T");
            map.Add("vlr_parcela", "T");
            map.Add("vlr_desconto", "T");
            map.Add("vlr_acr_fin", "T");
            map.Add("fora_pgto", "T");
            map.Add("vlr_comissao", "T");
            return map;
        }
    }
}
