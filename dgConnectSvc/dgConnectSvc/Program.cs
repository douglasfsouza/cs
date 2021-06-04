using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace dgConnectSvc
{
    class Program
    {
        static void Main(string[] args)
        {
            var uri = "http://sales.fourthcoffee.com/SalesService.svc/GetSalesPerson";
            var request = WebRequest.Create(uri) as HttpWebRequest;
            var username = "jespera";
            var password = "P$$W0rd";
            request.Credentials = new NetworkCredential(username, password);
            //or
            //request.Credentials = CredentialCache.DefaultCredentials;

            //or with certifate
            //var certificate = FourthCoffeeServices.GetCertificate();
            //request.ClientCertificates.Add(certificate);

            var response = request.GetResponse() as HttpWebResponse;
            var status = response.StatusCode;

            Console.WriteLine(status);
        }
    }
}
