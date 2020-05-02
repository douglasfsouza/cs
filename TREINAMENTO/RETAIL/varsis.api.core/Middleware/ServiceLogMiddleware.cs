using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Varsis.Data.Infrastructure;

namespace Varsis.Api.Core.Middleware
{
    public class ServiceLogMiddleware
    {
        readonly RequestDelegate _next;
        readonly ILogger<ServiceLoginMiddleware> _logger;

        public ServiceLogMiddleware(RequestDelegate next, ILogger<ServiceLoginMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, ServiceBase service)
        {
            await LogRequest(context.Request);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                await LogResponse(context);
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        async private Task LogRequest(HttpRequest request)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var payload = string.Empty;

                if (!HttpMethods.IsGet(request.Method))
                {
                    payload = await new StreamReader(request.Body).ReadToEndAsync();
                    var buffer = Encoding.UTF8.GetBytes(payload);
                    request.Body = new MemoryStream(buffer);
                }

                if (!string.IsNullOrEmpty(payload))
                {
                    var uri = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}";
                    _logger.LogDebug($"Request payload {uri}\r\n--{request.ContentType}\r\n{payload}\r\n--");
                }
            }
        }

        private async Task LogResponse(HttpContext context)
        {
            using (var buffer = new MemoryStream())
            {
                //invoke the rest of the pipeline
                await _next.Invoke(context);

                //reset the buffer and read out the contents
                buffer.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(buffer);
                using (var bufferReader = new StreamReader(buffer))
                {
                    string body = await bufferReader.ReadToEndAsync();

                    HttpRequest request = context.Request;
                    HttpResponse response = context.Response;

                    if (!string.IsNullOrEmpty(body))
                    {
                        var uri = $"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}";
                        _logger.LogDebug($"Response payload {uri}\r\n--{response.ContentType}\r\n{body}\r\n--");
                    }

                    //reset to start of stream
                    buffer.Seek(0, SeekOrigin.Begin);
                }
            }
        }
    }
}
