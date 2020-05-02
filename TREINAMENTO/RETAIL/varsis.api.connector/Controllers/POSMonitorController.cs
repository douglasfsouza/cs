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
    public class POSMonitorController : Core.Controllers.CommonController<Varsis.Data.Model.Connector.POSMonitor>
    {
        Data.Infrastructure.ServiceBase Service;

        public POSMonitorController(Data.Infrastructure.ServiceBase service) : base(service)
        {
            Service = service;
        }

        [HttpPost("initMonitor")]
        async public Task<ActionResult> InitMonitor([FromBody] InitMonitorRequest request)
        {
            ActionResult result = BadRequest();

            POSMonitorService monitorService = (POSMonitorService)Service.FindService<Model.POSMonitor>();

            try
            {
                await monitorService.InitMonitoring(new InitMonitorArgs()
                {
                    branchIdLegacy = request.branchIdLegacy
                });

                Core.Utilities.ApiResponseBase response = new Core.Utilities.ApiResponseBase()
                {
                    success = true,
                    data = null,
                    message = null
                };

                result = Ok(response);
            }
            catch(Exception ex)
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

        [HttpPost("listSummary")]
        async public Task<ActionResult> ListSummary([FromBody] Core.QueryRequest request)
        {
            ActionResult result = BadRequest();

            POSMonitorService monitorService = (POSMonitorService)Service.FindService<Model.POSMonitor>();

            List<Criteria> criterias = new List<Criteria>();
            long page = request.page == null ? -1 : request.page.Value;
            long size = request.size == null ? -1 : request.size.Value;

            request?.criterias.ForEach(c =>
            {
                bool ignoreCriteria = false;

                if (c.Field.ToLower().Equals("status") && c.Value == "-1")
                {
                    ignoreCriteria = true;
                }

                if (!ignoreCriteria)
                {
                    criterias.Add(new Criteria()
                    {
                        Field = c.Field,
                        Operator = c.Operator,
                        Value = c.Value
                    });
                }
            });

            try
            {
                var lista = await monitorService.ListSummary(criterias, page, size);

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