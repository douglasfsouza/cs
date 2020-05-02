using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model.Connector;
using System.Linq;

namespace Varsis.Data.Serviceb1.Connector
{
    public class SalesInvoiceService : IEntityService<SalesInvoice>, IEntityServiceWithReturn<SalesInvoice>
    {
        const string SL_TABLE_NAME = "Invoices";

        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public SalesInvoiceService(ServiceLayerConnector serviceLayerConnector)
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

            table.name = "VSITMMCONFIG";
            table.description = "Midia+ - Configuração";
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

        private List<TableColumn> createColumns()
        {
            List<TableColumn> lista = new List<TableColumn>();

            lista.Add(new TableColumn()
            {
                name = "NATUREZA",
                dataType = "db_Alpha",
                size = 2,
                mandatory = true,
                description = "Natureza"
            });

            lista.Add(new TableColumn()
            {
                name = "TIPOPESSOA",
                dataType = "db_Alpha",
                size = 2,
                mandatory = true,
                description = "Tipo Pessoa"
            });

            lista.Add(new TableColumn()
            {
                name = "TIPOORDEMFAT",
                dataType = "db_Alpha",
                size = 2,
                mandatory = false,
                description = "Tipo Ordem Faturamento"
            });

            lista.Add(new TableColumn()
            {
                name = "USAGE",
                dataType = "db_Numeric",
                size = 10,
                mandatory = true,
                description = "Utilização"
            });

            lista.Add(new TableColumn()
            {
                name = "USAGE",
                dataType = "db_Numeric",
                size = 10,
                mandatory = true,
                description = "Utilização"
            });

            lista.Add(new TableColumn()
            {
                name = "TIPOTRIB",
                dataType = "db_Alpha",
                size = 1,
                mandatory = true,
                description = "Tipo de tributação"
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
                keys = new string[] { "NATUREZA", "TIPOPESSOA", "TIPOORDEMFAT" }
            });

            return lista;
        }


        public Task Delete(SalesInvoice entity)
        {
            throw new NotSupportedException();
        }

        public Task Delete(List<Criteria> criterias)
        {
            throw new NotSupportedException();
        }

        public Task<SalesInvoice> Find(List<Criteria> criterias)
        {
            throw new NotSupportedException();
        }

        async public Task Insert(SalesInvoice entity)
        {
            string record = await toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Post(SL_TABLE_NAME, record);

            if (!response.success)
            {
                string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                throw new ApplicationException(message);
            }
        }

        public Task Insert(List<SalesInvoice> entities)
        {
            throw new NotSupportedException();
        }
        async Task<SalesInvoice> IEntityServiceWithReturn<SalesInvoice>.Insert(SalesInvoice entity)
        {
            string record = await toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Post(SL_TABLE_NAME, record);

            if (!response.success)
            {
                string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
                throw new ApplicationException(message);
            }

            dynamic responseRecord = Global.parseQueryToObject(response.data);

            var copy = JsonConvert.SerializeObject(entity);
            SalesInvoice responseEntity = JsonConvert.DeserializeObject<SalesInvoice>(copy);
            responseEntity.Response = new SalesInvoiceResponse()
            {
                NumeroNotaFiscal = responseRecord.SequenceSerial,
                SerieNotaFiscal = responseRecord.SeriesString
            };

            return responseEntity;
        }

        Task<List<SalesInvoice>> IEntityServiceWithReturn<SalesInvoice>.Insert(List<SalesInvoice> entities)
        {
            throw new NotSupportedException();
        }

        async public Task<Varsis.Data.Infrastructure.Pagination> TotalLinhas(long? size, List<Criteria> criterias)
        {
            return new Varsis.Data.Infrastructure.Pagination();
        }

