using SQLite;

namespace MEIUtil.Models;

public sealed class MeiProfile
{
    [PrimaryKey]
    public int Id { get; set; } = 1;

    [MaxLength(160)]
    public string OwnerName { get; set; } = string.Empty;

    [MaxLength(180)]
    public string BusinessName { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Cnpj { get; set; } = string.Empty;

    public DateTime OpenedAt { get; set; } = DateTime.Today;

    public decimal AnnualRevenueLimit { get; set; } = 81_000m;
}
