using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Hosting;

namespace DspODataFramework.infra
{
    /// <summary>
    /// Built with assistance from https://gist.github.com/raghuramn/5556691#file-dollarformathandler-cs-L42
    /// <para>
    /// Checks compatibility against MaxDataServiceVersion per OData v3 Spec
    /// http://www.odata.org/documentation/odata-version-3-0/json-verbose-format/#clientserviceformatcompatibilityandversions
    /// </para>
    /// </summary>
    public class FormatSupportHandler : DelegatingHandler
    {
        private readonly string jsonMediaType = "application/json";
        private readonly string verboseJsonMediaType = "application/json;odata=verbose"; // creates OData v2 parseable response
        private readonly string maxDataServiceVersionHeaderKey = "MaxDataServiceVersion";
        private readonly string formatQueryParameterKey = "$format";
        private readonly string dataServiceVersionODataV2 = "2.0";

        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var queryParams = request.GetQueryNameValuePairs();
            var dollarFormat = queryParams.Where(kvp => kvp.Key == formatQueryParameterKey).Select(kvp => kvp.Value).FirstOrDefault();
            var maxDataServiceVersionHeader = request.Headers.Where(h => h.Key == maxDataServiceVersionHeaderKey).Select(h => h.Value).FirstOrDefault();
            string maxDataServiceVersion = null;

            if (maxDataServiceVersionHeader != null)
                maxDataServiceVersion = maxDataServiceVersionHeader.ElementAtOrDefault(0);

            if (dollarFormat != null)
            {
                request.Headers.Accept.Clear();
                try
                {
                    request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(dollarFormat));
                }
                catch (FormatException fe)
                {
                    // log invalid format request
                    System.Diagnostics.Debug.WriteLine(string.Format("Error adding $format Accept header '{0}': {1}", dollarFormat, fe.Message));
                    request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(verboseJsonMediaType)); // fallback on verbose JSON
                }

                // remove $format from the request
                request.Properties[HttpPropertyKeys.RequestQueryNameValuePairsKey] = queryParams.Where(kvp => kvp.Key != formatQueryParameterKey);
            }
            else
            {
                if (maxDataServiceVersion == dataServiceVersionODataV2 &&
                    request.Headers.Accept.FirstOrDefault().MediaType == jsonMediaType)
                {
                    request.Headers.Accept.Clear();
                    request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(verboseJsonMediaType));
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
