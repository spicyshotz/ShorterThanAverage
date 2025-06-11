using Npgsql;
using System.Threading.Tasks;

public class UrlDatabase
{
    private readonly NpgsqlConnection _connection;

    public UrlDatabase(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<(string url, string shortUrl)> InsertUrlAsync(string fullUrl, string shortUrl)
    {
        await _connection.OpenAsync();
        try
        {
            using var cmd = new NpgsqlCommand(@"
            INSERT INTO shortener.""shortenerTable"" (""URL"", ""ShortURL"")
            VALUES (@URL, @ShortURL)
            ON CONFLICT (""ShortURL"")
            DO UPDATE SET ""ShortURL"" = EXCLUDED.""ShortURL""
            RETURNING *;
            ", _connection);
            cmd.Parameters.AddWithValue("URL", fullUrl);
            cmd.Parameters.AddWithValue("ShortURL", shortUrl);
            using var reader = await cmd.ExecuteReaderAsync();

            if (reader.Read())
            {
                string url = reader.GetString(reader.GetOrdinal("URL"));
                string shortenedUrl = reader.GetString(reader.GetOrdinal("ShortURL"));

                return (url, shortenedUrl);
            }
            return (fullUrl, shortUrl);
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    public async Task<string?> GetFullUrlAsync(string shortUrl)
    {
        await _connection.OpenAsync();
        try
        {
            using var cmd = new NpgsqlCommand("SELECT \"URL\" FROM shortener.\"shortenerTable\" WHERE \"ShortURL\" = @ShortURL", _connection);
            cmd.Parameters.AddWithValue("ShortURL", shortUrl);
            var result = await cmd.ExecuteScalarAsync();
            return result as string;
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
}