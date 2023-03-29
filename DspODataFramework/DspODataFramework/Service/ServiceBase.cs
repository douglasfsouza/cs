using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http.OData.Query;
using System.Web.Http.OData;
using System.Web;
using DspODataFramework.infra.attributes;
using DspODataFramework.infra;

namespace DspODataFramework.Service
{
    public class ServiceBase<T> where T : class, new()
    {
        //readonly ServiceLayerConnector _serviceLayerConnector;

        readonly Regex removeSearchFocus;
        readonly Regex removeSearch;
        readonly Regex removeInlineCount;
        readonly Regex removeODataDecimalMark;
        readonly Regex removeODataDoubleMark;
        readonly Regex replaceSubstringOf;

        public ServiceBase( )
        {
           // _serviceLayerConnector = serviceLayerConnector;

            removeSearchFocus = new Regex("&search-focus=[A-Za-z0-9]+");
            removeSearch = new Regex("&search=?(?:[A-Za-z]|)+");
            removeInlineCount = new Regex(@"&\$inlinecount=[A-Za-z0-9]+");
            removeODataDecimalMark = new Regex(@"(:?(gt|ge|lt|le|eq|ne)\s([\d]+)(m|l))");
            removeODataDoubleMark = new Regex(@"(:?(gt|ge|lt|le|eq|ne)\s([\d]+)(d))");
            replaceSubstringOf = new Regex(@"substringof\(('[\w\W]+?(?>'),\w+)?(?>\))");
        }

        public virtual bool FromSemanticLayer
        {
            get
            {
                return false;
            }
        }

        public virtual string TableName
        {
            get
            {
                Type type = typeof(T);

                var attr = type.GetCustomAttribute<ODataTableAttribute>();

                if (!string.IsNullOrWhiteSpace(attr?.PhysicalName))
                {
                    string name = attr.PhysicalName;

                    if (attr.IsCustom)
                    {
                        name = $"U_{name}";
                    }

                    return name;
                }

                throw new NotImplementedException();
            }
        }

        public virtual string UnderlyingTableName { get; }

        public virtual string FieldForCount
        {
            get
            {
                return null;
            }
        }

        public virtual bool RewriteQueryNeeded
        {
            get
            {
                return false;
            }
        }

        public virtual string GroupIdFieldName
        {
            get
            {
                return null;
            }
        }

        public virtual string GroupIdValue
        {
            get
            {
                return null;
            }
        }

        async public virtual Task<List<T>> List(string queryString, bool parseFields = false)
        {
            (_, List<T> result) = await ListWithCount(queryString, parseFields);
            return result;
        }

        private (string, string) PrepareQuery(ListWithCountArgs<T> args)
        {
            string SERVICE_NAME = this.TableName;
            object key = args.Key;
            bool hasKey = false;

            if (key != null)
            {
                if (key.GetType() == typeof(string))
                {
                    hasKey = (string)key == string.Empty ? false : true;
                }
                else if (key.GetType() == typeof(int))
                {
                    hasKey = (int)key == 0 ? false : true;
                }
                else if (key.GetType() == typeof(long))
                {
                    hasKey = (long)key == 0 ? false : true;
                }
            }

            string queryString = args.queryOptions.Request.RequestUri.Query;
            string queryRowCount = string.Empty;

            queryString = InjectGroupId(queryString);

            var members = this.ParseMembers();

            //vamos tirar o sinal de início de query "?". ele será ajustado antes de retornar
            if (queryString.StartsWith("?"))
            {
                queryString = queryString.Substring(1);
            }

            if (hasKey)
            {
                if (key.GetType() == typeof(string))
                {
                    SERVICE_NAME = $"{SERVICE_NAME}('{key}')";
                }
                else
                {
                    SERVICE_NAME = $"{SERVICE_NAME}({key})";
                }
            }

            //O URLDecode é necessário para que os regex de correção abaixo funcionem
            //O front-end fisca livre para mandar a query da forma que for necessário
            //Entretanto, só vamos executar o UrlDecode se a url estiver encoded
            if (Regex.IsMatch(queryString, "%[0-9a-f]{2}"))
            {
                queryString = HttpUtility.UrlDecode(queryString);
            }

            //--> Troca os nomes amigáveis das propriedades pelos nomes físicos
            queryString = ChangeInternalNameToReal(queryString, members.RealNameFor);

            //--> Remove os campos que não existem no backend
            foreach (var ignore in members.IgnoreOnBackend)
            {
                queryString = Regex.Replace(queryString, $",{ignore}\\b", "");
                queryString = Regex.Replace(queryString, $"\\b{ignore},", "");
            }

            //--> Remove clausulas inválidas para B1 Service Layer
            queryString = removeSearchFocus.Replace(queryString, "");
            queryString = removeSearch.Replace(queryString, "");
            queryString = removeODataDecimalMark.Replace(queryString, new MatchEvaluator((m) =>
            {
                var @operator = m.Groups[2].Value;
                var value = m.Groups[3].Value;

                return $"{@operator} {value}";
            }));

            queryString = removeODataDoubleMark.Replace(queryString, new MatchEvaluator((m) =>
            {
                var @operator = m.Groups[2].Value;
                var value = m.Groups[3].Value;

                return $"{@operator} {value}";
            }));
            //---

            if (args.parseQuery != null)
            {
                queryString = args.parseQuery(queryString);
                /*
                if (queryString == null || queryString == string.Empty)
                {
                    throw new ArgumentOutOfRangeException("query", "Não pode estar vazia");
                }
                */
            }

            //
            // Monta a query final para seleção dos dados
            //
            if (queryString == null || queryString == string.Empty)
            {
                queryString = SERVICE_NAME;
            }
            else
            {
                if (queryString.StartsWith("?"))
                {
                    queryString = queryString.Substring(1);
                }

                //queryString = HttpUtility.UrlEncode(queryString);
                queryString = Uri.EscapeDataString(queryString);

                if (hasKey)
                {
                    queryString = $"{SERVICE_NAME}&{queryString}";
                }
                else
                {
                    queryString = $"{SERVICE_NAME}?{queryString}";
                }
            }

            return (queryString, "");
        }

