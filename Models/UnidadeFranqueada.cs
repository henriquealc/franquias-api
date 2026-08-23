namespace Franquias.Api.Models;

public class UnidadeFranqueada
{
    public int Id { get; set; }
    public string NomeUnidade { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string NomeResponsavel { get; set; } = string.Empty;
    public string ContatoResponsavel { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public bool Ativa { get; set; } = true;

    // Relacionamento: cada unidade pertence a uma franqueadora
    public int FranqueadoraId { get; set; }
    public Franqueadora? Franqueadora { get; set; }
}