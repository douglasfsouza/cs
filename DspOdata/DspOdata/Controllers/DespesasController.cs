using Dsp.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Varsis.OData.Attributes;

namespace DspOdata.Controllers
{
    [Route("api/[controller]")]
    [ODataEntitySet("Despesas", EntityType = typeof(Despesas))]
    public class DespesasController : ODataController
    {
        private readonly DspContext _context;

        public DespesasController(DspContext context)
        {
            _context = context;
        }

        // GET: api/Despesas
        [HttpGet]
        [EnableQuery]
        public async Task<PageResult<Despesas>> GetDespesas(ODataQueryOptions<Despesas> queryOptions)
        {
            // return await _context.Despesas.ToListAsync();

            //string queryString = queryOptions.Request.RequestUri.Query;

            //ListWithCountArgs<Despesas> args = new ListWithCountArgs<Despesas>()
            //{
            //    Key = key,
            //    queryOptions = queryOptions
            //};

            var lista = await _context.Despesas.ToListAsync();

            var enumerable = lista as IEnumerable<Despesas>;

            var result = new PageResult<Despesas>(enumerable, null, null);
           

            return result;
        }

        // GET: api/Despesas/5
        [HttpGet("{id}")]

        [EnableQuery]
        // public async Task<PageResult<Despesas>> GetDespesas(int id)
        async public Task<PageResult<Despesas>> Get(ODataQueryOptions<Despesas> queryOptions, [FromODataUri] string key = null)

        //public async Task<ActionResult<Despesas>> GetDespesas(int id)
        {
            var lista = await _context.Despesas.ToListAsync();

            var enumerable = lista as IEnumerable<Despesas>;

            var result = new PageResult<Despesas>(enumerable, null, null);
            return result;

            //var despesas = await _context.Despesas.FindAsync(id);

            //if (despesas == null)
            //{
            //    return NotFound();
            //}

            //return despesas;
        }

        // PUT: api/Despesas/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDespesas(int id, Despesas despesas)
        {
            if (id != despesas.Id)
            {
                return BadRequest();
            }

            _context.Entry(despesas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DespesasExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Despesas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Despesas>> PostDespesas(Despesas despesas)
        {
            _context.Despesas.Add(despesas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDespesas", new { id = despesas.Id }, despesas);
        }

        [HttpPost("GetList")]
        public List<Despesas> GetList(List<Criteria> criterias)
        {
            return _context.ListarDespesas(_context, criterias);
        }

        [HttpPost("GetListSummary")]
        public List<Despesas> GetListSummary(List<Criteria> criterias)
        {
            return _context.ListSummary(_context, criterias);
        }

        [HttpPost("GetListSearch")]
        public List<Despesas> GetListSearch(List<Criteria> criterias)
        {
            return _context.ListSearch(_context, criterias);
        }


        // DELETE: api/Despesas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Despesas>> DeleteDespesas(int id)
        {
            var despesas = await _context.Despesas.FindAsync(id);
            if (despesas == null)
            {
                return NotFound();
            }

            _context.Despesas.Remove(despesas);
            await _context.SaveChangesAsync();

            return despesas;
        }

        private bool DespesasExists(int id)
        {
            return _context.Despesas.Any(e => e.Id == id);
        }
    }
}
