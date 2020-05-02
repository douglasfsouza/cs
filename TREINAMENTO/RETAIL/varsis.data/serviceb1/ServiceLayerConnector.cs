using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Varsis.Data.Infrastructure;
using System.Net.Http.Headers;
using WebUtilities = Microsoft.AspNetCore.WebUtilities;
using System.IO;
using Microsoft.AspNetCore.WebUtilities;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Varsis.Data.Serviceb1
{
    public class ServiceLayerConnector
    {
        readonly ILogger _logger;

        private HttpClient httpClient;

        private CookieContainer cookies;
        private HttpClientHandler handler;

        private String _SessionId;
        private String _RouteId;

        private Uri _SLServer;

        public string SessionId => _SessionId;
        public string RouteId => _RouteId;

        public ServiceLayerConnector(IConfiguration appConfiguration, ILogger logger)
        {
            _logger = logger;

            ConfigurationService serviceConfig = new ConfigurationService();

            string environment = appConfiguration["environment"];

            appConfiguration.GetSection(environment)
                            .GetSection("businessone")
                            .Bind(serviceConfig);

            _SLServer = new Uri(serviceConfig.serviceLayer);

            cookies = new CookieContainer();
            handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromMinutes(120);
            httpClient.BaseAddress = _SLServer;
            httpClient.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=0");

            logger.LogDebug($"Service Layer: {_SLServer}");
        }

        public void SetToken(string token)
        {
            (_SessionId, _RouteId) = fromToken(token);

            if (string.IsNullOrEmpty(_SessionId))
            {
                throw new ArgumentException("Token inválido");
            }
        }

        public async Task<string> Login(Credentials credentials)
        {
            var data = new Dictionary<string, string> {
                    { "UserName", credentials.userName},
                    { "Password", credentials.password},
                    { "CompanyDB",credentials.database}
            };

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            _logger.LogInformation($"Login: {credentials.userName}@{credentials.database}");

            HttpResponseMessage response = await httpClient.PostAsync("Login", content);

            string token = string.Empty;

            if (!response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                string message = string.Empty;

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ServiceLayerError error = parseError(body);
                    message = $"Error Connecting to Service Layer: {error.code} {error.message}";
                }
                else
                {
                    message = $"Error Connecting to Service Layer: {response.StatusCode} {response.ReasonPhrase}";
                }

                _logger.LogError(message);
            }
            else
            {
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic finalResult = JsonConvert.DeserializeObject(responseString);

                if (finalResult != null)
                {
                    _SessionId = finalResult.SessionId;

                    var responseCookies = cookies.GetCookies(_SLServer).Cast<Cookie>();

                    var ck = responseCookies.FirstOrDefault(m => m.Name == "ROUTEID");

                    _RouteId = ck.Value;
                }

                _logger.LogInformation("Login succeded");

                token = toToken(_SessionId, _RouteId);
            }

            return token;
        }

        public async Task Logout()
        {
            //var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync("Logout", null);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException("Erro ao efetuar logout");
            }
        }

        private string toToken(string session, string route)
        {
            if (session == string.Empty)
            {
                return string.Empty;
            }

            string source = $"{session};{route}";

            byte[] buffer = Encoding.UTF8.GetBytes(source);

            string token = Convert.ToBase64String(buffer);

            return token;
        }

        private (string, string) fromToken(string token)
        {
            if (token == string.Empty)
            {
                return ("", "");
            }

            byte[] buffer = Convert.FromBase64String(token);
            string target = Encoding.UTF8.GetString(buffer);
            string[] values = target.Split(';');

            if (values.Length == 2)
            {
                return (values[0], values[1]);
            }
            else
            {
                return ("", "");
            }
        }

        public async Task<string> getQueryResult(string query, bool noRoute = false)
        {
            addSecurityCookies(noRoute);

            _logger.LogInformation($"GET {_SLServer}{query}");

            HttpResponseMessage response= await httpClient.GetAsync(query);

            string responseString = string.Empty;

            if (!response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                string message = string.Empty;

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ServiceLayerError error = parseError(body);
                    message = $"Error running query {_SLServer}{query}: {error.code} {error.message}";
                }
                else
                {
                    message = $"Error running query {_SLServer}{query}: {response.StatusCode} {response.ReasonPhrase}";
                }

                _logger.LogError(message);
            }
            else
            {
                _logger.LogInformation($"Success: {_SLServer}{query}");
                responseString = await response.Content.ReadAsStringAsync();
            }

            return responseString;
        }

        public async Task<ServiceLayerResponse> Post(string query, string payload, bool noRoute = false, bool returnContent = false)
        {
            return await Send(HttpMethod.Post, query, payload, noRoute, returnContent);
        }

        public async Task<ServiceLayerResponse> Post(IBatchProducer batch, bool returnContent = false)
        {
            MultipartContent multipart = new MultipartContent("mixed");

            StringBuilder messageBuilder = new StringBuilder(512);
            string messageTemplate = string.Empty;

            messageBuilder.AppendLine("{0} {1}");
            messageBuilder.AppendLine("Content-Type: application/json; charset=utf-8");
            if (!returnContent)
            {
                messageBuilder.AppendLine("Prefer: return-no-content");
            }
            
            messageBuilder.AppendLine("");
            messageBuilder.AppendLine("{2}");
            messageBuilder.AppendLine("");

            messageTemplate = messageBuilder.ToString();

            int index = 0;

            IBatchProvider provider = (IBatchProvider)batch;

            foreach (var s in provider.Items)
            {
                index++;
                string message = string.Format(messageTemplate, s.method.Method, s.query, s.payload);

                StringContent json = new StringContent(message, Encoding.UTF8, "application/json");

                json.Headers.ContentType = new MediaTypeHeaderValue("application/http");
                json.Headers.Add("Content-Transfer-Encoding", "binary");
                json.Headers.Add("Content-ID", index.ToString());

                multipart.Add(json);

                _logger.LogInformation($"POST (batch) {_SLServer}{s.query}");
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(s.payload);
                }
            }

            return await Send(HttpMethod.Post, "$batch", multipart, false);
        }

        public IBatchProducer CreateBatch()
        {
            IBatchProducer batch = new ServiceLayerBatch();
            return batch;
        }

        public async Task<ServiceLayerResponse> PostMany(string query, List<string> payload, bool noRoute = false, bool returnContent = false)
        {
            MultipartContent multipart = new MultipartContent("mixed");

            StringBuilder messageBuilder = new StringBuilder(512);
            string messageTemplate = string.Empty;

            messageBuilder.AppendLine($"POST /{query}");
            messageBuilder.AppendLine("Content-Type: application/json; charset=utf-8");
            if (!returnContent)
            {
                messageBuilder.AppendLine("Prefer: return-no-content");
            }
            messageBuilder.AppendLine("");
            messageBuilder.AppendLine("{0}");
            messageBuilder.AppendLine("");

            messageTemplate = messageBuilder.ToString();

            int index = 0;

            foreach (var s in payload)
            {
                index++;
                string message = string.Format(messageTemplate, s);

                StringContent json = new StringContent(message, Encoding.UTF8, "application/json");

                json.Headers.ContentType = new MediaTypeHeaderValue("application/http");
                json.Headers.Add("Content-Transfer-Encoding", "binary");
                json.Headers.Add("Content-ID", index.ToString());

                multipart.Add(json);
            }

            return await Send(HttpMethod.Post, "$batch", multipart, noRoute);
        }

        public async Task<ServiceLayerResponse> Put(string query, string payload, bool noRoute = false, bool returnContent = false)
        {
            return await Send(HttpMethod.Put, query, payload, noRoute);
        }
        public async Task<ServiceLayerResponse> Patch(string query, string payload, bool noRoute = false, bool returnContent = false)
        {
            return await Send(HttpMethod.Patch, query, payload, noRoute);
        }
        public async Task<ServiceLayerResponse> Delete(string query, string payload, bool noRoute = false)
        {
            return await Send(HttpMethod.Delete, query, payload, noRoute);
        }
        public async Task<ServiceLayerResponse> Send(HttpMethod method, string query, string payload, bool noRoute = false, bool returnContent = false)
        {
            _logger.LogInformation($"{method.Method.ToUpper()} {_SLServer}{query}");
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(payload);
            }

            StringContent json = new StringContent(payload, Encoding.UTF8, "application/json");
            return await Send(method, query, json, noRoute, returnContent);
        }
        public async Task<ServiceLayerResponse> Send(HttpMethod method, string query, HttpContent payload, bool noRoute = false, bool returnContent = false)
        {
            ServiceLayerResponse slResponse = new ServiceLayerResponse();

            addSecurityCookies(noRoute);

            HttpRequestMessage message = new HttpRequestMessage(method, query);
            message.Content = payload;
            if (!returnContent)
            {
                message.Headers.Add("Prefer", "return-no-content");
            }
            HttpResponseMessage response = new HttpResponseMessage();

            DateTime inicio = DateTime.Now;
            try
            {
                 response = await httpClient.SendAsync(message);
            }
            catch (Exception e)
            {

            }

            var fim = DateTime.Now - inicio;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                ServiceLayerError error = parseError(responseContent);

                slResponse.success = false;
                slResponse.errorCode = error.code;
                slResponse.errorMessage = error.message.value;

                _logger.LogError($"{method.Method.ToUpper()} {query}: {error.code} {error.message.value}");
            }
            else if (!response.IsSuccessStatusCode)
            {
                slResponse.success = false;
                slResponse.errorCode = response.StatusCode.ToString();
                slResponse.errorMessage = response.ReasonPhrase;

                _logger.LogError($"{method.Method.ToUpper()} {query}: {slResponse.errorCode} {slResponse.errorMessage}");
            }
            else
            {
                slResponse.success = true;
                slResponse.errorCode = "";
                slResponse.errorMessage = "";

                _logger.LogInformation($"{method.Method.ToUpper()} {query}: Success");

                if (payload is MultipartContent)
                {
                    slResponse.internalResponses = await parseMultiPartResponse(response);

                    if (slResponse.internalResponses != null)
                    {
                        foreach(var r in slResponse.internalResponses)
                        {
                            if (!r.success)
                            {
                                _logger.LogError($"{method.Method.ToUpper()} {query}: {r.errorCode} {r.errorMessage}");
                            }
                        }
                    }
                }
                else
                {
                    slResponse.data = await response.Content.ReadAsStringAsync();
                }
            }

            return slResponse;
        }

        async public Task<List<ServiceLayerResponse>> parseMultiPartResponse(HttpResponseMessage response)
        {
            List<ServiceLayerResponse> responseList = new List<ServiceLayerResponse>();

            var qp = from p in response.Content.Headers.ContentType.Parameters
                     where p.Name == "boundary"
                     select p;

            var parameter = qp.FirstOrDefault();

            string boundary = parameter == null ? "" : parameter.Value;

            using (Stream responseMessage = await response.Content.ReadAsStreamAsync())
            using (StreamReader reader = new StreamReader(responseMessage))
            {
                int statusCode = 0;
                bool beginData = false;
                string statusMessage = string.Empty;
                StringBuilder submessage = new StringBuilder(512);

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (line.StartsWith($"--{boundary}"))
                    {
                        if (submessage.Length > 0)
                        {
                            ServiceLayerResponse slResponse = new ServiceLayerResponse();

                            if (statusCode == (int)HttpStatusCode.BadRequest)
                            {
                                ServiceLayerError error = parseError(submessage.ToString());
                                slResponse.success = false;
                                slResponse.errorCode = error.code;
                                slResponse.errorMessage = error.message.value;

                            }
                            else if (statusCode >= 300)
                            {
                                slResponse.success = false;
                                slResponse.errorCode = statusCode.ToString();
                                slResponse.errorMessage = statusMessage;
                            }
                            else
                            {
                                slResponse.success = true;
                                slResponse.errorCode = "";
                                slResponse.errorMessage = "";
                                slResponse.data = submessage.ToString();
                            }

                            responseList.Add(slResponse);

                            submessage.Clear();
                            statusCode = 0;
                            statusMessage = string.Empty;
                            beginData = false;
                        }
                    }
                    else if (line.StartsWith("HTTP"))
                    {
                        string[] httpResponse = line.Split(' ');
                        statusCode = int.Parse(httpResponse[1]);
                        statusMessage = string.Join(' ', httpResponse, 2, httpResponse.Length - 2);
                    }
                    else if (statusCode != 0 && line == string.Empty)
                    {
                        beginData = true;
                    }
                    else if (beginData)
                    {
                        submessage.Append(line);
                    }
                }
            }

            return responseList;
        }

        public async Task<string> getQueryResultSemantic(string query)
        {
            string querySemantic = $"sml.svc/{query}";
            string result = await this.getQueryResult(querySemantic);
            result = result.Replace("-NULL-", "");
            return result;
        }

        private ServiceLayerError parseError(string error)
        {
            ServiceLayerError result = null;

            using (System.IO.StringReader sreader = new System.IO.StringReader(error))
            using (JsonTextReader jsonTextReader = new JsonTextReader(sreader))
            {
                JToken j = JToken.ReadFrom(jsonTextReader);

                string response = j.SelectToken("error").ToString();

                result = JsonConvert.DeserializeObject<ServiceLayerError>(response);
            }

            return result;
        }

        private void addSecurityCookies(bool noRoute = false)
        {
            if (cookies.GetCookies(_SLServer).Count(m => m.Name == "B1SESSION") == 0)
            {
                cookies.Add(_SLServer, new Cookie("B1SESSION", _SessionId));
            }

            if (noRoute)
            {
                if (cookies.GetCookies(_SLServer).Count(m => m.Name == "ROUTEID") > 0)
                {
                    var cookie = cookies.GetCookies(_SLServer)["ROUTEID"];
                    cookie.Value = ".node0";
                }
                else
                {
                    cookies.Add(_SLServer, new Cookie("ROUTEID", ".node0"));
                }
            }
            else
            {
                if (cookies.GetCookies(_SLServer).Count(m => m.Name == "ROUTEID") == 0)
                {
                    cookies.Add(_SLServer, new Cookie("ROUTEID", _RouteId));
                }
                else
                {
                    var cookie = cookies.GetCookies(_SLServer)["ROUTEID"];
                    cookie.Expired = false;
                }
            }
        }
    }
}
