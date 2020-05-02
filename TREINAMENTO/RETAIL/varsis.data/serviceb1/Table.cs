using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Varsis.Data.Serviceb1
{
    public class Table
    {
        const string FIELD_ALREADY_EXISTS = "-2035";

        readonly ServiceLayerConnector _serviceLayerConnector;

        public Table(ServiceLayerConnector serviceLayerConnector)
        {
            _serviceLayerConnector = serviceLayerConnector;
        }

        public string name { get; set; }
        public string description { get; set; }
        public string tableType { get; set; }
        public bool custom { get; set; } = true;
        public List<TableColumn> columns { get; set; }
        public List<TableIndexes> indexes { get; set; }

        async public Task createMulti()
        {
            bool exists = await this.tableExists();

            if (!exists)
            {
                string table = tablePayload();
                ServiceLayerResponse response = await _serviceLayerConnector.Post("UserTablesMD", table, true);

                if (!response.success)
                {
                    // TODO: REGISTRAR LOG DE TABELA NÃO CRIADA
                    System.Console.WriteLine($"Erro ao criar a tabela '{this.name}': {response.errorCode}-{response.errorMessage}");
                }
            }

            foreach (var c in columns)
            {
                string newColumn = columnPayload(c);

                ServiceLayerResponse response = await _serviceLayerConnector.Post("UserFieldsMD", newColumn, true);

                if (!response.success /*&& response.errorCode != FIELD_ALREADY_EXISTS*/ )
                {
                    //TODO: REGISTRAR LOG DE CAMPOS NÃO CRIADOS
                    System.Console.WriteLine($"Erro ao criar o campo '{c.name}': {response.errorCode}-{response.errorMessage}");
                }
            }

            foreach (var i in indexes)
            {
                string newIndex = indexPayload(i);
                ServiceLayerResponse response = await _serviceLayerConnector.Post("UserKeysMD", newIndex, true);

                if (!response.success)
                {
                    // TODO: REGISTRAR LOG DE INDICES NÃO CRIADOS
                    System.Console.WriteLine($"Erro ao criar o índice '{i.name}': {response.errorCode}-{response.errorMessage}");
                }
            }

        }

        async public Task create()
        {
            if (custom)
            {
                bool exists = await this.tableExists();
                IBatchProducer batch = _serviceLayerConnector.CreateBatch();

                if (!exists)
                {
                    string table = tablePayload();

                    ServiceLayerResponse response = await _serviceLayerConnector.Post("UserTablesMD", table, true);

                    if (!response.success)
                    {
                        // TODO: REGISTRAR LOG DE TABELA NÃO CRIADA
                        System.Console.WriteLine($"Erro ao criar a tabela '{this.name}': {response.errorCode}-{response.errorMessage}");
                    }
                }
            }

            foreach (var c in columns)
            {
                string newColumn = columnPayload(c);
                ServiceLayerResponse response = await _serviceLayerConnector.Post("UserFieldsMD", newColumn, true);

                if (!response.success && response.errorCode != FIELD_ALREADY_EXISTS)
                {
                    // TODO: REGISTRAR LOG DE CAMPOS NÃO CRIADOS
                    System.Console.WriteLine($"Erro ao criar o campo '{c.name}': {response.errorCode}-{response.errorMessage}");
                }
            }

            if (custom)
            {
                foreach (var i in indexes)
                {
                    string newIndex = indexPayload(i);

                    ServiceLayerResponse response = await _serviceLayerConnector.Post("UserKeysMD", newIndex, true);

                    if (!response.success)
                    {
                        // TODO: REGISTRAR LOG DE INDICES NÃO CRIADOS
                        System.Console.WriteLine($"Erro ao criar o índice '{i.name}': {response.errorCode}-{response.errorMessage}");
                    }
                }
            }

            System.Console.WriteLine("OK");
        }

        private string tablePayload()
        {
            string result = string.Empty;

            dynamic table = new ExpandoObject();

            table.TableName = this.name;
            table.TableDescription = this.description;
            table.TableType = this.tableType;

            result = JsonConvert.SerializeObject(table);

            return result;
        }

        private string columnPayload(TableColumn c)
        {
            string result = string.Empty;

            dynamic item = new ExpandoObject();

            if (custom)
            {
                item.TableName = $"@{this.name.ToUpper()}";
            }
            else
            {
                item.TableName = this.name.ToUpper();
            }

            item.Name = c.name;
            item.Description = c.description;
            item.Mandatory = c.mandatory ? "tYES" : "tNO";
            item.Type = c.dataType;

            if (c.size != 0)
            {
                item.EditSize = c.size;
            }

            if (!string.IsNullOrEmpty(c.dataTypeSub))
            {
                item.SubType = c.dataTypeSub;
            }

            result = JsonConvert.SerializeObject(item);

            return result;
        }

        private string indexPayload(TableIndexes i)
        {
            string result = string.Empty;

            dynamic index = new ExpandoObject();

            index.TableName = $"@{this.name.ToUpper()}";
            index.KeyName = i.name;
            index.Unique = i.isUnique ? "tYES" : "tNO";
            index.UserKeysMD_Elements = new dynamic[i.keys.Length];

            for (int j = 0; j < i.keys.Length; j++)
            {
                index.UserKeysMD_Elements[j] = new ExpandoObject();
                index.UserKeysMD_Elements[j].ColumnAlias = i.keys[j];
            }

            result = JsonConvert.SerializeObject(index);

            return result;
        }

        async private Task<bool> tableExists()
        {
            bool result = false;

            string query = $"UserTablesMD('{name.ToUpper()}')";

            string data = await _serviceLayerConnector.getQueryResult(query);

            dynamic table = Global.parseQueryToObject(data);

            if (table != null)
            {
                if (table.TableName == name.ToUpper())
                {
                    result = true;
                }
            }

            return result;
        }
    }
}
