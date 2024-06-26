﻿using DspODataFramework.infra.attributes;
using DspODataFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using System.Xml.Linq;
using System.CodeDom;
using DspODataFramework.Service;
using System.Web.Http.OData.Extensions;
using Amazon.Runtime.Internal.Util;

namespace DspODataFramework.Controllers
{
    [ODataEntitySet("Expenses", EntityType = typeof(Expense))]
    public class ExpensesController : ODataController
    {
        //readonly ExpenseService _service;
        //public ExpensesController(ExpenseService service)  
        //{
        //    _service = service;
        //}

        //[EnableQuery]
               
        //public async Task<PageResult<Expense>> Get(ODataQueryOptions<Expense> queryOptions, [FromODataUri] string key = null)
        //{
        //    ExpenseService _service = new ExpenseService();
        //    var lista = await _service.GetList(queryOptions.ToString());

        //    foreach (var item in lista)
        //    {
        //        item.Code = item.Code ?? Guid.NewGuid().ToString();
        //    }

        //    List<Expense> l = new List<Expense>();

        //    if (key != null)
        //    {
        //        foreach (var item in lista)
        //        {
        //            if (item.Code == key)
        //            {
        //                l.Add(item);
        //            }
        //        }
        //        lista = l;
        //    }

        //    var enumerable = lista as IEnumerable<Expense>;
        //    long count = lista.Count();

        //    var result = new PageResult<Expense>(enumerable, null, count > 0 ? (long?)count : null);

        //    return result;
        //}

        public async Task<IQueryable<Expense>> Get(ODataQueryOptions<Expense> queryOptions, [FromODataUri] string key = null)
        {
            //List<Expense> lista = new List<Expense>();

            //lista.Add ( new Expense
            //{
            //    Code = Guid.NewGuid().ToString(),
            //    Value = 1000,
            //    Year = 2023,
            //    Month = 5,
            //    Description = "Testao",
            //    Type = "C"
            //});
            ExpenseService _service = new ExpenseService();
            var lista = await _service.GetList(queryOptions.ToString());

            foreach (var item in lista)
            {
                item.Code = item.Code ?? Guid.NewGuid().ToString();
            }

            List<Expense> l = new List<Expense>();

            if (key != null)
            {
                foreach (var item in lista)
                {
                    if (item.Code == key)
                    {
                        l.Add(item);
                    }
                }
                lista = l;
            }

            var result = lista.AsQueryable<Expense>();
            var count = lista.Count();
            if (count > 0)
            {
                Request.ODataProperties().TotalCount = count;
            }

            if (queryOptions.SelectExpand?.SelectExpandClause != null)
            {
                Request.ODataProperties().SelectExpandClause = queryOptions.SelectExpand.SelectExpandClause;
            }

            return result;
        }

        [HttpGet]
        [ActionName("$count")]
        public IHttpActionResult GetCount([FromUri(Name = "$filter")] string filter = "")
        {
            HttpResponseMessage response = new HttpResponseMessage();

            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent("699", System.Text.Encoding.UTF8, "text/plain");
            return ResponseMessage(response);
        }

        public async Task<IHttpActionResult> Post([FromBody] Expense entity)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            IHttpActionResult result = BadRequest();

            ExpenseService _service = new ExpenseService();

            try
            {
                var created = await _service.Insert(entity);
                result = Created<Expense>(created);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Content = new StringContent(ex.Message, Encoding.UTF8);
                result = ResponseMessage(response);
            }

            return result;
        }

        [AcceptVerbs("MERGE","PATCH")]
        public async Task<IHttpActionResult> Merge([FromODataUri] string key, Delta<Expense> delta)
        {
            ExpenseService _service = new ExpenseService();

            HttpResponseMessage response = new HttpResponseMessage();
            IHttpActionResult result = BadRequest();

            try
            {
                var entity = await _service.Update(key, delta);
                result = Ok(entity);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Content = new StringContent(ex.Message, Encoding.UTF8);
                result = ResponseMessage(response);
            }

            return result;
        }

        public async Task<IHttpActionResult> Delete([FromODataUri] string key)
        {
            ExpenseService _service = new ExpenseService();

            HttpResponseMessage response = new HttpResponseMessage();
            IHttpActionResult result = BadRequest();

            try
            {
                await _service.Delete(key);
                response.StatusCode = HttpStatusCode.NoContent;
                response.ReasonPhrase = "Registro excluído com sucesso";
                result = ResponseMessage(response);
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                response.Content = new StringContent(ex.Message, Encoding.UTF8);
                result = ResponseMessage(response);
            }

            return result;
        }


    }
}
