using System.Threading.Tasks;

public interface IUrlDatabase
{
    Task<(string url, string shortUrl)> InsertUrlAsync(string fullUrl, string shortUrl);
    Task<string?> GetFullUrlAsync(string shortCode);
    Task<(string? url, string? shortCode)> CheckUrlExistanceAsync(string fullUrl);
}