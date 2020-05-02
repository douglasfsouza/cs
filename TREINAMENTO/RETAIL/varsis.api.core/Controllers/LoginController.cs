using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Varsis.Data.Infrastructure;
using Varsis.Api.Core.Utilities;

namespace Varsis.Api.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        ServiceBase _service;

        public LoginController(ServiceBase service)
        {
            _service = service;
        }

        [HttpPost]
        async public Task<ActionResult> Post([FromBody] Credentials credentials)
        {
            ActionResult result = BadRequest();

            try
            {
                string token = await _service.Login(credentials);

                ApiResponseBase response = new ApiResponseBase()
                {
                    success = true,
                    data = token,
                    message = null
                };

                result = Ok(response);
            }
            catch (Exception ex)
            {
                ApiResponseBase response = new ApiResponseBase()
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