        private (string, string) PrepareQueryFromSematicLayer(ListWithCountArgs<T> args)
        {
            string SERVICE_NAME = this.TableName;
            object key = args.Key;
            bool hasKey = false;

            if (key != null)
            {
                if (key.GetType() == typeof(string))
                {
                    hasKey = (string)key == string.Empty ? false : true;
                }
                else if (key.GetType() == typeof(int))
                {
                    hasKey = (int)key == 0 ? false : true;
                }
                else if (key.GetType() == typeof(long))
                {
                    hasKey = (long)key == 0 ? false : true;
                }
            }

            string queryString = args.queryOptions.Request.RequestUri.Query;
            string queryRowCount = string.Empty;

            var members = this.ParseMembers();

            if (hasKey)
            {
                string keyName = GetKeyMember();

                if (key.GetType() == typeof(string))
                {
                    SERVICE_NAME = $"{SERVICE_NAME}?$filter={keyName} eq '{key}'";
                }
                else
                {
                    SERVICE_NAME = $"{SERVICE_NAME}?$filter={keyName} eq {key}";
                }
            }
            else
            {
                queryString = InjectGroupId(queryString);

                if (this.RewriteQueryNeeded)
                {
                    queryString = RewriteQuery(args.queryOptions);
                }

                if (args.queryOptions.InlineCount != null)
                {
                    queryString = removeInlineCount.Replace(queryString, "");

                    string filter = args.queryOptions.Filter?.RawValue ?? string.Empty;

                    if (FieldForCount == null || FieldForCount == string.Empty)
                    {
                        queryRowCount = "/$count";

                        if (filter != string.Empty)
                        {
                            queryRowCount = $"{queryRowCount}?$filter={filter}";
                        }
                    }
                    else if (filter == string.Empty)
                    {
                        queryRowCount = $"?$apply=aggregate({FieldForCount} with sum as {FieldForCount})";
                    }
                    else
                    {
                        queryRowCount = $"?$apply=filter({filter})/aggregate({FieldForCount} with sum as {FieldForCount})";
                    }

                    queryRowCount = InjectGroupId(queryRowCount);

                }
            }

            //O URLDecode é necessário para que os regex de correção abaixo funcionem
            //O front-end fisca livre para mandar a query da forma que for necessário
            //Entretanto, só vamos executar o UrlDecode se a url estiver encoded
            if (Regex.IsMatch(queryString, "%[0-9a-f]{2}"))
            {
                queryString = HttpUtility.UrlDecode(queryString);
            }

            //--> Troca os nomes amigáveis das propriedades pelos nomes físicos
            queryString = ChangeInternalNameToReal(queryString, members.RealNameFor);
            queryRowCount = ChangeInternalNameToReal(queryRowCount, members.RealNameFor);

            //--> Remove os campos que não existem no backend
            foreach (var ignore in members.IgnoreOnBackend)
            {
                queryString = Regex.Replace(queryString, $",{ignore}\\b", "");
                queryString = Regex.Replace(queryString, $"\\b{ignore},", "");
            }

            //--> Remove clausulas inválidas para B1 Service Layer
            queryString = removeSearchFocus.Replace(queryString, "");
            queryString = removeSearch.Replace(queryString, "");
            queryString = removeODataDecimalMark.Replace(queryString, new MatchEvaluator((m) =>
            {
                var @operator = m.Groups[2].Value;
                var value = m.Groups[3].Value;

                return $"{@operator} {value}";
            }));
            queryString = removeODataDoubleMark.Replace(queryString, new MatchEvaluator((m) =>
            {
                var @operator = m.Groups[2].Value;
                var value = m.Groups[3].Value;

                return $"{@operator} {value}";
            }));

            if (queryRowCount != string.Empty)
            {
                queryRowCount = removeSearchFocus.Replace(queryRowCount, "");
                queryRowCount = removeSearch.Replace(queryRowCount, "");
                queryRowCount = removeODataDecimalMark.Replace(queryRowCount, new MatchEvaluator((m) =>
                {
                    var @operator = m.Groups[2].Value;
                    var value = m.Groups[3].Value;

                    return $"{@operator} {value}";
                }));
                queryRowCount = removeODataDoubleMark.Replace(queryRowCount, new MatchEvaluator((m) =>
                {
                    var @operator = m.Groups[2].Value;
                    var value = m.Groups[3].Value;

                    return $"{@operator} {value}";
                }));
            }
            //---

            // Tratamento para a função "substringof" para a semantic layer
            // A semantic layer implementa OData V4, onde a função foi substituída por "contains"
            queryString = replaceSubstringOf.Replace(queryString, new MatchEvaluator((m) =>
            {
                var arguments = m.Groups[1].Value.Split(',');
                var value = m.Value;

                return $"contains({arguments[1]},{arguments[0]})";
            }));

            queryRowCount = replaceSubstringOf.Replace(queryRowCount, new MatchEvaluator((m) =>
            {
                var arguments = m.Groups[1].Value.Split(',');
                var value = m.Value;

                return $"contains({arguments[1]},{arguments[0]})";
            }));
            //---

            if (args.parseQuery != null)
            {
                queryString = args.parseQuery(queryString);
            }

            //
            // Monta a query final para seleção dos dados
            //
            if (queryString == null || queryString == string.Empty)
            {
                queryString = SERVICE_NAME;
            }
            else
            {
                if (queryString.StartsWith("?"))
                {
                    queryString = queryString.Substring(1);
                }

                //queryString = HttpUtility.UrlEncode(queryString);
                queryString = Uri.EscapeDataString(queryString);

                if (hasKey)
                {
                    queryString = $"{SERVICE_NAME}&{queryString}";
                }
                else
                {
                    queryString = $"{SERVICE_NAME}?{queryString}";
                }
            }

            if (queryRowCount != string.Empty)
            {
                if (args.parseQuery != null)
                {
                    queryRowCount = args.parseQuery(queryRowCount);
                    if (queryRowCount == null || queryRowCount == string.Empty)
                    {
                        throw new ArgumentOutOfRangeException("queryRowCount", "Não pode estar vazia");
                    }
                }

                //
                // Monta a query final para contagem dos dados
                //
                if (queryRowCount.StartsWith("?"))
                {
                    queryRowCount = queryRowCount.Substring(1);
                    queryRowCount = Uri.EscapeDataString(queryRowCount);
                    queryRowCount = $"{SERVICE_NAME}?{queryRowCount}";
                }
                else if (queryRowCount.StartsWith("/"))
                {
                    queryRowCount = Uri.EscapeDataString(queryRowCount);
                    queryRowCount = $"{SERVICE_NAME}{queryRowCount}";
                }
                else
                {
                    queryRowCount = Uri.EscapeDataString(queryRowCount);
                    queryRowCount = $"{SERVICE_NAME}&{queryRowCount}";
                }
            }

            return (queryString, queryRowCount);
        }

