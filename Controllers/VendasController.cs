using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Franquias.Api.Data;
using Franquias.Api.Models;
using Franquias.Api.DTOs;

namespace Franquias.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VendasController : ControllerBase
{
    private readonly AppDbContext _context;

    public VendasController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/vendas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Venda>>> GetVendas()
    {
        return await _context.Vendas
            .Include(v => v.UnidadeFranqueada)
            .Include(v => v.Itens)
                .ThenInclude(i => i.ProdutoServico)
            .ToListAsync();
    }

    // GET: api/vendas/unidade/1?inicio=2026-01-01&fim=2026-12-31
    [HttpGet("unidade/{unidadeId}")]
    public async Task<ActionResult<IEnumerable<Venda>>> GetVendasPorUnidade(int unidadeId, DateTime? inicio, DateTime? fim)
    {
        var query = _context.Vendas
            .Include(v => v.Itens)
            .Where(v => v.UnidadeFranqueadaId == unidadeId);

        if (inicio.HasValue)
        {
            query = query.Where(v => v.Data >= inicio.Value);
        }

        if (fim.HasValue)
        {
            query = query.Where(v => v.Data <= fim.Value);
        }

        return await query.ToListAsync();
    }

    // POST: api/vendas
    [HttpPost]
    public async Task<ActionResult<Venda>> PostVenda(VendaDto vendaDto)
    {
        if (vendaDto.Itens == null || vendaDto.Itens.Count == 0)
        {
            return BadRequest("A venda deve conter pelo menos um item.");
        }

        var unidade = await _context.UnidadesFranqueadas.FindAsync(vendaDto.UnidadeFranqueadaId);

        if (unidade == null)
        {
            return BadRequest("Unidade não encontrada.");
        }

        if (!unidade.Ativa)
        {
            return BadRequest("Unidade inativa não pode registrar vendas.");
        }

        var venda = new Venda
        {
            UnidadeFranqueadaId = vendaDto.UnidadeFranqueadaId,
            Data = DateTime.Now
        };

        decimal totalVenda = 0;

        foreach (var itemDto in vendaDto.Itens)
        {
            var produto = await _context.ProdutosServicos.FindAsync(itemDto.ProdutoServicoId);

            if (produto == null)
            {
                return BadRequest($"Produto {itemDto.ProdutoServicoId} não encontrado.");
            }

            var entradas = await _context.MovimentacoesEstoque
                .Where(m => m.ProdutoServicoId == itemDto.ProdutoServicoId
                         && m.UnidadeFranqueadaId == vendaDto.UnidadeFranqueadaId
                         && m.Tipo == TipoMovimentacao.Entrada)
                .SumAsync(m => m.Quantidade);

            var saidas = await _context.MovimentacoesEstoque
                .Where(m => m.ProdutoServicoId == itemDto.ProdutoServicoId
                         && m.UnidadeFranqueadaId == vendaDto.UnidadeFranqueadaId
                         && m.Tipo == TipoMovimentacao.Saida)
                .SumAsync(m => m.Quantidade);

            var saldoAtual = entradas - saidas;

            if (saldoAtual - itemDto.Quantidade < 0)
            {
                return BadRequest($"Estoque insuficiente para o produto '{produto.Nome}'. Saldo atual: {saldoAtual}, solicitado: {itemDto.Quantidade}.");
            }

            var itemVenda = new ItemVenda
            {
                ProdutoServicoId = itemDto.ProdutoServicoId,
                Quantidade = itemDto.Quantidade,
                PrecoUnitario = produto.PrecoBase
            };

            venda.Itens.Add(itemVenda);
            totalVenda += itemDto.Quantidade * produto.PrecoBase;

            _context.MovimentacoesEstoque.Add(new MovimentacaoEstoque
            {
                Tipo = TipoMovimentacao.Saida,
                Quantidade = itemDto.Quantidade,
                Data = DateTime.Now,
                Observacao = $"Baixa automática - Venda",
                ProdutoServicoId = itemDto.ProdutoServicoId,
                UnidadeFranqueadaId = vendaDto.UnidadeFranqueadaId
            });
        }

        venda.ValorTotal = totalVenda;

        _context.Vendas.Add(venda);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVendas), new { }, venda);
    }
}