        async public Task<List<SalesInvoice>> List(List<Criteria> criterias, long page, long size)
        {
            var filter = parseCriterias(criterias);

            string query = Global.MakeODataQuery(SL_TABLE_NAME, null, filter.Length == 0 ? null : filter, null, page, size);

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<SalesInvoice> result = new List<SalesInvoice>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        public Task Update(SalesInvoice entity)
        {
            throw new NotSupportedException();
        }

        public Task Update(List<SalesInvoice> entities)
        {
            throw new NotSupportedException();
        }

        async private Task<string> toJson(SalesInvoice entity)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            dynamic config = await FindConfig(entity.Natureza, entity.TipoPessoa, entity.TipoOrdemFat);

            if (config == null)
            {
                throw new ArgumentException($"Configuração não encontrada. (natureza: {entity.Natureza}, tipo pessoa: {entity.TipoPessoa}, tipo ordem faturamento: {entity.TipoOrdemFat}");
            }

            record.DocType = "dDocument_Items";
            record.DocObjectCode = "oInvoices";
            record.CardCode = entity.CodigoCliente;
            record.BPLid = entity.CodigoEmpresa;
            record.DocDate = entity.DataLancamento;
            record.TaxeDate = entity.DataLancamento;
            record.U_SKILL_DTPRE = entity.DataLiberacao;
            record.U_VS_RP = entity.NumeroRP;
            record.U_VS_NEG = entity.NumeroNegociacao;
            record.GroupNum = entity.CodigoCondicaoPagto;
            record.Header = entity.Observacao;
            record.SequenceCode = 31;
            record.U_Skill_TipTrib = config.TipoTributacao;

            record.Model = "46";
            record.U_finNfe = "1";

            record.Comments = "INTEGRAÇÃO MÍDIA+";

            record.DocumentLines = this.makeDocumentLines(entity.Itens, (long)config.Utilizacao);
            record.DocumentInstallments = this.makeDocumentInstallments(entity.Parcelas);

            result = JsonConvert.SerializeObject(record, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return result;
        }

        private List<dynamic> makeDocumentLines(List<SalesInvoiceItem> items, long utilizacao)
        {
            List<dynamic> result = new List<dynamic>();

            items.ForEach(i =>
            {
                dynamic item = new ExpandoObject();

                item.ItemCode = i.CodigoProduto;
                item.Quantity = i.Quantidade;
                item.Price = i.ValorLiquido;
                item.CogsOcrCod = i.VerticalNegocio;
                item.CogsOcrCod2 = i.Programacao;
                item.CogsOcrCod3 = i.Escritorio;
                item.Usage = utilizacao;

                result.Add(item);
            });

            return result;
        }
        private List<dynamic> makeDocumentInstallments(List<SalesInvoiceInstallment> installments)
        {
            List<dynamic> result = new List<dynamic>();

            installments.ForEach(i =>
            {
                dynamic item = new ExpandoObject();

                item.DueDate = i.DataVencimento;
                item.Total = i.ValorParcela;

                result.Add(item);
            });

            return result;
        }

        private SalesInvoice toRecord(dynamic record)
        {
            SalesInvoice entity = new SalesInvoice();

            entity.CodigoCliente = record.CardCode;
            entity.CodigoEmpresa = record.BPLid;
            entity.DataLancamento = Convert.ToDateTime((string)record.DocDate).ToString("yyyy-MM-dd");
            entity.DataLiberacao = Convert.ToDateTime((string)record.U_SKILL_DTPRE).ToString("yyyy-MM-dd");
            entity.NumeroRP = record.U_VS_RP;
            entity.NumeroNegociacao = record.U_VS_NEG;
            entity.CodigoCondicaoPagto = record.GroupNum;

            if (record.DocumentLines != null)
            {
                entity.Itens = new List<SalesInvoiceItem>();

                foreach (var line in record.DocumentoLines)
                {
                    SalesInvoiceItem item = new SalesInvoiceItem();

                    item.CodigoProduto = line.ItemCode;
                    item.Quantidade = Convert.ToDouble(line.Quantity);
                    item.ValorLiquido = Convert.ToDouble(line.Price);
                    item.VerticalNegocio = line.CogsOcrCod;
                    item.Programacao = line.CogsOcrCod2;
                    item.Escritorio = line.CogsOcrCod3;

                    entity.Itens.Add(item);
                }
            }

            if (record.DocumentInstallments != null)
            {
                entity.Parcelas = new List<SalesInvoiceInstallment>();

                foreach (var installment in record.DocumentInstallments)
                {
                    SalesInvoiceInstallment parcela = new SalesInvoiceInstallment();

                    parcela.DataVencimento = Convert.ToDateTime((string)installment.DueDate).ToString("yyyy-MM-dd");
                    parcela.ValorParcela = Convert.ToDouble(installment.Total);

                    entity.Parcelas.Add(parcela);

                    break;
                }
            }

            return entity;
        }

        async private Task<dynamic> FindConfig(string natureza, string tipoPessoa, string tipoOrdemFaturamento)
        {
            dynamic result = null;

            string[] queryArgs = new string[]
            {
                "U_VSITMMCONFIG",
                "?$filter=",
                $"U_NATUREZA eq '{natureza}' and ",
                $"U_TIPOPESSOA eq '{tipoPessoa}' and ",
                (tipoOrdemFaturamento ?? "") == "" ? $"(U_TIPOORDEMFAT eq null or U_TIPOORDEMFAT eq '')" :
                                                     $"U_TIPOORDEMFAT eq '{tipoOrdemFaturamento}'"
            };

            string query = string.Join("", queryArgs);

            string data = await _serviceLayerConnector.getQueryResult(query);
            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            result = new ExpandoObject();

            if (lista != null)
            {
                dynamic row = lista[0];
                result.Utilizacao = Convert.ToInt64(row.U_USAGE);
                result.TipoTributacao = row.U_TIPOTRIB;
            }
            else
            {
                result = null;
            }

            return result;
        }

        private string[] parseCriterias(List<Criteria> criterias)
        {
            List<string> filter = new List<string>();
            if (criterias != null)
            {
                if (criterias?.Count != 0)
                {
                    foreach (var c in criterias)
                    {
                        string type = string.Empty;
                        string field = string.Empty;

                        if (_FieldMap.ContainsKey(c.Field.ToLower()))
                        {
                            field = _FieldMap[c.Field.ToLower()];
                            type = _FieldType[c.Field.ToLower()];
                        }
                        else
                        {
                            field = c.Field;
                            type = "T";
                        }

                        string value = string.Empty;

                        if (type == "T")
                        {
                            value = $"'{c.Value}'";
                        }
                        else if (type == "N")
                        {
                            value = $"{c.Value}";
                        }

                        switch (c.Operator.ToLower())
                        {
                            case "startswith":
                                filter.Add($"startswith({field},{value})");
                                break;

                            default:
                                filter.Add($"{field} {c.Operator.ToLower()} {value}");
                                break;
                        }
                    }
                }
            }

            return filter.ToArray();
        }
        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("numerorp", "U_VS_RP");

            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("numerorp", "T");

            return map;
        }

    }
}
