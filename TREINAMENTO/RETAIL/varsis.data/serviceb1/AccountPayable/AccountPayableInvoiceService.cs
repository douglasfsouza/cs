using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model.AccountPayable;

namespace Varsis.Data.Serviceb1.Integration.AccountPayable
{
    public class AccountPayableInvoiceService : IEntityService<AccountPayableInvoice>
    {
        const string SL_SERVICE_NAME = "U_VSPAGTITULOS";

        readonly ServiceLayerConnector _serviceLayerConnector;
                
        readonly Dictionary<string, string> _FieldMap;
        readonly Dictionary<string, string> _FieldType;

        public AccountPayableInvoiceService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = mountFieldMap();
            _FieldType = mountFieldType();
        }

        public Task<bool> Create()
        {
            throw new NotSupportedException();
        }

        async public Task Delete(AccountPayableInvoice entity)
        {
            string json = toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Delete($"{SL_SERVICE_NAME}('{entity.RecId}')", json);

            if (!response.success)
            {
                string message = $"{entity.EntityName}: Erro ao excluir {entity.NotaFiscal}-{entity.Parcela}, {response.errorCode} {response.errorMessage}";
                throw new OperationCanceledException(message);
            }
        }

        async public Task Delete(List<Criteria> criterias)
        {
            try
            {
                List<AccountPayableInvoice> lista = await this.List(criterias,-1,-1);

                lista!.ForEach(async ap =>
                {
                    await this.Delete(ap);
                });
            }
            catch
            {
                throw;
            }
        }

        async public Task<AccountPayableInvoice> Find(List<Criteria> criterias)
        {
            AccountPayableInvoice result = null;

            try
            {
                List<AccountPayableInvoice> lista = await this.List(criterias,-1,-1);

                if (lista.Count > 0)
                {
                    result = lista[0];
                }
            }
            catch
            {
                throw;
            }

            return result;
        }

        async public Task Insert(AccountPayableInvoice entity)
        {
            string json = toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Post($"{SL_SERVICE_NAME}", json);

            if (!response.success)
            {
                string message = $"{entity.EntityName}: Erro ao incluir {entity.NotaFiscal}-{entity.Parcela}, {response.errorCode} {response.errorMessage}";
                throw new OperationCanceledException(message);
            }
        }

        async public Task Insert(List<AccountPayableInvoice> entities)
        {
            try
            {
                foreach (var e in entities)
                {
                    await this.Insert(e);
                }
            }
            catch
            {
                throw;
            }
        }
        async public Task<Varsis.Data.Infrastructure.Pagination> TotalLinhas(long? size, List<Criteria> criterias)
        {
            return new Varsis.Data.Infrastructure.Pagination();
        }

