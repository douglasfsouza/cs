using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dgOData.Models;
using Microsoft.AspNet.OData;

namespace dgOData.Controllers
{
    [EnableQuery]
    [Route("api/[controller]")]
    [ApiController]
    public class EscolasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EscolasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Escolas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Escola>>> GetEscolas()
        {
            return await _context.Escolas.ToListAsync();
        }

        // GET: api/Escolas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Escola>> GetEscola(int id)
        {
            var escola = await _context.Escolas.FindAsync(id);

            if (escola == null)
            {
                return NotFound();
            }

            return escola;
        }

        // PUT: api/Escolas/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEscola(int id, Escola escola)
        {
            if (id != escola.EscolaId)
            {
                return BadRequest();
            }

            _context.Entry(escola).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EscolaExists(id))
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

        // POST: api/Escolas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Escola>> PostEscola(Escola escola)
        {
            _context.Escolas.Add(escola);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEscola", new { id = escola.EscolaId }, escola);
        }

        // DELETE: api/Escolas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Escola>> DeleteEscola(int id)
        {
            var escola = await _context.Escolas.FindAsync(id);
            if (escola == null)
            {
                return NotFound();
            }

            _context.Escolas.Remove(escola);
            await _context.SaveChangesAsync();

            return escola;
        }

        private bool EscolaExists(int id)
        {
            return _context.Escolas.Any(e => e.EscolaId == id);
        }
    }
}
