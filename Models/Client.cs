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

    [Ignore]
    public bool HasPhone =>
        !string.IsNullOrWhiteSpace(Phone);

    [Ignore]
    public bool HasEmail =>
        !string.IsNullOrWhiteSpace(Email);

    [Ignore]
    public string Initials
    {
        get
        {
            var parts = (Name ?? string.Empty)
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return "ME";

            if (parts.Length == 1)
            {
                return parts[0]
                    [..Math.Min(2, parts[0].Length)]
                    .ToUpperInvariant();
            }

            return $"{parts[0][0]}{parts[^1][0]}"
                .ToUpperInvariant();
        }
    }
}