        async public virtual Task<(long Count, List<T> Rows)> ListWithCount(ListWithCountArgs<T> args)
        {
            long count = 0;
            List<T> rows = null;

            ////
            //// Esta rotina existe para contornar a cláusula "inlinecount" que retorna dados errados em calculation views
            ////
            //string query = string.Empty;
            //string queryRowCount = string.Empty;

            //if (FromSemanticLayer)
            //{
            //    (query, queryRowCount) = PrepareQueryFromSematicLayer(args);
            //}
            //else
            //{
            //    (query, queryRowCount) = PrepareQuery(args);
            //}

            //try
            //{
            //    IBatchProducer batch = _serviceLayerConnector.CreateBatch();

            //    batch.Post(System.Net.Http.HttpMethod.Get, $"/{query}", null);

            //    if (!string.IsNullOrEmpty(queryRowCount))
            //    {
            //        batch.Post(System.Net.Http.HttpMethod.Get, $"/{queryRowCount}", null);
            //    }

            //    var response = await _serviceLayerConnector.Get(batch, FromSemanticLayer);

            //    if (response == null || !response.success)
            //    {
            //        throw new Exception(response.errorMessage);
            //    }
            //    else
            //    {
            //        // Cuida do retorno dos registros
            //        ServiceLayerResponse responseQuery = response.internalResponses[0];

            //        rows = new List<T>();

            //        object key = args.Key;
            //        bool hasKey = false;

            //        if (key != null)
            //        {
            //            if (key.GetType() == typeof(string))
            //            {
            //                hasKey = (string)key == string.Empty ? false : true;
            //            }
            //            else if (key.GetType() == typeof(int))
            //            {
            //                hasKey = (int)key == 0 ? false : true;
            //            }
            //            else if (key.GetType() == typeof(long))
            //            {
            //                hasKey = (long)key == 0 ? false : true;
            //            }
            //        }

            //        if (FromSemanticLayer || !hasKey)
            //        {
            //            var list = Helper.parseQueryToCollection(responseQuery.data);

            //            if (list != null && list.Count != 0)
            //            {
            //                foreach (var item in list)
            //                {
            //                    T target = null;
            //                    if (args.parseRowFunction == null)
            //                    {
            //                        target = Helper.ParseTo<T>(item);
            //                    }
            //                    else
            //                    {
            //                        target = args.parseRowFunction(item);
            //                    }
            //                    rows.Add(target);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            var item = Helper.parseQueryToObject(responseQuery.data);
            //            if (item != null)
            //            {
            //                T target = null;
            //                if (args.parseRowFunction == null)
            //                {
            //                    target = Helper.ParseTo<T>(item);
            //                }
            //                else
            //                {
            //                    target = args.parseRowFunction(item);
            //                }
            //                rows.Add(target);
            //            }
            //        }

            //        if (!FromSemanticLayer)
            //        {
            //            count = Helper.parseODataCount(responseQuery.data);
            //        }

            //        // Pega o retorno do rowcount (se tiver)
            //        if (response.internalResponses.Count == 2)
            //        {
            //            var responseCount = response.internalResponses[1];

            //            if (FieldForCount == null || FieldForCount == string.Empty)
            //            {
            //                long.TryParse(responseCount.data, out count);
            //            }
            //            else
            //            {
            //                var countList = Helper.parseQueryToCollection(responseCount.data);

            //                if (countList != null && countList.Count != 0)
            //                {
            //                    IDictionary<string, object> item = (IDictionary<string, object>)countList[0];

            //                    if (item.ContainsKey(FieldForCount))
            //                    {
            //                        count = Convert.ToInt64(item[FieldForCount]);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception($"Erro ao obter dados para alteração:{ex.Message}");
            //}

            return (count, rows);
        }

