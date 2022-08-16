using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace dgsServiceLayerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //Login();

            string domain = "10.101.222.27";
            string sessionId = "3fe14280-07a3-11ed-8000-02001700a8b9";

             Get(sessionId, domain);
        }

        private static void Login()
        {
            string data = "{    \"CompanyDB\": \"HOMOLOGACAO\",    \"UserName\": \"manager\",       \"Password\": \"Varsis@02\"}";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://10.101.222.27:50000/b1s/v1/Login");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            httpWebRequest.Headers.Add("B1S-WCFCompatible", "true");
            httpWebRequest.Headers.Add("B1S-MetadataWithoutSession", "true");
            httpWebRequest.Accept = "*/*";
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            { streamWriter.Write(data); }

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Console.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private static void Get(string SessionId, string Domain)
        {
            var httpWebGetRequest = (HttpWebRequest)WebRequest.Create("https://10.101.222.27:50000/b1s/v1/BusinessPartners");
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
            cookies.Add(new Cookie("B1SESSION", SessionId) { Domain = Domain });
            cookies.Add(new Cookie("ROUTEID", ".node1") { Domain = Domain });
            httpWebGetRequest.CookieContainer = cookies;

            try
            {
                var httpGetResponse = (HttpWebResponse)httpWebGetRequest.GetResponse();
                using (var streamReader = new StreamReader(httpGetResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Console.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                 
            }
            
        }
    }
}
