using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData;
using System.Web.Http;
using System.Xml.Linq;
using DspODataFramework.Service;

namespace DspODataFramework.Controllers
{
    public class ControllerBase<T> : ODataController where T : class, new()
    {
        readonly ServiceBase<T> _service;

        public ControllerBase(ServiceBase<T> service)
        {
            _service = service;
        }

        [HttpGet]
        [ActionName("$count")]
        async public virtual Task<IHttpActionResult> GetCount([FromUri(Name = "$filter")] string filter = "")
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                long result = await _service.Count(filter);

                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StringContent(result.ToString(), System.Text.Encoding.UTF8, "text/plain");
                return ResponseMessage(response);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ReasonPhrase = ex.Message;
            }

            return ResponseMessage(response);
        }

        public static HttpResponseMessage CreateDownloadResponse(string mimeType, byte[] data)
        {
            HttpResponseMessage result = null;

            MemoryStream memo = new MemoryStream(data);

            var responseContent = new StreamContent(memo);

            responseContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
            responseContent.Headers.ContentLength = data.Length;

            responseContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            responseMessage.Content = responseContent;

            result = responseMessage;

            return result;
        }
    }
}