        async public virtual Task<(long Count, List<T> Rows)> ListWithCount(string queryString, bool parseFields = false)
        {
            return await this.ListWithCount(queryString, null, parseFields);
        }

        async public virtual Task<(long Count, List<T> Rows)> ListWithCount(string queryString, Func<ExpandoObject, T> parseRowFunction, bool parseFields = false)
        {
            return (0, null);

            //string SERVICE_NAME = this.TableName;

            //var members = this.ParseMembers();

            //if (queryString != string.Empty && queryString[0] != '?')
            //{
            //    queryString = $"?{queryString}";
            //}

            //var query = $"{SERVICE_NAME}{queryString}";

            ////O URLDecode é necessário para que os regex de correção abaixo funcionem
            ////O front-end fisca livre para mandar a query da forma que for necessário
            ////Entretanto, só vamos executar o UrlDecode se a url estiver encoded
            //if (Regex.IsMatch(query, "%[0-9a-f]{2}"))
            //{
            //    query = HttpUtility.UrlDecode(query);
            //}

            //if (parseFields)
            //{
            //    query = ChangeInternalNameToReal(query, members.RealNameFor);
            //}

            ////--> Remove os campos que não existem no backend
            //foreach (var ignore in members.IgnoreOnBackend)
            //{
            //    query = Regex.Replace(query, $",{ignore}\\b", "");
            //    query = Regex.Replace(query, $"\\b{ignore},", "");
            //}

            ////--> Remove clausulas inválidas para B1 Service Layer
            //query = removeSearchFocus.Replace(query, "");
            //query = removeSearch.Replace(query, "");
            ////---

            //var data = await _serviceLayerConnector.getQueryResult(query);

            //var list = Helper.parseQueryToCollection(data);
            //var count = Helper.parseODataCount(data);
            //List<T> result = new List<T>();

            //if (list != null && list.Count != 0)
            //{
            //    foreach (var item in list)
            //    {
            //        T target = null;
            //        if (parseRowFunction == null)
            //        {
            //            target = Helper.ParseTo<T>(item);
            //        }
            //        else
            //        {
            //            target = parseRowFunction(item);
            //        }
            //        result.Add(target);
            //    }
            //}

            //return (count, result);
        }

