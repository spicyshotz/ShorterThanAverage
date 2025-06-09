using System;
using System.Security.Cryptography;
using System.Text;

public class URL
{
    public string FullUrl { get; set; }
    private string _shortenedUrl;
    public string ShortenedUrl
    {
        get => _shortenedUrl;
        set => _shortenedUrl = string.IsNullOrWhiteSpace(value) ? GenerateShortenedUrl(FullUrl) : value;
    }

    public URL(string fullUrl, string shortenedUrl = null)
    {
        FullUrl = fullUrl;
        ShortenedUrl = shortenedUrl;
    }

    private string GenerateShortenedUrl(string url)
    {
        using (var sha256 = SHA256.Create()) // use md5 instead!! fasterr :^)
        {
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(url));
            var sb = new StringBuilder(); // use base62 encoding instead!! also faster (i think) and will also do uppercase letters
            for (int i = 0; i < 3; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}