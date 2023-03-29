using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DspODataFramework.infra
{
    public class JavascriptDateHandler : DelegatingHandler
    {
        async protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var content = request.Content;
            var buffer = await content.ReadAsByteArrayAsync();
            var payload = System.Text.Encoding.UTF8.GetString(buffer);
            var fixedPayload = fixPayLoad(payload);

            if (payload != fixedPayload)
            {
                request.Content = createNewContent(content, fixedPayload);
            }

            // --> Envia a mensagem para processamento
            var response = await base.SendAsync(request, cancellationToken);
            //---

            //--> SAPUI5 espera receber datas no format JavaScript mesmo (/Date(123456)/)
            //if (response.Content != null)
            //{
            //    content = response.Content;
            //    payload = await content.ReadAsStringAsync();
            //    fixedPayload = fixPayLoad(payload);

            //    if (payload != fixedPayload)
            //    {
            //        response.Content = createNewContent(content, fixedPayload);
            //    }
            //}

            return response;
        }

        private string fixPayLoad(string content)
        {
            var settings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateParseHandling = DateParseHandling.DateTime
            };

            string convertDatePattern = @"\\/Date\([0-9]+\)\\/";
            MatchEvaluator convertDateFormat = new MatchEvaluator((m) =>
            {
                string result = m.Value;
                if (m.Success)
                {
                    string value = $"\"{result}\"";
                    DateTime date = JsonConvert.DeserializeObject<DateTime>(value, settings);
                    result = JsonConvert.SerializeObject(date.Date);
                    result = result.Replace("\"", "");
                }
                return result;
            });

            string removeContentLengthPattern = @"Content-Length: [0-9]+[?:(\n||\r)][?:(||\n)]";
            MatchEvaluator removeContentLengthFormat = new MatchEvaluator((m) =>
            {
                string result = m.Value;
                if (m.Success)
                {
                    result = "";
                }
                return result;
            });


            string payload = Regex.Replace(content, convertDatePattern, convertDateFormat);

            if (payload != content)
            {
                payload = Regex.Replace(payload, removeContentLengthPattern, removeContentLengthFormat);
            }

            return payload;
        }

        private HttpContent createNewContent(HttpContent contentBase, string payload)
        {
            var newContent = new StringContent(payload, System.Text.Encoding.UTF8, contentBase.Headers.ContentType.MediaType);

            //--> Copiando headers para manter integridade da mensagem
            foreach (var h in contentBase.Headers.Where(m => !m.Key.ToLowerInvariant().Equals("content-type")))
            {
                try
                {
                    newContent.Headers.Add(h.Key, h.Value);
                }
                catch
                { }
            };

            //--> Copiando content-type para manter integridade da mensagem
            newContent.Headers.ContentType.MediaType = contentBase.Headers.ContentType.MediaType;
            newContent.Headers.ContentType.CharSet = contentBase.Headers.ContentType.CharSet;
            foreach (var p in contentBase.Headers.ContentType.Parameters)
            {
                newContent.Headers.ContentType.Parameters.Add(new NameValueHeaderValue(p.Name, p.Value));
            }

            newContent.Headers.ContentLength = payload.Length;

            return newContent;
        }

    }
}