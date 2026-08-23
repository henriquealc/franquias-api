using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Franquias.Api.Data;
using Franquias.Api.Models;

namespace Franquias.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RoyaltiesController : ControllerBase
{
    private readonly AppDbContext _context;

    public RoyaltiesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/royalties
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Royalty>>> GetRoyalties()
    {
        return await _context.Royalties
            .Include(r => r.UnidadeFranqueada)
            .ToListAsync();
    }

    // GET: api/royalties/unidade/1
    [HttpGet("unidade/{unidadeId}")]
    public async Task<ActionResult<IEnumerable<Royalty>>> GetPorUnidade(int unidadeId)
    {
        return await _context.Royalties
            .Where(r => r.UnidadeFranqueadaId == unidadeId)
            .ToListAsync();
    }

    // POST: api/royalties/calcular
    [HttpPost("calcular")]
    public async Task<ActionResult<Royalty>> Calcular(int unidadeFranqueadaId, DateTime periodoInicio, DateTime periodoFim)
    {
        var unidade = await _context.UnidadesFranqueadas
            .Include(u => u.Franqueadora)
            .FirstOrDefaultAsync(u => u.Id == unidadeFranqueadaId);

        if (unidade == null)
        {
            return BadRequest("Unidade não encontrada.");
        }

        if (unidade.Franqueadora == null)
        {
            return BadRequest("Unidade sem franqueadora vinculada.");
        }

        var faturamento = await _context.Vendas
            .Where(v => v.UnidadeFranqueadaId == unidadeFranqueadaId
                     && v.Data >= periodoInicio
                     && v.Data <= periodoFim)
            .SumAsync(v => v.ValorTotal);

        var percentual = unidade.Franqueadora.PercentualRoyalty;
        var valorRoyalty = faturamento * (percentual / 100);

        var royalty = new Royalty
        {
            UnidadeFranqueadaId = unidadeFranqueadaId,
            PeriodoInicio = periodoInicio,
            PeriodoFim = periodoFim,
            Faturamento = faturamento,
            PercentualAplicado = percentual,
            ValorRoyalty = valorRoyalty,
            Status = StatusPagamento.Pendente
        };

        _context.Royalties.Add(royalty);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRoyalties), new { }, royalty);
    }

    // PUT: api/royalties/5/pagar
    [HttpPut("{id}/pagar")]
    public async Task<IActionResult> MarcarComoPago(int id)
    {
        var royalty = await _context.Royalties.FindAsync(id);

        if (royalty == null)
        {
            return NotFound();
        }

        royalty.Status = StatusPagamento.Pago;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}