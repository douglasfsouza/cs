using Chamados2.Context;
using Chamados2.Models;
using Chamados2.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Chamados2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly Chamados2DbContext _ctx;
        private readonly Customer _customer;

        public CustomerController(Chamados2DbContext ctx, Customer customer)
        {
            _ctx = ctx;
            _customer = customer;
        }
        
        [HttpGet]
        [Route("getCustomers")]
        public async Task<List<Customer>> GetCustomers()
        {
            CustomerService service = new CustomerService(_ctx,_customer);
            List<Customer> customers = await service.GetCustomers();
            return customers;
        }

        

        
        [HttpPatch]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] Customer customer)
        {
            Customer resultCustomer = null;
            string error = null;
            CustomerService service = new CustomerService(_ctx, _customer);
            try
            {
                (resultCustomer, error) = await service.Update(customer);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            if (error == null)
            {
                return Ok(resultCustomer);
            }
            else
            {
                return BadRequest(error);
            }
        }

        [HttpPost]
        [Route("insert")]
        public async Task<IActionResult> Insert([FromBody] Customer customer)
        {
            Customer resultCustomer = null;
            string error = null;
            CustomerService service = new CustomerService(_ctx, _customer);
            try
            {
                (resultCustomer, error) = await service.Insert(customer);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            if (error == null)
            {
                return Ok(resultCustomer);
            }
            else
            {
                return BadRequest(error);
            }
        }
    }
}
