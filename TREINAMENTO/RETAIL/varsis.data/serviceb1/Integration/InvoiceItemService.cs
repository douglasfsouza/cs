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
    public class InvoiceItemService : IEntityService<Model.Integration.InvoiceItem>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public InvoiceItemService(ServiceLayerConnector serviceLayerConnector)
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
            
            table.name = "VSISINTINVOHI";
            table.description = "ITEM - Nota fiscal Importação";
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

        public Task Delete(InvoiceItem entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotImplementedException();
        }

        async public Task<InvoiceItem> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"U_VSISINTINVOH('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            InvoiceItem invoiceItem = toRecord(record);

            // Recupera as linhas da nota iscal
            string[] filter = new string[]
            {
                $"U_INVOICECODE eq '{recid}'"
            };

            query = Global.MakeODataQuery("U_VSISINTINVOI", null, filter);

            data = await _serviceLayerConnector.getQueryResult(query);

            return invoiceItem;
        }

        async public Task Insert(InvoiceItem entity)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();

            entity.status = Data.Model.Integration.InvoiceItem.InvoiceItemIntegrationStatus.Importing;
            string record = toJson(entity);
            batch.Post(HttpMethod.Post, "/U_VSISINTINVOHI", record);

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
                entity.status = InvoiceItem.InvoiceItemIntegrationStatus.Created;
                record = toJson(entity);
                response = await _serviceLayerConnector.Patch($"U_VSISINTINVOH('{entity.RecId}')", record, true);

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

        public Task Insert(List<InvoiceItem> entities)
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
        async public Task<List<InvoiceItem>> List(List<Criteria> criterias, long page, long size)
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

            List<InvoiceItem> result = new List<InvoiceItem>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        public Task Update(InvoiceItem entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<InvoiceItem> entities)
        {
            throw new NotImplementedException();
        }

        private List<TableColumn> createColumns()
        {
            List<TableColumn> lista = new List<TableColumn>();


            lista.Add(new TableColumn()
            {
                name = "IDNOTA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "SEQUENCIAL",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "ITEM",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "EAN",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "AGENDA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "CRF",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "CFOP",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "FIGURA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "CODSECAO",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TIPOEMBALAGEM",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BASEEMBALAGEM",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "QUANTIUNIT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORUNIT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "CUSTOUNIT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORCONTABIL",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORMERCADORIA",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TRIBUTACAOICMS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "OPERACAOICMS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "CSTICMS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "BASEICMS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQUOTAICMS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORICMS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "REDUCAOICMS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ISENTOICMS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "NAOTRIBICMS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "OUTROSICMS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "FRONTEIRA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQSUBSTTRIB",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "VALORPVV",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQUOTAICMF",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORICMF",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORPAUTA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "CSTIPI",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "BASEIPI",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQUOTAIPI",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "VALORIPI",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORFECOP",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "CSTPIS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "SITUACAOPIS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "CATEGORIAPIS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORCONTABILPIS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "BASEPIS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQUOTAPIS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "VALORPIS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "CSTCOFINS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "SITUACAOCOFINS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "CATEGORIACOFINS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORCONTABILCOFINS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BASECOFINS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQUOTACOFINS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORCOFINS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "TIPOCONTRIB",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "NATRECEITA",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "BASEIRRF",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORIRRF",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQUOTAIRRF",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "BASEINSS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORINSS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQUOTAINSS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "BASEISS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "VALORISS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQUOTAISS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "BASECSLL",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORCSLL",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQUOTACSLL",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BASEPISRET",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORPIRET",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQUOTARET",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BASECOFINSRET",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORCORET",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQUOTACOFRET",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORFRETE",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORSEGURO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORDODESCONTO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORACRESCIMO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORDESPACESS",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "SITUACAO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });



            lista.Add(new TableColumn()
            {
                name = "OPERACAO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BASEICMSRETANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORICMSRETANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQUOTAICMSRETANT",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BASEICMSDESO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORICMSDESO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQUOTAICMSDESO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BASEICMSRESS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORICMSRESS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQUOTAICMSRESS",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BASEICMSEFETIVO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORICMSEFETIVO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQUOTAICMSEFETIVO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "REDUCAOBASEICMSEFETIVO",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BASEDIFALIQUOTAORG",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORDIFALIQUOTAORG",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQDIFALIQUOTAORG",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BASEDIFALIQUOTADST",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORDIFALIQUOTADST",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQDIFALIQUOTADST",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BASEFECOP",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQUOTAFECOP",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BASEFECOPST",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORFECOPST",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQUOTAFECOPST",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "BASEFECOPRET",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "VALORFECOPRET",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "ALIQUOTAFECOPRET",
                dataType = "db_Numeric",
                mandatory = false,
                size = 11,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "NUMERODLOTE",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DTVALLOTE",
                dataType = "db_Numeric",
                mandatory = false,
                size = 7,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "DTFABLOTE",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "CLASSIFLOTE",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "CODIGOAGGLOTE",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });


            //nome maior que 8 caracteres
            lista.Add(new TableColumn()
            {
                name = "INFOADICIONAL",
                dataType = "db_Alpha",
                mandatory = false,
                size = 20,
                description = ""
            });

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

        private string toJson(InvoiceItem invoiceItem)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();
            record.Code = invoiceItem.RecId.ToString();
            record.Name = invoiceItem.RecId.ToString();
            record.U_IDNOTA = invoiceItem.IDNOTA;
            record.U_SEQUENCIAL = invoiceItem.SEQUENCIAL;
            record.U_ITEM = invoiceItem.ITEM;
            record.U_EAN = invoiceItem.EAN;
            record.U_AGENDA = invoiceItem.AGENDA;
            record.U_CRF = invoiceItem.CRF;
            record.U_CFOP = invoiceItem.CFOP;
            record.U_FIGURA = invoiceItem.FIGURA;
            record.U_CODSECAO = invoiceItem.CODSECAO;
            record.U_TIPOEMBALAGEM = invoiceItem.TIPOEMBALAGEM;
            record.U_BASEEMBALAGEM = invoiceItem.BASEEMBALAGEM;
            record.U_QUANTIUNIT = invoiceItem.QUANTIUNIT;
            record.U_VALORUNIT = invoiceItem.VALORUNIT;
            record.U_CUSTOUNIT = invoiceItem.CUSTOUNIT;
            record.U_VALORCONTABIL = invoiceItem.VALORCONTABIL;
            record.U_VALORMERCADORIA = invoiceItem.VALORMERCADORIA;
            record.U_TRIBUTACAOICMS = invoiceItem.TRIBUTACAOICMS;
            record.U_OPERACAOICMS = invoiceItem.OPERACAOICMS;
            record.U_CSTICMS = invoiceItem.CSTICMS;
            record.U_BASEICMS = invoiceItem.BASEICMS;
            record.U_ALIQUOTAICMS = invoiceItem.ALIQUOTAICMS;
            record.U_VALORICMS = invoiceItem.VALORICMS;
            record.U_REDUCAOICMS = invoiceItem.REDUCAOICMS;
            record.U_ISENTOICMS = invoiceItem.ISENTOICMS;
            record.U_NAOTRIBICMS = invoiceItem.NAOTRIBICMS;
            record.U_OUTROSICMS = invoiceItem.OUTROSICMS;
            record.U_FRONTEIRA = invoiceItem.FRONTEIRA;
            record.U_ALIQSUBSTTRIB = invoiceItem.ALIQSUBSTTRIB;
            record.U_VALORPVV = invoiceItem.VALORPVV;
            record.U_ALIQUOTAICMF = invoiceItem.ALIQUOTAICMF;
            record.U_VALORICMF = invoiceItem.VALORICMF;
            record.U_VALORPAUTA = invoiceItem.VALORPAUTA;
            record.U_CSTIPI = invoiceItem.CSTIPI;
            record.U_BASEIPI = invoiceItem.BASEIPI;
            record.U_ALIQUOTAIPI = invoiceItem.ALIQUOTAIPI;
            record.U_VALORIPI = invoiceItem.VALORIPI;
            record.U_VALORFECOP = invoiceItem.VALORFECOP;
            record.U_CSTPIS = invoiceItem.CSTPIS;
            record.U_SITUACAOPIS = invoiceItem.SITUACAOPIS;
            record.U_CATEGORIAPIS = invoiceItem.CATEGORIAPIS;
            record.U_VALORCONTABILPIS = invoiceItem.VALORCONTABILPIS;
            record.U_BASEPIS = invoiceItem.BASEPIS;
            record.U_ALIQUOTAPIS = invoiceItem.ALIQUOTAPIS;
            record.U_VALORPIS = invoiceItem.VALORPIS;
            record.U_CSTCOFINS = invoiceItem.CSTCOFINS;
            record.U_SITUACAOCOFINS = invoiceItem.SITUACAOCOFINS;
            record.U_CATEGORIACOFINS = invoiceItem.CATEGORIACOFINS;
            record.U_VALORCONTABILCOFINS = invoiceItem.VALORCONTABILCOFINS;
            record.U_BASECOFINS = invoiceItem.BASECOFINS;
            record.U_ALIQUOTACOFINS = invoiceItem.ALIQUOTACOFINS;
            record.U_VALORCOFINS = invoiceItem.VALORCOFINS;
            record.U_TIPOCONTRIB = invoiceItem.TIPOCONTRIB;
            record.U_NATRECEITA = invoiceItem.NATRECEITA;
            record.U_BASEIRRF = invoiceItem.BASEIRRF;
            record.U_VALORIRRF = invoiceItem.VALORIRRF;
            record.U_ALIQUOTAIRRF = invoiceItem.ALIQUOTAIRRF;
            record.U_BASEINSS = invoiceItem.BASEINSS;
            record.U_VALORINSS = invoiceItem.VALORINSS;
            record.U_ALIQUOTAINSS = invoiceItem.ALIQUOTAINSS;
            record.U_BASEISS = invoiceItem.BASEISS;
            record.U_VALORISS = invoiceItem.VALORISS;
            record.U_ALIQUOTAISS = invoiceItem.ALIQUOTAISS;
            record.U_BASECSLL = invoiceItem.BASECSLL;
            record.U_VALORCSLL = invoiceItem.VALORCSLL;
            record.U_ALIQUOTACSLL = invoiceItem.ALIQUOTACSLL;
            record.U_BASEPISRET = invoiceItem.BASEPISRET;
            record.U_VALORPIRET = invoiceItem.VALORPIRET;
            record.U_ALIQUOTARET = invoiceItem.ALIQUOTARET;
            record.U_BASECOFINSRET = invoiceItem.BASECOFINSRET;
            record.U_VALORCORET = invoiceItem.VALORCORET;
            record.U_ALIQUOTACOFRET = invoiceItem.ALIQUOTACOFRET;
            record.U_VALORFRETE = invoiceItem.VALORFRETE;
            record.U_VALORSEGURO = invoiceItem.VALORSEGURO;
            record.U_VALORDODESCONTO = invoiceItem.VALORDODESCONTO;
            record.U_VALORACRESCIMO = invoiceItem.VALORACRESCIMO;
            record.U_VALORDESPACESS = invoiceItem.VALORDESPACESS;
            record.U_SITUACAO = invoiceItem.SITUACAO;
            record.U_OPERACAO = invoiceItem.OPERACAO;
            record.U_BASEICMSRETANT = invoiceItem.BASEICMSRETANT;
            record.U_VALORICMSRETANT = invoiceItem.VALORICMSRETANT;
            record.U_ALIQUOTAICMSRETANT = invoiceItem.ALIQUOTAICMSRETANT;
            record.U_BASEICMSDESO = invoiceItem.BASEICMSDESO;
            record.U_VALORICMSDESO = invoiceItem.VALORICMSDESO;
            record.U_ALIQUOTAICMSDESO = invoiceItem.ALIQUOTAICMSDESO;
            record.U_BASEICMSRESS = invoiceItem.BASEICMSRESS;
            record.U_VALORICMSRESS = invoiceItem.VALORICMSRESS;
            record.U_ALIQUOTAICMSRESS = invoiceItem.ALIQUOTAICMSRESS;
            record.U_BASEICMSEFETIVO = invoiceItem.BASEICMSEFETIVO;
            record.U_VALORICMSEFETIVO = invoiceItem.VALORICMSEFETIVO;
            record.U_ALIQUOTAICMSEFETIVO = invoiceItem.ALIQUOTAICMSEFETIVO;
            record.U_REDUCAOBASEICMSEFETIVO = invoiceItem.REDUCAOBASEICMSEFETIVO;
            record.U_BASEDIFALIQUOTAORG = invoiceItem.BASEDIFALIQUOTAORG;
            record.U_VALORDIFALIQUOTAORG = invoiceItem.VALORDIFALIQUOTAORG;
            record.U_ALIQDIFALIQUOTAORG = invoiceItem.ALIQDIFALIQUOTAORG;
            record.U_BASEDIFALIQUOTADST = invoiceItem.BASEDIFALIQUOTADST;
            record.U_VALORDIFALIQUOTADST = invoiceItem.VALORDIFALIQUOTADST;
            record.U_ALIQDIFALIQUOTADST = invoiceItem.ALIQDIFALIQUOTADST;
            record.U_BASEFECOP = invoiceItem.BASEFECOP;
            record.U_ALIQUOTAFECOP = invoiceItem.ALIQUOTAFECOP;
            record.U_BASEFECOPST = invoiceItem.BASEFECOPST;
            record.U_VALORFECOPST = invoiceItem.VALORFECOPST;
            record.U_ALIQUOTAFECOPST = invoiceItem.ALIQUOTAFECOPST;
            record.U_BASEFECOPRET = invoiceItem.BASEFECOPRET;
            record.U_VALORFECOPRET = invoiceItem.VALORFECOPRET;
            record.U_ALIQUOTAFECOPRET = invoiceItem.ALIQUOTAFECOPRET;
            record.U_NUMERODLOTE = invoiceItem.NUMERODLOTE;
            record.U_DTVALLOTE = invoiceItem.DTVALLOTE;
            record.U_DTFABLOTE = invoiceItem.DTFABLOTE;
            record.U_CLASSIFLOTE = invoiceItem.CLASSIFLOTE;
            record.U_CODIGOAGGLOTE = invoiceItem.CODIGOAGGLOTE;
            record.U_INFOADICIONAL = invoiceItem.INFOADICIONAL;
            record.U_LASTUPDATE = invoiceItem.LASTUPDATE;


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

        private InvoiceItem toRecord(dynamic record)
        {
            InvoiceItem cadEntidade = new InvoiceItem();
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
