namespace Franquias.Api.Models;

public class Franqueadora
{
    public int Id { get; set; }
    public string NomeFantasia { get; set; } = string.Empty;
    public string RazaoSocial { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Contato { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public bool Ativa { get; set; } = true;
    public decimal PercentualRoyalty { get; set; }
}