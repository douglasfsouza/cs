using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Varsis.Api.Connector;
using Varsis.Api.Connector.Contracts;
using Varsis.Data.Infrastructure;
using Varsis.Data.Serviceb1.Connector;
using Model = Varsis.Data.Model.Connector;

namespace Varsis.Api.Connector.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class POSBranchConfigController : Core.Controllers.CommonController<Varsis.Data.Model.Connector.POSBranchConfig>
    {
        Data.Infrastructure.ServiceBase Service;

        public POSBranchConfigController(Data.Infrastructure.ServiceBase service) : base(service)
        {
            Service = service;
        }

 
        [HttpPost("listSummary")]
        async public Task<ActionResult> ListSummary([FromBody] Core.QueryRequest request)
        {
            ActionResult result = BadRequest();

            POSBranchConfigService service = (POSBranchConfigService)Service.FindService<Model.POSBranchConfig>();

            List<Criteria> criterias = new List<Criteria>();
            long page = request.page == null ? -1 : request.page.Value;
            long size = request.size == null ? -1 : request.size.Value;

            try
            {
                var lista = await service.ListSummary(criterias, page, size);

                Core.Utilities.ApiResponseBase response = new Core.Utilities.ApiResponseBase()
                {
                    success = true,
                    data = lista,
                    message = null
                };

                result = Ok(response);
            }
            catch (Exception ex)
            {
                Core.Utilities.ApiResponseBase response = new Core.Utilities.ApiResponseBase()
                {
                    success = false,
                    data = null,
                    message = ex.Message
                };

                result = BadRequest(response);
            }

            return result;
        }
    }
}