namespace Franquias.Api.Models;

public enum PrioridadeChamado
{
    Baixa,
    Media,
    Alta
}

public enum StatusChamado
{
    Aberto,
    EmAndamento,
    Encerrado
}

public class ChamadoSuporte
{
    public int Id { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public PrioridadeChamado Prioridade { get; set; }
    public StatusChamado Status { get; set; } = StatusChamado.Aberto;
    public DateTime DataAbertura { get; set; } = DateTime.Now;
    public DateTime? DataEncerramento { get; set; }

    public int UnidadeFranqueadaId { get; set; }
    public UnidadeFranqueada? UnidadeFranqueada { get; set; }
}