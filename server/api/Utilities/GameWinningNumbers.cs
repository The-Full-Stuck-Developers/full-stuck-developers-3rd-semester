namespace api.Utilities;

using System.Globalization;

public sealed class GameWinningNumbers
{
    private readonly HashSet<int> _numbers;

    public string Value => string.Join(",", _numbers.OrderBy(x => x));

    public int Count => _numbers.Count;

    public GameWinningNumbers(string input)
    {
        _numbers = Parse(input);
    }

    public bool IsGuessedBy(GameWinningNumbers userNumbers)
    {
        return _numbers.All(userNumbers._numbers.Contains);
    }

    private static HashSet<int> Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new HashSet<int>();

        return input
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => int.Parse(x.Trim(), CultureInfo.InvariantCulture))
            .ToHashSet();
    }
}
