using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Varsis.Api.Core.Middleware
{
    public static class ServiceLoginMiddlewareExtensions
    {
        public static IApplicationBuilder UseServiceLogin(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ServiceLoginMiddleware>();
        }
    }
}
