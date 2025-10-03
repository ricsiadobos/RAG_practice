using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RAG.Core;
using RAG.Core.Interfaces;

namespace RAG.Infrastructure;

public sealed class OpenAiProvider : IAiProvider
{
    private readonly HttpClient _http;
    private readonly AiSettings _cfg;
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public OpenAiProvider(HttpClient http, AiSettings cfg)
    {
        _http = http;
        _cfg = cfg;

        // Alap HTTP fejlécek
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _cfg.ApiKey);
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("RAG-Practice/1.0");
    }

    public async Task<string> ChatAsync(string system, string prompt, int maxTokens, double temperature, CancellationToken ct)
    {
        var body = new
        {
            model = _cfg.ChatModel,
            messages = new object[]
            {
                new { role = "system", content = system },
                new { role = "user",   content = prompt }
            },
            max_tokens = maxTokens,
            temperature = temperature
        };

        using var resp = await SendWithRetryAsync(() =>
            new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Content = new StringContent(JsonSerializer.Serialize(body, JsonOpts), Encoding.UTF8, "application/json")
            }, ct);

        using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync(ct));
        var root = doc.RootElement;

        // a válasz a choices[0].message.content-ben van
        var content = root
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return content ?? string.Empty;
    }

    public async Task<float[]> EmbedAsync(string text, CancellationToken ct)
    {
        var body = new
        {
            model = _cfg.EmbeddingModel,
            input = text
        };

        using var resp = await SendWithRetryAsync(() =>
            new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/embeddings")
            {
                Content = new StringContent(JsonSerializer.Serialize(body, JsonOpts), Encoding.UTF8, "application/json")
            }, ct);

        using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync(ct));
        var arr = doc.RootElement.GetProperty("data")[0].GetProperty("embedding").EnumerateArray();

        // Biztonság kedvéért kezeljük, ha float/double jön
        var list = new List<float>(1536);
        foreach (var el in arr)
        {
            if (el.TryGetSingle(out var f)) list.Add(f);
            else list.Add((float)el.GetDouble());
        }
        return list.ToArray();
    }

    // ----------------- Segédfüggvények -----------------

    /// <summary>
    /// Általános HTTP küldés retry/backoff-fal.
    /// 429 és 5xx kódokra újrapróbál, kivéve ha a body quota/credit hibát jelez.
    /// </summary>
    private async Task<HttpResponseMessage> SendWithRetryAsync(Func<HttpRequestMessage> makeRequest, CancellationToken ct)
    {
        const int maxAttempts = 5;
        var rng = new Random();

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            using var req = makeRequest();

            var resp = await _http.SendAsync(req, ct);
            if (resp.IsSuccessStatusCode)
                return resp;

            // Olvassuk ki a hibatestet diagnosztikához
            string body = string.Empty;
            try { body = await resp.Content.ReadAsStringAsync(ct); } catch { /* ignore */ }

            // Ha quota/billing jellegű a hiba → ne próbálkozzunk
            if ((int)resp.StatusCode == 429 &&
                (Contains(body, "insufficient_quota") || Contains(body, "exceeded your current quota") || Contains(body, "billing")))
            {
                resp.Dispose();
                throw new HttpRequestException(
                    "OpenAI 429 - insufficient_quota/billing: Úgy tűnik nincs elérhető keret vagy hiányzik a számlázási beállítás. " +
                    "Ellenőrizd a platform.openai.com Billing oldalt. " +
                    $"Részletek: {Trim(body)}");
            }

            // 429/5xx → retry (ha van még próbálkozás)
            if (((int)resp.StatusCode == 429 || (int)resp.StatusCode >= 500) && attempt < maxAttempts)
            {
                // Retry-After tisztelete, ha van
                TimeSpan delay = TimeSpan.FromSeconds(Math.Min(2 * attempt, 10)); // 2s,4s,6s,8s,10s
                if (resp.Headers.TryGetValues("Retry-After", out var vals))
                {
                    var s = vals.FirstOrDefault();
                    if (int.TryParse(s, out var secs))
                        delay = TimeSpan.FromSeconds(Math.Clamp(secs, 1, 30));
                }

                // Kis jitter, hogy ne egyszerre verjük a limitet
                delay += TimeSpan.FromMilliseconds(rng.Next(0, 400));

                resp.Dispose();
                await Task.Delay(delay, ct);
                continue;
            }

            // Nem retry-olható hiba vagy elfogytak a próbák → részletes kivétel
            var statusText = $"{(int)resp.StatusCode} {resp.StatusCode}";
            resp.Dispose();
            throw new HttpRequestException($"OpenAI hiba ({statusText}). Részletek: {Trim(body)}");
        }

        throw new HttpRequestException("OpenAI hívás sikertelen többszöri próbálkozás után.");

        static bool Contains(string s, string needle)
            => !string.IsNullOrEmpty(s) && s.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;

        static string Trim(string s)
            => string.IsNullOrEmpty(s) ? "" : (s.Length > 800 ? s[..800] + "…" : s);
    }
}