        async public virtual Task<T> Find(string key)
        {
            return null;

            //string SERVICE_NAME = !string.IsNullOrWhiteSpace(this.UnderlyingTableName) ? this.UnderlyingTableName : this.TableName;

            //var query = $"{SERVICE_NAME}('{key}')";

            //var data = await _serviceLayerConnector.getQueryResult(query);

            //var record = Helper.parseQueryToObject(data);

            //T result = null;

            //if (record != null)
            //{
            //    result = Helper.ParseTo<T>(record);
            //}

            //return result;
        }

        async public virtual Task<T> Find(long key)
        {
            return null;

            //string SERVICE_NAME = !string.IsNullOrWhiteSpace(this.UnderlyingTableName) ? this.UnderlyingTableName : this.TableName;

            //var query = $"{SERVICE_NAME}({key})";

            //var data = await _serviceLayerConnector.getQueryResult(query);

            //var record = Helper.parseQueryToObject(data);

            //T result = null;

            //if (record != null)
            //{
            //    result = Helper.ParseTo<T>(record);
            //}

            //return result;
        }

        async public virtual Task<List<T>> List(string fields, string filter, string orderby, int skip = 0, int top = 0, string inlineCount = "")
        {
            string link = "";
            string args = "";

            if (fields != string.Empty)
            {
                args = $"{args}{link}$select={fields}";
                link = "&";
            }

            if (filter != string.Empty)
            {
                args = $"{args}{link}$filter={filter}";
                link = "&";
            }

            if (orderby != string.Empty)
            {
                args = $"{args}{link}$orderby={orderby}";
                link = "&";
            }

            if (skip != 0)
            {
                args = $"{args}{link}$skip={skip}";
                link = "&";
            }

            if (top != 0)
            {
                args = $"{args}{link}$top={top}";
                link = "&";
            }

            if (inlineCount != string.Empty)
            {
                args = $"{args}{link}$inlinecount={inlineCount}";
                link = "&";
            }

            args = args.Replace(' ', '+');

            return await this.List(args);
        }

        public virtual string MakeRecord(T entity, bool insert = false)
        {
            return MakeRecord(entity, insert: false, unchangedFields: null);
        }

        private string MakeRecord(T entity, IEnumerable<string> unchangedFields, bool insert = false)
        {
            var payload = JsonConvert.SerializeObject(entity, new JsonSerializerSettings()
            {
                ContractResolver = new ContractResolverHandler()
                {
                    ModelType = typeof(T),
                    IgnoreList = unchangedFields?.ToList()
                }
            });

            return payload;
        }

        async public virtual Task<T> Update(string key, T entity)
        {
            return await this.UpdateInternal($"'{key}'", entity);
        }

        async public virtual Task<T> Update(string key, Delta<T> entity)
        {
            T original = await this.Find(key);

            if (original == null)
            {
                throw new KeyNotFoundException();
            }

            entity.Patch(original);

            var unchangedFields = entity.GetUnchangedPropertyNames();

            return await this.UpdateInternal($"'{key}'", original, unchangedFields);
        }

        async public virtual Task<T> Update(long key, Delta<T> entity)
        {
            T original = await this.Find(key);

            if (original == null)
            {
                throw new KeyNotFoundException();
            }

            entity.Patch(original);

            var unchangedFields = entity.GetUnchangedPropertyNames();

            return await this.UpdateInternal($"{key}", original, unchangedFields);
        }

        async public virtual Task<T> Update(long key, T entity)
        {
            return await this.UpdateInternal($"{key}", entity);
        }

        async public virtual Task<bool> Delete(string key)
        {
            return true;

            //string tableName = !string.IsNullOrWhiteSpace(this.UnderlyingTableName) ? this.UnderlyingTableName : this.TableName;

            //var data = await _serviceLayerConnector.Delete($"{tableName}('{key}')", "");

            //if (!data.success)
            //{
            //    throw new Exception($"Erro ao excluir {TableName}\r\n{data.errorMessage}");
            //}

            //return true;
        }

        async public virtual Task<bool> Delete(long key)
        {
            return true;

            //string tableName = !string.IsNullOrWhiteSpace(this.UnderlyingTableName) ? this.UnderlyingTableName : this.TableName;

            //var data = await _serviceLayerConnector.Delete($"{tableName}({key})", "");

            //if (!data.success)
            //{
            //    throw new Exception($"Erro ao excluir {TableName}\r\n{data.errorMessage}");
            //}

            //return true;
        }

        async public virtual Task<T> Insert(T entity)
        {
            return null;

            //string tableName = !string.IsNullOrWhiteSpace(this.UnderlyingTableName) ? this.UnderlyingTableName : this.TableName;

            //string payload = this.MakeRecord(entity, true);

            //string query = $"{tableName}";

            //var data = await _serviceLayerConnector.Post(query, payload, false, true);

            //if (!data.success)
            //{
            //    throw new Exception($"Erro ao gravar {tableName}\r\n{data.errorMessage}");
            //}

            //var matchRecord = Regex.Match(data.data, @"{.+}", RegexOptions.Singleline);

            //T record = default;

            //if (matchRecord.Success)
            //{
            //    try
            //    {
            //        var dataRecord = matchRecord.Value;
            //        record = JsonConvert.DeserializeObject<T>(dataRecord);
            //    }
            //    catch (Exception)
            //    {

            //    }
            //}

            //return record;
        }

