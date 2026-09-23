using SQLite;

namespace MEIUtil.Models;

public sealed class Quote
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(160)]
    public string ClientName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public decimal Total { get; set; }

    [MaxLength(30)]
    public string Status { get; set; } = "Pendente";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime ValidUntil { get; set; } = DateTime.Today.AddDays(10);

    [Ignore]
    public string Number => $"#{Id:00000}";

    [Ignore]
    public bool IsExpired =>
        ValidUntil.Date < DateTime.Today;

    [Ignore]
    public string ValidityText =>
        IsExpired
            ? $"Expirou em {ValidUntil:dd/MM/yyyy}"
            : $"Válido até {ValidUntil:dd/MM/yyyy}";
}
