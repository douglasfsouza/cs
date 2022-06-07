using Chamados2.Context;
using Chamados2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Chamados2.Services
{
    public class IssueService
    {  
        private readonly Chamados2DbContext _ctx;
        private readonly Issue _issue;
        const int itensPorPagina = 5;
        public IssueService(Chamados2DbContext ctx, Issue issue)
        {
            _ctx = ctx;
            _issue = issue;
        }     

        
        public async Task<List<IssueClient>> GetIssues(int pagina = 1)
        {            
            int pag = (pagina - 1) * itensPorPagina;
            var issues = await (from i in _ctx.Issues
                                join c in _ctx.Customers on i.IdCliente equals c.Id
                                select new
                                {
                                    Id = i.Id,
                                    IdCliente = i.IdCliente,
                                    Cliente = c.NomeFantasia,
                                    Status = i.Status,
                                    Asssunto = i.Assunto,
                                    Abertura = i.Abertura,
                                    Complemento = i.Complemento,
                                    UserId = i.UserId
                                }).Skip(pag).Take(itensPorPagina).ToListAsync();

            List<IssueClient> result = new List<IssueClient>();
            issues.ForEach(e =>
            result.Add(new IssueClient
            {
                Id = e.Id,
                IdCliente = e.IdCliente,
                Cliente = e.Cliente,
                Status = e.Status,
                Abertura = e.Abertura,
                Assunto = e.Asssunto,
                Complemento = e.Complemento,
                UserId = e.UserId
            }
            ));         




           // List < Issue > issues = _ctx.Issues.Select(x => new Issue { Id = x.Id, IdCliente = x.IdCliente, Abertura = x.Abertura, Assunto = x.Assunto, Status = x.Status, Complemento = x.Complemento, UserId = x.UserId }).ToList();
            return result;
        }

        public async Task<(IssueClient rsIssue, string rsError)> GetIssue(int Id)
        {
            var issue = await _ctx.Issues.FirstOrDefaultAsync(x => x.Id == Id);
            if (issue == null)
            {
                return (null,"Chamado não encontrado");
            }
            var client = await _ctx.Customers.FirstOrDefaultAsync(x => x.Id == issue.IdCliente);
            IssueClient result = new IssueClient
            {
                Id = issue.Id,
                IdCliente = issue.IdCliente,
                UserId = issue.UserId,
                Abertura = issue.Abertura,
                Assunto = issue.Assunto,
                Cliente = client.NomeFantasia,
                Complemento = issue.Complemento,
                Status = issue.Status
            };

            return (result, null);
        }

        public async Task<(Issue issue, string error)> Update(Issue pIssue)
        {
            var issue = await _ctx.Issues.FirstOrDefaultAsync(x => x.Id == pIssue.Id);
            if (issue == null)
            {
                return (pIssue, "Chamado não cadastrado");
            }
            else
            {
                try
                {
                    issue.Assunto = pIssue.Assunto;
                    issue.Id = pIssue.Id;
                    issue.IdCliente = pIssue.IdCliente;
                    issue.Status = pIssue.Status;
                    issue.Complemento = pIssue.Complemento;
                    issue.UserId = pIssue.UserId;

                    _ctx.Issues.Update(issue);
                    _ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    return (pIssue, ex.Message); 
                }
                
                return (pIssue, null);
            }
        }

        public async Task<(Issue issue, string error)> Insert(Issue pIssue)
        {
            Issue issue = null;

            try
            {
                issue = new Issue
                {
                    Status = pIssue.Status,
                    Abertura = pIssue.Abertura,
                    IdCliente = pIssue.IdCliente,
                    Assunto = pIssue.Assunto,
                    Complemento = pIssue.Complemento,
                    UserId = pIssue.UserId
            };               

                _ctx.Issues.Add(issue);
                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                return (pIssue, ex.Message);
            }

            return (issue, null);

        }

    }
}
