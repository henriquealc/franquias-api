namespace Franquias.Api.Models;

public enum TipoMovimentacao
{
    Entrada,
    Saida
}

public class MovimentacaoEstoque
{
    public int Id { get; set; }
    public TipoMovimentacao Tipo { get; set; }
    public int Quantidade { get; set; }
    public DateTime Data { get; set; } = DateTime.Now;
    public string Observacao { get; set; } = string.Empty;

    public int ProdutoServicoId { get; set; }
    public ProdutoServico? ProdutoServico { get; set; }

    public int UnidadeFranqueadaId { get; set; }
    public UnidadeFranqueada? UnidadeFranqueada { get; set; }
}