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
    public class VSITINVOICEService : IEntityService<Model.Integration.VSITINVOICE>
    {
        readonly ServiceLayerConnector _serviceLayerConnector;
        Dictionary<string, string> _FieldMap;
        Dictionary<string, string> _FieldType;

        public VSITINVOICEService(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
            _FieldMap = mountFieldMap();
            _FieldType = mountFieldType();
        }

        async public Task<bool> Create()
        {
            bool result = false;

            result = await createTable();
            if (result)
            {
                result = await createItemsTable();
            }

            if (result)
            {
                result = await createVencTable();
            }
            return result;
        }
        async private Task<bool> createItemsTable()
        {
            bool result = false;

            Table table = new Table(_serviceLayerConnector);

            table.name = "VSITINVOICEITEM";
            table.description = "itens da nota fiscal";
            table.tableType = "bott_NoObject";
            table.columns = createColumnsItem();
            table.indexes = createIndexesItem();

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

        private List<TableIndexes> createIndexesItem()
        {
            List<TableIndexes> lista = new List<TableIndexes>();
            return lista;
        }

        private List<TableColumn> createColumnsItem()
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


        async private Task<bool> createVencTable()
        {
            bool result = false;

            Table table = new Table(_serviceLayerConnector);

            table.name = "VSITINVOICEVENC";
            table.description = "Vencimento Nota";
            table.tableType = "bott_NoObject";
            table.columns = createColumnsVenc();
            table.indexes = createIndexesVenc();

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

        private List<TableIndexes> createIndexesVenc()
        {
            List<TableIndexes> lista = new List<TableIndexes>();
            return lista;
        }

        private List<TableColumn> createColumnsVenc()
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

        async private Task<bool> createTable()
        {
            bool result = false;

            Table table = new Table(_serviceLayerConnector);

            table.name = "VSITINVOICE";
            table.description = "Cadastro de VSITINVOICE";
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

        public Task Delete(VSITINVOICE entity)
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
            string query = Global.MakeODataQuery("U_VSITINVOICE/$count", null, filter.Count == 0 ? null : filter.ToArray(), null, 1, 0);
            string data = await _serviceLayerConnector.getQueryResult(query);
            page.Linhas = Convert.ToInt64(data);
            page.Paginas = (Convert.ToInt64(data) / size.Value) + 1;
            page.qtdPorPagina = size.Value == 0 ? Convert.ToInt64(data) : size.Value;
            return page;
        }
        async public Task<VSITINVOICE> Find(List<Criteria> criterias)
        {
            VSITINVOICE entidade = null;
            string recid = criterias[0].Value;
            try
            {
                string query = Global.BuildQuery($"U_VSITINVOICE('{recid}')");

                string data = await _serviceLayerConnector.getQueryResult(query);

                ExpandoObject record = Global.parseQueryToObject(data);

                entidade = toRecord(record);
                //here due

                Serviceb1.Integration.VSITVENCService venctoS = new VSITVENCService(_serviceLayerConnector);
                List<Criteria> filtro = new List<Criteria>();
                filtro.Add(new Criteria
                {
                    Field = "nro_nota",
                    Operator = "eq",
                    Value = entidade.nta.ToString()
                });

                filtro.Add(new Criteria
                {
                    Field = "serie",
                    Operator = "eq",
                    Value = entidade.ser.ToString()
                });

                filtro.Add(new Criteria
                {
                    Field = "loj_org",
                    Operator = "eq",
                    Value = entidade.org.ToString()
                });

                filtro.Add(new Criteria
                {
                    Field = "dta_agenda",
                    Operator = "eq",
                    Value = entidade.dta.ToString()
                });

                filtro.Add(new Criteria
                {
                    Field = "oper",
                    Operator = "eq",
                    Value = entidade.age.ToString()
                });


                entidade.vsitvenc = await venctoS.List(filtro, 1, 0);

                Serviceb1.Integration.VSITITEMService itemS = new VSITITEMService(_serviceLayerConnector);
                List<Criteria> filtroitem = new List<Criteria>();
                filtroitem.Add(new Criteria
                {
                    Field = "id",
                    Operator = "eq",
                    Value = entidade.id.ToString()
                });

                entidade.vsititem = await itemS.List(filtroitem, 1, 0);


                // Recupera as linhas da nota fiscal
                //string[] filter = new string[]
                //{
                //    $"Code eq '{recid}'"
                //};

                //query = Global.MakeODataQuery("U_VSITINVOICE", null, filter);

                //data = await _serviceLayerConnector.getQueryResult(query);
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return entidade;
        }

        async public Task Insert(VSITINVOICE entity)
        {
            IBatchProducer batch = _serviceLayerConnector.CreateBatch();
            entity.status = Data.Model.Integration.VSITINVOICE.VSITINVOICEIntegrationStatus.Importing;
            string record = toJson(entity);
            batch.Post(HttpMethod.Post, "/U_VSITINVOICE", record);
            ServiceLayerResponse response = await _serviceLayerConnector.Post(batch);
            
            //separa os items em pacotes de 100
            int tamanho = 100;
            for (int ini = 0; ini <= entity.vsititem.Count(); ini = ini + tamanho)
            {
                IBatchProducer novoBatch = _serviceLayerConnector.CreateBatch();
                var novo = entity.vsititem.Skip(ini).Take(tamanho);
                foreach (var i in novo)
                {
                    record = toJsonItem(i);
                    novoBatch.Post(HttpMethod.Post, "/U_VSITINVOICEITEM", record);
                }
                response = await _serviceLayerConnector.Post(novoBatch);
            }

            //separa os vencimentos em pacotes de 100
            tamanho = 100;
            for (int ini = 0; ini <= entity.vsitvenc.Count(); ini = ini + tamanho)
            {
                IBatchProducer novoBatch = _serviceLayerConnector.CreateBatch();
                var novo = entity.vsitvenc.Skip(ini).Take(tamanho);
                foreach (var i in novo)
                {
                    record = ToJsonVenc(i);
                    novoBatch.Post(HttpMethod.Post, "/U_VSITINVOICEVENC", record);
                }
                response = await _serviceLayerConnector.Post(novoBatch);
            }

            //foreach (var i in entity.vsititem)
            //{
            //    record = toJsonItem(i);
            //    batch.Post(HttpMethod.Post, "/U_VSITINVOICEITEM", record);
            //}
            //if (!response.success)
            //{
            //    string message = $"Erro ao enviar transação de '{entity.EntityName}': {response.errorCode}-{response.errorMessage}";
            //    Console.WriteLine(message);
            //    throw new ApplicationException(message);
            //}
         }

        public Task Insert(List<VSITINVOICE> entities)
        {
            throw new NotImplementedException();
        }

        async public Task<List<VSITINVOICE>> List(List<Criteria> criterias, long page, long size)
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

            string query = Global.MakeODataQuery("U_VSITINVOICE", null, filter.Count == 0 ? null : filter.ToArray());

            List<VSITINVOICE> result = new List<VSITINVOICE>();

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    result.Add(toRecord(o));
                }
            }


            return result;
        }

        async public Task<List<VSITINVOICE>> ListSummary(List<Criteria> criterias)
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

            string[] queryArgs = new string[]
            {
                "$crossjoin(U_VSITINVOICE,U_VSITCFOPTOUSAGEMAP)",
                "?$expand=",
                "U_VSITCFOPTOUSAGEMAP($select=U_CFOP,U_LGCYUSAGE,U_DOCTYPE),",
                "U_VSITINVOICE($select=Code, U_AGE,U_ORG,U_DST,U_NTA,U_SER,U_CFO,U_DTA,U_DTA_EMI,U_SIT,U_STATUS,U_LAST_UPDATE)",
                "&$filter=",
                "(U_VSITCFOPTOUSAGEMAP/U_CFOP eq U_VSITINVOICE/U_CFO) and ",
                "(U_VSITCFOPTOUSAGEMAP/U_LGCYUSAGE eq U_VSITINVOICE/U_AGE)",
                filter.Count == 0 ? string.Empty : " and ",
                filter.Count == 0 ? string.Empty : string.Join(" and ", filter.ToArray()),
            };

            string query = string.Join("", queryArgs);

            List<VSITINVOICE> result = new List<VSITINVOICE>();

            string data = await _serviceLayerConnector.getQueryResult(query);

            List<ExpandoObject> lista = Global.parseQueryToCollection(data);

            if (lista != null)
            {
                foreach (dynamic o in lista)
                {
                    dynamic record = o.U_VSITINVOICE;
                    dynamic cfopToUsage = o.U_VSITCFOPTOUSAGEMAP;

                    VSITINVOICE entidade = new VSITINVOICE();

                    entidade.RecId = Guid.Parse(record.Code);
                    entidade.status = (VSITINVOICE.VSITINVOICEIntegrationStatus)record.U_STATUS;

                    entidade.age = record.U_AGE;
                    entidade.org = record.U_ORG;
                    entidade.dst = record.U_DST;
                    entidade.nta = record.U_NTA;
                    entidade.ser = record.U_SER;
                    entidade.cfo = record.U_CFO;
                    entidade.dta = record.U_DTA;
                    entidade.dta_emi = record.U_DTA_EMI;
                    entidade.sit = record.U_SIT;
                    entidade.lastupdate = record.U_LAST_UPDATE;

                    entidade.DocumentType = (CfopToUsageMap.DocumentTypeEnum)Convert.ToInt64(cfopToUsage.U_DOCTYPE);

                    result.Add(entidade);
                }
            }

            return result;
        }

        public Task Update(VSITINVOICE entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(List<VSITINVOICE> entities)
        {
            throw new NotImplementedException();
        }

        private List<TableColumn> createColumns()
        {
            List<TableColumn> lista = new List<TableColumn>();

            lista.Add(new TableColumn() { name = "STATUS", dataType = "db_Numeric", mandatory = false, size = 1, description = "STATUS" });
            lista.Add(new TableColumn() { name = "TIPO_PN", dataType = "db_Alpha", mandatory = false, size = 1, description = "TIPO_PN" });
            lista.Add(new TableColumn() { name = "LAST_UPDATE", dataType = "db_Alpha", mandatory = false, description = "LAST_UPDATE", size = 19 });
            lista.Add(new TableColumn() { name = "DATA_INCLUSAO", dataType = "db_Alpha", mandatory = false, description = "DATA_INCLUSAO", size = 19 });
            lista.Add(new TableColumn() { name = "DATA_INTEGRACAO", dataType = "db_Alpha", mandatory = false, description = "DATA_INTEGRACAO", size = 19 });
            lista.Add(new TableColumn() { name = "VER", dataType = "db_Alpha", mandatory = false, size = 100, description = "VER" });
            lista.Add(new TableColumn() { name = "FNT_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "FNT_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FUN_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "FUN_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CPL_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "CPL_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CGC_CPF", dataType = "db_Alpha", mandatory = false, size = 19, description = "CGC_CPF" });
            lista.Add(new TableColumn() { name = "T01_PAG", dataType = "db_Alpha", mandatory = false, size = 100, description = "T01_PAG" });
            lista.Add(new TableColumn() { name = "T04_FIS", dataType = "db_Alpha", mandatory = false, size = 100, description = "T04_FIS" });
            lista.Add(new TableColumn() { name = "FNT_FFI", dataType = "db_Float", mandatory = false, size = 15, description = "FNT_FFI", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "EFT_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "EFT_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FIN", dataType = "db_Numeric", mandatory = false, size = 11, description = "FIN" });
            lista.Add(new TableColumn() { name = "FCR_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "FCR_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "EMP", dataType = "db_Numeric", mandatory = false, size = 11, description = "EMP" });
            lista.Add(new TableColumn() { name = "VOL", dataType = "db_Float", mandatory = false, size = 15, description = "VOL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ID", dataType = "db_Numeric", mandatory = false, size = 11, description = "ID" });
            lista.Add(new TableColumn() { name = "PIR_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "PIR_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "VEN", dataType = "db_Numeric", mandatory = false, size = 11, description = "VEN" });
            lista.Add(new TableColumn() { name = "INS_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "INS_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CTB", dataType = "db_Numeric", mandatory = false, size = 11, description = "CTB" });
            lista.Add(new TableColumn() { name = "ALT_DTA", dataType = "db_Numeric", mandatory = false, size = 11, description = "ALT_DTA" });
            lista.Add(new TableColumn() { name = "CSL_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "CSL_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ORG", dataType = "db_Numeric", mandatory = false, size = 11, description = "ORG" });
            lista.Add(new TableColumn() { name = "CSL_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "CSL_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "COR_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "COR_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FNT_GRE", dataType = "db_Float", mandatory = false, size = 15, description = "FNT_GRE", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ISS_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "ISS_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FIL_VEN", dataType = "db_Numeric", mandatory = false, size = 11, description = "FIL_VEN" });
            lista.Add(new TableColumn() { name = "IPS_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "IPS_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CND_PGT", dataType = "db_Numeric", mandatory = false, size = 11, description = "CND_PGT" });
            lista.Add(new TableColumn() { name = "T11_TPO", dataType = "db_Alpha", mandatory = false, size = 100, description = "T11_TPO" });
            lista.Add(new TableColumn() { name = "IPI_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "IPI_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CHV_NFE", dataType = "db_Alpha", mandatory = false, size = 100, description = "CHV_NFE" });
            lista.Add(new TableColumn() { name = "FPS_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "FPS_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DIF_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "DIF_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "TPO_CBR", dataType = "db_Alpha", mandatory = false, size = 100, description = "TPO_CBR" });
            lista.Add(new TableColumn() { name = "PVV_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "PVV_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "COF_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "COF_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "SER", dataType = "db_Alpha", mandatory = false, size = 100, description = "SER" });
            lista.Add(new TableColumn() { name = "T03_ENS", dataType = "db_Alpha", mandatory = false, size = 100, description = "T03_ENS" });
            lista.Add(new TableColumn() { name = "TPO_FRT", dataType = "db_Numeric", mandatory = false, size = 11, description = "TPO_FRT" });
            lista.Add(new TableColumn() { name = "EFT_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "EFT_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "IPI_ADI", dataType = "db_Float", mandatory = false, size = 15, description = "IPI_ADI", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CMP", dataType = "db_Numeric", mandatory = false, size = 11, description = "CMP" });
            lista.Add(new TableColumn() { name = "PIS_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "PIS_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "COR_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "COR_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "TRN_CPF", dataType = "db_Numeric", mandatory = false, size = 11, description = "TRN_CPF" });
            lista.Add(new TableColumn() { name = "DFO_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "DFO_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "IPS_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "IPS_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DTA", dataType = "db_Numeric", mandatory = false, size = 11, description = "DTA" });
            lista.Add(new TableColumn() { name = "TRN_PLA", dataType = "db_Alpha", mandatory = false, size = 100, description = "TRN_PLA" });
            lista.Add(new TableColumn() { name = "CFS", dataType = "db_Numeric", mandatory = false, size = 11, description = "CFS" });
            lista.Add(new TableColumn() { name = "DTA_EMI", dataType = "db_Numeric", mandatory = false, size = 11, description = "DTA_EMI" });
            lista.Add(new TableColumn() { name = "TRN_UFP", dataType = "db_Alpha", mandatory = false, size = 100, description = "TRN_UFP" });
            lista.Add(new TableColumn() { name = "PES", dataType = "db_Float", mandatory = false, size = 15, description = "PES", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DFD_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "DFD_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "T05_REC", dataType = "db_Alpha", mandatory = false, size = 100, description = "T05_REC" });
            lista.Add(new TableColumn() { name = "DTA_APA", dataType = "db_Numeric", mandatory = false, size = 11, description = "DTA_APA" });
            lista.Add(new TableColumn() { name = "CTB_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "CTB_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "INT_DTA", dataType = "db_Numeric", mandatory = false, size = 11, description = "INT_DTA" });
            lista.Add(new TableColumn() { name = "TPO_PAG", dataType = "db_Numeric", mandatory = false, size = 11, description = "TPO_PAG" });
            lista.Add(new TableColumn() { name = "ICM_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "ICM_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PIR_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "PIR_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "INT_HRS", dataType = "db_Numeric", mandatory = false, size = 11, description = "INT_HRS" });
            lista.Add(new TableColumn() { name = "IRF_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "IRF_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "POR", dataType = "db_Numeric", mandatory = false, size = 11, description = "POR" });
            lista.Add(new TableColumn() { name = "PVV_ADI", dataType = "db_Float", mandatory = false, size = 15, description = "PVV_ADI", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "T09_ITE", dataType = "db_Alpha", mandatory = false, size = 100, description = "T09_ITE" });
            lista.Add(new TableColumn() { name = "ALT_USU", dataType = "db_Alpha", mandatory = false, size = 100, description = "ALT_USU" });
            lista.Add(new TableColumn() { name = "INT_CTB", dataType = "db_Alpha", mandatory = false, size = 100, description = "INT_CTB" });
            lista.Add(new TableColumn() { name = "DSC_TRB", dataType = "db_Float", mandatory = false, size = 15, description = "DSC_TRB", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CPL_TRB", dataType = "db_Float", mandatory = false, size = 15, description = "CPL_TRB", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FCS_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "FCS_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "T02_NTA", dataType = "db_Alpha", mandatory = false, size = 100, description = "T02_NTA" });
            lista.Add(new TableColumn() { name = "COF_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "COF_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "MER_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "MER_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "INT_USU", dataType = "db_Alpha", mandatory = false, size = 100, description = "INT_USU" });
            lista.Add(new TableColumn() { name = "CPL_ISE", dataType = "db_Float", mandatory = false, size = 15, description = "CPL_ISE", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DST", dataType = "db_Numeric", mandatory = false, size = 11, description = "DST" });
            lista.Add(new TableColumn() { name = "MDL", dataType = "db_Alpha", mandatory = false, size = 100, description = "MDL" });
            lista.Add(new TableColumn() { name = "ICM_ISE", dataType = "db_Float", mandatory = false, size = 15, description = "ICM_ISE", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CRF", dataType = "db_Numeric", mandatory = false, size = 11, description = "CRF" });
            lista.Add(new TableColumn() { name = "ALT_HRS", dataType = "db_Numeric", mandatory = false, size = 11, description = "ALT_HRS" });
            lista.Add(new TableColumn() { name = "PVV_FFI", dataType = "db_Float", mandatory = false, size = 15, description = "PVV_FFI", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ISS_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "ISS_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FRT_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "FRT_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "SEG_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "SEG_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DTA_PRC", dataType = "db_Numeric", mandatory = false, size = 11, description = "DTA_PRC" });
            lista.Add(new TableColumn() { name = "CRD_PRE", dataType = "db_Float", mandatory = false, size = 15, description = "CRD_PRE", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DTA_BAS", dataType = "db_Numeric", mandatory = false, size = 11, description = "DTA_BAS" });
            lista.Add(new TableColumn() { name = "FIL_PRI", dataType = "db_Numeric", mandatory = false, size = 11, description = "FIL_PRI" });
            lista.Add(new TableColumn() { name = "FCP_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "FCP_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "INS_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "INS_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "CFO", dataType = "db_Numeric", mandatory = false, size = 11, description = "CFO" });
            lista.Add(new TableColumn() { name = "SIT_NFE", dataType = "db_Numeric", mandatory = false, size = 11, description = "SIT_NFE" });
            lista.Add(new TableColumn() { name = "FPS_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "FPS_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "SER_FIS", dataType = "db_Alpha", mandatory = false, size = 100, description = "SER_FIS" });
            lista.Add(new TableColumn() { name = "DES_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "DES_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DSP_FIN", dataType = "db_Float", mandatory = false, size = 15, description = "DSP_FIN", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "TRN_FAT", dataType = "db_Numeric", mandatory = false, size = 11, description = "TRN_FAT" });
            lista.Add(new TableColumn() { name = "PRV", dataType = "db_Numeric", mandatory = false, size = 11, description = "PRV" });
            lista.Add(new TableColumn() { name = "IPI_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "IPI_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "T13_EMI", dataType = "db_Alpha", mandatory = false, size = 100, description = "T13_EMI" });
            lista.Add(new TableColumn() { name = "EST", dataType = "db_Alpha", mandatory = false, size = 100, description = "EST" });
            lista.Add(new TableColumn() { name = "FCS_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "FCS_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "T12_DBR", dataType = "db_Alpha", mandatory = false, size = 100, description = "T12_DBR" });
            lista.Add(new TableColumn() { name = "EDI", dataType = "db_Alpha", mandatory = false, size = 100, description = "EDI" });
            lista.Add(new TableColumn() { name = "DFD_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "DFD_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "T06_CTB", dataType = "db_Alpha", mandatory = false, size = 100, description = "T06_CTB" });
            lista.Add(new TableColumn() { name = "IRF_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "IRF_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "ICM_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "ICM_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PVV_GRE", dataType = "db_Float", mandatory = false, size = 15, description = "PVV_GRE", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "NTA", dataType = "db_Numeric", mandatory = false, size = 11, description = "NTA" });
            lista.Add(new TableColumn() { name = "DFO_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "DFO_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FCP_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "FCP_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "NTA_NFE", dataType = "db_Numeric", mandatory = false, size = 11, description = "NTA_NFE" });
            lista.Add(new TableColumn() { name = "FCR_VAL", dataType = "db_Float", mandatory = false, size = 15, description = "FCR_VAL", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "FNT_ADI", dataType = "db_Float", mandatory = false, size = 15, description = "FNT_ADI", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "PIS_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "PIS_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DES_BAS", dataType = "db_Float", mandatory = false, size = 15, description = "DES_BAS", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "DSC_ISE", dataType = "db_Float", mandatory = false, size = 15, description = "DSC_ISE", dataTypeSub = "st_Measurement" });
            lista.Add(new TableColumn() { name = "SIT", dataType = "db_Alpha", mandatory = false, size = 100, description = "SIT" });
            lista.Add(new TableColumn() { name = "AGE", dataType = "db_Numeric", mandatory = false, size = 11, description = "AGE" });

            lista.Add(new TableColumn() { name = "TIPO_PN", dataType = "db_Alpha", mandatory = false, size = 11, description = "TIPO_PN" });
            lista.Add(new TableColumn() { name = "DOCENTRY", dataType = "db_Numeric", mandatory = false, size = 11, description = "DOCENTRY" });

            return lista;
        }

        private List<TableIndexes> createIndexes()
        {
            List<TableIndexes> lista = new List<TableIndexes>();
            return lista;
        }


        private string ToJsonVenc(VSITVENC entidade)
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

        private string toJsonItem(VSITITEM entidade)
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

        private string toJson(VSITINVOICE entidade)
        {
            string result = string.Empty;
            dynamic record = new ExpandoObject();
            record.Code = entidade.RecId.ToString();
            record.Name = entidade.RecId.ToString();
            record.U_STATUS = (VSITINVOICE.VSITINVOICEIntegrationStatus.Created);
            record.U_LAST_UPDATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            record.U_DATA_INCLUSAO = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            record.U_VER = entidade.ver;
            record.U_FNT_VAL = entidade.fnt_val;
            record.U_FUN_VAL = entidade.fun_val;
            record.U_CPL_VAL = entidade.cpl_val;
            record.U_CGC_CPF = entidade.cgc_cpf;
            record.U_T01_PAG = entidade.t01_pag;
            record.U_T04_FIS = entidade.t04_fis;
            record.U_FNT_FFI = entidade.fnt_ffi;
            record.U_EFT_BAS = entidade.eft_bas;
            record.U_FIN = entidade.fin;
            record.U_FCR_BAS = entidade.fcr_bas;
            record.U_EMP = entidade.emp;
            record.U_VOL = entidade.vol;
            record.U_ID = entidade.id;
            record.U_PIR_BAS = entidade.pir_bas;
            record.U_VEN = entidade.ven;
            record.U_INS_BAS = entidade.ins_bas;
            record.U_CTB = entidade.ctb;
            record.U_ALT_DTA = entidade.alt_dta;
            record.U_CSL_BAS = entidade.csl_bas;
            record.U_ORG = entidade.org;
            record.U_CSL_VAL = entidade.csl_val;
            record.U_COR_VAL = entidade.cor_val;
            record.U_FNT_GRE = entidade.fnt_gre;
            record.U_ISS_VAL = entidade.iss_val;
            record.U_FIL_VEN = entidade.fil_ven;
            record.U_IPS_BAS = entidade.ips_bas;
            record.U_CND_PGT = entidade.cnd_pgt;
            record.U_T11_TPO = entidade.t11_tpo;
            record.U_IPI_VAL = entidade.ipi_val;
            record.U_CHV_NFE = entidade.chv_nfe;
            record.U_FPS_VAL = entidade.fps_val;
            record.U_DIF_VAL = entidade.dif_val;
            record.U_TPO_CBR = entidade.tpo_cbr;
            record.U_PVV_VAL = entidade.pvv_val;
            record.U_COF_VAL = entidade.cof_val;
            record.U_SER = entidade.ser;
            record.U_T03_ENS = entidade.t03_ens;
            record.U_TPO_FRT = entidade.tpo_frt;
            record.U_EFT_VAL = entidade.eft_val;
            record.U_IPI_ADI = entidade.ipi_adi;
            record.U_CMP = entidade.cmp;
            record.U_PIS_VAL = entidade.pis_val;
            record.U_COR_BAS = entidade.cor_bas;
            record.U_TRN_CPF = entidade.trn_cpf;
            record.U_DFO_BAS = entidade.dfo_bas;
            record.U_IPS_VAL = entidade.ips_val;
            record.U_DTA = entidade.dta;
            record.U_TRN_PLA = entidade.trn_pla;
            record.U_CFS = entidade.cfs;
            record.U_DTA_EMI = entidade.dta_emi;
            record.U_TRN_UFP = entidade.trn_ufp;
            record.U_PES = entidade.pes;
            record.U_DFD_BAS = entidade.dfd_bas;
            record.U_T05_REC = entidade.t05_rec;
            record.U_DTA_APA = entidade.dta_apa;
            record.U_CTB_VAL = entidade.ctb_val;
            record.U_INT_DTA = entidade.int_dta;
            record.U_TPO_PAG = entidade.tpo_pag;
            record.U_ICM_BAS = entidade.icm_bas;
            record.U_PIR_VAL = entidade.pir_val;
            record.U_INT_HRS = entidade.int_hrs;
            record.U_IRF_BAS = entidade.irf_bas;
            record.U_POR = entidade.por;
            record.U_PVV_ADI = entidade.pvv_adi;
            record.U_T09_ITE = entidade.t09_ite;
            record.U_ALT_USU = entidade.alt_usu;
            record.U_INT_CTB = entidade.int_ctb;
            record.U_DSC_TRB = entidade.dsc_trb;
            record.U_CPL_TRB = entidade.cpl_trb;
            record.U_FCS_BAS = entidade.fcs_bas;
            record.U_T02_NTA = entidade.t02_nta;
            record.U_COF_BAS = entidade.cof_bas;
            record.U_MER_VAL = entidade.mer_val;
            record.U_INT_USU = entidade.int_usu;
            record.U_CPL_ISE = entidade.cpl_ise;
            record.U_DST = entidade.dst;
            record.U_MDL = entidade.mdl;
            record.U_ICM_ISE = entidade.icm_ise;
            record.U_CRF = entidade.crf;
            record.U_ALT_HRS = entidade.alt_hrs;
            record.U_PVV_FFI = entidade.pvv_ffi;
            record.U_ISS_BAS = entidade.iss_bas;
            record.U_FRT_VAL = entidade.frt_val;
            record.U_SEG_VAL = entidade.seg_val;
            record.U_DTA_PRC = entidade.dta_prc;
            record.U_CRD_PRE = entidade.crd_pre;
            record.U_DTA_BAS = entidade.dta_bas;
            record.U_FIL_PRI = entidade.fil_pri;
            record.U_FCP_BAS = entidade.fcp_bas;
            record.U_INS_VAL = entidade.ins_val;
            record.U_CFO = entidade.cfo;
            record.U_SIT_NFE = entidade.sit_nfe;
            record.U_FPS_BAS = entidade.fps_bas;
            record.U_SER_FIS = entidade.ser_fis;
            record.U_DES_VAL = entidade.des_val;
            record.U_DSP_FIN = entidade.dsp_fin;
            record.U_TRN_FAT = entidade.trn_fat;
            record.U_PRV = entidade.prv;
            record.U_IPI_BAS = entidade.ipi_bas;
            record.U_T13_EMI = entidade.t13_emi;
            record.U_EST = entidade.est;
            record.U_FCS_VAL = entidade.fcs_val;
            record.U_T12_DBR = entidade.t12_dbr;
            record.U_EDI = entidade.edi;
            record.U_DFD_VAL = entidade.dfd_val;
            record.U_T06_CTB = entidade.t06_ctb;
            record.U_IRF_VAL = entidade.irf_val;
            record.U_ICM_VAL = entidade.icm_val;
            record.U_PVV_GRE = entidade.pvv_gre;
            record.U_NTA = entidade.nta;
            record.U_DFO_VAL = entidade.dfo_val;
            record.U_FCP_VAL = entidade.fcp_val;
            record.U_NTA_NFE = entidade.nta_nfe;
            record.U_FCR_VAL = entidade.fcr_val;
            record.U_FNT_ADI = entidade.fnt_adi;
            record.U_PIS_BAS = entidade.pis_bas;
            record.U_DES_BAS = entidade.des_bas;
            record.U_DSC_ISE = entidade.dsc_ise;
            record.U_SIT = entidade.sit;
            record.U_AGE = entidade.age;
            record.U_NTA = entidade.nta;
            record.U_SER = entidade.ser;
            record.U_TIPO_PN = entidade.tipo_pn;




            result = JsonConvert.SerializeObject(record);
            return result;
        }

        private VSITINVOICE toRecord(dynamic record)
        {
            VSITINVOICE entidade = new VSITINVOICE();


            entidade.RecId = Guid.Parse(record.Code);
            entidade.status = (VSITINVOICE.VSITINVOICEIntegrationStatus)record.U_STATUS;

            entidade.lastupdate = record.U_LAST_UPDATE;
            entidade.ver = record.U_VER;
            entidade.fnt_val = Convert.ToDouble(record.U_FNT_VAL);
            entidade.fun_val = Convert.ToDouble(record.U_FUN_VAL);
            entidade.cpl_val = Convert.ToDouble(record.U_CPL_VAL);
            entidade.cgc_cpf = record.U_CGC_CPF;
            entidade.t01_pag = record.U_T01_PAG;
            entidade.t04_fis = record.U_T04_FIS;
            entidade.fnt_ffi = record.U_FNT_FFI;
            entidade.eft_bas = Convert.ToDouble(record.U_EFT_BAS);
            entidade.fin = record.U_FIN;
            entidade.fcr_bas = Convert.ToDouble(record.U_FCR_BAS);
            entidade.emp = record.U_EMP;
            entidade.vol = Convert.ToDouble(record.U_VOL);
            entidade.id = record.U_ID;
            entidade.pir_bas = Convert.ToDouble(record.U_PIR_BAS);
            entidade.ven = record.U_VEN;
            entidade.ins_bas = Convert.ToDouble(record.U_INS_BAS);
            entidade.ctb = record.U_CTB;
            entidade.alt_dta = record.U_ALT_DTA;
            entidade.csl_bas = Convert.ToDouble(record.U_CSL_BAS);
            entidade.org = record.U_ORG;
            entidade.csl_val = Convert.ToDouble(record.U_CSL_VAL);
            entidade.cor_val = Convert.ToDouble(record.U_COR_VAL);
            entidade.fnt_gre = Convert.ToDouble(record.U_FNT_GRE);
            entidade.iss_val = Convert.ToDouble(record.U_ISS_VAL);
            entidade.fil_ven = record.U_FIL_VEN;
            entidade.ips_bas = Convert.ToDouble(record.U_IPS_BAS);
            entidade.cnd_pgt = record.U_CND_PGT;
            entidade.t11_tpo = record.U_T11_TPO;
            entidade.ipi_val = Convert.ToDouble(record.U_IPI_VAL);
            entidade.chv_nfe = record.U_CHV_NFE;
            entidade.fps_val = Convert.ToDouble(record.U_FPS_VAL);
            entidade.dif_val = record.U_DIF_VAL;
            entidade.tpo_cbr = record.U_TPO_CBR;
            entidade.pvv_val = Convert.ToDouble(record.U_PVV_VAL);
            entidade.cof_val = Convert.ToDouble(record.U_COF_VAL);
            entidade.ser = record.U_SER;
            entidade.t03_ens = record.U_T03_ENS;
            entidade.tpo_frt = record.U_TPO_FRT;
            entidade.eft_val = Convert.ToDouble(record.U_EFT_VAL);
            entidade.ipi_adi = record.U_IPI_ADI;
            entidade.cmp = record.U_CMP;
            entidade.pis_val = Convert.ToDouble(record.U_PIS_VAL);
            entidade.cor_bas = Convert.ToDouble(record.U_COR_BAS);
            entidade.trn_cpf = record.U_TRN_CPF;
            entidade.dfo_bas = Convert.ToDouble(record.U_DFO_BAS);
            entidade.ips_val = Convert.ToDouble(record.U_IPS_VAL);
            entidade.dta = record.U_DTA;
            entidade.trn_pla = record.U_TRN_PLA;
            entidade.cfs = record.U_CFS;
            entidade.dta_emi = record.U_DTA_EMI;
            entidade.trn_ufp = record.U_TRN_UFP;
            entidade.pes = Convert.ToDouble(record.U_PES);
            //if (string.IsNullOrEmpty(record.U_DFD_BAS))
            entidade.dfd_bas = record.U_DFD_BAS;
            //else
            //    entidade.dfd_bas = Convert.ToDouble(record.U_DFD_BAS);

            entidade.t05_rec = record.U_T05_REC;
            entidade.dta_apa = record.U_DTA_APA;
            entidade.ctb_val = Convert.ToDouble(record.U_CTB_VAL);
            entidade.int_dta = record.U_INT_DTA;
            entidade.tpo_pag = record.U_TPO_PAG;
            entidade.icm_bas = Convert.ToDouble(record.U_ICM_BAS);
            entidade.pir_val = Convert.ToDouble(record.U_PIR_VAL);
            entidade.int_hrs = record.U_INT_HRS;
            entidade.irf_bas = Convert.ToDouble(record.U_IRF_BAS);
            entidade.por = record.U_POR;
            entidade.pvv_adi = record.U_PVV_ADI;
            entidade.t09_ite = record.U_T09_ITE;
            entidade.alt_usu = record.U_ALT_USU;
            entidade.int_ctb = record.U_INT_CTB;

            //if (String.IsNullOrEmpty(record.U_DSC_TRB))
            entidade.dsc_trb = record.U_DSC_TRB;
            //else
            //    entidade.dsc_trb = Convert.ToDouble(record.U_DSC_TRB);

            entidade.cpl_trb = Convert.ToDouble(record.U_CPL_TRB);
            entidade.fcs_bas = Convert.ToDouble(record.U_FCS_BAS);
            entidade.t02_nta = record.U_T02_NTA;
            entidade.cof_bas = Convert.ToDouble(record.U_COF_BAS);
            entidade.mer_val = Convert.ToDouble(record.U_MER_VAL);
            entidade.int_usu = record.U_INT_USU;
            entidade.cpl_ise = Convert.ToDouble(record.U_CPL_ISE);
            entidade.dst = record.U_DST;
            entidade.mdl = record.U_MDL;
            entidade.icm_ise = Convert.ToDouble(record.U_ICM_ISE);
            entidade.crf = record.U_CRF;
            entidade.alt_hrs = record.U_ALT_HRS;
            //if (String.IsNullOrEmpty(record.U_PVV_FFI))
            entidade.pvv_ffi = record.U_PVV_FFI;
            //else
            //    entidade.pvv_ffi = Convert.ToDouble(record.U_PVV_FFI);
            entidade.iss_bas = Convert.ToDouble(record.U_ISS_BAS);
            entidade.frt_val = Convert.ToDouble(record.U_FRT_VAL);
            entidade.seg_val = Convert.ToDouble(record.U_SEG_VAL);
            entidade.dta_prc = record.U_DTA_PRC;
            entidade.crd_pre = Convert.ToDouble(record.U_CRD_PRE);
            entidade.dta_bas = record.U_DTA_BAS;
            entidade.fil_pri = record.U_FIL_PRI;
            entidade.fcp_bas = Convert.ToDouble(record.U_FCP_BAS);
            entidade.ins_val = Convert.ToDouble(record.U_INS_VAL);
            entidade.cfo = record.U_CFO;
            entidade.sit_nfe = record.U_SIT_NFE;
            entidade.fps_bas = Convert.ToDouble(record.U_FPS_BAS);
            entidade.ser_fis = record.U_SER_FIS;
            entidade.des_val = Convert.ToDouble(record.U_DES_VAL);
            entidade.dsp_fin = Convert.ToDouble(record.U_DSP_FIN);
            entidade.trn_fat = record.U_TRN_FAT;
            entidade.prv = record.U_PRV;
            entidade.ipi_bas = Convert.ToDouble(record.U_IPI_BAS);
            entidade.t13_emi = record.U_T13_EMI;
            entidade.est = record.U_EST;
            entidade.fcs_val = Convert.ToDouble(record.U_FCS_VAL);
            entidade.t12_dbr = record.U_T12_DBR;
            entidade.edi = record.U_EDI;
            entidade.dfd_val = Convert.ToDouble(record.U_DFD_VAL);
            entidade.t06_ctb = record.U_T06_CTB;
            entidade.irf_val = Convert.ToDouble(record.U_IRF_VAL);
            entidade.icm_val = Convert.ToDouble(record.U_ICM_VAL);
            entidade.pvv_gre = Convert.ToDouble(record.U_PVV_GRE);
            entidade.nta = record.U_NTA;
            entidade.dfo_val = Convert.ToDouble(record.U_DFO_VAL);
            entidade.fcp_val = Convert.ToDouble(record.U_FCP_VAL);
            entidade.nta_nfe = record.U_NTA_NFE;
            entidade.fcr_val = Convert.ToDouble(record.U_FCR_VAL);
            entidade.fnt_adi = Convert.ToDouble(record.U_FNT_ADI);
            entidade.pis_bas = Convert.ToDouble(record.U_PIS_BAS);
            entidade.des_bas = Convert.ToDouble(record.U_DES_BAS);
            entidade.dsc_ise = Convert.ToDouble(record.U_DSC_ISE);
            entidade.sit = record.U_SIT;
            entidade.age = record.U_AGE;
            entidade.tipo_pn = record.U_TIPO_PN;

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
            map.Add("ver", "U_VER");
            map.Add("fnt_val", "U_FNT_VAL");
            map.Add("fun_val", "U_FUN_VAL");
            map.Add("cpl_val", "U_CPL_VAL");
            map.Add("cgc_cpf", "U_CGC_CPF");
            map.Add("t01_pag", "U_T01_PAG");
            map.Add("t04_fis", "U_T04_FIS");
            map.Add("fnt_ffi", "U_FNT_FFI");
            map.Add("eft_bas", "U_EFT_BAS");
            map.Add("fin", "U_FIN");
            map.Add("fcr_bas", "U_FCR_BAS");
            map.Add("emp", "U_EMP");
            map.Add("vol", "U_VOL");
            map.Add("id", "U_ID");
            map.Add("pir_bas", "U_PIR_BAS");
            map.Add("ven", "U_VEN");
            map.Add("ins_bas", "U_INS_BAS");
            map.Add("ctb", "U_CTB");
            map.Add("alt_dta", "U_ALT_DTA");
            map.Add("csl_bas", "U_CSL_BAS");
            map.Add("org", "U_ORG");
            map.Add("csl_val", "U_CSL_VAL");
            map.Add("cor_val", "U_COR_VAL");
            map.Add("fnt_gre", "U_FNT_GRE");
            map.Add("iss_val", "U_ISS_VAL");
            map.Add("fil_ven", "U_FIL_VEN");
            map.Add("ips_bas", "U_IPS_BAS");
            map.Add("cnd_pgt", "U_CND_PGT");
            map.Add("t11_tpo", "U_T11_TPO");
            map.Add("ipi_val", "U_IPI_VAL");
            map.Add("chv_nfe", "U_CHV_NFE");
            map.Add("fps_val", "U_FPS_VAL");
            map.Add("dif_val", "U_DIF_VAL");
            map.Add("tpo_cbr", "U_TPO_CBR");
            map.Add("pvv_val", "U_PVV_VAL");
            map.Add("cof_val", "U_COF_VAL");
            map.Add("ser", "U_SER");
            map.Add("t03_ens", "U_T03_ENS");
            map.Add("tpo_frt", "U_TPO_FRT");
            map.Add("eft_val", "U_EFT_VAL");
            map.Add("ipi_adi", "U_IPI_ADI");
            map.Add("cmp", "U_CMP");
            map.Add("pis_val", "U_PIS_VAL");
            map.Add("cor_bas", "U_COR_BAS");
            map.Add("trn_cpf", "U_TRN_CPF");
            map.Add("dfo_bas", "U_DFO_BAS");
            map.Add("ips_val", "U_IPS_VAL");
            map.Add("dta", "U_DTA");
            map.Add("trn_pla", "U_TRN_PLA");
            map.Add("cfs", "U_CFS");
            map.Add("dta_emi", "U_DTA_EMI");
            map.Add("trn_ufp", "U_TRN_UFP");
            map.Add("pes", "U_PES");
            map.Add("dfd_bas", "U_DFD_BAS");
            map.Add("t05_rec", "U_T05_REC");
            map.Add("dta_apa", "U_DTA_APA");
            map.Add("ctb_val", "U_CTB_VAL");
            map.Add("int_dta", "U_INT_DTA");
            map.Add("tpo_pag", "U_TPO_PAG");
            map.Add("icm_bas", "U_ICM_BAS");
            map.Add("pir_val", "U_PIR_VAL");
            map.Add("int_hrs", "U_INT_HRS");
            map.Add("irf_bas", "U_IRF_BAS");
            map.Add("por", "U_POR");
            map.Add("pvv_adi", "U_PVV_ADI");
            map.Add("t09_ite", "U_T09_ITE");
            map.Add("alt_usu", "U_ALT_USU");
            map.Add("int_ctb", "U_INT_CTB");
            map.Add("dsc_trb", "U_DSC_TRB");
            map.Add("cpl_trb", "U_CPL_TRB");
            map.Add("fcs_bas", "U_FCS_BAS");
            map.Add("t02_nta", "U_T02_NTA");
            map.Add("cof_bas", "U_COF_BAS");
            map.Add("mer_val", "U_MER_VAL");
            map.Add("int_usu", "U_INT_USU");
            map.Add("cpl_ise", "U_CPL_ISE");
            map.Add("dst", "U_DST");
            map.Add("mdl", "U_MDL");
            map.Add("icm_ise", "U_ICM_ISE");
            map.Add("crf", "U_CRF");
            map.Add("alt_hrs", "U_ALT_HRS");
            map.Add("pvv_ffi", "U_PVV_FFI");
            map.Add("iss_bas", "U_ISS_BAS");
            map.Add("frt_val", "U_FRT_VAL");
            map.Add("seg_val", "U_SEG_VAL");
            map.Add("dta_prc", "U_DTA_PRC");
            map.Add("crd_pre", "U_CRD_PRE");
            map.Add("dta_bas", "U_DTA_BAS");
            map.Add("fil_pri", "U_FIL_PRI");
            map.Add("fcp_bas", "U_FCP_BAS");
            map.Add("ins_val", "U_INS_VAL");
            map.Add("cfo", "U_CFO");
            map.Add("sit_nfe", "U_SIT_NFE");
            map.Add("fps_bas", "U_FPS_BAS");
            map.Add("ser_fis", "U_SER_FIS");
            map.Add("des_val", "U_DES_VAL");
            map.Add("dsp_fin", "U_DSP_FIN");
            map.Add("trn_fat", "U_TRN_FAT");
            map.Add("prv", "U_PRV");
            map.Add("ipi_bas", "U_IPI_BAS");
            map.Add("t13_emi", "U_T13_EMI");
            map.Add("est", "U_EST");
            map.Add("fcs_val", "U_FCS_VAL");
            map.Add("t12_dbr", "U_T12_DBR");
            map.Add("edi", "U_EDI");
            map.Add("dfd_val", "U_DFD_VAL");
            map.Add("t06_ctb", "U_T06_CTB");
            map.Add("irf_val", "U_IRF_VAL");
            map.Add("icm_val", "U_ICM_VAL");
            map.Add("pvv_gre", "U_PVV_GRE");
            map.Add("nta", "U_NTA");
            map.Add("dfo_val", "U_DFO_VAL");
            map.Add("fcp_val", "U_FCP_VAL");
            map.Add("nta_nfe", "U_NTA_NFE");
            map.Add("fcr_val", "U_FCR_VAL");
            map.Add("fnt_adi", "U_FNT_ADI");
            map.Add("pis_bas", "U_PIS_BAS");
            map.Add("des_bas", "U_DES_BAS");
            map.Add("dsc_ise", "U_DSC_ISE");
            map.Add("sit", "U_SIT");
            map.Add("age", "U_AGE");
            map.Add("documenttype", "U_VSITCFOPTOUSAGEMAP/U_DOCTYPE");

            return map;
        }
        private Dictionary<string, string> mountFieldType()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("status", "N");
            map.Add("ver", "N");
            map.Add("fnt_val", "N");
            map.Add("fun_val", "N");
            map.Add("cpl_val", "N");
            map.Add("cgc_cpf", "N");
            map.Add("t01_pag", "N");
            map.Add("t04_fis", "N");
            map.Add("fnt_ffi", "N");
            map.Add("eft_bas", "N");
            map.Add("fin", "T");
            map.Add("fcr_bas", "N");
            map.Add("emp", "N");
            map.Add("vol", "N");
            map.Add("id", "T");
            map.Add("pir_bas", "N");
            map.Add("ven", "T");
            map.Add("ins_bas", "N");
            map.Add("ctb", "T");
            map.Add("alt_dta", "T");
            map.Add("csl_bas", "N");
            map.Add("org", "T");
            map.Add("csl_val", "N");
            map.Add("cor_val", "N");
            map.Add("fnt_gre", "T");
            map.Add("iss_val", "N");
            map.Add("fil_ven", "T");
            map.Add("ips_bas", "N");
            map.Add("cnd_pgt", "T");
            map.Add("t11_tpo", "N");
            map.Add("ipi_val", "N");
            map.Add("chv_nfe", "N");
            map.Add("fps_val", "N");
            map.Add("dif_val", "N");
            map.Add("tpo_cbr", "N");
            map.Add("pvv_val", "N");
            map.Add("cof_val", "N");
            map.Add("ser", "N");
            map.Add("t03_ens", "N");
            map.Add("tpo_frt", "T");
            map.Add("eft_val", "N");
            map.Add("ipi_adi", "N");
            map.Add("cmp", "T");
            map.Add("pis_val", "N");
            map.Add("cor_bas", "N");
            map.Add("trn_cpf", "T");
            map.Add("dfo_bas", "N");
            map.Add("ips_val", "N");
            map.Add("dta", "T");
            map.Add("trn_pla", "N");
            map.Add("cfs", "T");
            map.Add("dta_emi", "T");
            map.Add("trn_ufp", "N");
            map.Add("pes", "N");
            map.Add("dfd_bas", "N");
            map.Add("t05_rec", "N");
            map.Add("dta_apa", "T");
            map.Add("ctb_val", "N");
            map.Add("int_dta", "T");
            map.Add("tpo_pag", "T");
            map.Add("icm_bas", "N");
            map.Add("pir_val", "N");
            map.Add("int_hrs", "T");
            map.Add("irf_bas", "N");
            map.Add("por", "T");
            map.Add("pvv_adi", "N");
            map.Add("t09_ite", "N");
            map.Add("alt_usu", "N");
            map.Add("int_ctb", "N");
            map.Add("dsc_trb", "N");
            map.Add("cpl_trb", "N");
            map.Add("fcs_bas", "N");
            map.Add("t02_nta", "N");
            map.Add("cof_bas", "N");
            map.Add("mer_val", "N");
            map.Add("int_usu", "N");
            map.Add("cpl_ise", "N");
            map.Add("dst", "T");
            map.Add("mdl", "N");
            map.Add("icm_ise", "N");
            map.Add("crf", "T");
            map.Add("alt_hrs", "T");
            map.Add("pvv_ffi", "N");
            map.Add("iss_bas", "N");
            map.Add("frt_val", "N");
            map.Add("seg_val", "T");
            map.Add("dta_prc", "T");
            map.Add("crd_pre", "T");
            map.Add("dta_bas", "N");
            map.Add("fil_pri", "T");
            map.Add("fcp_bas", "N");
            map.Add("ins_val", "N");
            map.Add("cfo", "T");
            map.Add("sit_nfe", "T");
            map.Add("fps_bas", "N");
            map.Add("ser_fis", "N");
            map.Add("des_val", "N");
            map.Add("dsp_fin", "N");
            map.Add("trn_fat", "T");
            map.Add("prv", "T");
            map.Add("ipi_bas", "N");
            map.Add("t13_emi", "N");
            map.Add("est", "N");
            map.Add("fcs_val", "N");
            map.Add("t12_dbr", "N");
            map.Add("edi", "N");
            map.Add("dfd_val", "N");
            map.Add("t06_ctb", "N");
            map.Add("irf_val", "N");
            map.Add("icm_val", "N");
            map.Add("pvv_gre", "N");
            map.Add("nta", "T");
            map.Add("dfo_val", "N");
            map.Add("fcp_val", "N");
            map.Add("nta_nfe", "T");
            map.Add("fcr_val", "N");
            map.Add("fnt_adi", "N");
            map.Add("pis_bas", "N");
            map.Add("des_bas", "N");
            map.Add("dsc_ise", "N");
            map.Add("sit", "N");
            map.Add("age", "T");
            map.Add("documenttype", "N");

            return map;
        }
    }
}
