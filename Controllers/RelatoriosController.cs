using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Franquias.Api.Data;
using Franquias.Api.Models;

namespace Franquias.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RelatoriosController : ControllerBase
{
    private readonly AppDbContext _context;

    public RelatoriosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/relatorios/faturamento-por-unidade
    [HttpGet("faturamento-por-unidade")]
    public async Task<ActionResult<IEnumerable<object>>> FaturamentoPorUnidade()
    {
        var resultado = await _context.Vendas
            .Include(v => v.UnidadeFranqueada)
            .GroupBy(v => v.UnidadeFranqueada!.NomeUnidade)
            .Select(g => new
            {
                Unidade = g.Key,
                Faturamento = g.Sum(v => v.ValorTotal)
            })
            .OrderByDescending(x => x.Faturamento)
            .ToListAsync();

        return resultado;
    }

    // GET: api/relatorios/produtos-mais-vendidos
    [HttpGet("produtos-mais-vendidos")]
    public async Task<ActionResult<IEnumerable<object>>> ProdutosMaisVendidos()
    {
        var resultado = await _context.ItensVenda
            .Include(i => i.ProdutoServico)
            .GroupBy(i => i.ProdutoServico!.Nome)
            .Select(g => new
            {
                Produto = g.Key,
                QuantidadeVendida = g.Sum(i => i.Quantidade)
            })
            .OrderByDescending(x => x.QuantidadeVendida)
            .ToListAsync();

        return resultado;
    }

    // GET: api/relatorios/chamados-por-status
    [HttpGet("chamados-por-status")]
    public async Task<ActionResult<IEnumerable<object>>> ChamadosPorStatus()
    {
        var resultado = await _context.ChamadosSuporte
            .GroupBy(c => c.Status)
            .Select(g => new
            {
                Status = g.Key.ToString(),
                Quantidade = g.Count()
            })
            .ToListAsync();

        return resultado;
    }

    // GET: api/relatorios/estoque-critico?minimo=10
    [HttpGet("estoque-critico")]
    public async Task<ActionResult<IEnumerable<object>>> EstoqueCritico(int minimo = 10)
    {
        var movimentacoes = await _context.MovimentacoesEstoque
            .Include(m => m.ProdutoServico)
            .Include(m => m.UnidadeFranqueada)
            .ToListAsync();

        var saldos = movimentacoes
            .GroupBy(m => new { m.ProdutoServicoId, Produto = m.ProdutoServico!.Nome, m.UnidadeFranqueadaId, Unidade = m.UnidadeFranqueada!.NomeUnidade })
            .Select(g => new
            {
                g.Key.Produto,
                g.Key.Unidade,
                Saldo = g.Sum(m => m.Tipo == TipoMovimentacao.Entrada ? m.Quantidade : -m.Quantidade)
            })
            .Where(x => x.Saldo < minimo)
            .ToList();

        return saldos;
    }
}