using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Varsis.Data.Model.Connector;

namespace Varsis.Api.Connector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class POSInvoiceController : Core.Controllers.CommonController<POSInvoice>
    {
        public POSInvoiceController(Data.Infrastructure.ServiceBase service) : base(service)
        {
        }
    }
}