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
    public class VSITENTIDADEService : IEntityService<Model.Integration.VSITENTIDADE>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public VSITENTIDADEService(ServiceLayerConnector serviceLayerConnector)
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
            table.name = "VSITENTIDADE";
            table.description = "Cadastro de entidades";
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

        public Task Delete(VSITENTIDADE entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task<VSITENTIDADE> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"U_VSITENTIDADE('{recid}')");
            string data = await _serviceLayerConnector.getQueryResult(query);
            ExpandoObject record = Global.parseQueryToObject(data);
            VSITENTIDADE cadEntidade = toRecord(record);

            Serviceb1.Integration.VSITENTIDADECONTService contatoS = new VSITENTIDADECONTService(_serviceLayerConnector);
            List<Criteria> filtro = new List<Criteria>();
            filtro.Add(new Criteria
            {
                Field = "codigo",
                Operator = "eq",
                Value = cadEntidade.codigo.ToString()
            });
            cadEntidade.contatos = await contatoS.List(filtro, 1, -1);


            // Recupera as linhas da nota iscal
            string[] filter = new string[]
            {
                $"Code eq '{recid}'"
            };

            query = Global.MakeODataQuery("U_VSITENTIDADE", null, filter);

            data = await _serviceLayerConnector.getQueryResult(query);

            return cadEntidade;
        }

        async public Task Insert(VSITENTIDADE entity)
        {
            //verifica se ja tem
            try
            {
                IBatchProducer batch = _serviceLayerConnector.CreateBatch();
                entity.cod_x25 = parseCountry(entity.cod_x25);
                entity.status = Data.Model.Integration.VSITENTIDADE.VSITENTIDADEIntegrationStatus.Importing;
                string record = toJson(entity);

                batch.Post(HttpMethod.Post, "/U_VSITENTIDADE", record);
                ServiceLayerResponse response = await _serviceLayerConnector.Post(batch);
                if (!response.success)
                {
                    string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                    Console.WriteLine(message);
                    throw new ApplicationException(message);
                }
            }
            catch(Exception e)
            {

            }        
        }

        private string parseCountry(dynamic value)
        {
            string origem = value;
            string result = null;
            if (origem.Length >= 12)
            {
                result = origem.Substring(9, 3);
            }
            
            return result;
        }
        public Task Insert(List<VSITENTIDADE> entities)
        {
            throw new NotImplementedException();
        }

        async public Task<List<VSITENTIDADE>> List(List<Criteria> criterias, long page, long size)
        {
            List<string> filter = new List<string>();
            int cont = 0;
            if (criterias?.Count != 0)
            {
                foreach(var c in criterias)
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
                    else if(type == "N")
                    {
                        filter.Add($"{field} {c.Operator.ToLower()} {c.Value}");
                    }

                }

                //if (cont.Equals(criterias.Count))
                //{
                //    var status = criterias.Where(c => c.Field == "status").ToList();
                //    if (status.Count == 0)
                //    {
                //        filter.Add($"{"U_STATUS"} {"ne"} 2");
                //    }
                //}
            }

            //if (filter.Count == 0)
            //{
            //    filter.Add($"{"U_STATUS"} {"ne"} 2");
            //}

            string query = Global.MakeODataQuery("U_VSITENTIDADE", null, filter.Count == 0 ? null : filter.ToArray(),null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<VSITENTIDADE> result = new List<VSITENTIDADE>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

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
            string query = Global.MakeODataQuery("U_VSITENTIDADE/$count",null,filter.Count == 0 ? null : filter.ToArray(), null,1,0);
            string data = await _serviceLayerConnector.getQueryResult(query);
            page.Linhas = Convert.ToInt64(data);
            page.Paginas = (Convert.ToInt64(data) / size.Value) + 1;
            page.qtdPorPagina = size.Value == 0 ? Convert.ToInt64(data) : size.Value;
            return page;
        }
        public Task Update(VSITENTIDADE entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<VSITENTIDADE> entities)
        {
            throw new NotImplementedException();
        }

        private List<TableColumn> createColumns()
        {
            List<TableColumn> lista = new List<TableColumn>();
            lista.Add(new TableColumn()
            {
                name = "NATUREZA",
                dataType = "db_Alpha",
                mandatory = false,
                size = 7,
                description = "NATUREZA"
            });
            lista.Add(new TableColumn()
            {
                name = "CODIGO",
                dataType = "db_Numeric",
                mandatory = true,
                size = 7,
                description = "CODIGO"
            });
            lista.Add(new TableColumn()
            {
                name = "NOME_FANTASIA",
                dataType = "db_Alpha",
                mandatory = false,
                size = 30,
                description = "NOME_FANTASIA"
            });
            lista.Add(new TableColumn()
            {
                name = "RAZAO_SOCIAL",
                dataType = "db_Alpha",
                mandatory = false,
                size = 30,
                description = "RAZAO_SOCIAL"
            });
            lista.Add(new TableColumn()
            {
                name = "CGC_CPF",
                dataType = "db_Alpha",
                mandatory = false,
                size = 15,     
                description = "CGC_CPF"
            });
            lista.Add(new TableColumn()
            {
                name = "INSC_EST_IDENT",
                dataType = "db_Alpha",
                mandatory = false,
                size = 15,
                description = "INSC_EST_IDENT"
            });
            lista.Add(new TableColumn()
            {
                name = "INSC_MUN",
                dataType = "db_Alpha",
                mandatory = false,
                size = 15,
                description = "INSC_MUN"
            });
            lista.Add(new TableColumn()
            {
                name = "CEP",
                dataType = "db_Numeric",
                mandatory = false,
                size = 9,
                description = "CEP"
            });
            lista.Add(new TableColumn()
            {
                name = "BAIRRO",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = "BAIRRO"
            });
            lista.Add(new TableColumn()
            {
                name = "CIDADE",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = "CIDADE"
            });
            lista.Add(new TableColumn()
            {
                name = "ESTADO",
                dataType = "db_Alpha",
                mandatory = false,
                size = 2,
                description = "ESTADO"
            });
            lista.Add(new TableColumn()
            {
                name = "COD_X25",
                dataType = "db_Alpha",
                mandatory = false,
                size = 15,
                description = "COD_X25"
            });
            lista.Add(new TableColumn()
            {
                name = "LOJ_CLI",
                dataType = "db_Alpha",
                mandatory = false,
                size = 1,
                description = "LOJ_CLI"
            });
            lista.Add(new TableColumn()
            {
                name = "INSC_EST_SUBST",
                dataType = "db_Alpha",
                mandatory = false,
                size = 15,
                description = "INSC_EST_SUBST"
            });
            lista.Add(new TableColumn()
            {
                name = "COD_MUNICIPIO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = "COD_MUNICIPIO"
            });
            lista.Add(new TableColumn()
            {
                name = "NIRE",
                dataType = "db_Alpha",
                mandatory = false,
                size = 15,
                description = "NIRE"
            });
            lista.Add(new TableColumn()
            {
                name = "SUFRAMA",
                dataType = "db_Alpha",
                mandatory = false,
                size = 15,
                description = "SUFRAMA"
            });

            lista.Add(new TableColumn()
            {
                name = "LOCALIDADE",
                dataType = "db_Alpha",
                mandatory = false,
                size = 40,
                description = "LOCALIDADE"
            });
            lista.Add(new TableColumn()
            {
                name = "NR_INTERIOR",
                dataType = "db_Alpha",
                mandatory = false,
                size = 10,
                description = "NR_INTERIOR"
            });
            lista.Add(new TableColumn()
            {
                name = "DATA_FECHA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 6,
                description = "DATA_FECHA"
            });
            lista.Add(new TableColumn()
            {
                name = "STATUS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 1,
                description = "STATUS"
            });
            lista.Add(new TableColumn()
            {
                name = "LASTUPDATE",
                dataType = "db_Alpha",
                size = 19,
                mandatory = false,
                description = "LASTUPDATE"
            });
            lista.Add(new TableColumn()
            {
                name = "DATA_INCLUSAO",
                dataType = "db_Alpha",
                size = 19,
                mandatory = false,
                description = "DATA_INCLUSAO"
            });
            lista.Add(new TableColumn()
            {
                name = "DATA_INTEGRACAO",
                dataType = "db_Alpha",
                size = 19,
                mandatory = false,
                description = "DATA_INTEGRACAO"
            });
            lista.Add(new TableColumn()
            {
                name = "FOR_CONTATO",
                dataType = "db_Alpha",
                mandatory = false,
                size = 60,
                description = "FOR_CONTATO"
            });
            lista.Add(new TableColumn()
            {
                name = "CLI_CONTATO",
                dataType = "db_Alpha",
                mandatory = false,
                size = 60,
                description = "CLI_CONTATO"
            });
            lista.Add(new TableColumn()
            {
                name = "FORPRI",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = "FORPRI"
            });
            lista.Add(new TableColumn()
            {
                name = "DATA_CAD",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = "DATA_CAD"
            });
            lista.Add(new TableColumn()
            {
                name = "DTA_FLINHA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 8,
                description = "DTA_FLINHA"
            });
            lista.Add(new TableColumn()
            {
                name = "SITUACAO",
                dataType = "db_Alpha",
                mandatory = false,
                size = 1,
                description = "SITUACAO"
            });
            lista.Add(new TableColumn()
            {
                name = "NR_EXTERIOR",
                dataType = "db_Alpha",
                mandatory = false,
                size = 60,
                description = "NR_EXTERIOR"
            });
            lista.Add(new TableColumn()
            {
                name = "BANCO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = "BANCO"
            });
            lista.Add(new TableColumn()
            {
                name = "CONTA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = "CONTA"
            });
            lista.Add(new TableColumn()
            {
                name = "AGENCIA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = "AGENCIA"
            });
            lista.Add(new TableColumn()
            {
                name = "DIG_CONTA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 1,
                description = "DIG_CONTA"
            });
            lista.Add(new TableColumn()
            {
                name = "DIG_AGEN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 1,
                description = "DIG_AGEN"
            });
            lista.Add(new TableColumn()
            {
                name = "CARGO",
                dataType = "db_Alpha",
                mandatory = false,
                size = 100,
                description = "CARGO"
            });

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

        private string toJson(VSITENTIDADE cadEntidade)
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.Code = cadEntidade.RecId.ToString();
            record.Name = cadEntidade.RecId.ToString();
            record.U_NATUREZA = cadEntidade.natureza;
            record.U_CODIGO = cadEntidade.codigo;
            record.U_NOME_FANTASIA = cadEntidade.nome_fantasia;
            record.U_RAZAO_SOCIAL = cadEntidade.razao_social;
            record.U_CGC_CPF = cadEntidade.cgc_cpf;
            record.U_INSC_EST_IDENT = cadEntidade.insc_est_ident;
            record.U_INSC_MUN = cadEntidade.insc_mun;
            record.U_CEP = cadEntidade.cep;
            record.U_BAIRRO = cadEntidade.bairro;
            record.U_CIDADE = cadEntidade.cidade;
            record.U_ESTADO = cadEntidade.estado;
            record.U_COD_X25 = cadEntidade.cod_x25;
            record.U_LOJ_CLI = cadEntidade.loj_cli;
            record.U_INSC_EST_SUBST = cadEntidade.insc_est_subst;
            record.U_COD_MUNICIPIO = cadEntidade.cod_municipio;
            record.U_SUFRAMA = cadEntidade.suframa;
            record.U_LOCALIDADE = cadEntidade.localidade;
            record.U_NR_INTERIOR = cadEntidade.nr_interior;
            record.U_DATA_FECHA = cadEntidade.data_fecha;
            record.U_FOR_CONTATO = cadEntidade.for_contato;
            record.U_CLI_CONTATO = cadEntidade.cli_contato;
            record.U_FORPRI = cadEntidade.forpri;
            record.U_DATA_CAD = cadEntidade.data_cad;
            record.U_DTA_FLINHA = cadEntidade.dta_flinha;
            record.U_SITUACAO = cadEntidade.situacao;
            record.U_NR_EXTERIOR = cadEntidade.nr_exterior;
            record.U_BANCO = cadEntidade.banco;
            record.U_CONTA = cadEntidade.conta;
            record.U_AGENCIA = cadEntidade.agencia;
            record.U_DIG_CONTA = cadEntidade.dig_conta;
            record.U_DIG_AGEN = cadEntidade.dig_agen;
            record.U_CARGO = cadEntidade.cargo;
            record.U_STATUS = (VSITENTIDADE.VSITENTIDADEIntegrationStatus.Created);
            record.U_LASTUPDATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            record.U_DATA_INCLUSAO = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            result = JsonConvert.SerializeObject(record);
            return result;
        }

        private VSITENTIDADE toRecord(dynamic record)
        {
            VSITENTIDADE cadEntidade = new VSITENTIDADE();
            cadEntidade.RecId = Guid.Parse(record.Code);
            cadEntidade.codigo = record.U_CODIGO;
            cadEntidade.nome_fantasia = record.U_NOME_FANTASIA;
            cadEntidade.razao_social = record.U_RAZAO_SOCIAL;
            cadEntidade.cgc_cpf = record.U_CGC_CPF;
            cadEntidade.insc_est_ident = record.U_INSC_EST_IDENT;
            cadEntidade.insc_mun = record.U_INSC_MUN;
            cadEntidade.cep = record.U_CEP;
            cadEntidade.bairro = record.U_BAIRRO;
            cadEntidade.cidade = record.U_CIDADE;
            cadEntidade.estado = record.U_ESTADO;
            cadEntidade.cod_x25 = record.U_COD_X25;
            cadEntidade.loj_cli = record.U_LOJ_CLI;
            cadEntidade.insc_est_subst = record.U_INSC_EST_SUBST;
            cadEntidade.cod_municipio = record.U_COD_MUNICIPIO != null ? Convert.ToString(record.U_COD_MUNICIPIO)  : null;
            cadEntidade.nire = record.U_NIRE;
            cadEntidade.suframa = record.U_SUFRAMA;
            cadEntidade.localidade = record.U_LOCALIDADE;
            cadEntidade.nr_interior = record.U_NR_INTERIOR;
            cadEntidade.data_fecha = record.U_DATA_FECHA;
            cadEntidade.for_contato = record.U_FOR_CONTATO;
            cadEntidade.cli_contato = record.U_CLI_CONTATO;
            cadEntidade.forpri = record.U_FORPRI;
            cadEntidade.data_cad = record.U_DATA_CAD;
            cadEntidade.dta_flinha = record.U_DTA_FLINHA;
            cadEntidade.situacao = record.U_SITUACAO;
            cadEntidade.nr_exterior = record.U_NR_EXTERIOR;
            cadEntidade.banco = record.U_BANCO;
            cadEntidade.conta = record.U_CONTA;
            cadEntidade.agencia = record.U_AGENCIA;
            cadEntidade.dig_conta = record.U_DIG_CONTA;
            cadEntidade.dig_agen = record.U_DIG_AGEN;
            cadEntidade.cargo = record.U_CARGO;
            cadEntidade.lastupdate = parseDate(record.U_LASTUPDATE);
            cadEntidade.data_inclusao = parseDate(record.U_DATA_INCLUSAO);
            cadEntidade.data_integracao = parseDate(record.U_DATA_INTEGRACAO);
            cadEntidade.natureza = record.U_NATUREZA;
            cadEntidade.codigo = record.U_CODIGO;
            cadEntidade.status = (VSITENTIDADE.VSITENTIDADEIntegrationStatus)record.U_STATUS;
            return cadEntidade;
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
            map.Add("loj_cli", "U_LOJ_CLI");
            map.Add("codigo", "U_CODIGO");
            map.Add("nome_fantasia", "U_NOME_FANTASIA");
            map.Add("razao_social", "U_RAZAO_SOCIAL");
            map.Add("cgc_cpf", "U_CGC_CPF");
            map.Add("insc_est_ident", "U_INSC_EST_IDENT");
            map.Add("insc_mun", "U_INSC_MUN");
            map.Add("cep", "U_CEP");
            map.Add("bairro", "U_BAIRRO");
            map.Add("cidade", "U_CIDADE");
            map.Add("estado", "U_ESTADO");
            map.Add("cod_x25", "U_COD_X25");
            map.Add("insc_est_subst", "U_INSC_EST_SUBST");
            map.Add("cod_municipio", "U_COD_MUNICIPIO");
            map.Add("nire", "U_NIRE");
            map.Add("suframa", "U_SUFRAMA");
            map.Add("localidade", "U_LOCALIDADE");
            map.Add("nr_interior", "U_NR_INTERIOR");
            map.Add("data_fecha", "U_DATA_FECHA");
            map.Add("lastupdate", "U_LASTUPDATE");
            map.Add("data_inclusao", "U_DATA_INCLUSAO");
            map.Add("data_integracao", "U_DATA_INTEGRACAO");
            map.Add("status", "U_STATUS");
            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("loj_cli", "T");
            map.Add("codigo", "N");
            map.Add("nome_fantasia", "T");
            map.Add("razao_social", "T");
            map.Add("cgc_cpf", "T");
            map.Add("insc_est_ident", "T");
            map.Add("insc_mun", "T");
            map.Add("cep", "N");
            map.Add("bairro", "T");
            map.Add("cidade", "T");
            map.Add("estado", "T");
            map.Add("cod_x25", "T");
            map.Add("insc_est_subst", "T");
            map.Add("cod_municipio", "N");
            map.Add("nire", "T");
            map.Add("suframa", "T");
            map.Add("localidade", "T");
            map.Add("nr_interior", "T");
            map.Add("data_fecha", "N");
            map.Add("lastupdate", "T");
            map.Add("data_inclusao", "T");
            map.Add("data_integracao", "T");
            map.Add("status", "N");

            return map;
        }
    }
}
