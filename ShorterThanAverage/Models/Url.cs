using System;
using System.Security.Cryptography;
using System.Text;
using Base62;

public class Url
{
    public string FullUrl { get; set; }
    private string _shortenedUrl;
    public string ShortenedUrl
    {
        get => _shortenedUrl;
        set => _shortenedUrl = string.IsNullOrWhiteSpace(value) ? GenerateShortenedUrl(FullUrl) : value;
    }

    public Url(string fullUrl, string? shortenedUrl = null)
    {
        FullUrl = fullUrl;
        ShortenedUrl = shortenedUrl;
    }

    private string GenerateShortenedUrl(string url)
    {
        using (var md5 = MD5.Create())
        {
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(url));
            var encoded = hash.ToBase62();
            return encoded.Substring(0, 6);
        }
    }

    public void RegenerateShortenedUrl()
    {
        var encoded = this.ShortenedUrl.ToBase62();
        this.ShortenedUrl = encoded.Substring(0, 6);
    }
}