using System.Text.RegularExpressions;

public class UrlShortenerService
{
    private readonly UrlDatabase _db;

    public UrlShortenerService(UrlDatabase db)
    {
        _db = db;
    }

    public async Task<string> ShortenUrlAsync(string fullUrl, string? vanity)
    {
        // Validation logic (can be extracted further)
        if (!Uri.IsWellFormedUriString(fullUrl, UriKind.Absolute))
            throw new ArgumentException("Invalid URL format.");

        if (!string.IsNullOrEmpty(vanity) && !Regex.IsMatch(vanity, @"^[a-zA-Z0-9]+$"))
            throw new ArgumentException("Vanity URL must contain only a-z, A-Z, or 0-9 characters.");

        var dbResult = await _db.CheckUrlExistanceAsync(fullUrl);
        if (dbResult.url is not null)
        {
            if (!string.IsNullOrWhiteSpace(vanity) && dbResult.shortCode != vanity)
                throw new InvalidOperationException("Vanity Failed: Shortened URL already exists for this full URL.");
            return dbResult.shortCode!;
        }

        Url UrlObject = new Url(fullUrl, vanity);
        dbResult = await _db.InsertUrlAsync(UrlObject.FullUrl, UrlObject.ShortenedUrl);
        while (dbResult.url != UrlObject.FullUrl)
        {
            if (!string.IsNullOrWhiteSpace(vanity))
                throw new InvalidOperationException("Vanity URL already exists. Please choose a different one.");

            UrlObject.RegenerateShortenedUrl();
            dbResult = await _db.InsertUrlAsync(UrlObject.FullUrl, UrlObject.ShortenedUrl);
        }
        return UrlObject.ShortenedUrl;
    }

    public async Task<string?> GetOriginalUrlAsync(string shortCode)
    {
        return await _db.GetFullUrlAsync(shortCode);
    }
}