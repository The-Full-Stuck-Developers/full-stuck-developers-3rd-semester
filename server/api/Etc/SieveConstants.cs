namespace api.Etc;

public static class SieveConstants
{
    // Filter operators
    public new const string Equals = "==";
    public const string NotEquals = "!=";
    public const string Contains = "@=";
    public const string ContainsCaseInsensitive = "@=*";
    public const string StartsWith = "_=";
    public const string StartsWithCaseInsensitive = "_=*";
    public const string GreaterThan = ">";
    public const string LessThan = "<";
    public const string GreaterThanOrEqual = ">=";
    public const string LessThanOrEqual = "<=";

    // Example usage strings for documentation
    public const string FilterExample = "Name@=*john,Email@=*@example.com";
    public const string SortExample = "Name,-CreatedAt";
}