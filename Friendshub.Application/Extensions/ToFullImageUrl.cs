namespace Friendshub.Application.Extensions
{
    public static class StringExtensions
    {
        private const string BackendBaseUrl = "https://localhost:44326/";

        public static string ToFullImageUrl(this string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            // Ukloni eventualni vodeći /
            relativePath = relativePath.TrimStart('/');

            return $"{BackendBaseUrl}{relativePath}";
        }
    }
}
