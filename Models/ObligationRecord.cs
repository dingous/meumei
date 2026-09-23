using SQLite;

namespace MEIUtil.Models;

public sealed class ObligationRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(40), Indexed(Name = "UX_Obligation_Key", Unique = true)]
    public string Key { get; set; } = string.Empty;

    [MaxLength(40)]
    public string Type { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Reference { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }
    public bool IsDone { get; set; }

    [Ignore]
    public string StatusText => IsDone ? "Concluído" : DueDate.Date < DateTime.Today ? "Revisar prazo" : "Pendente";

    [Ignore]
    public string ActionText => IsDone ? "Reabrir" : "Marcar como concluído";
}
