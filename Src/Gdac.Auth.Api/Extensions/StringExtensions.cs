namespace Gdac.Auth.Api.Extensions;

internal static class StringExtensions
{
    internal static string Truncate(this string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..maxLength];
}
