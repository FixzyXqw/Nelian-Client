using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nelian
{
    public static class BanChecker
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private const string BanListUrl = "https://raw.githubusercontent.com/FixzyXqw/Nelian-Client/main/BanList.txt";

        public class BanResult
        {
            public bool IsBanned { get; set; } = false;
            public bool IsPermanent { get; set; } = false;
            public DateTime? ExpireDate { get; set; } = null;
            public string? Reason { get; set; } = null;
        }

        public static async Task<BanResult> CheckBanAsync(string username, string uuid)
        {
            var result = new BanResult();

            string content;
            try
            {
                content = await _httpClient.GetStringAsync(BanListUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BanList failed to download: {ex.Message}");
                return result;
            }

            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            string normalizedPlayerUuid = NormalizeUuid(uuid);

            foreach (var rawLine in lines)
            {
                string line = rawLine.Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("//", StringComparison.Ordinal))
                    continue;

                var parts = line.Split('/');
                if (parts.Length < 4)
                    continue;

                string reason = parts[0].Trim();
                string bannedUuid = parts[1].Trim();
                string bannedUsername = parts[2].Trim();
                string expirePart = parts[3].Trim();

                bool uuidMatch = !string.IsNullOrWhiteSpace(uuid) &&
                                  NormalizeUuid(bannedUuid).Equals(normalizedPlayerUuid, StringComparison.OrdinalIgnoreCase);

                bool usernameMatch = !string.IsNullOrWhiteSpace(username) &&
                                      bannedUsername.Equals(username, StringComparison.OrdinalIgnoreCase);

                if (!uuidMatch && !usernameMatch)
                    continue;

                if (expirePart.Equals("perm", StringComparison.OrdinalIgnoreCase))
                {
                    result.IsBanned = true;
                    result.IsPermanent = true;
                    result.Reason = reason;
                    return result;
                }

                if (DateTime.TryParseExact(
                        expirePart,
                        "dd:MM:yyyy:HH:mm",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime expireDate))
                {
                    DateTime trueNow = await GetTrueDateTimeAsync();

                    if (expireDate < trueNow)
                    {
                        continue;
                    }
                    else
                    {
                        result.IsBanned = true;
                        result.IsPermanent = false;
                        result.ExpireDate = expireDate;
                        result.Reason = reason;
                        return result;
                    }
                }
                else
                {
                    Console.WriteLine($"Time parse failure: {expirePart}");
                    continue;
                }
            }

            return result;
        }

        private static async Task<DateTime> GetTrueDateTimeAsync()
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Head, "https://www.google.com");
                using var response = await _httpClient.SendAsync(request);

                if (response.Headers.Date.HasValue)
                    return response.Headers.Date.Value.UtcDateTime.ToLocalTime();
            }
            catch
            {
            }

            return DateTime.Now;
        }

        private static string NormalizeUuid(string? uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return string.Empty;

            return uuid.Replace("-", "").Trim().ToLowerInvariant();
        }
    }
}
