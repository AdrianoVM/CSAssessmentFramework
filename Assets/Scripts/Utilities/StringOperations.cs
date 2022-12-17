using System.Text.RegularExpressions;

namespace Utilities
{
    /// <summary>
    /// Helper Class for extension methods for strings.
    /// </summary>
    public static class StringOperations
    {
        public static string CamelCaseToSpaces(this string s)
        {
            return Regex.Replace(s, "([a-z](?=[A-Z]|[0-9])|[A-Z](?=[A-Z][a-z]|[0-9])|[0-9](?=[^0-9]))", "$1 ");
        }
    }
}