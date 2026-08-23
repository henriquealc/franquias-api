using Microsoft.EntityFrameworkCore;
using Franquias.Api.Models;

namespace Franquias.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Franqueadora> Franqueadoras { get; set; }
    public DbSet<UnidadeFranqueada> UnidadesFranqueadas { get; set; }
    public DbSet<ProdutoServico> ProdutosServicos { get; set; }
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }
    public DbSet<Venda> Vendas { get; set; }
    public DbSet<ItemVenda> ItensVenda { get; set; }
    public DbSet<Fornecedor> Fornecedores { get; set; }
    public DbSet<Royalty> Royalties { get; set; }
    public DbSet<ChamadoSuporte> ChamadosSuporte { get; set; }
}