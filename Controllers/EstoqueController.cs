using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Franquias.Api.Data;
using Franquias.Api.Models;

namespace Franquias.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EstoqueController : ControllerBase
{
    private readonly AppDbContext _context;

    public EstoqueController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/estoque/saldo/{produtoId}/{unidadeId}
    [HttpGet("saldo/{produtoId}/{unidadeId}")]
    public async Task<ActionResult<int>> GetSaldo(int produtoId, int unidadeId)
    {
        var saldo = await CalcularSaldo(produtoId, unidadeId);
        return saldo;
    }

    // GET: api/estoque/movimentacoes/{unidadeId}
    [HttpGet("movimentacoes/{unidadeId}")]
    public async Task<ActionResult<IEnumerable<MovimentacaoEstoque>>> GetMovimentacoes(int unidadeId)
    {
        return await _context.MovimentacoesEstoque
            .Include(m => m.ProdutoServico)
            .Where(m => m.UnidadeFranqueadaId == unidadeId)
            .OrderByDescending(m => m.Data)
            .ToListAsync();
    }

    // POST: api/estoque/movimentar
    [HttpPost("movimentar")]
    public async Task<ActionResult<MovimentacaoEstoque>> Movimentar(MovimentacaoEstoque movimentacao)
    {
        if (movimentacao.Tipo == TipoMovimentacao.Saida)
        {
            var saldoAtual = await CalcularSaldo(movimentacao.ProdutoServicoId, movimentacao.UnidadeFranqueadaId);

            if (saldoAtual - movimentacao.Quantidade < 0)
            {
                return BadRequest($"Estoque insuficiente. Saldo atual: {saldoAtual}, quantidade solicitada: {movimentacao.Quantidade}.");
            }
        }

        movimentacao.Data = DateTime.Now;
        _context.MovimentacoesEstoque.Add(movimentacao);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMovimentacoes), new { unidadeId = movimentacao.UnidadeFranqueadaId }, movimentacao);
    }

    private async Task<int> CalcularSaldo(int produtoId, int unidadeId)
    {
        var entradas = await _context.MovimentacoesEstoque
            .Where(m => m.ProdutoServicoId == produtoId
                     && m.UnidadeFranqueadaId == unidadeId
                     && m.Tipo == TipoMovimentacao.Entrada)
            .SumAsync(m => m.Quantidade);

        var saidas = await _context.MovimentacoesEstoque
            .Where(m => m.ProdutoServicoId == produtoId
                     && m.UnidadeFranqueadaId == unidadeId
                     && m.Tipo == TipoMovimentacao.Saida)
            .SumAsync(m => m.Quantidade);

        return entradas - saidas;
    }
}