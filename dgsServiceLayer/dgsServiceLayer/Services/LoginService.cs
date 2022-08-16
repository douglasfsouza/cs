using dgsServiceLayer.Models;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace dgsServiceLayer.Services
{
    public class LoginService
    {
        private HttpClient httpClient;
        private CookieContainer cookies;
        private HttpClientHandler handler;

        private HttpClientHandler handlerAPIGateway;
        private CookieContainer cookiesAPIGateway;
        private HttpClient httpClientAPIGateway;

        private String _SessionId;
        private String _SessionIdAPIGateway;
        private String _RouteId;

        private Uri _SLServer;
        private Uri _RSServer;

        private const bool APIGatewayIsOn = true;
        private const string APIGatewayLogin = "/login";
        private const string APIGatewayLogout = "/logout";
        private const string APIGatewayBase = "https://10.101.222.239:60000";

        public LoginService()
        {
            _SLServer = new Uri("https://10.101.222.27:50000/b1s/v1/");

            cookies = new CookieContainer();
            handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            httpClient = new HttpClient(handler);
            httpClient.Timeout = TimeSpan.FromMinutes(120);
            httpClient.BaseAddress = _SLServer;

            httpClient.DefaultRequestHeaders.Add("Prefer", "odata.maxpagesize=0");

            httpClient.DefaultRequestHeaders.Add("B1S-CaseInsensitive", "true");

            httpClient.DefaultRequestHeaders.ExpectContinue = false;
        }
        public async Task<string> Login(Credentials credentials)
        {
            var data = new Dictionary<string, string> {
                    { "UserName", credentials.userName},
                    { "Password", credentials.password},
                    { "CompanyDB",credentials.database}
            };

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync("Login", content);

            string token = string.Empty;

            if (!response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                string message = string.Empty;

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ServiceLayerError error = parseError(body);
                    message = $"Error Connecting to Service Layer: {error.code} {error.message.value}";
                }
                else
                {
                    message = $"Error Connecting to Service Layer: {response.StatusCode} {response.ReasonPhrase}";
                }

                throw new UnauthorizedAccessException(message);
            }
            else
            {
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic finalResult = JsonConvert.DeserializeObject(responseString);

                _SessionId = string.Empty;
                _RouteId = string.Empty;

                if (finalResult != null)
                {
                    _SessionId = finalResult.SessionId;

                    var responseCookies = cookies.GetCookies(_SLServer).Cast<Cookie>();

                    var ck = responseCookies.FirstOrDefault(m => m.Name == "ROUTEID");

                    _RouteId = ck.Value;
                }

                if (_SessionId == string.Empty)
                {
                    throw new UnauthorizedAccessException("Invalid token");
                }

                //if (APIGatewayIsOn)
                //{
                //    _SessionIdAPIGateway = await LoginAPIGateway(credentials);
                //}

                token = toToken(_SessionId, _RouteId, _SessionIdAPIGateway);

                //TST
                //Cookie cookie = new Cookie("B1SESSION", _SessionId.ToString());
                //Cookie cookie2 = new Cookie("ROUTEID", ".node0");

                HttpResponseMessage rest = new HttpResponseMessage();
                System.Net.Http.Headers.CookieHeaderValue cookie = new System.Net.Http.Headers.CookieHeaderValue("B1SESSION", _SessionId);//
                cookie.Path = _SLServer.ToString();
                //List<CookieHeaderValue> mcookies = new List<CookieHeaderValue>(){ cookie };



                rest.Headers.AddCookies(myTst(cookie));



                //addSecurityCookies(true);
                //string tst = await  getQueryResult("https://10.101.222.27:50000/b1s/v1/BusinessPartners");

            }

            return token;
        }

        private IEnumerable<System.Net.Http.Headers.CookieHeaderValue> myTst(System.Net.Http.Headers.CookieHeaderValue c)
        {
            return new System.Net.Http.Headers.CookieHeaderValue[] { c };
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

        public string toToken(string session, string route, string apigateway)
        {
            if (session == string.Empty)
            {
                return string.Empty;
            }

            string source = $"{session};{route}";

            if (!string.IsNullOrWhiteSpace(apigateway))
            {
                source = $"{source};{apigateway}";
            }

            byte[] buffer = Encoding.UTF8.GetBytes(source);

            string token = Convert.ToBase64String(buffer);

            return token;
        }

        public async Task<string> getQueryResult(string query, bool noRoute = false)
        {
            string tk = "NThjMDNjOWUtMDdhNy0xMWVkLTgwMDAtMDIwMDE3MDBhOGI5Oy5ub2RlMw==";
            SetToken(tk);
            //addSecurityCookies(noRoute);
            addSecurityCookies(true);
            
           

            HttpResponseMessage response = null;

            try
            {
                response = await httpClient.GetAsync(query);
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }

            string responseString = string.Empty;

            if (!response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                string message = string.Empty;

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ServiceLayerError error = parseError(body);
                    message = $"Error running query {_SLServer}{query}: {error.code} {error.message.value}";
                }
                else
                {
                    message = $"Error running query {_SLServer}{query}: {response.StatusCode} {response.ReasonPhrase}";
                }
            }
            else
            {
                responseString = await response.Content.ReadAsStringAsync();
            }

            return responseString;
        }

        public async Task<string> getQueryResultDg(string pQuery, string token)
        {
            string query = $"{_SLServer}{pQuery}";
            string result = string.Empty;
            SetToken(token);
            string domain = "10.101.222.27";
            var httpWebGetRequest = (HttpWebRequest)WebRequest.Create(query);
            httpWebGetRequest.ContentType = "application/json";
            httpWebGetRequest.Method = "GET";
            httpWebGetRequest.KeepAlive = true;
            httpWebGetRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebGetRequest.Headers.Add("B1S-WCFCompatible", "true");
            httpWebGetRequest.Headers.Add("B1S-MetadataWithoutSession", "true");
            httpWebGetRequest.Accept = "*/*";
            httpWebGetRequest.ServicePoint.Expect100Continue = false;
            httpWebGetRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            httpWebGetRequest.AutomaticDecompression = DecompressionMethods.GZip;
            CookieContainer cookies = new CookieContainer();
            cookies.Add(new Cookie("B1SESSION", _SessionId) { Domain = domain });
            cookies.Add(new Cookie("ROUTEID", ".node1") { Domain = domain });
            httpWebGetRequest.CookieContainer = cookies;

            try
            {
                var httpGetResponse = (HttpWebResponse)httpWebGetRequest.GetResponse();
                using (var streamReader = new StreamReader(httpGetResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        private void addSecurityCookies(bool noRoute = false)
        {

            Cookie sessionCookie = null;
            Cookie routeCookie = null;

            foreach (Cookie c in cookies.GetCookies(_SLServer))
            {
                if (c.Name == "B1SESSION")
                {
                    sessionCookie = c;
                }
                else if (c.Name == "ROUTEID")
                {
                    routeCookie = c;
                }
            }

            if (sessionCookie == null)
            {
                cookies.Add(_SLServer, new Cookie("B1SESSION", _SessionId));
            }
            else if (sessionCookie.Value != _SessionId)
            {
                sessionCookie.Value = _SessionId;
            }

            if (routeCookie == null)
            {
                if (noRoute)
                {
                    cookies.Add(_SLServer, new Cookie("ROUTEID", ".node0"));
                }
                else
                {
                    cookies.Add(_SLServer, new Cookie("ROUTEID", _RouteId));
                }
            }
            else
            {
                if (noRoute)
                {
                    routeCookie.Value = ".node0";
                }
                else if (routeCookie.Value != _RouteId)
                {
                    routeCookie.Value = _RouteId;
                }
            }
        }

        private async Task<string> LoginAPIGateway(Credentials credentials)
        {
            var data = new Dictionary<string, string> {
                    { "UserName", credentials.userName},
                    { "Password", credentials.password},
                    { "CompanyDB",credentials.database}
            };

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClientAPIGateway.PostAsync(APIGatewayLogin, content);

            string token = string.Empty;

            if (!response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                string message = string.Empty;

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ServiceLayerError error = parseError(body);
                    message = $"Error Connecting to API Gateway: {error.code} {error.message.value}";
                }
                else
                {
                    message = $"Error Connecting to API Gateway: {response.StatusCode} {response.ReasonPhrase}";
                }

                throw new UnauthorizedAccessException(message);
            }
            else
            {
                var responseString = await response.Content.ReadAsStringAsync();

                var responseCookies = cookiesAPIGateway.GetCookies(_RSServer).Cast<Cookie>();

                var ck = responseCookies.FirstOrDefault(m => m.Name == "SESSION");

                _SessionIdAPIGateway = ck.Value ?? string.Empty;

                if (string.IsNullOrWhiteSpace(_SessionIdAPIGateway))
                {
                    throw new UnauthorizedAccessException("Invalid token");
                }

                token = _SessionIdAPIGateway;

            }

            return token;
        }

        public void SetToken(string token)
        {
            (_SessionId, _RouteId) = fromToken(token);

            if (string.IsNullOrEmpty(_SessionId))
            {
                throw new ArgumentException("Token inválido");
            }
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

            if (values.Length >= 2)
            {
                return (values[0], values[1]);
            }
            else
            {
                return ("", "");
            }
        }
    }
}
