using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Varsis.Api.Core.Middleware
{
    public static class ServiceLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseServiceLog(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ServiceLogMiddleware>();
        }
    }
}
