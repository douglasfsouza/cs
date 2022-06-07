using Chamados2.Context;
using Chamados2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Chamados2.Services
{
    public class CustomerService
    {  
        private readonly Chamados2DbContext _ctx;
        private readonly Customer _customer;
        public CustomerService(Chamados2DbContext ctx, Customer customer)
        {
            _ctx = ctx;
            _customer = customer;
        } 
      

        
        public async Task<List<Customer>> GetCustomers()
        {
            List<Customer> customers = _ctx.Customers.Select(x => new Customer{Id = x.Id, CNPJ = x.CNPJ, Endereco = x.Endereco, NomeFantasia = x.NomeFantasia }).ToList();
            return customers;
        }

        public async Task<Customer> GetCustomer(string CNPJ)
        {
            var customer = await _ctx.Customers.FirstOrDefaultAsync(x => x.CNPJ == CNPJ);
            return customer;
        }

        public async Task<(Customer customer, string error)> Update(Customer pCustomer)
        {
            var customer = await _ctx.Customers.FirstOrDefaultAsync(x => x.CNPJ == pCustomer.CNPJ);
            if (customer == null)
            {
                return (pCustomer, "Cliente não cadastrado");
            }
            else
            {
                try
                {
                    customer.NomeFantasia = pCustomer.NomeFantasia;
                    customer.CNPJ = pCustomer.CNPJ;
                    customer.Endereco = pCustomer.Endereco;               
                  
                    _ctx.Customers.Update(customer);
                    _ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    return (pCustomer, ex.Message); 
                }
                
                return (customer, null);
            }
        }

        public async Task<(Customer customer, string error)> Insert(Customer pCustomer)
        {
            var customer = await _ctx.Customers.FirstOrDefaultAsync(x => x.CNPJ == pCustomer.CNPJ);
            if (customer != null)
            {
                return (pCustomer, "Cliente já cadastrado");
            }

            try
            {
                customer = new Customer
                {
                    NomeFantasia = pCustomer.NomeFantasia,
                    CNPJ = pCustomer.CNPJ,
                    Endereco = pCustomer.Endereco
                };               

                _ctx.Customers.Add(customer);
                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                return (pCustomer, ex.Message);
            }

            return (customer, null);

        }

    }
}
