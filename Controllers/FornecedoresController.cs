using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Franquias.Api.Data;
using Franquias.Api.Models;

namespace Franquias.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FornecedoresController : ControllerBase
{
    private readonly AppDbContext _context;

    public FornecedoresController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/fornecedores
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Fornecedor>>> GetFornecedores()
    {
        return await _context.Fornecedores.ToListAsync();
    }

    // GET: api/fornecedores/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Fornecedor>> GetFornecedor(int id)
    {
        var fornecedor = await _context.Fornecedores.FindAsync(id);

        if (fornecedor == null)
        {
            return NotFound();
        }

        return fornecedor;
    }

    // GET: api/fornecedores/buscar?nome=xxx&cnpj=xxx
    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<Fornecedor>>> Buscar(string? nome, string? cnpj)
    {
        var query = _context.Fornecedores.AsQueryable();

        if (!string.IsNullOrEmpty(nome))
        {
            query = query.Where(f => f.Nome.Contains(nome));
        }

        if (!string.IsNullOrEmpty(cnpj))
        {
            query = query.Where(f => f.Cnpj == cnpj);
        }

        return await query.ToListAsync();
    }

    // POST: api/fornecedores
    [HttpPost]
    public async Task<ActionResult<Fornecedor>> PostFornecedor(Fornecedor fornecedor)
    {
        _context.Fornecedores.Add(fornecedor);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFornecedor), new { id = fornecedor.Id }, fornecedor);
    }

    // PUT: api/fornecedores/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutFornecedor(int id, Fornecedor fornecedor)
    {
        if (id != fornecedor.Id)
        {
            return BadRequest();
        }

        _context.Entry(fornecedor).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var fornecedor = await _context.Fornecedores.FindAsync(id);

        if (fornecedor == null)
            return NotFound("Fornecedor não encontrado.");

        _context.Fornecedores.Remove(fornecedor);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}