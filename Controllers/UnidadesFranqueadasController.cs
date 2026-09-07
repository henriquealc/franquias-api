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

    // GET: api/unidadesfranqueadas?pagina=1&tamanhoPagina=10&orderBy=nome
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnidadeFranqueada>>> GetUnidades(
        int pagina = 1,
        int tamanhoPagina = 10,
        string? orderBy = null)
    {
        var query = _context.UnidadesFranqueadas
            .Include(u => u.Franqueadora)
            .AsQueryable();

        query = orderBy switch
        {
            "nome" => query.OrderBy(u => u.NomeUnidade),
            "cidade" => query.OrderBy(u => u.Cidade),
            _ => query.OrderBy(u => u.Id)
        };

        return await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
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

    // GET: api/unidadesfranqueadas/buscar?nome=xxx&cidade=xxx&cnpj=xxx&responsavel=xxx
    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<UnidadeFranqueada>>> Buscar(
        string? nome,
        string? cidade,
        string? cnpj,
        string? responsavel)
    {
        var query = _context.UnidadesFranqueadas
            .Include(u => u.Franqueadora)
            .AsQueryable();

        if (!string.IsNullOrEmpty(nome))
        {
            query = query.Where(u => u.NomeUnidade.Contains(nome));
        }

        if (!string.IsNullOrEmpty(cidade))
        {
            query = query.Where(u => u.Cidade.Contains(cidade));
        }

        if (!string.IsNullOrEmpty(cnpj))
        {
            query = query.Where(u => u.Cnpj == cnpj);
        }

        if (!string.IsNullOrEmpty(responsavel))
        {
            query = query.Where(u => u.NomeResponsavel.Contains(responsavel));
        }

        return await query.ToListAsync();
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

    [Authorize(Roles = "Administrador,Gestor")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UnidadeFranqueadaUpdateDto dto)
    {
        var unidade = await _context.UnidadesFranqueadas.FindAsync(id);

        if (unidade == null)
            return NotFound("Unidade franqueada não encontrada.");

        unidade.NomeUnidade = dto.NomeUnidade;
        unidade.Endereco = dto.Endereco;
        unidade.NomeResponsavel = dto.NomeResponsavel;
        unidade.ContatoResponsavel = dto.ContatoResponsavel;
        unidade.Ativa = dto.Ativa;

        await _context.SaveChangesAsync();

        return Ok(unidade);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var unidade = await _context.UnidadesFranqueadas.FindAsync(id);

        if (unidade == null)
            return NotFound("Unidade franqueada não encontrada.");

        var temVenda = await _context.Vendas.AnyAsync(v => v.UnidadeFranqueadaId == id);
        var temRoyalty = await _context.Royalties.AnyAsync(r => r.UnidadeFranqueadaId == id);
        var temChamado = await _context.ChamadosSuporte.AnyAsync(c => c.UnidadeFranqueadaId == id);
        var temMovimentacao = await _context.MovimentacoesEstoque.AnyAsync(m => m.UnidadeFranqueadaId == id);

        if (temVenda || temRoyalty || temChamado || temMovimentacao)
        {
            return BadRequest("Não é possível excluir esta unidade: já existem vendas, royalties, chamados ou movimentações de estoque vinculados a ela. Considere inativá-la em vez de excluí-la.");
        }

        _context.UnidadesFranqueadas.Remove(unidade);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}