using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UserManagement.Application.Abstractions;

namespace UserManagement.Infrastructure.Services
{

    public class JsonLocalizationService : ILocalizationService
    {
        private readonly IHostEnvironment _env;

        public JsonLocalizationService(IHostEnvironment env)
        {
            _env = env;
        }

        public async Task<IDictionary<string, string>> GetStringsAsync(
            string lang,
            CancellationToken ct = default)
        {
            lang = string.IsNullOrWhiteSpace(lang) ? "en" : lang.ToLowerInvariant();

            var fileName = lang switch
            {
                "ar" => "strings.ar.json",
                _ => "strings.en.json"
            };

            var path = Path.Combine(_env.ContentRootPath, "Localization", fileName);

            if (!File.Exists(path))
            {
                return new Dictionary<string, string>();
            }

            await using var stream = File.OpenRead(path);

            var dict = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(
                stream,
                cancellationToken: ct);

            return dict ?? new Dictionary<string, string>();
        }
    }
}
