using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Varsis.Api.Connector;
using Varsis.Api.Core;
using Varsis.Api.Core.Utilities;
using Varsis.Data.Infrastructure;
using Varsis.Data.Model.Connector;

namespace Varsis.Api.Connector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class POSMonitorDetailController : Core.Controllers.CommonController<Varsis.Data.Model.Connector.POSMonitorDetail>
    {
        readonly Data.Infrastructure.ServiceBase _service;

        public POSMonitorDetailController(Data.Infrastructure.ServiceBase service) : base(service)
        {
            _service = service;
        }
    }
}