        async public virtual Task<long> Count(string criteria)
        {
            return 0;

            //bool returnRawNumber = false;
            //long result = 0;
            //var members = this.ParseMembers();

            //string tableName = this.TableName;

            //string filter = criteria?.Trim() == string.Empty ? "" : criteria;

            ////O URLDecode é necessário para que os regex de correção abaixo funcionem
            ////O front-end fisca livre para mandar a query da forma que for necessário
            ////Entretanto, só vamos executar o UrlDecode se a url estiver encoded
            //if (Regex.IsMatch(filter, "%[0-9a-f]{2}"))
            //{
            //    filter = HttpUtility.UrlDecode(filter);
            //}

            ////--> Troca os nomes amigáveis das propriedades pelos nomes físicos
            //filter = ChangeInternalNameToReal(filter, members.RealNameFor);

            ////--> Remove os campos que não existem no backend
            //foreach (var ignore in members.IgnoreOnBackend)
            //{
            //    filter = Regex.Replace(filter, $",{ignore}\\b", "");
            //    filter = Regex.Replace(filter, $"\\b{ignore},", "");
            //}

            ////--> Remove clausulas inválidas para B1 Service Layer
            //filter = removeSearchFocus.Replace(filter, "");
            //filter = removeSearch.Replace(filter, "");
            //filter = removeODataDecimalMark.Replace(filter, new MatchEvaluator((m) =>
            //{
            //    var @operator = m.Groups[2].Value;
            //    var value = m.Groups[3].Value;

            //    return $"{@operator} {value}";
            //}));

            //filter = removeODataDoubleMark.Replace(filter, new MatchEvaluator((m) =>
            //{
            //    var @operator = m.Groups[2].Value;
            //    var value = m.Groups[3].Value;

            //    return $"{@operator} {value}";
            //}));
            ////---

            //if (!string.IsNullOrWhiteSpace(GroupIdFieldName) && !string.IsNullOrWhiteSpace(GroupIdValue))
            //{
            //    if (string.IsNullOrWhiteSpace(filter))
            //    {
            //        filter = $"{GroupIdFieldName} eq '{GroupIdValue}'";
            //    }
            //    else
            //    {
            //        filter = $"{GroupIdFieldName} eq '{GroupIdValue}' and {filter}";
            //    }
            //}

            //string query = string.Empty;

            //if (this.FromSemanticLayer)
            //{
            //    if (FieldForCount == null || FieldForCount == string.Empty)
            //    {
            //        returnRawNumber = true;
            //        query = $"{tableName}/$count";

            //        if (filter != string.Empty)
            //        {
            //            query = $"{query}?$filter={filter}";
            //        }
            //    }
            //    else if (filter == string.Empty)
            //    {
            //        query = $"{tableName}?$apply=aggregate({FieldForCount} with sum as {FieldForCount})";
            //    }
            //    else
            //    {
            //        query = $"{tableName}?$apply=filter({filter})/aggregate({FieldForCount} with sum as {FieldForCount})";
            //    }
            //}
            //else
            //{
            //    returnRawNumber = true;

            //    query = $"{tableName}/$count";

            //    if (filter != string.Empty)
            //    {
            //        query = $"{query}?$filter={filter}";
            //    }
            //}

            //var data = await _serviceLayerConnector.getQueryResult(query);

            //if (!returnRawNumber && this.FromSemanticLayer)
            //{
            //    var countList = Helper.parseQueryToCollection(data);

            //    if (countList != null && countList.Count != 0)
            //    {
            //        IDictionary<string, object> item = (IDictionary<string, object>)countList[0];

            //        if (item.ContainsKey(FieldForCount))
            //        {
            //            result = Convert.ToInt64(item[FieldForCount]);
            //        }
            //    }
            //}
            //else
            //{
            //    long.TryParse(data, out result);
            //}

            //return result;
        }

        async private Task<T> UpdateInternal(string key, T entity)
        {
            return await UpdateInternal(key, entity, null);
        }

        async private Task<T> UpdateInternal(string key, T entity, IEnumerable<string> unchangedFields)
        {
            return null;

            //string tableName = !string.IsNullOrWhiteSpace(this.UnderlyingTableName) ? this.UnderlyingTableName : this.TableName;

            //string payload = this.MakeRecord(entity, unchangedFields);
            //string query = $"{tableName}({key})";

            //var data = await _serviceLayerConnector.Patch(query, payload);

            //if (!data.success)
            //{
            //    throw new Exception($"Erro ao gravar {TableName}\r\n{data.errorMessage}");
            //}
            //else
            //{
            //    return entity;
            //}
        }

