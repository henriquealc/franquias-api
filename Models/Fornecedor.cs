namespace Franquias.Api.Models;

public class Fornecedor
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Contato { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
}