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
    public class VSITITEMService : IEntityService<Model.Integration.VSITITEM>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public VSITITEMService(ServiceLayerConnector serviceLayerConnector)
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

            table.name = "VSITINVOICEITEM";
            table.description = "Cadastro de VSITINVOICEITEM";
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

        public Task Delete(VSITITEM entity)
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
            string query = Global.MakeODataQuery("U_VSITINVOICEITEM/$count", null, filter.Count == 0 ? null : filter.ToArray(), null, 1, 0);
            string data = await _serviceLayerConnector.getQueryResult(query);
            page.Linhas = Convert.ToInt64(data);
            page.Paginas = (Convert.ToInt64(data) / size.Value) + 1;
            page.qtdPorPagina = size.Value == 0 ? Convert.ToInt64(data) : size.Value;
            return page;
        }
        async public Task<VSITITEM> Find(List<Criteria> criterias)
        {
            string recid = criterias[0].Value;
            string query = Global.BuildQuery($"U_VSITINVOICEITEM('{recid}')");

            string data = await _serviceLayerConnector.getQueryResult(query);

            ExpandoObject record = Global.parseQueryToObject(data);

            VSITITEM entidade = toRecord(record);

            // Recupera as linhas da nota iscal
            string[] filter = new string[]
            {
                $"Code eq '{recid}'"
            };

            query = Global.MakeODataQuery("U_VSITINVOICEITEM", null, filter);

            data = await _serviceLayerConnector.getQueryResult(query);

            return entidade;
        }

        async public Task Insert(VSITITEM entity)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();

            entity.status = Data.Model.Integration.VSITITEM.VSITITEMIntegrationStatus.Importing;
            string record = toJson(entity);
            batch.Post(HttpMethod.Post, "/U_VSITINVOICEITEM", record);

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

        public Task Insert(List<VSITITEM> entities)
        {
            throw new NotImplementedException();
        }

        async public Task<List<VSITITEM>> List(List<Criteria> criterias, long page, long size)
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

            string query = Global.MakeODataQuery("U_VSITINVOICEITEM", null, filter.Count == 0 ? null : filter.ToArray());

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            List<VSITITEM> result = new List<VSITITEM>();

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }

            return result;
        }

        public Task Update(VSITITEM entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<VSITITEM> entities)
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
            lista.Add(new TableColumn() { name = "COF_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "COF_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ISS_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "ISS_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CUS_UNI", dataType = "db_Float", mandatory = false, size = 15, description = "CUS_UNI", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PIS_CTB", dataType = "db_Float", mandatory = false, size = 15, description = "PIS_CTB", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "TFI_STD", dataType = "db_Float", mandatory = false, size = 15, description = "TFI_STD", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DFO_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "DFO_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "INS_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "INS_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "TFI_CST", dataType = "db_Numeric", mandatory = false, size = 11, description = "TFI_CST" });
            lista.Add(new TableColumn() { name = "COD_CTR", dataType = "db_Numeric", mandatory = false, size = 11, description = "COD_CTR" });
            lista.Add(new TableColumn() { name = "FCS_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "FCS_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "VAL_UNI_FST", dataType = "db_Float", mandatory = false, size = 15, description = "VAL_UNI_FST", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "NAT_REC", dataType = "db_Numeric", mandatory = false, size = 11, description = "NAT_REC" });
            lista.Add(new TableColumn() { name = "ISS_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "ISS_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "VAL_UNI_FST_CPL", dataType = "db_Float", mandatory = false, size = 15, description = "VAL_UNI_FST_CPL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "T06_CTB", dataType = "db_Alpha", mandatory = false, size = 100, description = "T06_CTB" });
            lista.Add(new TableColumn() { name = "VLR_PAU", dataType = "db_Float", mandatory = false, size = 15, description = "VLR_PAU", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "TIP_CRE", dataType = "db_Numeric", mandatory = false, size = 11, description = "TIP_CRE" });
            lista.Add(new TableColumn() { name = "ICM_ORI", dataType = "db_Float", mandatory = false, size = 15, description = "ICM_ORI", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "VAL_UNI_ICM_OPE", dataType = "db_Float", mandatory = false, size = 15, description = "VAL_UNI_ICM_OPE", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PIR_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "PIR_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DFD_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "DFD_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "COF_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "COF_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "IPI_ADI", dataType = "db_Float", mandatory = false, size = 15, description = "IPI_ADI", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "IPS_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "IPS_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "COF_CST", dataType = "db_Numeric", mandatory = false, size = 11, description = "COF_CST" });
            lista.Add(new TableColumn() { name = "FNT_FFI", dataType = "db_Float", mandatory = false, size = 15, description = "FNT_FFI", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "BAS_UNI_IST", dataType = "db_Float", mandatory = false, size = 15, description = "BAS_UNI_IST", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "VAL_UNI_FST_RES", dataType = "db_Float", mandatory = false, size = 15, description = "VAL_UNI_FST_RES", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PIS_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "PIS_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "COF_SIT", dataType = "db_Numeric", mandatory = false, size = 11, description = "COF_SIT" });
            lista.Add(new TableColumn() { name = "COR_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "COR_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FCP_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "FCP_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "INS_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "INS_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FCR_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "FCR_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FPS_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "FPS_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PIS_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "PIS_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "VAL_UNI_ICM_EST", dataType = "db_Float", mandatory = false, size = 15, description = "VAL_UNI_ICM_EST", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "VAL_UNI_IST_RES", dataType = "db_Float", mandatory = false, size = 15, description = "VAL_UNI_IST_RES", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "IPI_CST", dataType = "db_Numeric", mandatory = false, size = 11, description = "IPI_CST" });
            lista.Add(new TableColumn() { name = "T27_CSL", dataType = "db_Alpha", mandatory = false, size = 100, description = "T27_CSL" });
            lista.Add(new TableColumn() { name = "VAL_UNI_ICM", dataType = "db_Float", mandatory = false, size = 15, description = "VAL_UNI_ICM", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CSL_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "CSL_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CPL_TRB", dataType = "db_Float", mandatory = false, size = 15, description = "CPL_TRB", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "SEQ", dataType = "db_Numeric", mandatory = false, size = 11, description = "SEQ" });
            lista.Add(new TableColumn() { name = "COD_MOT", dataType = "db_Alpha", mandatory = false, size = 100, description = "COD_MOT" });
            lista.Add(new TableColumn() { name = "DFO_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "DFO_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CSL_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "CSL_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FPS_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "FPS_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FCP_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "FCP_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "T18_IPI", dataType = "db_Alpha", mandatory = false, size = 100, description = "T18_IPI" });
            lista.Add(new TableColumn() { name = "FCS_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "FCS_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "SIT", dataType = "db_Alpha", mandatory = false, size = 100, description = "SIT" });
            lista.Add(new TableColumn() { name = "ICM_OUT", dataType = "db_Float", mandatory = false, size = 15, description = "ICM_OUT", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ICM_DIF", dataType = "db_Float", mandatory = false, size = 15, description = "ICM_DIF", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DFD_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "DFD_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FNT_DIF", dataType = "db_Float", mandatory = false, size = 15, description = "FNT_DIF", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "T19_IRF", dataType = "db_Alpha", mandatory = false, size = 100, description = "T19_IRF" });
            lista.Add(new TableColumn() { name = "MER_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "MER_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "TFI_STR", dataType = "db_Float", mandatory = false, size = 15, description = "TFI_STR", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PIS_CAT", dataType = "db_Numeric", mandatory = false, size = 11, description = "PIS_CAT" });
            lista.Add(new TableColumn() { name = "PIR_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "PIR_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DES_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "DES_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FPS_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "FPS_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "TFI_OPE", dataType = "db_Numeric", mandatory = false, size = 11, description = "TFI_OPE" });
            lista.Add(new TableColumn() { name = "TFI_RDZ", dataType = "db_Float", mandatory = false, size = 15, description = "TFI_RDZ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "T26_ISS", dataType = "db_Alpha", mandatory = false, size = 100, description = "T26_ISS" });
            lista.Add(new TableColumn() { name = "FCP_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "FCP_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DFD_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "DFD_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DSP_FIN", dataType = "db_Float", mandatory = false, size = 15, description = "DSP_FIN", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "BAS_EMB", dataType = "db_Numeric", mandatory = false, size = 11, description = "BAS_EMB" });
            lista.Add(new TableColumn() { name = "ICM_RED", dataType = "db_Float", mandatory = false, size = 15, description = "ICM_RED", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PIS_CST", dataType = "db_Numeric", mandatory = false, size = 11, description = "PIS_CST" });
            lista.Add(new TableColumn() { name = "CSL_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "CSL_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "IPI_DIF", dataType = "db_Float", mandatory = false, size = 15, description = "IPI_DIF", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ICM_ISE", dataType = "db_Float", mandatory = false, size = 15, description = "ICM_ISE", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PIR_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "PIR_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ICM_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "ICM_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ICM_STR", dataType = "db_Float", mandatory = false, size = 15, description = "ICM_STR", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "T25_PIS", dataType = "db_Alpha", mandatory = false, size = 100, description = "T25_PIS" });
            lista.Add(new TableColumn() { name = "FCS_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "FCS_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "EFT_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "EFT_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FUN_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "FUN_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PVV_GRE", dataType = "db_Float", mandatory = false, size = 15, description = "PVV_GRE", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "QTD_UNI", dataType = "db_Float", mandatory = false, size = 15, description = "QTD_UNI", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PIS_SIT", dataType = "db_Numeric", mandatory = false, size = 11, description = "PIS_SIT" });
            lista.Add(new TableColumn() { name = "CTB_DIF", dataType = "db_Float", mandatory = false, size = 15, description = "CTB_DIF", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ICM_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "ICM_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "TFI_RDF", dataType = "db_Float", mandatory = false, size = 15, description = "TFI_RDF", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FNT_GRE", dataType = "db_Float", mandatory = false, size = 15, description = "FNT_GRE", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PVV_ADI", dataType = "db_Float", mandatory = false, size = 15, description = "PVV_ADI", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "SEC", dataType = "db_Numeric", mandatory = false, size = 11, description = "SEC" });
            lista.Add(new TableColumn() { name = "TFI_FNT", dataType = "db_Float", mandatory = false, size = 15, description = "TFI_FNT", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "IPS_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "IPS_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FNT_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "FNT_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FUN_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "FUN_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "IRF_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "IRF_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "INS_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "INS_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "BAS_ORI", dataType = "db_Float", mandatory = false, size = 15, description = "BAS_ORI", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ICM_NTR", dataType = "db_Float", mandatory = false, size = 15, description = "ICM_NTR", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "IPS_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "IPS_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CPL_ISE", dataType = "db_Float", mandatory = false, size = 15, description = "CPL_ISE", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "BAS_DIF", dataType = "db_Float", mandatory = false, size = 15, description = "BAS_DIF", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DES_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "DES_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "EFT_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "EFT_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DSC_TRB", dataType = "db_Float", mandatory = false, size = 15, description = "DSC_TRB", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DES_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "DES_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "COF_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "COF_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "IRF_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "IRF_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "T23_INS", dataType = "db_Alpha", mandatory = false, size = 100, description = "T23_INS" });
            lista.Add(new TableColumn() { name = "DSC_ISE", dataType = "db_Float", mandatory = false, size = 15, description = "DSC_ISE", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ICM_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "ICM_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "T05_REC", dataType = "db_Alpha", mandatory = false, size = 100, description = "T05_REC" });
            lista.Add(new TableColumn() { name = "T07_EST", dataType = "db_Alpha", mandatory = false, size = 100, description = "T07_EST" });
            lista.Add(new TableColumn() { name = "FCR_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "FCR_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "COF_CAT", dataType = "db_Numeric", mandatory = false, size = 11, description = "COF_CAT" });
            lista.Add(new TableColumn() { name = "DCR", dataType = "db_Alpha", mandatory = false, size = 100, description = "DCR" });
            lista.Add(new TableColumn() { name = "T08_CST", dataType = "db_Alpha", mandatory = false, size = 100, description = "T08_CST" });
            lista.Add(new TableColumn() { name = "COR_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "COR_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "QTD_FAT", dataType = "db_Float", mandatory = false, size = 15, description = "QTD_FAT", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "EFT_RED", dataType = "db_Float", mandatory = false, size = 15, description = "EFT_RED", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "VAL_UNI_IST_CPL", dataType = "db_Float", mandatory = false, size = 15, description = "VAL_UNI_IST_CPL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "AGE", dataType = "db_Numeric", mandatory = false, size = 11, description = "AGE" });
            lista.Add(new TableColumn() { name = "PRC_EMB", dataType = "db_Float", mandatory = false, size = 15, description = "PRC_EMB", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FCR_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "FCR_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CRD_PRE", dataType = "db_Float", mandatory = false, size = 15, description = "CRD_PRE", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PVV_FFI", dataType = "db_Float", mandatory = false, size = 15, description = "PVV_FFI", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "VAL_UNI_IST", dataType = "db_Float", mandatory = false, size = 15, description = "VAL_UNI_IST", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CFO", dataType = "db_Numeric", mandatory = false, size = 11, description = "CFO" });
            lista.Add(new TableColumn() { name = "ICM_FRO", dataType = "db_Numeric", mandatory = false, size = 11, description = "ICM_FRO" });
            lista.Add(new TableColumn() { name = "ICM_CST", dataType = "db_Numeric", mandatory = false, size = 11, description = "ICM_CST" });
            lista.Add(new TableColumn() { name = "TFI_SPN", dataType = "db_Numeric", mandatory = false, size = 11, description = "TFI_SPN" });
            lista.Add(new TableColumn() { name = "GRE", dataType = "db_Numeric", mandatory = false, size = 11, description = "GRE" });
            lista.Add(new TableColumn() { name = "PVV_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "PVV_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "T01_PAG", dataType = "db_Alpha", mandatory = false, size = 100, description = "T01_PAG" });
            lista.Add(new TableColumn() { name = "IPI_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "IPI_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ALQ_ORI", dataType = "db_Float", mandatory = false, size = 15, description = "ALQ_ORI", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DFO_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "DFO_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PIS_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "PIS_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ISE_DIF", dataType = "db_Float", mandatory = false, size = 15, description = "ISE_DIF", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CRF", dataType = "db_Numeric", mandatory = false, size = 11, description = "CRF" });
            lista.Add(new TableColumn() { name = "CTB_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "CTB_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ICM_OPE", dataType = "db_Numeric", mandatory = false, size = 11, description = "ICM_OPE" });
            lista.Add(new TableColumn() { name = "COF_CTB", dataType = "db_Float", mandatory = false, size = 15, description = "COF_CTB", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "COR_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "COR_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CPL_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "CPL_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FNT_ADI", dataType = "db_Float", mandatory = false, size = 15, description = "FNT_ADI", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ID", dataType = "db_Numeric", mandatory = false, size = 11, description = "ID" });
            lista.Add(new TableColumn() { name = "IPI_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "IPI_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "SEG_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "SEG_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DIF_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "DIF_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ITE", dataType = "db_Numeric", mandatory = false, size = 11, description = "ITE" });
            lista.Add(new TableColumn() { name = "FIG", dataType = "db_Numeric", mandatory = false, size = 11, description = "FIG" });
            lista.Add(new TableColumn() { name = "EFT_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "EFT_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "TPO_EMB", dataType = "db_Alpha", mandatory = false, size = 100, description = "TPO_EMB" });
            lista.Add(new TableColumn() { name = "FRT_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "FRT_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "TFI_ICM", dataType = "db_Float", mandatory = false, size = 15, description = "TFI_ICM", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FNT_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "FNT_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "IRF_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "IRF_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "NAT_CRE", dataType = "db_Numeric", mandatory = false, size = 11, description = "NAT_CRE" });
            lista.Add(new TableColumn() { name = "IPI_ALQ", dataType = "db_Float", mandatory = false, size = 15, description = "IPI_ALQ", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ISS_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "ISS_BAS", dataTypeSub = "st_Measurement" });


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

        private string toJson(VSITITEM entidade)
        {
            string result = string.Empty;

            dynamic record = new ExpandoObject();

            record.Code = entidade.RecId.ToString();
            record.Name = entidade.RecId.ToString();
            record.U_STATUS = (VSITITEM.VSITITEMIntegrationStatus.Created);
            record.U_LAST_UPDATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            record.U_DATA_INCLUSAO = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            record.U_COF_ALQ = entidade.cof_alq;
            record.U_ISS_VAL = entidade.iss_val;
            record.U_CUS_UNI = entidade.cus_uni;
            record.U_PIS_CTB = entidade.pis_ctb;
            record.U_TFI_STD = entidade.tfi_std;
            record.U_DFO_ALQ = entidade.dfo_alq;
            record.U_INS_ALQ = entidade.ins_alq;
            record.U_TFI_CST = entidade.tfi_cst;
            record.U_COD_CTR = entidade.cod_ctr;
            record.U_FCS_ALQ = entidade.fcs_alq;
            record.U_VAL_UNI_FST = entidade.val_uni_fst;
            record.U_NAT_REC = entidade.nat_rec;
            record.U_ISS_ALQ = entidade.iss_alq;
            record.U_VAL_UNI_FST_CPL = entidade.val_uni_fst_cpl;
            record.U_T06_CTB = entidade.t06_ctb;
            record.U_VLR_PAU = entidade.vlr_pau;
            record.U_TIP_CRE = entidade.tip_cre;
            record.U_ICM_ORI = entidade.icm_ori;
            record.U_VAL_UNI_ICM_OPE = entidade.val_uni_icm_ope;
            record.U_PIR_BAS = entidade.pir_bas;
            record.U_DFD_ALQ = entidade.dfd_alq;
            record.U_COF_BAS = entidade.cof_bas;
            record.U_IPI_ADI = entidade.ipi_adi;
            record.U_IPS_ALQ = entidade.ips_alq;
            record.U_COF_CST = entidade.cof_cst;
            record.U_FNT_FFI = entidade.fnt_ffi;
            record.U_BAS_UNI_IST = entidade.bas_uni_ist;
            record.U_VAL_UNI_FST_RES = entidade.val_uni_fst_res;
            record.U_PIS_BAS = entidade.pis_bas;
            record.U_COF_SIT = entidade.cof_sit;
            record.U_COR_ALQ = entidade.cor_alq;
            record.U_FCP_ALQ = entidade.fcp_alq;
            record.U_INS_VAL = entidade.ins_val;
            record.U_FCR_VAL = entidade.fcr_val;
            record.U_FPS_VAL = entidade.fps_val;
            record.U_PIS_ALQ = entidade.pis_alq;
            record.U_VAL_UNI_ICM_EST = entidade.val_uni_icm_est;
            record.U_VAL_UNI_IST_RES = entidade.val_uni_ist_res;
            record.U_IPI_CST = entidade.ipi_cst;
            record.U_T27_CSL = entidade.t27_csl;
            record.U_VAL_UNI_ICM = entidade.val_uni_icm;
            record.U_CSL_ALQ = entidade.csl_alq;
            record.U_CPL_TRB = entidade.cpl_trb;
            record.U_SEQ = entidade.seq;
            record.U_COD_MOT = entidade.cod_mot;
            record.U_DFO_BAS = entidade.dfo_bas;
            record.U_CSL_VAL = entidade.csl_val;
            record.U_FPS_BAS = entidade.fps_bas;
            record.U_FCP_VAL = entidade.fcp_val;
            record.U_T18_IPI = entidade.t18_ipi;
            record.U_FCS_VAL = entidade.fcs_val;
            record.U_SIT = entidade.sit;
            record.U_ICM_OUT = entidade.icm_out;
            record.U_ICM_DIF = entidade.icm_dif;
            record.U_DFD_BAS = entidade.dfd_bas;
            record.U_FNT_DIF = entidade.fnt_dif;
            record.U_T19_IRF = entidade.t19_irf;
            record.U_MER_VAL = entidade.mer_val;
            record.U_TFI_STR = entidade.tfi_str;
            record.U_PIS_CAT = entidade.pis_cat;
            record.U_PIR_ALQ = entidade.pir_alq;
            record.U_DES_ALQ = entidade.des_alq;
            record.U_FPS_ALQ = entidade.fps_alq;
            record.U_TFI_OPE = entidade.tfi_ope;
            record.U_TFI_RDZ = entidade.tfi_rdz;
            record.U_T26_ISS = entidade.t26_iss;
            record.U_FCP_BAS = entidade.fcp_bas;
            record.U_DFD_VAL = entidade.dfd_val;
            record.U_DSP_FIN = entidade.dsp_fin;
            record.U_BAS_EMB = entidade.bas_emb;
            record.U_ICM_RED = entidade.icm_red;
            record.U_PIS_CST = entidade.pis_cst;
            record.U_CSL_BAS = entidade.csl_bas;
            record.U_IPI_DIF = entidade.ipi_dif;
            record.U_ICM_ISE = entidade.icm_ise;
            record.U_PIR_VAL = entidade.pir_val;
            record.U_ICM_VAL = entidade.icm_val;
            record.U_ICM_STR = entidade.icm_str;
            record.U_T25_PIS = entidade.t25_pis;
            record.U_FCS_BAS = entidade.fcs_bas;
            record.U_EFT_VAL = entidade.eft_val;
            record.U_FUN_ALQ = entidade.fun_alq;
            record.U_PVV_GRE = entidade.pvv_gre;
            record.U_QTD_UNI = entidade.qtd_uni;
            record.U_PIS_SIT = entidade.pis_sit;
            record.U_CTB_DIF = entidade.ctb_dif;
            record.U_ICM_ALQ = entidade.icm_alq;
            record.U_TFI_RDF = entidade.tfi_rdf;
            record.U_FNT_GRE = entidade.fnt_gre;
            record.U_PVV_ADI = entidade.pvv_adi;
            record.U_SEC = entidade.sec;
            record.U_TFI_FNT = entidade.tfi_fnt;
            record.U_IPS_BAS = entidade.ips_bas;
            record.U_FNT_ALQ = entidade.fnt_alq;
            record.U_FUN_VAL = entidade.fun_val;
            record.U_IRF_VAL = entidade.irf_val;
            record.U_INS_BAS = entidade.ins_bas;
            record.U_BAS_ORI = entidade.bas_ori;
            record.U_ICM_NTR = entidade.icm_ntr;
            record.U_IPS_VAL = entidade.ips_val;
            record.U_CPL_ISE = entidade.cpl_ise;
            record.U_BAS_DIF = entidade.bas_dif;
            record.U_DES_VAL = entidade.des_val;
            record.U_EFT_BAS = entidade.eft_bas;
            record.U_DSC_TRB = entidade.dsc_trb;
            record.U_DES_BAS = entidade.des_bas;
            record.U_COF_VAL = entidade.cof_val;
            record.U_IRF_ALQ = entidade.irf_alq;
            record.U_T23_INS = entidade.t23_ins;
            record.U_DSC_ISE = entidade.dsc_ise;
            record.U_ICM_BAS = entidade.icm_bas;
            record.U_T05_REC = entidade.t05_rec;
            record.U_T07_EST = entidade.t07_est;
            record.U_FCR_ALQ = entidade.fcr_alq;
            record.U_COF_CAT = entidade.cof_cat;
            record.U_DCR = entidade.dcr;
            record.U_T08_CST = entidade.t08_cst;
            record.U_COR_VAL = entidade.cor_val;
            record.U_QTD_FAT = entidade.qtd_fat;
            record.U_EFT_RED = entidade.eft_red;
            record.U_VAL_UNI_IST_CPL = entidade.val_uni_ist_cpl;
            record.U_AGE = entidade.age;
            record.U_PRC_EMB = entidade.prc_emb;
            record.U_FCR_BAS = entidade.fcr_bas;
            record.U_CRD_PRE = entidade.crd_pre;
            record.U_PVV_FFI = entidade.pvv_ffi;
            record.U_VAL_UNI_IST = entidade.val_uni_ist;
            record.U_CFO = entidade.cfo;
            record.U_ICM_FRO = entidade.icm_fro;
            record.U_ICM_CST = entidade.icm_cst;
            record.U_TFI_SPN = entidade.tfi_spn;
            record.U_GRE = entidade.gre;
            record.U_PVV_VAL = entidade.pvv_val;
            record.U_T01_PAG = entidade.t01_pag;
            record.U_IPI_VAL = entidade.ipi_val;
            record.U_ALQ_ORI = entidade.alq_ori;
            record.U_DFO_VAL = entidade.dfo_val;
            record.U_PIS_VAL = entidade.pis_val;
            record.U_ISE_DIF = entidade.ise_dif;
            record.U_CRF = entidade.crf;
            record.U_CTB_VAL = entidade.ctb_val;
            record.U_ICM_OPE = entidade.icm_ope;
            record.U_COF_CTB = entidade.cof_ctb;
            record.U_COR_BAS = entidade.cor_bas;
            record.U_CPL_VAL = entidade.cpl_val;
            record.U_FNT_ADI = entidade.fnt_adi;
            record.U_ID = entidade.id;
            record.U_IPI_BAS = entidade.ipi_bas;
            record.U_SEG_VAL = entidade.seg_val;
            record.U_DIF_VAL = entidade.dif_val;
            record.U_ITE = entidade.ite;
            record.U_FIG = entidade.fig;
            record.U_EFT_ALQ = entidade.eft_alq;
            record.U_TPO_EMB = entidade.tpo_emb;
            record.U_FRT_VAL = entidade.frt_val;
            record.U_TFI_ICM = entidade.tfi_icm;
            record.U_FNT_VAL = entidade.fnt_val;
            record.U_IRF_BAS = entidade.irf_bas;
            record.U_NAT_CRE = entidade.nat_cre;
            record.U_IPI_ALQ = entidade.ipi_alq;
            record.U_ISS_BAS = entidade.iss_bas;
            result = JsonConvert.SerializeObject(record);
            return result;
        }

        private VSITITEM toRecord(dynamic record)
        {
            VSITITEM entidade = new VSITITEM();

            entidade.RecId = Guid.Parse(record.Code);
            entidade.status = (VSITITEM.VSITITEMIntegrationStatus)record.U_STATUS;
            entidade.lastupdate = record.U_LAST_UPDATE;
            entidade.cof_alq = record.U_COF_ALQ;
            entidade.iss_val = record.U_ISS_VAL;
            entidade.cus_uni = record.U_CUS_UNI;
            entidade.pis_ctb = record.U_PIS_CTB;
            entidade.tfi_std = record.U_TFI_STD;
            entidade.dfo_alq = record.U_DFO_ALQ;
            entidade.ins_alq = record.U_INS_ALQ;
            entidade.tfi_cst = record.U_TFI_CST;
            entidade.cod_ctr = record.U_COD_CTR;
            entidade.fcs_alq = record.U_FCS_ALQ;
            entidade.val_uni_fst = record.U_VAL_UNI_FST;
            entidade.nat_rec = record.U_NAT_REC;
            entidade.iss_alq = record.U_ISS_ALQ;
            entidade.val_uni_fst_cpl = record.U_VAL_UNI_FST_CPL;
            entidade.t06_ctb = record.U_T06_CTB;
            entidade.vlr_pau = record.U_VLR_PAU;
            entidade.tip_cre = record.U_TIP_CRE;
            entidade.icm_ori = record.U_ICM_ORI;
            entidade.val_uni_icm_ope = record.U_VAL_UNI_ICM_OPE;
            entidade.pir_bas = record.U_PIR_BAS;
            entidade.dfd_alq = record.U_DFD_ALQ;
            entidade.cof_bas = record.U_COF_BAS;
            entidade.ipi_adi = record.U_IPI_ADI;
            entidade.ips_alq = record.U_IPS_ALQ;
            entidade.cof_cst = record.U_COF_CST;
            entidade.fnt_ffi = record.U_FNT_FFI;
            entidade.bas_uni_ist = record.U_BAS_UNI_IST;
            entidade.val_uni_fst_res = record.U_VAL_UNI_FST_RES;
            entidade.pis_bas = record.U_PIS_BAS;
            entidade.cof_sit = record.U_COF_SIT;
            entidade.cor_alq = record.U_COR_ALQ;
            entidade.fcp_alq = record.U_FCP_ALQ;
            entidade.ins_val = record.U_INS_VAL;
            entidade.fcr_val = record.U_FCR_VAL;
            entidade.fps_val = record.U_FPS_VAL;
            entidade.pis_alq = record.U_PIS_ALQ;
            entidade.val_uni_icm_est = record.U_VAL_UNI_ICM_EST;
            entidade.val_uni_ist_res = record.U_VAL_UNI_IST_RES;
            entidade.ipi_cst = record.U_IPI_CST;
            entidade.t27_csl = record.U_T27_CSL;
            entidade.val_uni_icm = record.U_VAL_UNI_ICM;
            entidade.csl_alq = record.U_CSL_ALQ;
            entidade.cpl_trb = record.U_CPL_TRB;
            entidade.seq = record.U_SEQ;
            entidade.cod_mot = record.U_COD_MOT;
            entidade.dfo_bas = record.U_DFO_BAS;
            entidade.csl_val = record.U_CSL_VAL;
            entidade.fps_bas = record.U_FPS_BAS;
            entidade.fcp_val = record.U_FCP_VAL;
            entidade.t18_ipi = record.U_T18_IPI;
            entidade.fcs_val = record.U_FCS_VAL;
            entidade.sit = record.U_SIT;
            entidade.icm_out = record.U_ICM_OUT;
            entidade.icm_dif = record.U_ICM_DIF;
            entidade.dfd_bas = record.U_DFD_BAS;
            entidade.fnt_dif = record.U_FNT_DIF;
            entidade.t19_irf = record.U_T19_IRF;
            entidade.mer_val = record.U_MER_VAL;
            entidade.tfi_str = record.U_TFI_STR;
            entidade.pis_cat = record.U_PIS_CAT;
            entidade.pir_alq = record.U_PIR_ALQ;
            entidade.des_alq = record.U_DES_ALQ;
            entidade.fps_alq = record.U_FPS_ALQ;
            entidade.tfi_ope = record.U_TFI_OPE;
            entidade.tfi_rdz = record.U_TFI_RDZ;
            entidade.t26_iss = record.U_T26_ISS;
            entidade.fcp_bas = record.U_FCP_BAS;
            entidade.dfd_val = record.U_DFD_VAL;
            entidade.dsp_fin = record.U_DSP_FIN;
            entidade.bas_emb = record.U_BAS_EMB;
            entidade.icm_red = record.U_ICM_RED;
            entidade.pis_cst = record.U_PIS_CST;
            entidade.csl_bas = record.U_CSL_BAS;
            entidade.ipi_dif = record.U_IPI_DIF;
            entidade.icm_ise = record.U_ICM_ISE;
            entidade.pir_val = record.U_PIR_VAL;
            entidade.icm_val = record.U_ICM_VAL;
            entidade.icm_str = record.U_ICM_STR;
            entidade.t25_pis = record.U_T25_PIS;
            entidade.fcs_bas = record.U_FCS_BAS;
            entidade.eft_val = record.U_EFT_VAL;
            entidade.fun_alq = record.U_FUN_ALQ;
            entidade.pvv_gre = record.U_PVV_GRE;
            entidade.qtd_uni = record.U_QTD_UNI;
            entidade.pis_sit = record.U_PIS_SIT;
            entidade.ctb_dif = record.U_CTB_DIF;
            entidade.icm_alq = record.U_ICM_ALQ;
            entidade.tfi_rdf = record.U_TFI_RDF;
            entidade.fnt_gre = record.U_FNT_GRE;
            entidade.pvv_adi = record.U_PVV_ADI;
            entidade.sec = record.U_SEC;
            entidade.tfi_fnt = record.U_TFI_FNT;
            entidade.ips_bas = record.U_IPS_BAS;
            entidade.fnt_alq = record.U_FNT_ALQ;
            entidade.fun_val = record.U_FUN_VAL;
            entidade.irf_val = record.U_IRF_VAL;
            entidade.ins_bas = record.U_INS_BAS;
            entidade.bas_ori = record.U_BAS_ORI;
            entidade.icm_ntr = record.U_ICM_NTR;
            entidade.ips_val = record.U_IPS_VAL;
            entidade.cpl_ise = record.U_CPL_ISE;
            entidade.bas_dif = record.U_BAS_DIF;
            entidade.des_val = record.U_DES_VAL;
            entidade.eft_bas = record.U_EFT_BAS;
            entidade.dsc_trb = record.U_DSC_TRB;
            entidade.des_bas = record.U_DES_BAS;
            entidade.cof_val = record.U_COF_VAL;
            entidade.irf_alq = record.U_IRF_ALQ;
            entidade.t23_ins = record.U_T23_INS;
            entidade.dsc_ise = record.U_DSC_ISE;
            entidade.icm_bas = record.U_ICM_BAS;
            entidade.t05_rec = record.U_T05_REC;
            entidade.t07_est = record.U_T07_EST;
            entidade.fcr_alq = record.U_FCR_ALQ;
            entidade.cof_cat = record.U_COF_CAT;
            entidade.dcr = record.U_DCR;
            entidade.t08_cst = record.U_T08_CST;
            entidade.cor_val = record.U_COR_VAL;
            entidade.qtd_fat = record.U_QTD_FAT;
            entidade.eft_red = record.U_EFT_RED;
            entidade.val_uni_ist_cpl = record.U_VAL_UNI_IST_CPL;
            entidade.age = record.U_AGE;
            entidade.prc_emb = record.U_PRC_EMB;
            entidade.fcr_bas = record.U_FCR_BAS;
            entidade.crd_pre = record.U_CRD_PRE;
            entidade.pvv_ffi = record.U_PVV_FFI;
            entidade.val_uni_ist = record.U_VAL_UNI_IST;
            entidade.cfo = record.U_CFO;
            entidade.icm_fro = record.U_ICM_FRO;
            entidade.icm_cst = record.U_ICM_CST;
            entidade.tfi_spn = record.U_TFI_SPN;
            entidade.gre = record.U_GRE;
            entidade.pvv_val = record.U_PVV_VAL;
            entidade.t01_pag = record.U_T01_PAG;
            entidade.ipi_val = record.U_IPI_VAL;
            entidade.alq_ori = record.U_ALQ_ORI;
            entidade.dfo_val = record.U_DFO_VAL;
            entidade.pis_val = record.U_PIS_VAL;
            entidade.ise_dif = record.U_ISE_DIF;
            entidade.crf = record.U_CRF;
            entidade.ctb_val = record.U_CTB_VAL;
            entidade.icm_ope = record.U_ICM_OPE;
            entidade.cof_ctb = record.U_COF_CTB;
            entidade.cor_bas = record.U_COR_BAS;
            entidade.cpl_val = record.U_CPL_VAL;
            entidade.fnt_adi = record.U_FNT_ADI;
            entidade.id = record.U_ID;
            entidade.ipi_bas = record.U_IPI_BAS;
            entidade.seg_val = record.U_SEG_VAL;
            entidade.dif_val = record.U_DIF_VAL;
            entidade.ite = record.U_ITE;
            entidade.fig = record.U_FIG;
            entidade.eft_alq = record.U_EFT_ALQ;
            entidade.tpo_emb = record.U_TPO_EMB;
            entidade.frt_val = record.U_FRT_VAL;
            entidade.tfi_icm = record.U_TFI_ICM;
            entidade.fnt_val = record.U_FNT_VAL;
            entidade.irf_bas = record.U_IRF_BAS;
            entidade.nat_cre = record.U_NAT_CRE;
            entidade.ipi_alq = record.U_IPI_ALQ;
            entidade.iss_bas = record.U_ISS_BAS;
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
            map.Add("cof_alq", "U_COF_ALQ");
            map.Add("iss_val", "U_ISS_VAL");
            map.Add("cus_uni", "U_CUS_UNI");
            map.Add("pis_ctb", "U_PIS_CTB");
            map.Add("tfi_std", "U_TFI_STD");
            map.Add("dfo_alq", "U_DFO_ALQ");
            map.Add("ins_alq", "U_INS_ALQ");
            map.Add("tfi_cst", "U_TFI_CST");
            map.Add("cod_ctr", "U_COD_CTR");
            map.Add("fcs_alq", "U_FCS_ALQ");
            map.Add("val_uni_fst", "U_VAL_UNI_FST");
            map.Add("nat_rec", "U_NAT_REC");
            map.Add("iss_alq", "U_ISS_ALQ");
            map.Add("val_uni_fst_cpl", "U_VAL_UNI_FST_CPL");
            map.Add("t06_ctb", "U_T06_CTB");
            map.Add("vlr_pau", "U_VLR_PAU");
            map.Add("tip_cre", "U_TIP_CRE");
            map.Add("icm_ori", "U_ICM_ORI");
            map.Add("val_uni_icm_ope", "U_VAL_UNI_ICM_OPE");
            map.Add("pir_bas", "U_PIR_BAS");
            map.Add("dfd_alq", "U_DFD_ALQ");
            map.Add("cof_bas", "U_COF_BAS");
            map.Add("ipi_adi", "U_IPI_ADI");
            map.Add("ips_alq", "U_IPS_ALQ");
            map.Add("cof_cst", "U_COF_CST");
            map.Add("fnt_ffi", "U_FNT_FFI");
            map.Add("bas_uni_ist", "U_BAS_UNI_IST");
            map.Add("val_uni_fst_res", "U_VAL_UNI_FST_RES");
            map.Add("pis_bas", "U_PIS_BAS");
            map.Add("cof_sit", "U_COF_SIT");
            map.Add("cor_alq", "U_COR_ALQ");
            map.Add("fcp_alq", "U_FCP_ALQ");
            map.Add("ins_val", "U_INS_VAL");
            map.Add("fcr_val", "U_FCR_VAL");
            map.Add("fps_val", "U_FPS_VAL");
            map.Add("pis_alq", "U_PIS_ALQ");
            map.Add("val_uni_icm_est", "U_VAL_UNI_ICM_EST");
            map.Add("val_uni_ist_res", "U_VAL_UNI_IST_RES");
            map.Add("ipi_cst", "U_IPI_CST");
            map.Add("t27_csl", "U_T27_CSL");
            map.Add("val_uni_icm", "U_VAL_UNI_ICM");
            map.Add("csl_alq", "U_CSL_ALQ");
            map.Add("cpl_trb", "U_CPL_TRB");
            map.Add("seq", "U_SEQ");
            map.Add("cod_mot", "U_COD_MOT");
            map.Add("dfo_bas", "U_DFO_BAS");
            map.Add("csl_val", "U_CSL_VAL");
            map.Add("fps_bas", "U_FPS_BAS");
            map.Add("fcp_val", "U_FCP_VAL");
            map.Add("t18_ipi", "U_T18_IPI");
            map.Add("fcs_val", "U_FCS_VAL");
            map.Add("sit", "U_SIT");
            map.Add("icm_out", "U_ICM_OUT");
            map.Add("icm_dif", "U_ICM_DIF");
            map.Add("dfd_bas", "U_DFD_BAS");
            map.Add("fnt_dif", "U_FNT_DIF");
            map.Add("t19_irf", "U_T19_IRF");
            map.Add("mer_val", "U_MER_VAL");
            map.Add("tfi_str", "U_TFI_STR");
            map.Add("pis_cat", "U_PIS_CAT");
            map.Add("pir_alq", "U_PIR_ALQ");
            map.Add("des_alq", "U_DES_ALQ");
            map.Add("fps_alq", "U_FPS_ALQ");
            map.Add("tfi_ope", "U_TFI_OPE");
            map.Add("tfi_rdz", "U_TFI_RDZ");
            map.Add("t26_iss", "U_T26_ISS");
            map.Add("fcp_bas", "U_FCP_BAS");
            map.Add("dfd_val", "U_DFD_VAL");
            map.Add("dsp_fin", "U_DSP_FIN");
            map.Add("bas_emb", "U_BAS_EMB");
            map.Add("icm_red", "U_ICM_RED");
            map.Add("pis_cst", "U_PIS_CST");
            map.Add("csl_bas", "U_CSL_BAS");
            map.Add("ipi_dif", "U_IPI_DIF");
            map.Add("icm_ise", "U_ICM_ISE");
            map.Add("pir_val", "U_PIR_VAL");
            map.Add("icm_val", "U_ICM_VAL");
            map.Add("icm_str", "U_ICM_STR");
            map.Add("t25_pis", "U_T25_PIS");
            map.Add("fcs_bas", "U_FCS_BAS");
            map.Add("eft_val", "U_EFT_VAL");
            map.Add("fun_alq", "U_FUN_ALQ");
            map.Add("pvv_gre", "U_PVV_GRE");
            map.Add("qtd_uni", "U_QTD_UNI");
            map.Add("pis_sit", "U_PIS_SIT");
            map.Add("ctb_dif", "U_CTB_DIF");
            map.Add("icm_alq", "U_ICM_ALQ");
            map.Add("tfi_rdf", "U_TFI_RDF");
            map.Add("fnt_gre", "U_FNT_GRE");
            map.Add("pvv_adi", "U_PVV_ADI");
            map.Add("sec", "U_SEC");
            map.Add("tfi_fnt", "U_TFI_FNT");
            map.Add("ips_bas", "U_IPS_BAS");
            map.Add("fnt_alq", "U_FNT_ALQ");
            map.Add("fun_val", "U_FUN_VAL");
            map.Add("irf_val", "U_IRF_VAL");
            map.Add("ins_bas", "U_INS_BAS");
            map.Add("bas_ori", "U_BAS_ORI");
            map.Add("icm_ntr", "U_ICM_NTR");
            map.Add("ips_val", "U_IPS_VAL");
            map.Add("cpl_ise", "U_CPL_ISE");
            map.Add("bas_dif", "U_BAS_DIF");
            map.Add("des_val", "U_DES_VAL");
            map.Add("eft_bas", "U_EFT_BAS");
            map.Add("dsc_trb", "U_DSC_TRB");
            map.Add("des_bas", "U_DES_BAS");
            map.Add("cof_val", "U_COF_VAL");
            map.Add("irf_alq", "U_IRF_ALQ");
            map.Add("t23_ins", "U_T23_INS");
            map.Add("dsc_ise", "U_DSC_ISE");
            map.Add("icm_bas", "U_ICM_BAS");
            map.Add("t05_rec", "U_T05_REC");
            map.Add("t07_est", "U_T07_EST");
            map.Add("fcr_alq", "U_FCR_ALQ");
            map.Add("cof_cat", "U_COF_CAT");
            map.Add("dcr", "U_DCR");
            map.Add("t08_cst", "U_T08_CST");
            map.Add("cor_val", "U_COR_VAL");
            map.Add("qtd_fat", "U_QTD_FAT");
            map.Add("eft_red", "U_EFT_RED");
            map.Add("val_uni_ist_cpl", "U_VAL_UNI_IST_CPL");
            map.Add("age", "U_AGE");
            map.Add("prc_emb", "U_PRC_EMB");
            map.Add("fcr_bas", "U_FCR_BAS");
            map.Add("crd_pre", "U_CRD_PRE");
            map.Add("pvv_ffi", "U_PVV_FFI");
            map.Add("val_uni_ist", "U_VAL_UNI_IST");
            map.Add("cfo", "U_CFO");
            map.Add("icm_fro", "U_ICM_FRO");
            map.Add("icm_cst", "U_ICM_CST");
            map.Add("tfi_spn", "U_TFI_SPN");
            map.Add("gre", "U_GRE");
            map.Add("pvv_val", "U_PVV_VAL");
            map.Add("t01_pag", "U_T01_PAG");
            map.Add("ipi_val", "U_IPI_VAL");
            map.Add("alq_ori", "U_ALQ_ORI");
            map.Add("dfo_val", "U_DFO_VAL");
            map.Add("pis_val", "U_PIS_VAL");
            map.Add("ise_dif", "U_ISE_DIF");
            map.Add("crf", "U_CRF");
            map.Add("ctb_val", "U_CTB_VAL");
            map.Add("icm_ope", "U_ICM_OPE");
            map.Add("cof_ctb", "U_COF_CTB");
            map.Add("cor_bas", "U_COR_BAS");
            map.Add("cpl_val", "U_CPL_VAL");
            map.Add("fnt_adi", "U_FNT_ADI");
            map.Add("id", "U_ID");
            map.Add("ipi_bas", "U_IPI_BAS");
            map.Add("seg_val", "U_SEG_VAL");
            map.Add("dif_val", "U_DIF_VAL");
            map.Add("ite", "U_ITE");
            map.Add("fig", "U_FIG");
            map.Add("eft_alq", "U_EFT_ALQ");
            map.Add("tpo_emb", "U_TPO_EMB");
            map.Add("frt_val", "U_FRT_VAL");
            map.Add("tfi_icm", "U_TFI_ICM");
            map.Add("fnt_val", "U_FNT_VAL");
            map.Add("irf_bas", "U_IRF_BAS");
            map.Add("nat_cre", "U_NAT_CRE");
            map.Add("ipi_alq", "U_IPI_ALQ");
            map.Add("iss_bas", "U_ISS_BAS");
            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("status", "N");
            map.Add("cof_alq", "T");
            map.Add("iss_val", "T");
            map.Add("cus_uni", "T");
            map.Add("pis_ctb", "T");
            map.Add("tfi_std", "T");
            map.Add("dfo_alq", "T");
            map.Add("ins_alq", "T");
            map.Add("tfi_cst", "T");
            map.Add("cod_ctr", "T");
            map.Add("fcs_alq", "T");
            map.Add("val_uni_fst", "T");
            map.Add("nat_rec", "T");
            map.Add("iss_alq", "T");
            map.Add("val_uni_fst_cpl", "T");
            map.Add("t06_ctb", "N");
            map.Add("vlr_pau", "T");
            map.Add("tip_cre", "T");
            map.Add("icm_ori", "T");
            map.Add("val_uni_icm_ope", "T");
            map.Add("pir_bas", "T");
            map.Add("dfd_alq", "T");
            map.Add("cof_bas", "T");
            map.Add("ipi_adi", "T");
            map.Add("ips_alq", "T");
            map.Add("cof_cst", "T");
            map.Add("fnt_ffi", "T");
            map.Add("bas_uni_ist", "T");
            map.Add("val_uni_fst_res", "T");
            map.Add("pis_bas", "T");
            map.Add("cof_sit", "T");
            map.Add("cor_alq", "T");
            map.Add("fcp_alq", "T");
            map.Add("ins_val", "T");
            map.Add("fcr_val", "T");
            map.Add("fps_val", "T");
            map.Add("pis_alq", "T");
            map.Add("val_uni_icm_est", "T");
            map.Add("val_uni_ist_res", "T");
            map.Add("ipi_cst", "T");
            map.Add("t27_csl", "N");
            map.Add("val_uni_icm", "T");
            map.Add("csl_alq", "T");
            map.Add("cpl_trb", "T");
            map.Add("seq", "T");
            map.Add("cod_mot", "N");
            map.Add("dfo_bas", "T");
            map.Add("csl_val", "T");
            map.Add("fps_bas", "T");
            map.Add("fcp_val", "T");
            map.Add("t18_ipi", "N");
            map.Add("fcs_val", "T");
            map.Add("sit", "N");
            map.Add("icm_out", "T");
            map.Add("icm_dif", "T");
            map.Add("dfd_bas", "T");
            map.Add("fnt_dif", "T");
            map.Add("t19_irf", "N");
            map.Add("mer_val", "T");
            map.Add("tfi_str", "T");
            map.Add("pis_cat", "T");
            map.Add("pir_alq", "T");
            map.Add("des_alq", "T");
            map.Add("fps_alq", "T");
            map.Add("tfi_ope", "T");
            map.Add("tfi_rdz", "T");
            map.Add("t26_iss", "N");
            map.Add("fcp_bas", "T");
            map.Add("dfd_val", "T");
            map.Add("dsp_fin", "T");
            map.Add("bas_emb", "T");
            map.Add("icm_red", "T");
            map.Add("pis_cst", "T");
            map.Add("csl_bas", "T");
            map.Add("ipi_dif", "T");
            map.Add("icm_ise", "T");
            map.Add("pir_val", "T");
            map.Add("icm_val", "T");
            map.Add("icm_str", "T");
            map.Add("t25_pis", "N");
            map.Add("fcs_bas", "T");
            map.Add("eft_val", "T");
            map.Add("fun_alq", "T");
            map.Add("pvv_gre", "T");
            map.Add("qtd_uni", "T");
            map.Add("pis_sit", "T");
            map.Add("ctb_dif", "T");
            map.Add("icm_alq", "T");
            map.Add("tfi_rdf", "T");
            map.Add("fnt_gre", "T");
            map.Add("pvv_adi", "T");
            map.Add("sec", "T");
            map.Add("tfi_fnt", "T");
            map.Add("ips_bas", "T");
            map.Add("fnt_alq", "T");
            map.Add("fun_val", "T");
            map.Add("irf_val", "T");
            map.Add("ins_bas", "T");
            map.Add("bas_ori", "T");
            map.Add("icm_ntr", "T");
            map.Add("ips_val", "T");
            map.Add("cpl_ise", "T");
            map.Add("bas_dif", "T");
            map.Add("des_val", "T");
            map.Add("eft_bas", "T");
            map.Add("dsc_trb", "T");
            map.Add("des_bas", "T");
            map.Add("cof_val", "T");
            map.Add("irf_alq", "T");
            map.Add("t23_ins", "N");
            map.Add("dsc_ise", "T");
            map.Add("icm_bas", "T");
            map.Add("t05_rec", "N");
            map.Add("t07_est", "N");
            map.Add("fcr_alq", "T");
            map.Add("cof_cat", "T");
            map.Add("dcr", "N");
            map.Add("t08_cst", "N");
            map.Add("cor_val", "T");
            map.Add("qtd_fat", "T");
            map.Add("eft_red", "T");
            map.Add("val_uni_ist_cpl", "T");
            map.Add("age", "T");
            map.Add("prc_emb", "T");
            map.Add("fcr_bas", "T");
            map.Add("crd_pre", "T");
            map.Add("pvv_ffi", "T");
            map.Add("val_uni_ist", "T");
            map.Add("cfo", "T");
            map.Add("icm_fro", "T");
            map.Add("icm_cst", "T");
            map.Add("tfi_spn", "T");
            map.Add("gre", "T");
            map.Add("pvv_val", "T");
            map.Add("t01_pag", "N");
            map.Add("ipi_val", "T");
            map.Add("alq_ori", "T");
            map.Add("dfo_val", "T");
            map.Add("pis_val", "T");
            map.Add("ise_dif", "T");
            map.Add("crf", "T");
            map.Add("ctb_val", "T");
            map.Add("icm_ope", "T");
            map.Add("cof_ctb", "T");
            map.Add("cor_bas", "T");
            map.Add("cpl_val", "T");
            map.Add("fnt_adi", "T");
            map.Add("id", "N");
            map.Add("ipi_bas", "T");
            map.Add("seg_val", "T");
            map.Add("dif_val", "T");
            map.Add("ite", "T");
            map.Add("fig", "T");
            map.Add("eft_alq", "T");
            map.Add("tpo_emb", "N");
            map.Add("frt_val", "T");
            map.Add("tfi_icm", "T");
            map.Add("fnt_val", "T");
            map.Add("irf_bas", "T");
            map.Add("nat_cre", "T");
            map.Add("ipi_alq", "T");
            map.Add("iss_bas", "T");

            return map;
        }
    }
}
