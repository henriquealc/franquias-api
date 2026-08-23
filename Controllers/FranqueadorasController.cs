using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Franquias.Api.Data;
using Franquias.Api.Models;

namespace Franquias.Api.Controllers;

[Authorize(Roles = "Administrador")]
[ApiController]
[Route("api/[controller]")]
public class FranqueadorasController : ControllerBase
{
    private readonly AppDbContext _context;

    public FranqueadorasController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/franqueadoras
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Franqueadora>>> GetFranqueadoras()
    {
        return await _context.Franqueadoras.ToListAsync();
    }

    // GET: api/franqueadoras/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Franqueadora>> GetFranqueadora(int id)
    {
        var franqueadora = await _context.Franqueadoras.FindAsync(id);

        if (franqueadora == null)
        {
            return NotFound();
        }

        return franqueadora;
    }

    // POST: api/franqueadoras
    [HttpPost]
    public async Task<ActionResult<Franqueadora>> PostFranqueadora(Franqueadora franqueadora)
    {
        _context.Franqueadoras.Add(franqueadora);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFranqueadora), new { id = franqueadora.Id }, franqueadora);
    }

    // PUT: api/franqueadoras/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutFranqueadora(int id, Franqueadora franqueadora)
    {
        if (id != franqueadora.Id)
        {
            return BadRequest();
        }

        _context.Entry(franqueadora).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}