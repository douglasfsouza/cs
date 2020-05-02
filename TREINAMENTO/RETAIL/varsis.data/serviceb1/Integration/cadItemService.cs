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
    public class CadItemService : IEntityService<Model.Integration.CadItem>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public CadItemService(ServiceLayerConnector serviceLayerConnector)
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

            table.name = "VSISCADITEM";
            table.description = "Cadstro de Items";
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

        public Task Delete(CadItem entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task<CadItem> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"U_VSISINTINVOH('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            CadItem cadItem = toRecord(record);

            // Recupera as linhas da nota iscal
            string[] filter = new string[]
            {
                $"U_INVOICECODE eq '{recid}'"
            };

            query = Global.MakeODataQuery("U_VSISINTINVOI", null, filter);

            data = await _serviceLayerConnector.getQueryResult(query);

            return cadItem;
        }

        async public Task Insert(CadItem entity)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();

            entity.status = Data.Model.Integration.CadItem.CadItemIntegrationStatus.Importing;
            string record = toJson(entity);
            batch.Post(HttpMethod.Post, "/U_VSISCADITEM", record);

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
            //if (response.internalResponses.Count(m => m.success == false) == 0)
            //{
            //    //
            //    // O registro só será alterado se não houver erros
            //    //
            //    entity.status = CadItem.CadItemIntegrationStatus.Created;
            //    record = toJson(entity);
            //    response = await _serviceLayerConnector.Patch($"U_VSISINTINVOH('{entity.RecId}')", record, true);

            //    if (!response.success)
            //    {
            //        string message = $"Erro ao inserir dados em '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";

            //        Console.WriteLine(message);
            //        throw new ApplicationException(message);
            //    }
            //}
            //else
            //{
            //    // Erro no cabeçalho da nota fiscal
            //    if (response.internalResponses.Count == 1)
            //    {
            //        string message = $"Erro ao inserir dados em '{entity.EntityName}': {response.internalResponses[0].errorCode}-{response.internalResponses[0].errorMessage}";
            //        Console.WriteLine(message);
            //        throw new ApplicationException(message);
            //    }
            //    else
            //    {
            //        int position = response.internalResponses.Count - 1;
            //        //int item = position - 1;

            //        //string message = $"Erro ao inserir dados em '{entity.items[0].EntityName}' item {entity.items[item].itemId} : {response.internalResponses[position].errorCode}-{response.internalResponses[position].errorMessage}";
            //        //Console.WriteLine(message);
            //        throw new ApplicationException("Vazio");
            //    }
            //}
        }

        public Task Insert(List<CadItem> entities)
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
        async public Task<List<CadItem>> List(List<Criteria> criterias, long page, long size)
        {
            List<string> filter = new List<string>();

            if (criterias?.Count != 0)
            {
                foreach(var c in criterias)
                {
                    string field = _FieldMap[c.Field.ToLower()];
                    string type = _FieldType[c.Field.ToLower()];

                    if (type == "T")
                    {
                        filter.Add($"{field} {c.Operator.ToLower()} '{c.Value}'");
                    }
                    else if(type == "N")
                    {
                        filter.Add($"{field} {c.Operator.ToLower()} {c.Value}");
                    }
                }
            }

            string query = Global.MakeODataQuery("U_VSISINTINVOH", null, filter.Count == 0 ? null : filter.ToArray(), null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<CadItem> result = new List<CadItem>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        public Task Update(CadItem entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<CadItem> entities)
        {
            throw new NotImplementedException();
        }

        private List<TableColumn> createColumns()
        {
            List<TableColumn> lista = new List<TableColumn>();




            lista.Add(new TableColumn()
            {
                name = "DEPTO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "SECAO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "GRUPO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "SUBGRUPO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "CATEGORIA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "COD_FOR",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "COD_ITEM",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "DIGITO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "CODIGO_PAI",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "CODIGO_EAN13",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DESCRICAO",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DESC_REDUZ",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DESC_COML",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "REFERENCIA",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "EMB_FOR",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TPO_EMB_FOR",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "EAN_EMB_FOR",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "EMB_TRANSF",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TPO_EMB_TRANSF",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "EAN_EMB_TRANSF",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "EMB_VENDA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TPO_EMB_VENDA",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "EAN_EMB_VENDA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TIPO_PALLET",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BASE_PALLET",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALTURA_PALLET",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "COMPRIMENTO_EMB",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "COMPR_EMB_TRF",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "COMPR_EMB_VND",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "LARGURA_EMB",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "LARGURA_EMB_TRF",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "LARGURA_EMB_VND",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALTURA_EMB",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALTURA_EMB_TRF",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALTURA_EMB_VND",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "TIPO_PLU",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "PESO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "PESO_TRF",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "PESO_VND",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "DEPOSITO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "LINHA",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "CLASSE",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "POLIT_PRE",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "SIS_ABAST",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRZ_ENTRG",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DIA_VISIT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "FRQ_VISIT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "TIPO_ETQ",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "TIPO_PRO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "COMPRADOR",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "COND_PGTO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "COND_PGTO_ANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "COND_PGTO_MAN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "CDPG_VDA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TIPO_ETQ_GON",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "COR",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_ENT_LIN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_SAI_LIN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "NAT_FISCAL",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "ESTADO",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "COD_PAUTA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "PERC_IPI",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "QTDE_ETQ_GON",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PERC_BONIF",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PERC_BONIF_ANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PERC_BONIF_MAN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "ENTREGA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "FRETE",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "CUS_FOR",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "CUSF_ANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "CUSF_MAN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_CUS_FOR",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_CUSF_ANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_CUSF_MAN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DESP_ACES",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DESP_ACES_ANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DESP_ACES_MAN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "CUS_REP",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "CUSR_ANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "CUSR_MAN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_CUS_REP",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_CUSR_ANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_CUSR_MAN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "CUS_MED",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "CUSM_ANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "CUSM_MAN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_CUS_MED",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_CUSM_ANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_CUSM_MAN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "CUS_MED_C",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_CUS_MED_C",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRC_VEN_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRCV_ANT_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRCV_MAN_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "MRG_LUCRO_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DSC_MAX_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "COMISSAO_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_PRC_VEN_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_PRCV_ANT_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_PRCV_MAN_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRC_VEN_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRCV_ANT_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRCV_MAN_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "MRG_LUCRO_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DSC_MAX_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "COMISSAO_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_PRC_VEN_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_PRCV_ANT_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_PRCV_MAN_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRC_VEN_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRCV_ANT_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRCV_MAN_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "MRG_LUCRO_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DSC_MAX_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "COMISSAO_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_PRC_VEN_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_PRCV_ANT_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_PRCV_MAN_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRC_VEN_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRCV_ANT_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRCV_MAN_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "MRG_LUCRO_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DSC_MAX_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "COMISSAO_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_PRC_VEN_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_PRCV_ANT_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_PRCV_MAN_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRC_VEN_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRCV_ANT_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRCV_MAN_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "MRG_LUCRO_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DSC_MAX_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "COMISSAO_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_PRC_VEN_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_PRCV_ANT_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_PRCV_MAN_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TIP_OFT_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TIP_OFT_ANT_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "INI_OFT_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "INI_OFT_ANT_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "FIM_OFT_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "FIM_OFT_ANT_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRC_OFT_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRC_OFT_ANT_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "LIM_OFT_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "LIM_OFT_ANT_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "SAL_OFT_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "SAL_OFT_ANT_1",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TIP_OFT_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TIP_OFT_ANT_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "INI_OFT_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "INI_OFT_ANT_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "FIM_OFT_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "FIM_OFT_ANT_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRC_OFT_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRC_OFT_ANT_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "LIM_OFT_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "LIM_OFT_ANT_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "SAL_OFT_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "SAL_OFT_ANT_2",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TIP_OFT_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TIP_OFT_ANT_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "INI_OFT_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "INI_OFT_ANT_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "FIM_OFT_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "FIM_OFT_ANT_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRC_OFT_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRC_OFT_ANT_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "LIM_OFT_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "LIM_OFT_ANT_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "SAL_OFT_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "SAL_OFT_ANT_3",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TIP_OFT_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TIP_OFT_ANT_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "INI_OFT_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "INI_OFT_ANT_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "FIM_OFT_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "FIM_OFT_ANT_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRC_OFT_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRC_OFT_ANT_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "LIM_OFT_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "LIM_OFT_ANT_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "SAL_OFT_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "SAL_OFT_ANT_4",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TIP_OFT_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TIP_OFT_ANT_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "INI_OFT_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "INI_OFT_ANT_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "FIM_OFT_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "FIM_OFT_ANT_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRC_OFT_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRC_OFT_ANT_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "LIM_OFT_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "LIM_OFT_ANT_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "SAL_OFT_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "SAL_OFT_ANT_5",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "INI_BONUS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "INI_BONUS_ANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "FIM_BONUS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "PRES_ENT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRC_BONUS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "PRES_REP",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ESTQ_ATUAL",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "ESTQ_DP",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "ESTQ_LJ",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "QDE_PEND",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "CUS_INV",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ESTQ_PADRAO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "SAI_MED_CAL",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "TAMANHO",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "SAI_ACM_UN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "SAI_ACM_CUS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "SAI_ACM_VEN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "CUS_ULT_ENT_BRU",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ENT_ACM_UN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ENT_ACM_CUS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_ULT_FAT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ULT_QDE_FAT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ULT_QDE_ENT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "CUS_ULT_ENT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DAT_ULT_ENT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "ABC_F",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "ABC_S",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "ABC_T",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PERECIVEL",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PRZ_VALIDADE",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TOT_PEDIDO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TOT_FALTA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "COD_VAS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "DIG_VAS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "COD_ENG",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "DIG_ENG",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALOR_IPI",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALOR_IPI_ANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALOR_IPI_MAN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "CLASS_FIS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "QTD_AUT_PDV",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "MOEDA_VDA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BONI_MERC",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BONI_MERC_ANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BONI_MERC_MAN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "GRADE",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "MENS_AUX",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PROCEDENCIA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "CATEGORIA_ANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ESTRATEGIA_REP",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DESP_ACES_ISEN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DESP_ACES_ISEN_ANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DESP_ACES_ISEN_MAN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "FRETE_VALOR",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "FRETE_VALOR_ANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "FRETE_VALOR_MAN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BONIF_PER",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BONIF_PER_ANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BONIF_PER_MAN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VASILHAME",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PERMITE_DESC",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "QTD_OBRIG",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ENVIA_PDV",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ENVIA_BALANCA",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "PESADO_PDV",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "JPMA_FLAG1",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "JPMA_FLAG2",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "JPMA_FLAG3",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "LINHA_ANTERIOR",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "LINHA_VALIDA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "QUANT_EAN",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TOT_ESTOCADO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "EMPIL_MAX",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "TIPO_END",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "SAZONAL",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "MARCA_PROP",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "FILLER",
                dataType = "db_Numeric",
                mandatory = false,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "LASTUPDATE",
                dataType = "db_Date",
                mandatory = false,
                description = ""
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

        private string toJson(CadItem cadItem)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.Code = cadItem.RecId.ToString();
            record.Name = cadItem.RecId.ToString();
            record.U_DEPTO = cadItem.DEPTO;
            record.U_SECAO = cadItem.SECAO;
            record.U_GRUPO = cadItem.GRUPO;
            record.U_SUBGRUPO = cadItem.SUBGRUPO;
            record.U_CATEGORIA = cadItem.CATEGORIA;
            record.U_COD_FOR = cadItem.COD_FOR;
            record.U_COD_ITEM = cadItem.COD_ITEM;
            record.U_DIGITO = cadItem.DIGITO;
            record.U_CODIGO_PAI = cadItem.CODIGO_PAI;
            record.U_CODIGO_EAN13 = cadItem.CODIGO_EAN13;
            record.U_DESCRICAO = cadItem.DESCRICAO;
            record.U_DESC_REDUZ = cadItem.DESC_REDUZ;
            record.U_DESC_COML = cadItem.DESC_COML;
            record.U_REFERENCIA = cadItem.REFERENCIA;
            record.U_EMB_FOR = cadItem.EMB_FOR;
            record.U_TPO_EMB_FOR = cadItem.TPO_EMB_FOR;
            record.U_EAN_EMB_FOR = cadItem.EAN_EMB_FOR;
            record.U_EMB_TRANSF = cadItem.EMB_TRANSF;
            record.U_TPO_EMB_TRANSF = cadItem.TPO_EMB_TRANSF;
            record.U_EAN_EMB_TRANSF = cadItem.EAN_EMB_TRANSF;
            record.U_EMB_VENDA = cadItem.EMB_VENDA;
            record.U_TPO_EMB_VENDA = cadItem.TPO_EMB_VENDA;
            record.U_EAN_EMB_VENDA = cadItem.EAN_EMB_VENDA;
            record.U_TIPO_PALLET = cadItem.TIPO_PALLET;
            record.U_BASE_PALLET = cadItem.BASE_PALLET;
            record.U_ALTURA_PALLET = cadItem.ALTURA_PALLET;
            record.U_COMPRIMENTO_EMB = cadItem.COMPRIMENTO_EMB;
            record.U_COMPR_EMB_TRF = cadItem.COMPR_EMB_TRF;
            record.U_COMPR_EMB_VND = cadItem.COMPR_EMB_VND;
            record.U_LARGURA_EMB = cadItem.LARGURA_EMB;
            record.U_LARGURA_EMB_TRF = cadItem.LARGURA_EMB_TRF;
            record.U_LARGURA_EMB_VND = cadItem.LARGURA_EMB_VND;
            record.U_ALTURA_EMB = cadItem.ALTURA_EMB;
            record.U_ALTURA_EMB_TRF = cadItem.ALTURA_EMB_TRF;
            record.U_ALTURA_EMB_VND = cadItem.ALTURA_EMB_VND;
            record.U_TIPO_PLU = cadItem.TIPO_PLU;
            record.U_PESO = cadItem.PESO;
            record.U_PESO_TRF = cadItem.PESO_TRF;
            record.U_PESO_VND = cadItem.PESO_VND;
            record.U_DEPOSITO = cadItem.DEPOSITO;
            record.U_LINHA = cadItem.LINHA;
            record.U_CLASSE = cadItem.CLASSE;
            record.U_POLIT_PRE = cadItem.POLIT_PRE;
            record.U_SIS_ABAST = cadItem.SIS_ABAST;
            record.U_PRZ_ENTRG = cadItem.PRZ_ENTRG;
            record.U_DIA_VISIT = cadItem.DIA_VISIT;
            record.U_FRQ_VISIT = cadItem.FRQ_VISIT;
            record.U_TIPO_ETQ = cadItem.TIPO_ETQ;
            record.U_TIPO_PRO = cadItem.TIPO_PRO;
            record.U_COMPRADOR = cadItem.COMPRADOR;
            record.U_COND_PGTO = cadItem.COND_PGTO;
            record.U_COND_PGTO_ANT = cadItem.COND_PGTO_ANT;
            record.U_COND_PGTO_MAN = cadItem.COND_PGTO_MAN;
            record.U_CDPG_VDA = cadItem.CDPG_VDA;
            record.U_TIPO_ETQ_GON = cadItem.TIPO_ETQ_GON;
            record.U_COR = cadItem.COR;
            record.U_DAT_ENT_LIN = cadItem.DAT_ENT_LIN;
            record.U_DAT_SAI_LIN = cadItem.DAT_SAI_LIN;
            record.U_NAT_FISCAL = cadItem.NAT_FISCAL;
            record.U_ESTADO = cadItem.ESTADO;
            record.U_COD_PAUTA = cadItem.COD_PAUTA;
            record.U_PERC_IPI = cadItem.PERC_IPI;
            record.U_QTDE_ETQ_GON = cadItem.QTDE_ETQ_GON;
            record.U_PERC_BONIF = cadItem.PERC_BONIF;
            record.U_PERC_BONIF_ANT = cadItem.PERC_BONIF_ANT;
            record.U_PERC_BONIF_MAN = cadItem.PERC_BONIF_MAN;
            record.U_ENTREGA = cadItem.ENTREGA;
            record.U_FRETE = cadItem.FRETE;
            record.U_CUS_FOR = cadItem.CUS_FOR;
            record.U_CUSF_ANT = cadItem.CUSF_ANT;
            record.U_CUSF_MAN = cadItem.CUSF_MAN;
            record.U_DAT_CUS_FOR = cadItem.DAT_CUS_FOR;
            record.U_DAT_CUSF_ANT = cadItem.DAT_CUSF_ANT;
            record.U_DAT_CUSF_MAN = cadItem.DAT_CUSF_MAN;
            record.U_DESP_ACES = cadItem.DESP_ACES;
            record.U_DESP_ACES_ANT = cadItem.DESP_ACES_ANT;
            record.U_DESP_ACES_MAN = cadItem.DESP_ACES_MAN;
            record.U_CUS_REP = cadItem.CUS_REP;
            record.U_CUSR_ANT = cadItem.CUSR_ANT;
            record.U_CUSR_MAN = cadItem.CUSR_MAN;
            record.U_DAT_CUS_REP = cadItem.DAT_CUS_REP;
            record.U_DAT_CUSR_ANT = cadItem.DAT_CUSR_ANT;
            record.U_DAT_CUSR_MAN = cadItem.DAT_CUSR_MAN;
            record.U_CUS_MED = cadItem.CUS_MED;
            record.U_CUSM_ANT = cadItem.CUSM_ANT;
            record.U_CUSM_MAN = cadItem.CUSM_MAN;
            record.U_DAT_CUS_MED = cadItem.DAT_CUS_MED;
            record.U_DAT_CUSM_ANT = cadItem.DAT_CUSM_ANT;
            record.U_DAT_CUSM_MAN = cadItem.DAT_CUSM_MAN;
            record.U_CUS_MED_C = cadItem.CUS_MED_C;
            record.U_DAT_CUS_MED_C = cadItem.DAT_CUS_MED_C;
            record.U_PRC_VEN_1 = cadItem.PRC_VEN_1;
            record.U_PRCV_ANT_1 = cadItem.PRCV_ANT_1;
            record.U_PRCV_MAN_1 = cadItem.PRCV_MAN_1;
            record.U_MRG_LUCRO_1 = cadItem.MRG_LUCRO_1;
            record.U_DSC_MAX_1 = cadItem.DSC_MAX_1;
            record.U_COMISSAO_1 = cadItem.COMISSAO_1;
            record.U_DAT_PRC_VEN_1 = cadItem.DAT_PRC_VEN_1;
            record.U_DAT_PRCV_ANT_1 = cadItem.DAT_PRCV_ANT_1;
            record.U_DAT_PRCV_MAN_1 = cadItem.DAT_PRCV_MAN_1;
            record.U_PRC_VEN_2 = cadItem.PRC_VEN_2;
            record.U_PRCV_ANT_2 = cadItem.PRCV_ANT_2;
            record.U_PRCV_MAN_2 = cadItem.PRCV_MAN_2;
            record.U_MRG_LUCRO_2 = cadItem.MRG_LUCRO_2;
            record.U_DSC_MAX_2 = cadItem.DSC_MAX_2;
            record.U_COMISSAO_2 = cadItem.COMISSAO_2;
            record.U_DAT_PRC_VEN_2 = cadItem.DAT_PRC_VEN_2;
            record.U_DAT_PRCV_ANT_2 = cadItem.DAT_PRCV_ANT_2;
            record.U_DAT_PRCV_MAN_2 = cadItem.DAT_PRCV_MAN_2;
            record.U_PRC_VEN_3 = cadItem.PRC_VEN_3;
            record.U_PRCV_ANT_3 = cadItem.PRCV_ANT_3;
            record.U_PRCV_MAN_3 = cadItem.PRCV_MAN_3;
            record.U_MRG_LUCRO_3 = cadItem.MRG_LUCRO_3;
            record.U_DSC_MAX_3 = cadItem.DSC_MAX_3;
            record.U_COMISSAO_3 = cadItem.COMISSAO_3;
            record.U_DAT_PRC_VEN_3 = cadItem.DAT_PRC_VEN_3;
            record.U_DAT_PRCV_ANT_3 = cadItem.DAT_PRCV_ANT_3;
            record.U_DAT_PRCV_MAN_3 = cadItem.DAT_PRCV_MAN_3;
            record.U_PRC_VEN_4 = cadItem.PRC_VEN_4;
            record.U_PRCV_ANT_4 = cadItem.PRCV_ANT_4;
            record.U_PRCV_MAN_4 = cadItem.PRCV_MAN_4;
            record.U_MRG_LUCRO_4 = cadItem.MRG_LUCRO_4;
            record.U_DSC_MAX_4 = cadItem.DSC_MAX_4;
            record.U_COMISSAO_4 = cadItem.COMISSAO_4;
            record.U_DAT_PRC_VEN_4 = cadItem.DAT_PRC_VEN_4;
            record.U_DAT_PRCV_ANT_4 = cadItem.DAT_PRCV_ANT_4;
            record.U_DAT_PRCV_MAN_4 = cadItem.DAT_PRCV_MAN_4;
            record.U_PRC_VEN_5 = cadItem.PRC_VEN_5;
            record.U_PRCV_ANT_5 = cadItem.PRCV_ANT_5;
            record.U_PRCV_MAN_5 = cadItem.PRCV_MAN_5;
            record.U_MRG_LUCRO_5 = cadItem.MRG_LUCRO_5;
            record.U_DSC_MAX_5 = cadItem.DSC_MAX_5;
            record.U_COMISSAO_5 = cadItem.COMISSAO_5;
            record.U_DAT_PRC_VEN_5 = cadItem.DAT_PRC_VEN_5;
            record.U_DAT_PRCV_ANT_5 = cadItem.DAT_PRCV_ANT_5;
            record.U_DAT_PRCV_MAN_5 = cadItem.DAT_PRCV_MAN_5;
            record.U_TIP_OFT_1 = cadItem.TIP_OFT_1;
            record.U_TIP_OFT_ANT_1 = cadItem.TIP_OFT_ANT_1;
            record.U_INI_OFT_1 = cadItem.INI_OFT_1;
            record.U_INI_OFT_ANT_1 = cadItem.INI_OFT_ANT_1;
            record.U_FIM_OFT_1 = cadItem.FIM_OFT_1;
            record.U_FIM_OFT_ANT_1 = cadItem.FIM_OFT_ANT_1;
            record.U_PRC_OFT_1 = cadItem.PRC_OFT_1;
            record.U_PRC_OFT_ANT_1 = cadItem.PRC_OFT_ANT_1;
            record.U_LIM_OFT_1 = cadItem.LIM_OFT_1;
            record.U_LIM_OFT_ANT_1 = cadItem.LIM_OFT_ANT_1;
            record.U_SAL_OFT_1 = cadItem.SAL_OFT_1;
            record.U_SAL_OFT_ANT_1 = cadItem.SAL_OFT_ANT_1;
            record.U_TIP_OFT_2 = cadItem.TIP_OFT_2;
            record.U_TIP_OFT_ANT_2 = cadItem.TIP_OFT_ANT_2;
            record.U_INI_OFT_2 = cadItem.INI_OFT_2;
            record.U_INI_OFT_ANT_2 = cadItem.INI_OFT_ANT_2;
            record.U_FIM_OFT_2 = cadItem.FIM_OFT_2;
            record.U_FIM_OFT_ANT_2 = cadItem.FIM_OFT_ANT_2;
            record.U_PRC_OFT_2 = cadItem.PRC_OFT_2;
            record.U_PRC_OFT_ANT_2 = cadItem.PRC_OFT_ANT_2;
            record.U_LIM_OFT_2 = cadItem.LIM_OFT_2;
            record.U_LIM_OFT_ANT_2 = cadItem.LIM_OFT_ANT_2;
            record.U_SAL_OFT_2 = cadItem.SAL_OFT_2;
            record.U_SAL_OFT_ANT_2 = cadItem.SAL_OFT_ANT_2;
            record.U_TIP_OFT_3 = cadItem.TIP_OFT_3;
            record.U_TIP_OFT_ANT_3 = cadItem.TIP_OFT_ANT_3;
            record.U_INI_OFT_3 = cadItem.INI_OFT_3;
            record.U_INI_OFT_ANT_3 = cadItem.INI_OFT_ANT_3;
            record.U_FIM_OFT_3 = cadItem.FIM_OFT_3;
            record.U_FIM_OFT_ANT_3 = cadItem.FIM_OFT_ANT_3;
            record.U_PRC_OFT_3 = cadItem.PRC_OFT_3;
            record.U_PRC_OFT_ANT_3 = cadItem.PRC_OFT_ANT_3;
            record.U_LIM_OFT_3 = cadItem.LIM_OFT_3;
            record.U_LIM_OFT_ANT_3 = cadItem.LIM_OFT_ANT_3;
            record.U_SAL_OFT_3 = cadItem.SAL_OFT_3;
            record.U_SAL_OFT_ANT_3 = cadItem.SAL_OFT_ANT_3;
            record.U_TIP_OFT_4 = cadItem.TIP_OFT_4;
            record.U_TIP_OFT_ANT_4 = cadItem.TIP_OFT_ANT_4;
            record.U_INI_OFT_4 = cadItem.INI_OFT_4;
            record.U_INI_OFT_ANT_4 = cadItem.INI_OFT_ANT_4;
            record.U_FIM_OFT_4 = cadItem.FIM_OFT_4;
            record.U_FIM_OFT_ANT_4 = cadItem.FIM_OFT_ANT_4;
            record.U_PRC_OFT_4 = cadItem.PRC_OFT_4;
            record.U_PRC_OFT_ANT_4 = cadItem.PRC_OFT_ANT_4;
            record.U_LIM_OFT_4 = cadItem.LIM_OFT_4;
            record.U_LIM_OFT_ANT_4 = cadItem.LIM_OFT_ANT_4;
            record.U_SAL_OFT_4 = cadItem.SAL_OFT_4;
            record.U_SAL_OFT_ANT_4 = cadItem.SAL_OFT_ANT_4;
            record.U_TIP_OFT_5 = cadItem.TIP_OFT_5;
            record.U_TIP_OFT_ANT_5 = cadItem.TIP_OFT_ANT_5;
            record.U_INI_OFT_5 = cadItem.INI_OFT_5;
            record.U_INI_OFT_ANT_5 = cadItem.INI_OFT_ANT_5;
            record.U_FIM_OFT_5 = cadItem.FIM_OFT_5;
            record.U_FIM_OFT_ANT_5 = cadItem.FIM_OFT_ANT_5;
            record.U_PRC_OFT_5 = cadItem.PRC_OFT_5;
            record.U_PRC_OFT_ANT_5 = cadItem.PRC_OFT_ANT_5;
            record.U_LIM_OFT_5 = cadItem.LIM_OFT_5;
            record.U_LIM_OFT_ANT_5 = cadItem.LIM_OFT_ANT_5;
            record.U_SAL_OFT_5 = cadItem.SAL_OFT_5;
            record.U_SAL_OFT_ANT_5 = cadItem.SAL_OFT_ANT_5;
            record.U_INI_BONUS = cadItem.INI_BONUS;
            record.U_INI_BONUS_ANT = cadItem.INI_BONUS_ANT;
            record.U_FIM_BONUS = cadItem.FIM_BONUS;
            record.U_PRES_ENT = cadItem.PRES_ENT;
            record.U_PRC_BONUS = cadItem.PRC_BONUS;
            record.U_PRES_REP = cadItem.PRES_REP;
            record.U_ESTQ_ATUAL = cadItem.ESTQ_ATUAL;
            record.U_ESTQ_DP = cadItem.ESTQ_DP;
            record.U_ESTQ_LJ = cadItem.ESTQ_LJ;
            record.U_QDE_PEND = cadItem.QDE_PEND;
            record.U_CUS_INV = cadItem.CUS_INV;
            record.U_ESTQ_PADRAO = cadItem.ESTQ_PADRAO;
            record.U_SAI_MED_CAL = cadItem.SAI_MED_CAL;
            record.U_TAMANHO = cadItem.TAMANHO;
            record.U_SAI_ACM_UN = cadItem.SAI_ACM_UN;
            record.U_SAI_ACM_CUS = cadItem.SAI_ACM_CUS;
            record.U_SAI_ACM_VEN = cadItem.SAI_ACM_VEN;
            record.U_CUS_ULT_ENT_BRU = cadItem.CUS_ULT_ENT_BRU;
            record.U_ENT_ACM_UN = cadItem.ENT_ACM_UN;
            record.U_ENT_ACM_CUS = cadItem.ENT_ACM_CUS;
            record.U_DAT_ULT_FAT = cadItem.DAT_ULT_FAT;
            record.U_ULT_QDE_FAT = cadItem.ULT_QDE_FAT;
            record.U_ULT_QDE_ENT = cadItem.ULT_QDE_ENT;
            record.U_CUS_ULT_ENT = cadItem.CUS_ULT_ENT;
            record.U_DAT_ULT_ENT = cadItem.DAT_ULT_ENT;
            record.U_ABC_F = cadItem.ABC_F;
            record.U_ABC_S = cadItem.ABC_S;
            record.U_ABC_T = cadItem.ABC_T;
            record.U_PERECIVEL = cadItem.PERECIVEL;
            record.U_PRZ_VALIDADE = cadItem.PRZ_VALIDADE;
            record.U_TOT_PEDIDO = cadItem.TOT_PEDIDO;
            record.U_TOT_FALTA = cadItem.TOT_FALTA;
            record.U_COD_VAS = cadItem.COD_VAS;
            record.U_DIG_VAS = cadItem.DIG_VAS;
            record.U_COD_ENG = cadItem.COD_ENG;
            record.U_DIG_ENG = cadItem.DIG_ENG;
            record.U_VALOR_IPI = cadItem.VALOR_IPI;
            record.U_VALOR_IPI_ANT = cadItem.VALOR_IPI_ANT;
            record.U_VALOR_IPI_MAN = cadItem.VALOR_IPI_MAN;
            record.U_CLASS_FIS = cadItem.CLASS_FIS;
            record.U_QTD_AUT_PDV = cadItem.QTD_AUT_PDV;
            record.U_MOEDA_VDA = cadItem.MOEDA_VDA;
            record.U_BONI_MERC = cadItem.BONI_MERC;
            record.U_BONI_MERC_ANT = cadItem.BONI_MERC_ANT;
            record.U_BONI_MERC_MAN = cadItem.BONI_MERC_MAN;
            record.U_GRADE = cadItem.GRADE;
            record.U_MENS_AUX = cadItem.MENS_AUX;
            record.U_PROCEDENCIA = cadItem.PROCEDENCIA;
            record.U_CATEGORIA_ANT = cadItem.CATEGORIA_ANT;
            record.U_ESTRATEGIA_REP = cadItem.ESTRATEGIA_REP;
            record.U_DESP_ACES_ISEN = cadItem.DESP_ACES_ISEN;
            record.U_DESP_ACES_ISEN_ANT = cadItem.DESP_ACES_ISEN_ANT;
            record.U_DESP_ACES_ISEN_MAN = cadItem.DESP_ACES_ISEN_MAN;
            record.U_FRETE_VALOR = cadItem.FRETE_VALOR;
            record.U_FRETE_VALOR_ANT = cadItem.FRETE_VALOR_ANT;
            record.U_FRETE_VALOR_MAN = cadItem.FRETE_VALOR_MAN;
            record.U_BONIF_PER = cadItem.BONIF_PER;
            record.U_BONIF_PER_ANT = cadItem.BONIF_PER_ANT;
            record.U_BONIF_PER_MAN = cadItem.BONIF_PER_MAN;
            record.U_VASILHAME = cadItem.VASILHAME;
            record.U_PERMITE_DESC = cadItem.PERMITE_DESC;
            record.U_QTD_OBRIG = cadItem.QTD_OBRIG;
            record.U_ENVIA_PDV = cadItem.ENVIA_PDV;
            record.U_ENVIA_BALANCA = cadItem.ENVIA_BALANCA;
            record.U_PESADO_PDV = cadItem.PESADO_PDV;
            record.U_JPMA_FLAG1 = cadItem.JPMA_FLAG1;
            record.U_JPMA_FLAG2 = cadItem.JPMA_FLAG2;
            record.U_JPMA_FLAG3 = cadItem.JPMA_FLAG3;
            record.U_LINHA_ANTERIOR = cadItem.LINHA_ANTERIOR;
            record.U_LINHA_VALIDA = cadItem.LINHA_VALIDA;
            record.U_QUANT_EAN = cadItem.QUANT_EAN;
            record.U_TOT_ESTOCADO = cadItem.TOT_ESTOCADO;
            record.U_EMPIL_MAX = cadItem.EMPIL_MAX;
            record.U_TIPO_END = cadItem.TIPO_END;
            record.U_SAZONAL = cadItem.SAZONAL;
            record.U_MARCA_PROP = cadItem.MARCA_PROP;
            record.U_FILLER = cadItem.FILLER;
            record.U_LASTUPDATE = cadItem.LASTUPDATE;


            result = JsonConvert.SerializeObject(record);

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

        private CadItem toRecord(dynamic record)
        {
            CadItem cadItem = new CadItem();
            //invoice.RecId = Guid.Parse(record.Code);
            //invoice.invoiceDirection = (Invoice.InvoiceDirection)record.U_INVOICEDIRECTION;
            //invoice.invoiceId = record.U_INVOICEID;
            //invoice.invoiceSeries = record.U_INVOICESERIES;
            //invoice.invoiceDate = parseDate(record.U_INVOICEDATE);
            //invoice.issueDate = parseDate(record.U_ISSUEDATE);
            //invoice.recipientId = record.U_RECIPIENTID;
            //invoice.taxCodeDetermination = record.U_DETERMINATION;
            //invoice.cfop = (int)record.U_CFOP;
            //invoice.lastUpdate = parseDate(record.U_LASTUPDATE);
            //invoice.status = (Invoice.InvoiceIntegrationStatus)record.U_STATUS;

            return cadItem;
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
