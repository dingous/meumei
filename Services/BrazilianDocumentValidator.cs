namespace MEIUtil.Services;

public static class BrazilianDocumentValidator
{
    private static readonly int[] CnpjFirstWeights =
        [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

    private static readonly int[] CnpjSecondWeights =
        [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

    public static string Normalize(string? value)
        => string.Concat((value ?? string.Empty)
            .Where(char.IsAsciiLetterOrDigit))
            .ToUpperInvariant();

    public static bool IsValidCpfOrCnpj(string? value)
    {
        var normalized = Normalize(value);

        return normalized.Length switch
        {
            11 => IsValidCpf(normalized),
            14 => IsValidCnpj(normalized),
            _ => false
        };
    }

    public static bool IsValidCnpj(string? value)
    {
        var cnpj = Normalize(value);

        if (cnpj.Length != 14)
            return false;

        if (!cnpj[..12].All(char.IsAsciiLetterOrDigit) ||
            !char.IsAsciiDigit(cnpj[12]) ||
            !char.IsAsciiDigit(cnpj[13]))
        {
            return false;
        }

        if (cnpj.All(c => c == cnpj[0]))
            return false;

        var first = CalculateCnpjDigit(
            cnpj.AsSpan(0, 12),
            CnpjFirstWeights);

        var second = CalculateCnpjDigit(
            cnpj.AsSpan(0, 12),
            first,
            CnpjSecondWeights);

        return cnpj[12] - '0' == first &&
               cnpj[13] - '0' == second;
    }

    public static bool IsValidCpf(string? value)
    {
        var cpf = Normalize(value);

        if (cpf.Length != 11 ||
            !cpf.All(char.IsAsciiDigit))
        {
            return false;
        }

        if (cpf.All(c => c == cpf[0]))
            return false;

        var sum = 0;

        for (var i = 0; i < 9; i++)
            sum += (cpf[i] - '0') * (10 - i);

        var remainder = sum % 11;
        var first = remainder < 2 ? 0 : 11 - remainder;

        if (cpf[9] - '0' != first)
            return false;

        sum = 0;

        for (var i = 0; i < 10; i++)
            sum += (cpf[i] - '0') * (11 - i);

        remainder = sum % 11;
        var second = remainder < 2 ? 0 : 11 - remainder;

        return cpf[10] - '0' == second;
    }

    private static int CalculateCnpjDigit(
        ReadOnlySpan<char> baseCnpj,
        IReadOnlyList<int> weights)
    {
        var sum = 0;

        for (var i = 0; i < baseCnpj.Length; i++)
            sum += CnpjValue(baseCnpj[i]) * weights[i];

        var remainder = sum % 11;
        return remainder is 0 or 1 ? 0 : 11 - remainder;
    }

    private static int CalculateCnpjDigit(
        ReadOnlySpan<char> baseCnpj,
        int firstDigit,
        IReadOnlyList<int> weights)
    {
        var sum = 0;

        for (var i = 0; i < baseCnpj.Length; i++)
            sum += CnpjValue(baseCnpj[i]) * weights[i];

        sum += firstDigit * weights[^1];

        var remainder = sum % 11;
        return remainder is 0 or 1 ? 0 : 11 - remainder;
    }

    private static int CnpjValue(char value)
        => value - '0';
}
