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
    public class ServiceLoginMiddleware
    {
        readonly RequestDelegate _next;

        const string HEADER_VARSIS_TOKEN = "varsis_token";

        public ServiceLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ServiceBase service)
        {
            string token = string.Empty;

            // Resultado DEFAULT
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            if (context.Request.Headers.ContainsKey(HEADER_VARSIS_TOKEN))
            {
                token = context.Request.Headers[HEADER_VARSIS_TOKEN];

                if (isValidToken(token))
                {
                    service.SetToken(token);
                    context.Response.StatusCode = StatusCodes.Status200OK;
                }
            }
            else if (context.Request.Path.Value.StartsWith("/api/login", StringComparison.InvariantCultureIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
            }

            // Short-circuit
            if (context.Response.StatusCode == StatusCodes.Status200OK)
            {
                await _next.Invoke(context);
            }
        }

        private bool isValidToken(string token)
        {
            // TODO: Implementar validação da expiração do token
            return true;
        }
    }
}