        private string RewriteQuery(ODataQueryOptions<T> queryOptions)
        {
            string query = string.Empty;

            var members = ParseMembers();

            // Esta rotina só tem sentido se a query original possuir a cláusula $select
            // Neste caso, quando não houver, vamos retornar a query original, sem modificações
            if (queryOptions.SelectExpand == null)
            {
                query = queryOptions.Request.RequestUri.Query;

                if (Regex.IsMatch(query, "%[0-9a-f]{2}"))
                {
                    query = HttpUtility.UrlDecode(query);
                }

                query = ChangeInternalNameToReal(query, members.RealNameFor);

                foreach (var ignore in members.IgnoreOnBackend)
                {
                    query = query.Replace($",{ignore}", "")
                                 .Replace($"{ignore},", "");
                }

                return query;
            }

            string filter = queryOptions.Filter?.RawValue ?? string.Empty;

            filter = ChangeInternalNameToReal(filter, members.RealNameFor);

            List<string> queryArgs = new List<string>();
            List<string> groupby = new List<string>();
            List<string> aggregate = new List<string>();

            if (!string.IsNullOrWhiteSpace(GroupIdFieldName) && !string.IsNullOrWhiteSpace(GroupIdValue))
            {
                if (string.IsNullOrWhiteSpace(filter))
                {
                    filter = $"{GroupIdFieldName} eq '{GroupIdValue}'";
                }
                else
                {
                    filter = $"{GroupIdFieldName} eq '{GroupIdValue}' and {filter}";
                }
            }

            // Para que a semantic layer retorne totais corretamente
            // é necessário incluir um campo que force a SL a aplicar
            // os filtros sobre os detalhes e não sobre o resultado agrupado
            // Um campo do tipo "count distinct" resolve o problema
            // Estamos forçando a inclusão do campo "count distinct" para serviços da semantic layer
            if (FromSemanticLayer && !string.IsNullOrWhiteSpace(FieldForCount))
            {
                aggregate.Add($"{FieldForCount} with sum as {FieldForCount}");
            }
            var fields = queryOptions.SelectExpand?.RawSelect.Split(',') ?? new string[] { };

            // Os campos listados em "orderby" precisam fazer parte do group by ou de algum agregate
            // vamos adicionar os campos de orderby nesta lista

            var orderByFields = queryOptions.OrderBy?.RawValue.Replace("desc", "").Replace("asc", "").Split(',') ?? new string[] { };

            fields = fields.Concat(orderByFields).ToArray();

            foreach (var f in fields)
            {
                var fname = members.RealNameFor[f.Trim()];
                if (members.Attributes.Contains(f.Trim(), StringComparer.InvariantCultureIgnoreCase))
                {
                    groupby.Add(fname);
                }
                else if (members.Measures.Contains(f.Trim(), StringComparer.InvariantCultureIgnoreCase))
                {
                    string aggFN = members.AgregationFunction[f.Trim()];

                    aggregate.Add($"{fname} with {aggFN} as {fname}");
                }
            }

            queryArgs.Add("?$apply=");

            if (filter != string.Empty)
            {
                queryArgs.Add($"filter({filter})");

                if (groupby.Count != 0 || aggregate.Count != 0)
                {
                    queryArgs.Add("/");
                }
            }

            if (groupby.Count != 0)
            {
                queryArgs.Add($"groupby(({string.Join(",", groupby)}))");

                if (aggregate.Count != 0)
                {
                    queryArgs.Add("/");
                }
            }

            if (aggregate.Count != 0)
            {
                queryArgs.Add($"aggregate({string.Join(",", aggregate)})");
            }

            if (queryOptions.OrderBy != null)
            {
                queryArgs.Add($"&$orderby={queryOptions.OrderBy.RawValue}");
            }

            if (queryOptions.Skip != null)
            {
                queryArgs.Add($"&$skip={queryOptions.Skip.RawValue}");
            }

            if (queryOptions.Top != null)
            {
                queryArgs.Add($"&$top={queryOptions.Top.RawValue}");
            }

            if (queryOptions.InlineCount != null)
            {
                queryArgs.Add($"&$inlinecount={queryOptions.InlineCount.RawValue}");
            }

            query = string.Join("", queryArgs);

            return query;
        }

