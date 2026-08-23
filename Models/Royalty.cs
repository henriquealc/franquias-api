namespace Franquias.Api.Models;

public enum StatusPagamento
{
    Pendente,
    Pago
}

public class Royalty
{
    public int Id { get; set; }
    public DateTime PeriodoInicio { get; set; }
    public DateTime PeriodoFim { get; set; }
    public decimal Faturamento { get; set; }
    public decimal PercentualAplicado { get; set; }
    public decimal ValorRoyalty { get; set; }
    public StatusPagamento Status { get; set; } = StatusPagamento.Pendente;
    public DateTime DataCalculo { get; set; } = DateTime.Now;

    public int UnidadeFranqueadaId { get; set; }
    public UnidadeFranqueada? UnidadeFranqueada { get; set; }
}