using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Franquias.Api.Data;
using Franquias.Api.Models;

namespace Franquias.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChamadosSuporteController : ControllerBase
{
    private readonly AppDbContext _context;

    public ChamadosSuporteController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/chamadossuporte
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChamadoSuporte>>> GetChamados()
    {
        return await _context.ChamadosSuporte
            .Include(c => c.UnidadeFranqueada)
            .ToListAsync();
    }

    // GET: api/chamadossuporte/abertos?prioridade=Alta&unidadeId=1
    [HttpGet("abertos")]
    public async Task<ActionResult<IEnumerable<ChamadoSuporte>>> GetAbertos(PrioridadeChamado? prioridade, int? unidadeId)
    {
        var query = _context.ChamadosSuporte
            .Where(c => c.Status != StatusChamado.Encerrado);

        if (prioridade.HasValue)
        {
            query = query.Where(c => c.Prioridade == prioridade.Value);
        }

        if (unidadeId.HasValue)
        {
            query = query.Where(c => c.UnidadeFranqueadaId == unidadeId.Value);
        }

        return await query.ToListAsync();
    }

    // POST: api/chamadossuporte
    [HttpPost]
    public async Task<ActionResult<ChamadoSuporte>> PostChamado(ChamadoSuporte chamado)
    {
        chamado.DataAbertura = DateTime.Now;
        chamado.Status = StatusChamado.Aberto;

        _context.ChamadosSuporte.Add(chamado);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetChamados), new { }, chamado);
    }

    // PUT: api/chamadossuporte/5/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> AtualizarStatus(int id, StatusChamado novoStatus)
    {
        var chamado = await _context.ChamadosSuporte.FindAsync(id);

        if (chamado == null)
        {
            return NotFound();
        }

        chamado.Status = novoStatus;

        if (novoStatus == StatusChamado.Encerrado)
        {
            chamado.DataEncerramento = DateTime.Now;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }
}