        private (List<string> Attributes,
                List<string> Measures,
                List<string> IgnoreOnBackend,
                Dictionary<string, string> RealNameFor,
            Dictionary<string, string> AgregationFunction) ParseMembers()
        {
            List<string> attributes = new List<string>();
            List<string> measures = new List<string>();
            List<string> ignore = new List<string>();
            Dictionary<string, string> realNameFor = new Dictionary<string, string>();
            Dictionary<string, string> agregationFunction = new Dictionary<string, string>();

            Type type = typeof(T);

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            ODataTableAttribute attrTable = type.GetCustomAttribute<ODataTableAttribute>();

            foreach (var p in properties)
            {
                SAPODataPropertyAttribute attr = p.GetCustomAttribute<SAPODataPropertyAttribute>();

                if (attr == null || attr.AgregationRole == SAPODataPropertyAttribute.AgregationRoleEnum.None)
                {
                    attributes.Add(p.Name);
                }
                else if (attr.AgregationRole == SAPODataPropertyAttribute.AgregationRoleEnum.Dimension)
                {
                    attributes.Add(p.Name);
                }
                else if (attr.AgregationRole == SAPODataPropertyAttribute.AgregationRoleEnum.Measure)
                {
                    measures.Add(p.Name);
                }

                switch (attr?.AgregationFunction)
                {
                    case SAPODataPropertyAttribute.AgregationFunctionEnum.Avg:
                        agregationFunction[p.Name] = "average";
                        break;

                    case SAPODataPropertyAttribute.AgregationFunctionEnum.Sum:
                        agregationFunction[p.Name] = "sum";
                        break;

                    case SAPODataPropertyAttribute.AgregationFunctionEnum.Max:
                        agregationFunction[p.Name] = "max";
                        break;

                    case SAPODataPropertyAttribute.AgregationFunctionEnum.Min:
                        agregationFunction[p.Name] = "min";
                        break;
                }

                ODataPropertyBase attrProp = p.GetCustomAttribute<ODataPropertyBase>(true);

                if (attrProp?.IgnoreOnBackend ?? false)
                {
                    ignore.Add(p.Name);
                }

                if (attrProp == null ||
                    p.Name == "Code" ||
                    p.Name == "Name" ||
                    string.IsNullOrWhiteSpace(attrProp.PhysicalName))
                {
                    realNameFor[p.Name] = p.Name;
                }
                else
                {
                    bool isCustomTable = attrTable?.IsCustom ?? true;
                    bool isCustomField = isCustomTable;

                    if (attrProp != null)
                    {
                        isCustomField = attrProp.IsCustomSetted ? attrProp.IsCustom : isCustomTable;
                    }

                    if (attrProp.PhysicalName == "Code" ||
                        attrProp.PhysicalName == "Name")
                    {
                        realNameFor[p.Name] = attrProp.PhysicalName;
                    }
                    else if (isCustomField)
                    {
                        realNameFor[p.Name] = $"U_{attrProp.PhysicalName}";
                    }
                    else
                    {
                        realNameFor[p.Name] = attrProp.PhysicalName;
                    }
                }
            }

            return (attributes, measures, ignore, realNameFor, agregationFunction);
        }

        private string GetKeyMember()
        {
            string keyName = string.Empty;

            Type type = typeof(T);

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var p in properties)
            {
                KeyAttribute attr = p.GetCustomAttribute<KeyAttribute>();

                if (attr != null)
                {
                    keyName = p.Name;
                    break;
                }
            }

            return keyName;
        }

        private string InjectGroupId(string query)
        {
            string localQuery = query;

            if (string.IsNullOrWhiteSpace(GroupIdFieldName) || string.IsNullOrWhiteSpace(GroupIdValue))
            {
                return localQuery;
            }

            //
            // É preciso injetar o filtro por grupo de parceiro de negócios
            //
            string filterByGroup = $"{GroupIdFieldName} eq '{GroupIdValue}'";

            if (localQuery.Contains("$filter="))
            {
                int position = localQuery.IndexOf("$filter=");
                localQuery = localQuery.Replace("$filter=", $"$filter={filterByGroup} and ");
            }
            else if (localQuery.Contains("filter("))
            {
                int position = localQuery.IndexOf("filter(");
                localQuery = localQuery.Replace("filter(", $"filter({filterByGroup} and ");
            }
            else if (localQuery.Contains("?"))
            {
                int position = localQuery.IndexOf("?");
                localQuery = localQuery.Replace("?", $"?$filter={filterByGroup}&");
            }
            else
            {
                if (localQuery.EndsWith("/"))
                {
                    filterByGroup = $"?$filter={filterByGroup}";
                }
                else
                {
                    filterByGroup = $"/?$filter={filterByGroup}";
                }

                localQuery = $"{localQuery}{filterByGroup}";
            }

            return localQuery;
        }

        string ChangeInternalNameToReal(string query, Dictionary<string, string> realNames = null)
        {
            if (realNames == null)
            {
                var members = ParseMembers();
                realNames = members.RealNameFor;
            }

            foreach (var kv in realNames)
            {
                if (kv.Key != kv.Value)
                {
                    query = Regex.Replace(query, $"\\b{kv.Key}\\b", kv.Value);
                }
            }

            return query;
        }
    }
}
