using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model.AccountIncoming;

namespace Varsis.Data.Serviceb1.Integration.AccountIncoming
{
    public class AccountIncomingInvoiceService : IEntityService<AccountIncomingInvoice>
    {
        const string SL_SERVICE_NAME = "U_VSRECTITULOS";

        readonly ServiceLayerConnector _serviceLayerConnector;

        readonly Dictionary<string, string> _FieldMap;
        readonly Dictionary<string, string> _FieldType;

        public AccountIncomingInvoiceService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = mountFieldMap();
            _FieldType = mountFieldType();
        }

        public Task<bool> Create()
        {
            throw new NotSupportedException();
        }

        async public Task Delete(AccountIncomingInvoice entity)
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
                List<AccountIncomingInvoice> lista = await this.List(criterias, -1, -1);

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

        async public Task<AccountIncomingInvoice> Find(List<Criteria> criterias)
        {
            AccountIncomingInvoice result = null;

            try
            {
                List<AccountIncomingInvoice> lista = await this.List(criterias, -1, -1);

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

        async public Task Insert(AccountIncomingInvoice entity)
        {
            string json = toJson(entity);

            ServiceLayerResponse response = await _serviceLayerConnector.Post($"{SL_SERVICE_NAME}", json);

            if (!response.success)
            {
                string message = $"{entity.EntityName}: Erro ao incluir {entity.NotaFiscal}-{entity.Parcela}, {response.errorCode} {response.errorMessage}";
                throw new OperationCanceledException(message);
            }
        }

        async public Task Insert(List<AccountIncomingInvoice> entities)
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

        async public Task<List<AccountIncomingInvoice>> List(List<Criteria> criterias, long page, long size)
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
                    "Code,Name,U_TransId,U_LineId,U_Cod_Cli,U_Cod_Cli_Ori,U_Nota,U_Serie,U_Parcela,U_Vencto,U_Vencto_Ori,U_Dt_Recepcao,",
                    "U_Dt_Emissao,U_Dt_Agendamento,U_Dt_Pagto,U_Dt_Liquida,U_Dt_Lancto,U_Dt_Prev_Recto,U_Conta_Controle,U_Banco,",
                    "U_Empresa,U_Filial,U_Condicao_Pagto,U_Status,U_Situacao,",
                    "U_Meio_Pagto,U_Forma_Pagto,U_Tipo_Conta,U_Numero_Conta,U_Dig_Conta,U_Dig_Agencia,U_Agencia,",
                    "U_Valor_Bruto,U_Abat_Manual,U_Abat_Pagar,U_Desconto_Acordo,U_Desconto,U_Acrescimos,",
                    "U_Juros,U_Multa,U_Valor_Pago,U_Moeda,",
                    "U_Val_Tran_Moeda,U_Var_Cam_Real,U_Var_Cam_Nreal,U_Code_Pagto,U_Seu_Numero,U_Nosso_Numero,",
                    "U_Arquivo_Gerado,U_Arquivo_Importado,U_Usu_Gerou_Arquivo,U_Usu_Importou_Conf,U_Usu_Importou_Liq,U_Dt_geracao,U_Motivo_Baixa,",
                    "U_Numero_Boleto,U_Lote_EDI, U_Modelo_nf,U_Conta_Razao,U_Dt_confirma,U_Ocorrencia, U_Retencoes"
                };

                string selectList = string.Join("", selectListArgs);
                string expand = $"$expand=U_VSRECTITULOS($select={selectList})";

                query = $"{service}?{expand}&$filter={string.Join(" and ", filter)}";
                query = Uri.EscapeUriString(query);
            }
            else
            {
                query = Global.MakeODataQuery(service, null, filter.Length == 0 ? null : filter);
            }

            var data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<AccountIncomingInvoice> result = new List<AccountIncomingInvoice>();

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
                filter.Add($"(U_VSRECTITULOS/U_Cod_Cli eq U_S3ADVVENDOR/U_CARDCODE and U_S3ADVVENDOR/U_GROUPID eq '{vendorGroupCriteria.Value}')");

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
                "&$filter=BusinessPartners/CardCode eq U_VSRECTITULOS/U_Cod_Cli",
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
                "&$filter=PaymentTermsTypes/GroupNumber eq U_VSRECTITULOS/U_Condicao_Pagto",
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
                "&$filter=BusinessPlaces/BPLID eq U_VSRECTITULOS/U_Filial",
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
                "&$filter=Banks/BankCode eq U_VSRECTITULOS/U_Banco",
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
                $"&$filter=ChartOfAccounts/Code eq U_VSRECTITULOS/{fieldName}",
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

