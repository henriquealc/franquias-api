using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Franquias.Api.Data;
using Franquias.Api.Models;

namespace Franquias.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UnidadesFranqueadasController : ControllerBase
{
    private readonly AppDbContext _context;

    public UnidadesFranqueadasController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/unidadesfranqueadas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnidadeFranqueada>>> GetUnidades()
    {
        return await _context.UnidadesFranqueadas
            .Include(u => u.Franqueadora)
            .ToListAsync();
    }

    // GET: api/unidadesfranqueadas/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UnidadeFranqueada>> GetUnidade(int id)
    {
        var unidade = await _context.UnidadesFranqueadas
            .Include(u => u.Franqueadora)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (unidade == null)
        {
            return NotFound();
        }

        return unidade;
    }

    // POST: api/unidadesfranqueadas
    [HttpPost]
    public async Task<ActionResult<UnidadeFranqueada>> PostUnidade(UnidadeFranqueada unidade)
    {
        var franqueadoraExiste = await _context.Franqueadoras.AnyAsync(f => f.Id == unidade.FranqueadoraId);

        if (!franqueadoraExiste)
        {
            return BadRequest("A franqueadora informada não existe.");
        }

        var cnpjDuplicado = await _context.UnidadesFranqueadas.AnyAsync(u => u.Cnpj == unidade.Cnpj);

        if (cnpjDuplicado)
        {
            return BadRequest("Já existe uma unidade cadastrada com esse CNPJ.");
        }

        _context.UnidadesFranqueadas.Add(unidade);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUnidade), new { id = unidade.Id }, unidade);
    }
}