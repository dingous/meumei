using SQLite;

namespace MEIUtil.Models;

public sealed class Client
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(160), Indexed]
    public string Name { get; set; } = string.Empty;

    [MaxLength(40)]
    public string Document { get; set; } = string.Empty;

    [MaxLength(40)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(220)]
    public string Notes { get; set; } = string.Empty;
}