        async public Task<List<AccountPayableInvoice>> List(List<Criteria> criterias, long page, long size)
        {
            string service = SL_SERVICE_NAME;
            bool filteredByGroup = false;

            var filter = this.parseCriteria(criterias.ToList());

            string query = string.Empty;

            if (criterias.FirstOrDefault(m => m.Field.ToLower() == "vendorgroup") != null)
            {
                filteredByGroup = true;

                service = $"$crossjoin({SL_SERVICE_NAME},U_S3ADVVENDOR)";

                string[] selectListArgs = new string[]
                {
                    "Code,Name,U_TransId,U_LineId,U_Cod_Forne,U_Cod_Forne_Ori,U_Nota,U_Serie,U_Parcela,U_Vencto,U_Vencto_Ori,U_Dt_Recepcao,",
                    "U_Dt_Emissao,U_Dt_Agendamento,U_Dt_Pagto,U_Dt_Liquida,U_Dt_Lancto,U_Dt_Prev_Pagto,U_Conta_Controle,U_Banco,U_Banco_For,",
                    "U_Empresa,U_Filial,U_Bloqueado,U_Usuario_Liberacao,U_Dt_Liberacao,U_Dt_Bloqueio,U_Condicao_Pagto,U_Status,U_Situacao,",
                    "U_Meio_Pagto,U_Forma_Pagto,U_Tipo_Conta,U_Numero_Conta,U_Dig_Conta,U_Dig_Agencia,U_Agencia,U_Dig_Agencia_For,U_Agencia_For,",
                    "U_Dig_Conta_Banc_For,U_Conta_Banc_For,U_Valor_Bruto,U_Abat_Manual,U_Abat_Receber,U_Desconto_Acordo,U_Desconto,U_Acrescimos,",
                    "U_Retencoes,U_Funrural,U_Juros,U_Multa,U_Valor_Pago,U_Codigo_Moeda,U_Valor_Moeda_Cmp,U_Moeda_Pagto,U_Valor_Moeda_Pagto,",
                    "U_Valor_Bruto_Moeda,U_Variacao_Cambial,U_Juros_Moeda,U_Tipcodbar,U_Codbar,U_Code_Pagto,U_Seu_Numero,U_Nosso_Numero,U_Instr_Pagto,",
                    "U_Arquivo_Gerado,U_Arquivo_Importado,U_Usu_Gerou_Arquivo,U_Usu_Importou_Conf,U_Usu_Importou_Liq,U_Dt_geracao,U_Motivo_Baixa,",
                    "U_Numero_Boleto,U_Lote_EDI,U_Code_Antecipa,U_Modelo_NF,U_Conta_Razao,U_Dt_confirma,U_Ocorrencia"
                };

                string selectList = string.Join("", selectListArgs);
                string expand = $"$expand=U_VSPAGTITULOS($select={selectList})";

                query = $"{service}?{expand}&$filter={string.Join(" and ", filter)}";
                query = Uri.EscapeUriString(query);
            }
            else
            {
                query = Global.MakeODataQuery(service, null, filter.Length == 0 ? null : filter);
            }

            var data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<AccountPayableInvoice> result = new List<AccountPayableInvoice>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o, filteredByGroup));
                }
            }

            return result;
        }

        private string[] parseCriteria(List<Criteria> criterias)
        {
            var sliceCriteria = criterias.FirstOrDefault(m => m.Field.ToLower() == "slice");
            var vendorGroupCriteria = criterias.FirstOrDefault(m => m.Field.ToLower() == "vendorgroup");

            if (sliceCriteria != null)
            {
                criterias.Remove(sliceCriteria);
            }

            if (vendorGroupCriteria != null)
            {
                criterias.Remove(vendorGroupCriteria);
            }

            var filter = Global.parseCriterias(criterias, _FieldMap, _FieldType).ToList();

            if (sliceCriteria != null)
            {
                switch (sliceCriteria.Value.ToLower())
                {
                    case "open":
                        filter.Add($"(U_Dt_Pagto eq null and U_Vencto ge '{DateTime.Now.ToString("yyyy-MM-dd")}')");
                        break;
                    case "overdue":
                        filter.Add($"(U_Dt_Pagto eq null and U_Vencto lt '{DateTime.Now.ToString("yyyy-MM-dd")}')");
                        break;
                    case "closed":
                        filter.Add($"(U_Status eq 'L' or U_Status eq 'C' or U_Dt_Pagto ne null)");
                        break;
                    case "schedule":
                        filter.Add($"(U_Status eq 'A')");
                        break;
                    case "all":
                        break;
                    default:
                        break;
                }
            }

            if (vendorGroupCriteria != null)
            {
                filter.Add($"(U_VSPAGTITULOS/U_Cod_Forne eq U_S3ADVVENDOR/U_CARDCODE and U_S3ADVVENDOR/U_GROUPID eq '{vendorGroupCriteria.Value}')");

            }

            return filter.ToArray();
        }

        async private Task<List<dynamic>> listJoinBusinessPartners(string[] filter, bool filteredByGroup = false)
        {
            string service = $"$crossjoin({SL_SERVICE_NAME},BusinessPartners)";
            string filteredJoin = string.Empty;

            if (filteredByGroup)
            {
                service = $"$crossjoin({SL_SERVICE_NAME},BusinessPartners,U_S3ADVVENDOR)";
            }

            string[] queryArgs = new string[]
            {
                service,
                "?$expand=",
                "BusinessPartners($select=CardCode,CardName)",
                "&$filter=BusinessPartners/CardCode eq U_VSPAGTITULOS/U_Cod_Forne",
                filter.Length == 0 ? string.Empty : " and ",
                filter.Length == 0 ? string.Empty : string.Join(" and ", filter),
                "&$apply=groupby((BusinessPartners/CardCode,BusinessPartners/CardName))"
            };

            string query = string.Join("", queryArgs);

            string data = await _serviceLayerConnector.getQueryResult(query);
            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<dynamic> result = new List<dynamic>();

            lista?.ForEach((l =>
            {
                var i = (dynamic)l;

                dynamic newItem = new ExpandoObject();
                newItem.Code = i.BusinessPartners.CardCode;
                newItem.Name = i.BusinessPartners.CardName;

                result.Add(newItem);
            }));

            return result;
        }

        async private Task<List<dynamic>> listJoinPaymentTermsTypes(string[] filter, bool filteredByGroup = false)
        {
            string service = $"$crossjoin({SL_SERVICE_NAME},PaymentTermsTypes)";
            string filteredJoin = string.Empty;

            if (filteredByGroup)
            {
                service = $"$crossjoin({SL_SERVICE_NAME},PaymentTermsTypes,U_S3ADVVENDOR)";
            }

            string[] queryArgs = new string[]
            {
                service,
                "?$expand=",
                "PaymentTermsTypes($select=GroupNumber,PaymentTermsGroupName)",
                "&$filter=PaymentTermsTypes/GroupNumber eq U_VSPAGTITULOS/U_Condicao_Pagto",
                filter.Length == 0 ? string.Empty : " and ",
                filter.Length == 0 ? string.Empty : string.Join(" and ", filter),
                "&$apply=groupby((PaymentTermsTypes/GroupNumber,PaymentTermsTypes/PaymentTermsGroupName))"
            };

            string query = string.Join("", queryArgs);

            string data = await _serviceLayerConnector.getQueryResult(query);
            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<dynamic> result = new List<dynamic>();

            lista?.ForEach((l =>
            {
                var i = (dynamic)l;

                dynamic newItem = new ExpandoObject();
                newItem.Code = i.PaymentTermsTypes.GroupNumber;
                newItem.Name = i.PaymentTermsTypes.PaymentTermsGroupName;

                result.Add(newItem);
            }));

            return result;
        }

        async private Task<List<dynamic>> listJoinBusinessPlaces(string[] filter, bool filteredByGroup = false)
        {
            string service = $"$crossjoin({SL_SERVICE_NAME},BusinessPlaces)";
            string filteredJoin = string.Empty;

            if (filteredByGroup)
            {
                service = $"$crossjoin({SL_SERVICE_NAME},BusinessPlaces,U_S3ADVVENDOR)";
            }

            string[] queryArgs = new string[]
            {
                service,
                "?$expand=",
                "BusinessPlaces($select=BPLID,BPLName)",
                "&$filter=BusinessPlaces/BPLID eq U_VSPAGTITULOS/U_Filial",
                filter.Length == 0 ? string.Empty : " and ",
                filter.Length == 0 ? string.Empty : string.Join(" and ", filter),
                "&$apply=groupby((BusinessPlaces/BPLID,BusinessPlaces/BPLName))"
            };

            string query = string.Join("", queryArgs);

            string data = await _serviceLayerConnector.getQueryResult(query);
            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<dynamic> result = new List<dynamic>();

            lista?.ForEach((l =>
            {
                var i = (dynamic)l;

                dynamic newItem = new ExpandoObject();
                newItem.Code = i.BusinessPlaces.BPLID;
                newItem.Name = i.BusinessPlaces.BPLName;

                result.Add(newItem);
            }));

            return result;
        }

        async private Task<List<dynamic>> listJoinBanks(string[] filter, bool filteredByGroup = false)
        {
            string service = $"$crossjoin({SL_SERVICE_NAME},Banks)";
            string filteredJoin = string.Empty;

            if (filteredByGroup)
            {
                service = $"$crossjoin({SL_SERVICE_NAME},Banks,U_S3ADVVENDOR)";
            }

            string[] queryArgs = new string[]
            {
                service,
                "?$expand=",
                "Banks($select=BankCode,BankName)",
                "&$filter=Banks/BankCode eq U_VSPAGTITULOS/U_Banco",
                filter.Length == 0 ? string.Empty : " and ",
                filter.Length == 0 ? string.Empty : string.Join(" and ", filter),
                "&$apply=groupby((Banks/BankCode,Banks/BankName))"
            };

            string query = string.Join("", queryArgs);

            string data = await _serviceLayerConnector.getQueryResult(query);
            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<dynamic> result = new List<dynamic>();

            lista?.ForEach((l =>
            {
                var i = (dynamic)l;

                dynamic newItem = new ExpandoObject();
                newItem.Code = i.Banks.BankCode;
                newItem.Name = i.Banks.BankName;

                result.Add(newItem);
            }));

            return result;
        }

        async private Task<List<dynamic>> listJoinChartOfAccounts(string fieldName, string[] filter, bool filteredByGroup = false)
        {
            string service = $"$crossjoin({SL_SERVICE_NAME},ChartOfAccounts)";
            string filteredJoin = string.Empty;

            if (filteredByGroup)
            {
                service = $"$crossjoin({SL_SERVICE_NAME},ChartOfAccounts,U_S3ADVVENDOR)";
            }

            string[] queryArgs = new string[]
            {
                service,
                "?$expand=",
                "ChartOfAccounts($select=Code,Name)",
                $"&$filter=ChartOfAccounts/Code eq U_VSPAGTITULOS/{fieldName}",
                filter.Length == 0 ? string.Empty : " and ",
                filter.Length == 0 ? string.Empty : string.Join(" and ", filter),
                "&$apply=groupby((ChartOfAccounts/Code,ChartOfAccounts/Name))"
            };

            string query = string.Join("", queryArgs);

            string data = await _serviceLayerConnector.getQueryResult(query);
            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<dynamic> result = new List<dynamic>();

            lista?.ForEach((l =>
            {
                var i = (dynamic)l;

                dynamic newItem = new ExpandoObject();
                newItem.Code = i.ChartOfAccounts.Code;
                newItem.Name = i.ChartOfAccounts.Name;

                result.Add(newItem);
            }));

            return result;
        }

        async public Task<List<AccountPayableInvoiceSummary>> ListSummary(List<Criteria> criterias)
        {
            var criteriasLocal = criterias.ToList();
            bool filteredByGroup = criterias.FirstOrDefault(m => m.Field.ToLower() == "vendorgroup") != null;
            
            List<AccountPayableInvoice> lista = await this.List(criterias, -1, -1);

            List<AccountPayableInvoiceSummary> result = new List<AccountPayableInvoiceSummary>();

            var filter = this.parseCriteria(criteriasLocal);

            var businessPartners = await this.listJoinBusinessPartners(filter.ToArray(), filteredByGroup);
            var businessPlaces = await this.listJoinBusinessPlaces(filter.ToArray(), filteredByGroup);
            var ledgerAccounts = await this.listJoinChartOfAccounts("U_Conta_Razao", filter.ToArray(), filteredByGroup);
            var controlAccounts = await this.listJoinChartOfAccounts("U_Conta_Controle", filter.ToArray(), filteredByGroup);
            var paymentTerms = await this.listJoinPaymentTermsTypes(filter.ToArray(), filteredByGroup);
            var banks = await this.listJoinBanks(filter.ToArray(), filteredByGroup);

            Func<dynamic, string> formatName = (source) =>
            {
                return source == null ? string.Empty : $"{source.Code}-{source.Name}";
            };

            var invoiceQuery = (from invoice in lista
                                join bp in businessPartners on invoice.CodigoFornecedor equals bp.Code into groupBp
                                from subBp in groupBp.DefaultIfEmpty()
                                join bpl in businessPlaces on invoice.CodigoFilial.toLong() equals bpl.Code into groupBpl
                                from subBpl in groupBpl.DefaultIfEmpty()
                                join cof in ledgerAccounts on invoice.ContaRazao equals cof.Code into groupCof
                                from subCof in groupCof.DefaultIfEmpty()
                                join cofc in controlAccounts on invoice.ContaControle equals cofc.Code into groupCofc
                                from subCofc in groupCofc.DefaultIfEmpty()
                                join pt in paymentTerms on invoice.CondicaoPagamento equals pt.Code into groupPt
                                from subPt in groupPt.DefaultIfEmpty()
                                join bk in banks on invoice.Banco.ToString("000") equals bk.Code into groupBk
                                from subBk in groupBk.DefaultIfEmpty()
                                select new
                                {
                                    Code = invoice.Code,
                                    NomeFornecedor = formatName(subBp),
                                    NomeFilial = formatName(subBpl),
                                    NomeContaRazao = formatName(subCof),
                                    NomeContaControle = formatName(subCofc),
                                    NomeCondicaoPagamento = formatName(subPt),
                                    NomeBanco = formatName(subBk)
                                }).ToList();

            lista.ForEach(ap =>
            {
                AccountPayableInvoiceSummary record = initializeSummaryRecord(ap);

                var joins = invoiceQuery.Find(m => m.Code == ap.Code);

                record.NomeFornecedor = joins.NomeFornecedor;
                record.NomeFilial = joins.NomeFilial;
                record.NomeContaRazao = joins.NomeContaRazao;
                record.NomeContaControle = joins.NomeContaControle;
                record.NomeCondicaoPagamento = joins.NomeCondicaoPagamento;
                record.NomeBanco = joins.NomeBanco;

                result.Add(record);
            });

            return result;
        }

        private AccountPayableInvoiceSummary initializeSummaryRecord(AccountPayableInvoice record)
        {
            AccountPayableInvoiceSummary result;

            string json = JsonConvert.SerializeObject(record);

            result = JsonConvert.DeserializeObject<AccountPayableInvoiceSummary>(json);

            return result;
        }


        async public Task Update(AccountPayableInvoice entity)
        {
            string query = $"{SL_SERVICE_NAME}('{entity.RecId}')";
            string json = toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Patch(query, json);

            if (!response.success)
            {
                string message = $"{entity.EntityName}: Erro ao atualizar {entity.NotaFiscal}={entity.Parcela}, {response.errorCode} {response.errorMessage}";
                throw new OperationCanceledException(message);
            }
        }

        async public Task Update(List<AccountPayableInvoice> entities)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();

            entities.ForEach(e =>
            {
                var json = toJson(e);
                batch.Post(HttpMethod.Patch, $"/{SL_SERVICE_NAME}('{e.RecId}')", json);
            });

            var response = await _serviceLayerConnector.Post(batch);

            if (!response.success)
            {
                string message = $"{entities[0].EntityName}: Erro ao atualizar lista de títulos a pagar, {response.errorCode} {response.errorMessage}";
                throw new OperationCanceledException(message);
            }
            else if (response.internalResponses.Count(m => m.success == false) != 0)
            {
                var qtTotal = ((IBatchProvider)batch).Items.Count();
                var qtSuccess = response.internalResponses.Count(m => m.success == true);
                var qtError = response.internalResponses.Count(m => m.success == false);

                string message = $"Erro ao atualizar lista de títulos a pagar. {qtTotal} registros, {qtSuccess} sucessos, {qtError} erros: {response.errorCode} {response.errorMessage}";
                throw new OperationCanceledException(message);
            }
        }

        private string toJson(AccountPayableInvoice entity)
        {
            dynamic record = new ExpandoObject();

            record.Code = entity.RecId.ToString();
            record.Name = entity.RecId.ToString();
            record.U_Abat_Manual = entity.AbatimentoManual;
            record.U_Abat_Receber = entity.AbatimentoReceber;
            record.U_Acrescimos = entity.Acrescimos;
            record.U_Agencia = entity.Agencia;
            record.U_Agencia_For = entity.AgenciaFornecedor;
            record.U_Arquivo_Gerado = entity.ArquivoGerado;
            record.U_Arquivo_Importado = entity.ArquivoImportado;
            record.U_Banco = entity.Banco;
            record.U_Banco_For = entity.BancoFornecedor;
            record.U_Bloqueado = entity.Bloqueado;
            record.U_Cod_Forne = entity.CodigoFornecedor;
            record.U_Cod_Forne_Ori = entity.CodigoFornecedorOriginal;
            record.U_Codbar = entity.CodigoBarras;
            record.U_Code_Antecipa = entity.CodigoAntecipacao;
            record.U_Code_Pagto = entity.CodigoPagamento;
            record.U_Codigo_Moeda = entity.CodigoMoeda;
            record.U_Condicao_Pagto = entity.CondicaoPagamento;
            record.U_Conta_Banc_For = entity.ContaBancariaFornecedor;
            record.U_Conta_Controle = entity.ContaControle;
            record.U_Conta_Razao = entity.ContaRazao;
            record.U_Desconto = entity.Desconto;
            record.U_Desconto_Acordo = entity.DescontoAcordo;
            record.U_Dig_Agencia = entity.AgenciaDigito;
            record.U_Dig_Agencia_For = entity.AgenciaDigitoFornecedor;
            record.U_Dig_Conta = entity.ContaBancariaDigito;
            record.U_Dig_Conta_Banc_For = entity.ContaBancariaDigitoFornecedor;
            record.U_Dt_Agendamento = entity.DataAgendamento;
            record.U_Dt_Bloqueio = entity.DataBloqueio;
            record.U_Dt_Emissao = entity.DataEmissao;
            record.U_Dt_geracao = entity.DataGeracao;
            record.U_Dt_Lancto = entity.DataLancamento;
            record.U_Dt_Liberacao = entity.DataLiberacao;
            record.U_Dt_Liquida = entity.DataLiquida;
            record.U_Dt_Pagto = entity.DataPagamento;
            record.U_Dt_Prev_Pagto = entity.DataPagamentoPrevisto;
            record.U_Dt_Recepcao = entity.DataRecepcao;
            record.U_Empresa = entity.CodigoEmpresa;
            record.U_Filial = entity.CodigoFilial;
            record.U_Forma_Pagto = entity.FormaPagamento;
            record.U_Funrural = entity.Funrural;
            record.U_Instr_Pagto = entity.InstrucaoPagamento;
            record.U_Juros = entity.Juros;
            record.U_Juros_Moeda = entity.JurosMoeda;
            record.U_LineId = entity.IdLinha;
            record.U_Lote_EDI = entity.LoteEDI;
            record.U_Meio_Pagto = entity.MeioPagamento;
            record.U_Modelo_NF = entity.ModeloNF;
            record.U_Moeda_Pagto = entity.MoedaPagamento;
            record.U_Motivo_Baixa = entity.MotivoBaixa;
            record.U_Multa = entity.Multa;
            record.U_Nosso_Numero = entity.NossoNumero;
            record.U_Nota = entity.NotaFiscal;
            record.U_Numero_Boleto = entity.NumeroBoleto;
            record.U_Numero_Conta = entity.NumeroConta;
            record.U_Parcela = entity.Parcela;
            record.U_Retencoes = entity.Retencoes;
            record.U_Serie = entity.NotaFiscalSerie;
            record.U_Seu_Numero = entity.SeuNumero;
            record.U_Situacao = entity.Situacao;
            record.U_Status = entity.Status;
            record.U_Tipcodbar = entity.CodigoBarrasTipo;
            record.U_Tipo_Conta = entity.NumeroContaTipo;
            record.U_TransId = entity.IdTransacao;
            record.U_Usu_Gerou_Arquivo = entity.UsuarioGerouArquivo;
            record.U_Usu_Importou_Conf = entity.UsuarioImportouConfirmacao;
            record.U_Usu_Importou_Liq = entity.UsuarioImportouLiquidacao;
            record.U_Usuario_Liberacao = entity.UsuarioLiberacao;
            record.U_Valor_Bruto = entity.ValorBruto;
            record.U_Valor_Bruto_Moeda = entity.ValorBrutoMoeda;
            record.U_Valor_Moeda_Cmp = entity.ValorMoedaCmp;
            record.U_Valor_Moeda_Pagto = entity.ValorMoedaPagamento;
            record.U_Valor_Pago = entity.ValorPago;
            record.U_Variacao_Cambial = entity.VariacaoCambial;
            record.U_Vencto = entity.DataVencimento;
            record.U_Vencto_Ori = entity.DataVencimentoOriginal;

            var result = JsonConvert.SerializeObject(record);

            return result;
        }

        private AccountPayableInvoice toRecord(dynamic data, bool filteredByGroup = false)
        {
            AccountPayableInvoice entity = new AccountPayableInvoice();

            dynamic record = filteredByGroup ? data.U_VSPAGTITULOS : data;

            //entity.RecId = Guid.Parse(record.Code);

            entity.Code = Convert.ToString(record.Code);
            entity.AbatimentoManual = Convert.ToDouble(record.U_Abat_Manual);
            entity.AbatimentoReceber = Convert.ToDouble(record.U_Abat_Receber);
            entity.Acrescimos = Convert.ToDouble(record.U_Acrescimos);
            entity.Agencia = Convert.ToInt32(record.U_Agencia);
            entity.AgenciaFornecedor = Convert.ToInt32(record.U_Agencia_For);
            entity.ArquivoGerado = Convert.ToString(record.U_Arquivo_Gerado);
            entity.ArquivoImportado = Convert.ToString(record.U_Arquivo_Importado);
            entity.Banco = Convert.ToInt32(record.U_Banco);
            entity.BancoFornecedor = Convert.ToInt32(record.U_Banco_For);
            entity.Bloqueado = Convert.ToInt32(record.U_Bloqueado);
            entity.CodigoFornecedor = Convert.ToString(record.U_Cod_Forne);
            entity.CodigoFornecedorOriginal = Convert.ToString(record.U_Cod_Forne_Ori);
            entity.CodigoBarras = Convert.ToString(record.U_Codbar);
            entity.CodigoAntecipacao = Convert.ToString(record.U_Code_Antecipa);
            entity.CodigoPagamento = Convert.ToString(record.U_Code_Pagto);
            entity.CodigoMoeda = Convert.ToString(record.U_Codigo_Moeda);
            entity.CondicaoPagamento = Convert.ToInt32(record.U_Condicao_Pagto);
            entity.ContaBancariaFornecedor = Convert.ToString(record.U_Conta_Banc_For);
            entity.ContaControle = Convert.ToString(record.U_Conta_Controle);
            entity.ContaRazao = Convert.ToString(record.U_Conta_Razao);
            entity.Desconto = Convert.ToDouble(record.U_Desconto);
            entity.DescontoAcordo = Convert.ToDouble(record.U_Desconto_Acordo);
            entity.AgenciaDigito = Convert.ToString(record.U_Dig_Agencia);
            entity.AgenciaDigitoFornecedor = Convert.ToString(record.U_Dig_Agencia_For);
            entity.ContaBancariaDigito = Convert.ToString(record.U_Dig_Conta);
            entity.ContaBancariaDigitoFornecedor = Convert.ToString(record.U_Dig_Conta_Banc_For);
            entity.DataVencimento = ((string)record.U_Vencto).toDate().Value;
            entity.DataVencimentoOriginal = ((string)record.U_Vencto_Ori).toDate().Value;
            entity.DataAgendamento = ((string)record.U_Dt_Agendamento).toDate();
            entity.DataBloqueio = ((string)record.U_Dt_Bloqueio).toDate();
            entity.DataEmissao = ((string)record.U_Dt_Emissao).toDate().Value;
            entity.DataGeracao = ((string)record.U_Dt_geracao).toDate();
            entity.DataLancamento = ((string)record.U_Dt_Lancto).toDate();
            entity.DataLiberacao = ((string)record.U_Dt_Liberacao).toDate();
            entity.DataLiquida = ((string)record.U_Dt_Liquida).toDate();
            entity.DataPagamento = ((string)record.U_Dt_Pagto).toDate();
            entity.DataPagamentoPrevisto = ((string)record.U_Dt_Prev_Pagto).toDate();
            entity.DataRecepcao = ((string)record.U_Dt_Recepcao).toDate().Value;
            entity.CodigoEmpresa = Convert.ToString(record.U_Empresa);
            entity.CodigoFilial = Convert.ToString(record.U_Filial);
            entity.FormaPagamento = Convert.ToString(record.U_Forma_Pagto);
            entity.Funrural = Convert.ToDouble(record.U_Funrural);
            entity.InstrucaoPagamento = Convert.ToString(record.U_Instr_Pagto);
            entity.Juros = Convert.ToDouble(record.U_Juros);
            entity.JurosMoeda = Convert.ToDouble(record.U_Juros_Moeda);
            entity.IdLinha = Convert.ToInt32(record.U_LineId);
            entity.LoteEDI = Convert.ToInt64(record.U_Lote_EDI);
            entity.MeioPagamento = Convert.ToInt32(record.U_Meio_Pagto);
            entity.ModeloNF = Convert.ToString(record.U_Modelo_NF);
            entity.MoedaPagamento = Convert.ToString(record.U_Moeda_Pagto);
            entity.MotivoBaixa = Convert.ToInt32(record.U_Motivo_Baixa);
            entity.Multa = Convert.ToDouble(record.U_Multa);
            entity.NossoNumero = Convert.ToString(record.U_Nosso_Numero);
            entity.NotaFiscal = Convert.ToInt64(record.U_Nota);
            entity.NumeroBoleto = Convert.ToInt64(record.U_Numero_Boleto);
            entity.NumeroConta = Convert.ToString(record.U_Numero_Conta);
            entity.Parcela = Convert.ToInt32(record.U_Parcela);
            entity.Retencoes = Convert.ToDouble(record.U_Retencoes);
            entity.NotaFiscalSerie = Convert.ToString(record.U_Serie);
            entity.SeuNumero = Convert.ToString(record.U_Seu_Numero);
            entity.Situacao = Convert.ToString(record.U_Situacao);
            entity.Status = Convert.ToString(record.U_Status);
            entity.CodigoBarrasTipo = Convert.ToString(record.U_Tipcodbar);
            entity.NumeroContaTipo = Convert.ToInt32(record.U_Tipo_Conta);
            entity.IdTransacao = Convert.ToString(record.U_TransId);
            entity.UsuarioGerouArquivo = Convert.ToString(record.U_Usu_Gerou_Arquivo);
            entity.UsuarioImportouConfirmacao = Convert.ToString(record.U_Usu_Importou_Conf);
            entity.UsuarioImportouLiquidacao = Convert.ToString(record.U_Usu_Importou_Liq);
            entity.UsuarioLiberacao = Convert.ToString(record.U_Usuario_Liberacao);
            entity.ValorBruto = Convert.ToDouble(record.U_Valor_Bruto);
            entity.ValorBrutoMoeda = Convert.ToDouble(record.U_Valor_Bruto_Moeda);
            entity.ValorMoedaCmp = Convert.ToDouble(record.U_Valor_Moeda_Cmp);
            entity.ValorMoedaPagamento = Convert.ToDouble(record.U_Valor_Moeda_Pagto);
            entity.ValorPago = Convert.ToDouble(record.U_Valor_Pago);
            entity.VariacaoCambial = Convert.ToDouble(record.U_Variacao_Cambial);

            return entity;
        }

        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "Code");
            map.Add("code", "Code");
            map.Add("abatimentomanual", "U_Abat_Manual");
            map.Add("abatimentoreceber", "U_Abat_Receber");           
            map.Add("acrescimos", "U_Acrescimos");
            map.Add("agencia", "U_Agencia");
            map.Add("agenciafornecedor", "U_Agencia_For");
            map.Add("arquivogerado", "U_Arquivo_Gerado");
            map.Add("arquivoimportado", "U_Arquivo_Importado");
            map.Add("banco", "U_Banco");
            map.Add("bancofornecedor", "U_Banco_For");
            map.Add("bloqueado", "U_Bloqueado");
            map.Add("codigofornecedor", "U_Cod_Forne");
            map.Add("codigofornecedororiginal", "U_Cod_Forne_Ori");
            map.Add("codigobarras", "U_Codbar");
            map.Add("codigoantecipacao", "U_Code_Antecipa");
            map.Add("codigopagamento", "U_Code_Pagto");
            map.Add("codigomoeda", "U_Codigo_Moeda");
            map.Add("condicaopagamento", "U_Condicao_Pagto");
            map.Add("contabancariafornecedor", "U_Conta_Banc_For");
            map.Add("contacontrole", "U_Conta_Controle");
            map.Add("contarazao", "U_Conta_Razao");
            map.Add("desconto", "U_Desconto");
            map.Add("descontoacordo", "U_Desconto_Acordo");
            map.Add("agenciadigito", "U_Dig_Agencia");
            map.Add("agenciadigitofornecedor", "U_Dig_Agencia_For");
            map.Add("contabancariadigito", "U_Dig_Conta");
            map.Add("contabancariadigitofornecedor", "U_Dig_Conta_Banc_For");
            map.Add("dataagendamento", "U_Dt_Agendamento");
            map.Add("databloqueio", "U_Dt_Bloqueio");
            map.Add("dataemissao", "U_Dt_Emissao");
            map.Add("datageracao", "U_Dt_geracao");
            map.Add("datalancamento", "U_Dt_Lancto");
            map.Add("dataliberacao", "U_Dt_Liberacao");
            map.Add("dataliquida", "U_Dt_Liquida");
            map.Add("datapagamento", "U_Dt_Pagto");
            map.Add("datapagamentoprevisto", "U_Dt_Prev_Pagto");
            map.Add("datarecepcao", "U_Dt_Recepcao");
            map.Add("datavencimento", "U_Vencto");
            map.Add("datavencimentooriginal", "U_Vencto_Ori");
            map.Add("codigoempresa", "U_Empresa");
            map.Add("codigofilial", "U_Filial");
            map.Add("formapagamento", "U_Forma_Pagto");
            map.Add("funrural", "U_Funrural");
            map.Add("instrucaopagamento", "U_Instr_Pagto");
            map.Add("juros", "U_Juros");
            map.Add("jurosmoeda", "U_Juros_Moeda");
            map.Add("idlinha", "U_LineId");
            map.Add("loteedi", "U_Lote_EDI");
            map.Add("meiopagamento", "U_Meio_Pagto");
            map.Add("modelonf", "U_Modelo_NF");
            map.Add("moedapagamento", "U_Moeda_Pagto");
            map.Add("motivobaixa", "U_Motivo_Baixa");
            map.Add("multa", "U_Multa");
            map.Add("nossonumero", "U_Nosso_Numero");
            map.Add("notafiscal", "U_Nota");
            map.Add("numeroboleto", "U_Numero_Boleto");
            map.Add("numeroconta", "U_Numero_Conta");
            map.Add("parcela", "U_Parcela");
            map.Add("retencoes", "U_Retencoes");
            map.Add("notafiscalserie", "U_Serie");
            map.Add("seunumero", "U_Seu_Numero");
            map.Add("situacao", "U_Situacao");
            map.Add("status", "U_Status");
            map.Add("codigobarrastipo", "U_Tipcodbar");
            map.Add("numerocontatipo", "U_Tipo_Conta");
            map.Add("idtransacao", "U_TransId");
            map.Add("usuariogerouarquivo", "U_Usu_Gerou_Arquivo");
            map.Add("usuarioimportouconfirmacao", "U_Usu_Importou_Conf");
            map.Add("usuarioimportouliquidacao", "U_Usu_Importou_Liq");
            map.Add("usuarioliberacao", "U_Usuario_Liberacao");
            map.Add("valorbruto", "U_Valor_Bruto");
            map.Add("valorbrutomoeda", "U_Valor_Bruto_Moeda");
            map.Add("valormoedacmp", "U_Valor_Moeda_Cmp");
            map.Add("valormoedapagamento", "U_Valor_Moeda_Pagto");
            map.Add("valorpago", "U_Valor_Pago");
            map.Add("variacaocambial", "U_Variacao_Cambial");

            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "T");
            map.Add("code", "T");
            map.Add("abatimentomanual", "N");     
            map.Add("abatimentoreceber", "N");
            map.Add("acrescimos", "N");
            map.Add("agencia", "N");
            map.Add("agenciafornecedor", "N");
            map.Add("arquivogerado", "T");
            map.Add("arquivoimportado", "T");
            map.Add("banco", "N");
            map.Add("bancofornecedor", "N");
            map.Add("bloqueado", "N");
            map.Add("codigofornecedor", "T");
            map.Add("codigofornecedororiginal", "T");
            map.Add("codigobarras", "T");
            map.Add("codigoantecipacao", "T");
            map.Add("codigopagamento", "T");
            map.Add("codigomoeda", "T");
            map.Add("condicaopagamento", "N");
            map.Add("contabancariafornecedor", "T");
            map.Add("contacontrole", "T");
            map.Add("contarazao", "T");
            map.Add("desconto", "N");
            map.Add("descontoacordo", "N");
            map.Add("agenciadigito", "T");
            map.Add("agenciadigitofornecedor", "T");
            map.Add("contabancariadigito", "T");
            map.Add("contabancariadigitofornecedor", "T");
            map.Add("dataagendamento", "T");
            map.Add("databloqueio", "T");
            map.Add("dataemissao", "T");
            map.Add("datageracao", "T");
            map.Add("datalancamento", "T");
            map.Add("dataliberacao", "T");
            map.Add("dataliquida", "T");
            map.Add("datapagamento", "T");
            map.Add("datapagamentoprevisto", "T");
            map.Add("datarecepcao", "T");
            map.Add("datavencimento", "T");
            map.Add("datavencimentooriginal", "T");
            map.Add("codigoempresa", "T");
            map.Add("codigofilial", "T");
            map.Add("formapagamento", "T");
            map.Add("funrural", "N");
            map.Add("instrucaopagamento", "T");
            map.Add("juros", "N");
            map.Add("jurosmoeda", "N");
            map.Add("idlinha", "N");
            map.Add("loteedi", "N");
            map.Add("meiopagamento", "N");
            map.Add("modelonf", "T");
            map.Add("moedapagamento", "T");
            map.Add("motivobaixa", "N");
            map.Add("multa", "N");
            map.Add("nossonumero", "T");
            map.Add("notafiscal", "N");
            map.Add("numeroboleto", "N");
            map.Add("numeroconta", "T");
            map.Add("parcela", "N");
            map.Add("retencoes", "N");
            map.Add("notafiscalserie", "T");
            map.Add("seunumero", "T");
            map.Add("situacao", "T");
            map.Add("status", "T");
            map.Add("codigobarrastipo", "T");
            map.Add("numerocontatipo", "N");
            map.Add("idtransacao", "T");
            map.Add("usuariogerouarquivo", "T");
            map.Add("usuarioimportouconfirmacao", "T");
            map.Add("usuarioimportouliquidacao", "T");
            map.Add("usuarioliberacao", "T");
            map.Add("valorbruto", "N");
            map.Add("valorbrutomoeda", "N");
            map.Add("valormoedacmp", "N");
            map.Add("valormoedapagamento", "N");
            map.Add("valorpago", "N");
            map.Add("variacaocambial", "N");

            return map;
        }
        /*
        private Dictionary<string, FieldMapItem> mountFieldMap()
        {
            Dictionary<string, FieldMapItem> map = new Dictionary<string, FieldMapItem>();

            map.Add("recid", new FieldMapItem() { name = "Code", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("code", new FieldMapItem() { name = "Code", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("abatimentomanual", new FieldMapItem() { name = "U_Abat_Manual", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("abatimentoreceber", new FieldMapItem() { name = "U_Abat_Receber", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("acrescimos", new FieldMapItem() { name = "U_Acrescimos", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("agencia", new FieldMapItem() { name = "U_Agencia", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("agenciafornecedor", new FieldMapItem() { name = "U_Agencia_For", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("arquivogerado", new FieldMapItem() { name = "U_Arquivo_Gerado", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("arquivoimportado", new FieldMapItem() { name = "U_Arquivo_Importado", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("banco", new FieldMapItem() { name = "U_Banco", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("bancofornecedor", new FieldMapItem() { name = "U_Banco_For", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("bloqueado", new FieldMapItem() { name = "U_Bloqueado", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("codigofornecedor", new FieldMapItem() { name = "U_Cod_Forne", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("codigofornecedororiginal", new FieldMapItem() { name = "U_Cod_Forne_Ori", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("codigobarras", new FieldMapItem() { name = "U_Codbar", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("codigoantecipacao", new FieldMapItem() { name = "U_Code_Antecipa", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("codigopagamento", new FieldMapItem() { name = "U_Code_Pagto", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("codigomoeda", new FieldMapItem() { name = "U_Codigo_Moeda", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("condicaopagamento", new FieldMapItem() { name = "U_Condicao_Pagto", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("contabancariafornecedor", new FieldMapItem() { name = "U_Conta_Banc_For", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("contacontrole", new FieldMapItem() { name = "U_Conta_Controle", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("contarazao", new FieldMapItem() { name = "U_Conta_Razao", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("desconto", new FieldMapItem() { name = "U_Desconto", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("descontoacordo", new FieldMapItem() { name = "U_Desconto_Acordo", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("agenciadigito", new FieldMapItem() { name = "U_Dig_Agencia", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("agenciadigitofornecedor", new FieldMapItem() { name = "U_Dig_Agencia_For", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("contabancariadigito", new FieldMapItem() { name = "U_Dig_Conta", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("contabancariadigitofornecedor", new FieldMapItem() { name = "U_Dig_Conta_Banc_For", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("dataagendamento", new FieldMapItem() { name = "U_Dt_Agendamento", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("databloqueio", new FieldMapItem() { name = "U_Dt_Bloqueio", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("dataemissao", new FieldMapItem() { name = "U_Dt_Emissao", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("datageracao", new FieldMapItem() { name = "U_Dt_geracao", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("datalancamento", new FieldMapItem() { name = "U_Dt_Lancto", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("dataliberacao", new FieldMapItem() { name = "U_Dt_Liberacao", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("dataliquida", new FieldMapItem() { name = "U_Dt_Liquida", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("datapagamento", new FieldMapItem() { name = "U_Dt_Pagto", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("datapagamentoprevisto", new FieldMapItem() { name = "U_Dt_Prev_Pagto", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("datarecepcao", new FieldMapItem() { name = "U_Dt_Recepcao", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("datavencimento", new FieldMapItem() { name = "U_Vencto", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("datavencimentooriginal", new FieldMapItem() { name = "U_Vencto_Ori", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("codigoempresa", new FieldMapItem() { name = "U_Empresa", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("codigofilial", new FieldMapItem() { name = "U_Filial", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("formapagamento", new FieldMapItem() { name = "U_Forma_Pagto", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("funrural", new FieldMapItem() { name = "U_Funrural", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("instrucaopagamento", new FieldMapItem() { name = "U_Instr_Pagto", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("juros", new FieldMapItem() { name = "U_Juros", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("jurosmoeda", new FieldMapItem() { name = "U_Juros_Moeda", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("idlinha", new FieldMapItem() { name = "U_LineId", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("loteedi", new FieldMapItem() { name = "U_Lote_EDI", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("meiopagamento", new FieldMapItem() { name = "U_Meio_Pagto", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("modelonf", new FieldMapItem() { name = "U_Modelo_NF", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("moedapagamento", new FieldMapItem() { name = "U_Moeda_Pagto", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("motivobaixa", new FieldMapItem() { name = "U_Motivo_Baixa", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("multa", new FieldMapItem() { name = "U_Multa", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("nossonumero", new FieldMapItem() { name = "U_Nosso_Numero", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("notafiscal", new FieldMapItem() { name = "U_Nota", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("numeroboleto", new FieldMapItem() { name = "U_Numero_Boleto", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("numeroconta", new FieldMapItem() { name = "U_Numero_Conta", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("parcela", new FieldMapItem() { name = "U_Parcela", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("retencoes", new FieldMapItem() { name = "U_Retencoes", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("notafiscalserie", new FieldMapItem() { name = "U_Serie", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("seunumero", new FieldMapItem() { name = "U_Seu_Numero", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("situacao", new FieldMapItem() { name = "U_Situacao", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("status", new FieldMapItem() { name = "U_Status", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("codigobarrastipo", new FieldMapItem() { name = "U_Tipcodbar", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("numerocontatipo", new FieldMapItem() { name = "U_Tipo_Conta", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("idtransacao", new FieldMapItem() { name = "U_TransId", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("usuariogerouarquivo", new FieldMapItem() { name = "U_Usu_Gerou_Arquivo", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("usuarioimportouconfirmacao", new FieldMapItem() { name = "U_Usu_Importou_Conf", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("usuarioimportouliquidacao", new FieldMapItem() { name = "U_Usu_Importou_Liq", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("usuarioliberacao", new FieldMapItem() { name = "U_Usuario_Liberacao", type = FieldMapItem.FieldTypeEnum.Alpha });
            map.Add("valorbruto", new FieldMapItem() { name = "U_Valor_Bruto", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("valorbrutomoeda", new FieldMapItem() { name = "U_Valor_Bruto_Moeda", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("valormoedacmp", new FieldMapItem() { name = "U_Valor_Moeda_Cmp", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("valormoedapagamento", new FieldMapItem() { name = "U_Valor_Moeda_Pagto", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("valorpago", new FieldMapItem() { name = "U_Valor_Pago", type = FieldMapItem.FieldTypeEnum.Numeric });
            map.Add("variacaocambial", new FieldMapItem() { name = "U_Variacao_Cambial", type = FieldMapItem.FieldTypeEnum.Numeric });

            return map;
        }*/
    }
}

