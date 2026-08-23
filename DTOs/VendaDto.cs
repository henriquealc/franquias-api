namespace Franquias.Api.DTOs;

public class ItemVendaDto
{
    public int ProdutoServicoId { get; set; }
    public int Quantidade { get; set; }
}

public class VendaDto
{
    public int UnidadeFranqueadaId { get; set; }
    public List<ItemVendaDto> Itens { get; set; } = new List<ItemVendaDto>();
}