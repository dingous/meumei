using SQLite;

namespace MEIUtil.Models;

public static class TransactionTypes
{
    public const string Revenue = "Receita";
    public const string Expense = "Despesa";
}

public sealed class Transaction
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public DateTime Date { get; set; } = DateTime.Today;

    [MaxLength(20), Indexed]
    public string Type { get; set; } = TransactionTypes.Revenue;

    [MaxLength(180)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Category { get; set; } = "Geral";

    public decimal Amount { get; set; }

    public bool IsPaid { get; set; } = true;

    [Ignore]
    public string SignedAmount => $"{(Type == TransactionTypes.Expense ? "-" : "+")} {Amount:C2}";

    [Ignore]
    public string PaymentStatusText => IsPaid
        ? Type == TransactionTypes.Expense ? "Pago" : "Recebido"
        : Type == TransactionTypes.Expense ? "A pagar" : "A receber";
}