/*QUERY: /$crossjoin(U_VSPAGTITULOS,U_S3ADVVENDOR)?$expand=U_VSPAGTITULOS($select=U_Cod_Forne)&$filter=U_VSPAGTITULOS/U_Cod_Forne eq U_S3ADVVENDOR/U_CARDCODE and U_S3ADVVENDOR/U_GROUPID eq '61d95462-4ae7-4d94-9e8f-cbef1b15afa4'*/

/* LISTA DE CAMPOS
Code,Name,U_TransId,U_LineId,U_Cod_Forne,U_Cod_Forne_Ori,U_Nota,U_Serie,U_Parcela,U_Vencto,U_Vencto_Ori,U_Dt_Recepcao,
U_Dt_Emissao,U_Dt_Agendamento,U_Dt_Pagto,U_Dt_Liquida,U_Dt_Lancto,U_Dt_Prev_Pagto,U_Conta_Controle,U_Banco,U_Banco_For,
U_Empresa,U_Filial,U_Bloqueado,U_Usuario_Liberacao,U_Dt_Liberacao,U_Dt_Bloqueio,U_Condicao_Pagto,U_Status,U_Situacao,
U_Meio_Pagto,U_Forma_Pagto,U_Tipo_Conta,U_Numero_Conta,U_Dig_Conta,U_Dig_Agencia,U_Agencia,U_Dig_Agencia_For,U_Agencia_For,
U_Dig_Conta_Banc_For,U_Conta_Banc_For,U_Valor_Bruto,U_Abat_Manual,U_Abat_Receber,U_Desconto_Acordo,U_Desconto,U_Acrescimos,
U_Retencoes,U_Funrural,U_Juros,U_Multa,U_Valor_Pago,U_Codigo_Moeda,U_Valor_Moeda_Cmp,U_Moeda_Pagto,U_Valor_Moeda_Pagto,
U_Valor_Bruto_Moeda,U_Variacao_Cambial,U_Juros_Moeda,U_Tipcodbar,U_Codbar,U_Code_Pagto,U_Seu_Numero,U_Nosso_Numero,U_Instr_Pagto,
U_Arquivo_Gerado,U_Arquivo_Importado,U_Usu_Gerou_Arquivo,U_Usu_Importou_Conf,U_Usu_Importou_Liq,U_Dt_geracao,U_Motivo_Baixa,
U_Numero_Boleto,U_Lote_EDI,U_Code_Antecipa,U_Modelo_NF,U_Conta_Razao,U_Dt_confirma,U_Ocorrencia
*/
