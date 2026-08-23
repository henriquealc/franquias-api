namespace Franquias.Api.Models;

public class Venda
{
    public int Id { get; set; }
    public DateTime Data { get; set; } = DateTime.Now;
    public decimal ValorTotal { get; set; }

    public int UnidadeFranqueadaId { get; set; }
    public UnidadeFranqueada? UnidadeFranqueada { get; set; }

    public List<ItemVenda> Itens { get; set; } = new List<ItemVenda>();
}