using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Varsis.Api.Core.Utilities;
using Varsis.Data.Infrastructure;

namespace Varsis.Api.Core.Controllers
{
    public class CommonController<T> : ControllerBase where T : EntityBase
    {
        private readonly ServiceBase _service;

        public CommonController(ServiceBase service)
        {
            _service = service;
        }

        [HttpGet]
        async public virtual Task<ActionResult> Get([FromQuery] Utilities.ListRequestBase request)
        {
            ActionResult result;
            try
            {
                List<T> rows = await _service.List<T>(new List<Criteria>());

                ApiResponseBase response = new ApiResponseBase()
                {
                    success = true,
                    data = rows,
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

        [HttpPost("query")]
        async public virtual Task<ActionResult> query([FromBody] QueryRequest request)
        {
            ActionResult result;
            List<Criteria> cri = new List<Criteria>();
            if (request != null)
            {
                if (request.criterias != null)
                {
                    foreach (var criteria in request.criterias)
                    {
                        cri.Add(new Criteria
                        {
                            Field = criteria.Field,
                            Operator = criteria.Operator,
                            Value = criteria.Value
                        });
                    }
                }
            }
            else
            {
                request = new QueryRequest();
            }

            List<T> rows = await _service.List<T>(cri, request.page.Value, request.size.Value);
            ApiResponseBase response = new ApiResponseBase()
            {
                success = true,
                data = rows,
                message = null,
                paginacao = await _service.totalLinhas<T>(request.size, cri)
            };

            result = Ok(response);

            return result;
        }

        [HttpGet("{id}")]
        async public virtual Task<ActionResult> Get(string id)
        {
            ActionResult result;

            try
            {
                T record = await findRecord(id);

                if (record == null)
                {
                    result = NotFound();
                }
                else
                {
                    ApiResponseBase response = new ApiResponseBase()
                    {
                        success = true,
                        data = record,
                        message = null
                    };

                    result = Ok(response);
                }
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

        [HttpPost]
        async public virtual Task<ActionResult> Post([FromBody] T contract)
        {
            ActionResult result;

            try
            {
                await _service.Insert(contract);

                ApiResponseBase response = new ApiResponseBase()
                {
                    success = true,
                    data = null,
                    message = null
                };

                result = CreatedAtAction(nameof(Post), response);
            }
            catch(Exception ex)
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

        [HttpPost("create")]
        async public virtual Task<ActionResult> Create()
        {
            ActionResult result = BadRequest();

            try
            {
                await _service.Create<T>();
                ApiResponseBase response = new ApiResponseBase()
                {
                    success = true,
                    data = null,
                    message = null
                };
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

        [HttpPut("{id}")]
        async public virtual Task<ActionResult> Put(string id, [FromBody] T contract)
        {
            ActionResult result;

            try
            {
                T record = await findRecord(id);

                if (record == null)
                {
                    result = NotFound();
                }
                else
                {
                    await _service.Update(contract);

                    ApiResponseBase response = new ApiResponseBase()
                    {
                        success = true,
                        data = null,
                        message = null
                    };

                    result = AcceptedAtAction(nameof(Put), response);
                }
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

        [HttpDelete("{id}")]
        async public virtual Task<ActionResult> Delete(string id)
        {
            ActionResult result;

            try
            {
                T record = await findRecord(id);

                if (record == null)
                {
                    result = NotFound();
                }
                else
                {
                    await _service.Delete(record);

                    ApiResponseBase response = new ApiResponseBase()
                    {
                        success = true,
                        data = null,
                        message = null
                    };

                    result = AcceptedAtAction(nameof(Delete), response);
                }
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
        async private Task<T> findRecord(string id)
        {
            var criterias = new List<Criteria>();

            criterias.Add(new Criteria()
            {
                Field = "Code",
                Operator = "eq",
                Value = id
            });

            T record = await _service.Find<T>(criterias);

            return record;
        }
    }
}
