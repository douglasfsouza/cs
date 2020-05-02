using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Varsis.Api.Connector.Contracts;
using Varsis.Api.Core.Utilities;
using Varsis.Data.Infrastructure;
using Varsis.Data.Serviceb1;

namespace Varsis.Api.Connector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessPartnerController : Core.Controllers.CommonController<Varsis.Data.Model.Connector.BusinessPartners>
    {
        Data.Infrastructure.ServiceBase Service;

        public BusinessPartnerController(Data.Infrastructure.ServiceBase service) : base(service)
        {
            Service = service;
        }

    }
}