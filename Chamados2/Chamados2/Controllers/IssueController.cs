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
    public class IssueController : ControllerBase
    {
        private readonly Chamados2DbContext _ctx;
        private readonly Issue _issue;

        public IssueController(Chamados2DbContext ctx, Issue issue)
        {
            _ctx = ctx;
            _issue = issue;
        }
        
        [HttpGet]
        [Route("getIssues")]
        public async Task<List<IssueClient>> GetIssues(int Pagina = 1)
        {
            IssueService service = new IssueService(_ctx,_issue);
            List<IssueClient> issues = await service.GetIssues(Pagina);
            return issues;
        }       

        [HttpGet]
        [Route("getIssue")]
        public async Task<IActionResult> GetIssue(int Id)
        {
            string error = null;
            IssueService service = new IssueService(_ctx, _issue);
            IssueClient issue = null;
            (issue, error) = await service.GetIssue(Id);
            if (error == null)
            {
                return Ok(issue);
            }
            else
            { 
                return BadRequest(error);
            }           
        }
        
        [HttpPatch]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] Issue issue)
        {
            Issue resultIssue = null;
            string error = null;
            IssueService service = new IssueService(_ctx, _issue);
            try
            {
                (resultIssue, error) = await service.Update(issue);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            if (error == null)
            {
                return Ok(resultIssue);
            }
            else
            {
                return BadRequest(error);
            }
        }

        [HttpPost]
        [Route("insert")]
        public async Task<IActionResult> Insert([FromBody] Issue issue)
        {
            Issue resultIssue = null;
            string error = null;
            IssueService service = new IssueService(_ctx, _issue);
            try
            {
                (resultIssue, error) = await service.Insert(issue);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            if (error == null)
            {
                return Ok(resultIssue);
            }
            else
            {
                return BadRequest(error);
            }
        }
    }
}
