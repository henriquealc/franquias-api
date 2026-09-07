using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Franquias.Api.Data;
using Franquias.Api.Models;

namespace Franquias.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProdutosServicosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosServicosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/produtosservicos?pagina=1&tamanhoPagina=10&orderBy=nome
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoServico>>> GetProdutos(
        int pagina = 1,
        int tamanhoPagina = 10,
        string? orderBy = null)
    {
        var query = _context.ProdutosServicos.AsQueryable();

        query = orderBy switch
        {
            "nome" => query.OrderBy(p => p.Nome),
            "preco" => query.OrderBy(p => p.PrecoBase),
            _ => query.OrderBy(p => p.Id)
        };

        return await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();
    }

    // GET: api/produtosservicos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProdutoServico>> GetProduto(int id)
    {
        var produto = await _context.ProdutosServicos.FindAsync(id);

        if (produto == null)
        {
            return NotFound();
        }

        return produto;
    }

    // GET: api/produtosservicos/categoria/Bebidas
    [HttpGet("categoria/{categoria}")]
    public async Task<ActionResult<IEnumerable<ProdutoServico>>> GetPorCategoria(string categoria)
    {
        return await _context.ProdutosServicos
            .Where(p => p.Categoria == categoria)
            .ToListAsync();
    }

    // GET: api/produtosservicos/buscar?nome=xxx&categoria=xxx&ativo=true
    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<ProdutoServico>>> Buscar(
        string? nome,
        string? categoria,
        bool? ativo)
    {
        var query = _context.ProdutosServicos.AsQueryable();

        if (!string.IsNullOrEmpty(nome))
        {
            query = query.Where(p => p.Nome.Contains(nome));
        }

        if (!string.IsNullOrEmpty(categoria))
        {
            query = query.Where(p => p.Categoria == categoria);
        }

        if (ativo.HasValue)
        {
            query = query.Where(p => p.Ativo == ativo.Value);
        }

        return await query.ToListAsync();
    }

    // POST: api/produtosservicos
    [HttpPost]
    public async Task<ActionResult<ProdutoServico>> PostProduto(ProdutoServico produto)
    {
        _context.ProdutosServicos.Add(produto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto);
    }

    // PUT: api/produtosservicos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduto(int id, ProdutoServico produto)
    {
        if (id != produto.Id)
        {
            return BadRequest();
        }

        _context.Entry(produto).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var produto = await _context.ProdutosServicos.FindAsync(id);

        if (produto == null)
            return NotFound("Produto ou serviço não encontrado.");

        var temMovimentacao = await _context.MovimentacoesEstoque
            .AnyAsync(m => m.ProdutoServicoId == id);

        var temItemVenda = await _context.ItensVenda
            .AnyAsync(i => i.ProdutoServicoId == id);

        if (temMovimentacao || temItemVenda)
        {
            return BadRequest("Não é possível excluir este produto: já existem movimentações de estoque ou vendas vinculadas a ele. Considere inativá-lo em vez de excluí-lo.");
        }

        _context.ProdutosServicos.Remove(produto);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}