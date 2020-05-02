using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using Varsis.Data.Infrastructure;

namespace Varsis.Data.Serviceb1
{
    public static class Global
    {
        public static string MakeODataQuery(string table, string[] selectList = null, string[] filterList = null, string[] orderByList = null, long? pagina = 1, long? size = null)
        {
            System.Text.StringBuilder strb = new System.Text.StringBuilder(1024);

            bool firstClause = true;

            strb.Append(table);
            strb.Append("?");

            if (selectList != null)
            {
                firstClause = false;

                strb.Append("$select=");
                bool first = true;

                foreach (string s in selectList)
                {
                    if (!first)
                    {
                        strb.Append(",");
                    }
                    strb.Append(s);
                    first = false;
                }
            }

            if (filterList != null)
            {
                if (!firstClause)
                {
                    strb.Append("&");
                }

                firstClause = false;
                strb.Append("$filter=");
                bool first = true;

                foreach (string s in filterList)
                {
                    if (!first)
                    {
                        strb.Append(" and ");
                    }
                    strb.Append(s);
                    first = false;
                }
            }

            if (orderByList != null)
            {
                if (!firstClause)
                {
                    strb.Append("&");
                }

                firstClause = false;
                strb.Append("$orderby=");
                bool first = true;

                foreach (string s in orderByList)
                {
                    if (!first)
                    {
                        strb.Append(",");
                    }
                    strb.Append(s);
                    first = false;
                }
            }

            if (size > 0)
            {
                if (!firstClause)
                {
                    strb.Append("&");
                }

                firstClause = false;
                pagina = pagina - 1;
                long skip = pagina.Value * size.Value;
                strb.Append(String.Format("$top={0}&$skip={1}", size, skip));
            }

            return Uri.EscapeUriString(strb.ToString());
        }

        public static DateTime? toDate(this string value)
        {
            DateTime? result = null;
            DateTime convert;



            if (DateTime.TryParse(value, out convert))
            {
                result = convert;
            }



            return result;
        }



        public static int toInt(this string value)
        {
            int result = 0;
            int.TryParse(value, out result);
            return result;
        }
        public static long toLong(this string value)
        {
            long result = 0;
            long.TryParse(value, out result);
            return result;
        }

        public static string BuildQuery(string table, string[] selectList = null, string[] filterList = null, string[] orderByList = null)
        {
            StringBuilder fields = new StringBuilder(1024);
            StringBuilder filter = new StringBuilder(1024);
            StringBuilder order = new StringBuilder(1024);
            StringBuilder query = new StringBuilder(1024);

            if (selectList != null)
            {
                foreach (string s in selectList)
                {
                    if (fields.Length != 0)
                    {
                        fields.Append(",");
                    }
                    fields.Append(s);
                }
            }

            if (filterList != null)
            {
                foreach (string s in filterList)
                {
                    if (filter.Length != 0)
                    {
                        filter.Append(",");
                    }
                    filter.Append(s);
                }
            }

            if (orderByList != null)
            {
                foreach (string s in orderByList)
                {
                    if (order.Length != 0)
                    {
                        order.Append(",");
                    }
                    order.Append(s);
                }
            }

            query.Append($"{table}");

            if (fields.Length > 0 || filter.Length > 0 || order.Length > 0)
            {
                query.Append("?");

                bool first = true;

                if (fields.Length > 0)
                {
                    if (!first)
                    {
                        query.Append("&");
                    }
                    query.Append(fields);
                    first = false;
                }

                if (filter.Length > 0)
                {
                    if (!first)
                    {
                        query.Append("&");
                    }
                    query.Append(filter);
                    first = false;
                }

                if (order.Length > 0)
                {
                    if (!first)
                    {
                        query.Append("&");
                    }
                    query.Append(order);
                    first = false;
                }
            }

            return Uri.EscapeUriString(query.ToString());
        }

        public static string MakeODataQuerySemantic(string view, string[] parameters, string[] selectList = null, string[] filterList = null, string[] orderByList = null)
        {
            System.Text.StringBuilder strb = new System.Text.StringBuilder(1024);

            bool firstClause = true;

            if (parameters == null || parameters.Length == 0)
            {
                strb.Append($"{view}");
            }
            else
            {
                var p = String.Join(",", parameters);
                strb.Append($"{view}Parameters({p})/{view}");
            }

            if (selectList != null && selectList.Length > 0)
            {
                strb.Append(firstClause ? "?" : "&");
                firstClause = false;

                var s = String.Join(",", selectList);
                strb.Append($"$select={s}");
            }

            if (filterList != null && filterList.Length > 0)
            {
                strb.Append(firstClause ? "?" : "&");
                firstClause = false;

                var f = String.Join(" and ", filterList);
                strb.Append($"$filter={f}");
            }

            if (orderByList != null && orderByList.Length > 0)
            {
                strb.Append(firstClause ? "?" : "&");
                firstClause = false;

                var o = String.Join(",", orderByList);
                strb.Append($"$orderby={o}");
            }

            return Uri.EscapeUriString(strb.ToString());
        }

        public static List<ExpandoObject> parseQueryToCollection(string query)
        {
            List<ExpandoObject> result = null;

            try
            {
                using (System.IO.StringReader sreader = new System.IO.StringReader(query))
                using (JsonTextReader jsonTextReader = new JsonTextReader(sreader))
                {
                    JToken j = JToken.ReadFrom(jsonTextReader);

                    string response = j.SelectToken("value").ToString();

                    JsonSerializerSettings settings = new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Include
                    };

                    List<ExpandoObject> items = JsonConvert.DeserializeObject<List<ExpandoObject>>(response, settings);
                    result = items;
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }

        public static ExpandoObject parseQueryToObject(string query)
        {
            ExpandoObject result = null;

            try
            {
                using (System.IO.StringReader sreader = new System.IO.StringReader(query))
                using (JsonTextReader jsonTextReader = new JsonTextReader(sreader))
                {
                    JToken j = JToken.ReadFrom(jsonTextReader);

                    string response = j.ToString();

                    JsonSerializerSettings settings = new JsonSerializerSettings()
                    {
                        NullValueHandling = NullValueHandling.Include
                    };

                    ExpandoObject item = JsonConvert.DeserializeObject<ExpandoObject>(response, settings);
                    result = item;
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }

        public static  string[] parseCriterias(List<Criteria> criterias, Dictionary<string, string> fieldMap, Dictionary<string, string> fieldType)
        {
            List<string> filter = new List<string>();
            if (criterias?.Count != 0)
            {
                foreach (var c in criterias)
                {
                    string type = string.Empty;
                    string field = string.Empty;

                    if (fieldMap.ContainsKey(c.Field.ToLower()))
                    {
                        field = fieldMap[c.Field.ToLower()];
                        type = fieldType[c.Field.ToLower()];
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

            return filter.ToArray();
        }


    }
}