        async public Task<List<AccountIncomingInvoiceSummary>> ListSummary(List<Criteria> criterias)
        {
            var criteriasLocal = criterias.ToList();
            bool filteredByGroup = criterias.FirstOrDefault(m => m.Field.ToLower() == "vendorgroup") != null;

            List<AccountIncomingInvoice> lista = await this.List(criterias, -1, -1);

            List<AccountIncomingInvoiceSummary> result = new List<AccountIncomingInvoiceSummary>();

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
                                join bp in businessPartners on invoice.CodigoCliente equals bp.Code into groupBp
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
                AccountIncomingInvoiceSummary record = initializeSummaryRecord(ap);

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

        private AccountIncomingInvoiceSummary initializeSummaryRecord(AccountIncomingInvoice record)
        {
            AccountIncomingInvoiceSummary result;

            string json = JsonConvert.SerializeObject(record);

            result = JsonConvert.DeserializeObject<AccountIncomingInvoiceSummary>(json);

            return result;
        }


        async public Task Update(AccountIncomingInvoice entity)
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

        async public Task Update(List<AccountIncomingInvoice> entities)
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
                string message = $"{entities[0].EntityName}: Erro ao atualizar lista de títulos a receber, {response.errorCode} {response.errorMessage}";
                throw new OperationCanceledException(message);
            }
            else if (response.internalResponses.Count(m => m.success == false) != 0)
            {
                var qtTotal = ((IBatchProvider)batch).Items.Count();
                var qtSuccess = response.internalResponses.Count(m => m.success == true);
                var qtError = response.internalResponses.Count(m => m.success == false);

                string message = $"Erro ao atualizar lista de títulos a receber. {qtTotal} registros, {qtSuccess} sucessos, {qtError} erros: {response.errorCode} {response.errorMessage}";
                throw new OperationCanceledException(message);
            }
        }

        private string toJson(AccountIncomingInvoice entity)
        {
            dynamic record = new ExpandoObject();

            record.Code = entity.RecId.ToString();
            record.Name = entity.RecId.ToString();
            record.U_Abat_Manual = entity.AbatimentoManual;
            record.U_Abat_Pagar = entity.AbatimentoPagar;
            record.U_Acrescimos = entity.Acrescimos;
            record.U_Agencia = entity.Agencia;
           
            record.U_Arquivo_Gerado = entity.ArquivoGerado;
            record.U_Arquivo_Importado = entity.ArquivoImportado;
            record.U_Banco = entity.Banco;          
         
            record.U_Cod_Cli = entity.CodigoCliente;
            record.U_Cod_Cli_Ori = entity.CodigoClienteOriginal;      
          
            record.U_Code_Diar = entity.CodigoDiario;
            record.U_Moeda = entity.Moeda;
            record.U_Condicao_Pagto = entity.CondicaoPagamento;
           
            record.U_Conta_Controle = entity.ContaControle;
            record.U_Conta_Razao = entity.ContaRazao;
            record.U_Desconto = entity.Desconto;
            record.U_Desconto_Acordo = entity.DescontoAcordo;
            record.U_Dig_Agencia = entity.AgenciaDigito;
            
            record.U_Dig_Conta = entity.ContaBancariaDigito;           
            record.U_Dt_Agendamento = entity.DataAgendamento;           
            record.U_Dt_Emissao = entity.DataEmissao;
            record.U_Dt_geracao = entity.DataGeracao;
            record.U_Dt_Lancto = entity.DataLancamento;
       
            record.U_Dt_Liquida = entity.DataLiquida;
            record.U_Dt_Pagto = entity.DataPagamento;
            record.U_Dt_Prev_Recto = entity.DataRecebimentoPrevisto;
            record.U_Dt_Recepcao = entity.DataRecepcao;
            record.U_Empresa = entity.CodigoEmpresa;
            record.U_Filial = entity.CodigoFilial;
            record.U_Forma_Pagto = entity.FormaPagamento;           
           
            record.U_Juros = entity.Juros;          
            record.U_LineId = entity.IdLinha;
            record.U_Lote_EDI = entity.LoteEDI;
            record.U_Meio_Pagto = entity.MeioPagamento;
            record.U_Modelo_NF = entity.ModeloNF;
          
            record.U_Motivo_Baixa = entity.MotivoBaixa;
            record.U_Multa = entity.Multa;
            record.U_Nosso_Numero = entity.NossoNumero;
            record.U_Nota = entity.NotaFiscal;
            record.U_Numero_Boleto = entity.NumeroBoleto;
            record.U_Numero_Conta = entity.NumeroConta;
            record.U_Parcela = entity.Parcela;
            
            record.U_Serie = entity.NotaFiscalSerie;
            record.U_Seu_Numero = entity.SeuNumero;
            record.U_Situacao = entity.Situacao;
            record.U_Status = entity.Status;            
            record.U_Tipo_Conta = entity.NumeroContaTipo;
            record.U_TransId = entity.IdTransacao;
            record.U_Usu_Gerou_Arquivo = entity.UsuarioGerouArquivo;
            record.U_Usu_Importou_Conf = entity.UsuarioImportouConfirmacao;
            record.U_Usu_Importou_Liq = entity.UsuarioImportouLiquidacao;            
            record.U_Valor_Bruto = entity.ValorBruto;
            record.U_Val_Tran_Moeda = entity.ValTranMoeda;                     
            record.U_Valor_Pago = entity.ValorPago;
            record.U_Variacao_Cambial_Real = entity.VarCamReal;
            record.U_Var_Cam_Nreal = entity.VarCamNreal;
            record.U_Vencto = entity.DataVencimento;
            record.U_Vencto_Ori = entity.DataVencimentoOriginal;
            record.U_Retencoes = entity.Retencoes;


            var result = JsonConvert.SerializeObject(record);

            return result;
        }

        private AccountIncomingInvoice toRecord(dynamic data, bool filteredByGroup = false)
        {
            AccountIncomingInvoice entity = new AccountIncomingInvoice();

            dynamic record = filteredByGroup ? data.U_VSRECTITULOS : data;

            //entity.RecId = Guid.Parse(record.Code);

            entity.Code = Convert.ToString(record.Code);
            entity.AbatimentoManual = Convert.ToDouble(record.U_Abat_Manual);
            entity.AbatimentoPagar = Convert.ToDouble(record.U_Abat_Pagar);
            entity.Acrescimos = Convert.ToDouble(record.U_Acrescimos);
            entity.Agencia = Convert.ToInt32(record.U_Agencia);
           
            entity.ArquivoGerado = Convert.ToString(record.U_Arquivo_Gerado);
            entity.ArquivoImportado = Convert.ToString(record.U_Arquivo_Importado);
            entity.Banco = Convert.ToInt32(record.U_Banco);
            
            entity.CodigoCliente = Convert.ToString(record.U_Cod_Cli);
            entity.CodigoClienteOriginal = Convert.ToString(record.U_Cod_Cli_Ori);            
           
            entity.CodigoDiario = Convert.ToString(record.U_Code_Diar);
            entity.Moeda = Convert.ToString(record.U_Moeda);
            entity.CondicaoPagamento = Convert.ToInt32(record.U_Condicao_Pagto);
            
            entity.ContaControle = Convert.ToString(record.U_Conta_Controle);
            entity.ContaRazao = Convert.ToString(record.U_Conta_Razao);
            entity.Desconto = Convert.ToDouble(record.U_Desconto);
            entity.DescontoAcordo = Convert.ToDouble(record.U_Desconto_Acordo);
            entity.AgenciaDigito = Convert.ToString(record.U_Dig_Agencia);
           
            entity.ContaBancariaDigito = Convert.ToString(record.U_Dig_Conta);           
            entity.DataVencimento = ((string)record.U_Vencto).toDate().Value;
            entity.DataVencimentoOriginal = ((string)record.U_Vencto_Ori).toDate().Value;
            entity.DataAgendamento = ((string)record.U_Dt_Agendamento).toDate();
            
            entity.DataEmissao = ((string)record.U_Dt_Emissao).toDate().Value;
            entity.DataGeracao = ((string)record.U_Dt_geracao).toDate();
            entity.DataLancamento = ((string)record.U_Dt_Lancto).toDate();
           
            entity.DataLiquida = ((string)record.U_Dt_Liquida).toDate();
            entity.DataPagamento = ((string)record.U_Dt_Pagto).toDate();
            entity.DataRecebimentoPrevisto = ((string)record.U_Dt_Prev_Recto).toDate();
            entity.DataRecepcao = ((string)record.U_Dt_Recepcao).toDate().Value;
            entity.CodigoEmpresa = Convert.ToString(record.U_Empresa);
            entity.CodigoFilial = Convert.ToString(record.U_Filial);
            entity.FormaPagamento = Convert.ToString(record.U_Forma_Pagto);           
         
            entity.Juros = Convert.ToDouble(record.U_Juros);            
            entity.IdLinha = Convert.ToInt32(record.U_LineId);
            entity.LoteEDI = Convert.ToInt64(record.U_Lote_EDI);
            entity.MeioPagamento = record.U_Meio_Pagto;
            entity.ModeloNF = Convert.ToString(record.U_Modelo_nf);
            
            entity.MotivoBaixa = Convert.ToInt32(record.U_Motivo_Baixa);
            entity.Multa = Convert.ToDouble(record.U_Multa);
            entity.NossoNumero = Convert.ToString(record.U_Nosso_Numero);
            entity.NotaFiscal = Convert.ToInt64(record.U_Nota);
            entity.NumeroBoleto = Convert.ToInt64(record.U_Numero_Boleto);
            entity.NumeroConta = Convert.ToString(record.U_Numero_Conta);
            entity.Parcela = Convert.ToInt32(record.U_Parcela);
           
            entity.NotaFiscalSerie = Convert.ToString(record.U_Serie);
            entity.SeuNumero = Convert.ToString(record.U_Seu_Numero);
            entity.Situacao = Convert.ToString(record.U_Situacao);
            entity.Status = Convert.ToString(record.U_Status);           
            entity.NumeroContaTipo = Convert.ToInt32(record.U_Tipo_Conta);
            entity.IdTransacao = Convert.ToString(record.U_TransId);
            entity.UsuarioGerouArquivo = Convert.ToString(record.U_Usu_Gerou_Arquivo);
            entity.UsuarioImportouConfirmacao = Convert.ToString(record.U_Usu_Importou_Conf);
            entity.UsuarioImportouLiquidacao = Convert.ToString(record.U_Usu_Importou_Liq);
           
            entity.ValorBruto = Convert.ToDouble(record.U_Valor_Bruto);
            entity.ValTranMoeda = Convert.ToDouble(record.U_Val_Tran_Moeda);    
            
            entity.ValorPago = Convert.ToDouble(record.U_Valor_Pago);
            entity.VarCamReal = Convert.ToDouble(record.U_Var_Cam_Real);
            entity.VarCamNreal = Convert.ToDouble(record.U_Var_Cam_Nreal);
            entity.Retencoes = Convert.ToDouble(record.U_Retencoes);

            return entity;
        }

        private Dictionary<string, string> mountFieldMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "Code");
            map.Add("code", "Code");
            map.Add("abatimentomanual", "U_Abat_Manual");
            map.Add("abatimentopagar", "U_Abat_Pagar");
            map.Add("acrescimos", "U_Acrescimos");
            map.Add("agencia", "U_Agencia");
            
            map.Add("arquivogerado", "U_Arquivo_Gerado");
            map.Add("arquivoimportado", "U_Arquivo_Importado");
            map.Add("banco", "U_Banco");
            
            map.Add("codigocliente", "U_Cod_Cli");
            map.Add("codigoclienteoriginal", "U_Cod_Cli_Ori");          
            
            map.Add("codigodiario", "U_Code_Diar");
            map.Add("moeda", "U_Moeda");
            map.Add("condicaopagamento", "U_Condicao_Pagto");
            
            map.Add("contacontrole", "U_Conta_Controle");
            map.Add("contarazao", "U_Conta_Razao");
            map.Add("desconto", "U_Desconto");
            map.Add("descontoacordo", "U_Desconto_Acordo");
            map.Add("agenciadigito", "U_Dig_Agencia");
            
            map.Add("contabancariadigito", "U_Dig_Conta");            
            map.Add("dataagendamento", "U_Dt_Agendamento");
      
            map.Add("dataemissao", "U_Dt_Emissao");
            map.Add("datageracao", "U_Dt_geracao");
            map.Add("datalancamento", "U_Dt_Lancto");
          
            map.Add("dataliquida", "U_Dt_Liquida");
            map.Add("datapagamento", "U_Dt_Pagto");
            map.Add("datarecebimentoprevisto", "U_Dt_Prev_Recto");
            map.Add("datarecepcao", "U_Dt_Recepcao");
            map.Add("datavencimento", "U_Vencto");
            map.Add("datavencimentooriginal", "U_Vencto_Ori");
            map.Add("codigoempresa", "U_Empresa");
            map.Add("codigofilial", "U_Filial");
            map.Add("formapagamento", "U_Forma_Pagto");     
          
            map.Add("juros", "U_Juros");           
            map.Add("idlinha", "U_LineId");
            map.Add("loteedi", "U_Lote_EDI");
            map.Add("meiopagamento", "U_Meio_Pagto");
            map.Add("modelonf", "U_Modelo_NF");
           
            map.Add("motivobaixa", "U_Motivo_Baixa");
            map.Add("multa", "U_Multa");
            map.Add("nossonumero", "U_Nosso_Numero");
            map.Add("notafiscal", "U_Nota");
            map.Add("numeroboleto", "U_Numero_Boleto");
            map.Add("numeroconta", "U_Numero_Conta");
            map.Add("parcela", "U_Parcela");
           
            map.Add("notafiscalserie", "U_Serie");
            map.Add("seunumero", "U_Seu_Numero");
            map.Add("situacao", "U_Situacao");
            map.Add("status", "U_Status");            
            map.Add("numerocontatipo", "U_Tipo_Conta");
            map.Add("idtransacao", "U_TransId");
            map.Add("usuariogerouarquivo", "U_Usu_Gerou_Arquivo");
            map.Add("usuarioimportouconfirmacao", "U_Usu_Importou_Conf");
            map.Add("usuarioimportouliquidacao", "U_Usu_Importou_Liq");
         
            map.Add("valorbruto", "U_Valor_Bruto");
            map.Add("valtranmoeda", "U_Val_Tran_Moeda");        
            
            map.Add("valorpago", "U_Valor_Pago");
            map.Add("varcamreal", "U_Var_Cam_Real");
            map.Add("varcamnreal", "U_Var_Cam_Nreal");
            map.Add("retencoes", "U_Retencoes");

            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("recid", "T");
            map.Add("code", "T");
            map.Add("abatimentomanual", "N");
            map.Add("abatimentopagar", "N");
            map.Add("acrescimos", "N");
            map.Add("agencia", "N");
           
            map.Add("arquivogerado", "T");
            map.Add("arquivoimportado", "T");
            map.Add("banco", "N");
          
            map.Add("codigocliente", "T");
            map.Add("codigoclienteoriginal", "T");            
         
            map.Add("codigodiario", "T");
            map.Add("moeda", "T");
            map.Add("condicaopagamento", "N");
           
            map.Add("contacontrole", "T");
            map.Add("contarazao", "T");
            map.Add("desconto", "N");
            map.Add("descontoacordo", "N");
            map.Add("agenciadigito", "T");
           
            map.Add("contabancariadigito", "T");           
            map.Add("dataagendamento", "T");     
            map.Add("dataemissao", "T");
            map.Add("datageracao", "T");
            map.Add("datalancamento", "T");
       
            map.Add("dataliquida", "T");
            map.Add("datapagamento", "T");
            map.Add("datarecebimentoprevisto", "T");
            map.Add("datarecepcao", "T");
            map.Add("datavencimento", "T");
            map.Add("datavencimentooriginal", "T");
            map.Add("codigoempresa", "T");
            map.Add("codigofilial", "T");
            map.Add("formapagamento", "T");
           
            map.Add("juros", "N");           
            map.Add("idlinha", "N");
            map.Add("loteedi", "N");
            map.Add("meiopagamento", "N");
            map.Add("modelonf", "T");
          
            map.Add("motivobaixa", "N");
            map.Add("multa", "N");
            map.Add("nossonumero", "T");
            map.Add("notafiscal", "N");
            map.Add("numeroboleto", "N");
            map.Add("numeroconta", "T");
            map.Add("parcela", "N");
            
            map.Add("notafiscalserie", "T");
            map.Add("seunumero", "T");
            map.Add("situacao", "T");
            map.Add("status", "T");           
            map.Add("numerocontatipo", "N");
            map.Add("idtransacao", "T");
            map.Add("usuariogerouarquivo", "T");
            map.Add("usuarioimportouconfirmacao", "T");
            map.Add("usuarioimportouliquidacao", "T");
         
            map.Add("valorbruto", "N");
            map.Add("valtranmoeda", "N");
            map.Add("valorpago", "N");
            map.Add("varcamreal", "N");
            map.Add("varcamnreal", "N");
            map.Add("retencoes", "N");

            return map;
        }        
    }
}
