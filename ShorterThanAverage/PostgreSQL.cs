using Npgsql;
using System.Threading.Tasks;

public class UrlDatabase
{
    private readonly NpgsqlConnection _connection;

    public UrlDatabase(NpgsqlConnection connection)
    {
        _connection = connection;
    }

    public async Task InsertUrlAsync(string fullUrl, string shortUrl)
    {
        await _connection.OpenAsync();
        try
        {
            using var add = new NpgsqlCommand("INSERT INTO shortener.\"shortenerTable\" (\"URL\", \"ShortURL\") VALUES (@URL, @ShortURL)", _connection);
            add.Parameters.AddWithValue("URL", fullUrl);
            add.Parameters.AddWithValue("ShortURL", shortUrl);
            await add.ExecuteNonQueryAsync();
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
            using var get = new NpgsqlCommand("SELECT \"URL\" FROM shortener.\"shortenerTable\" WHERE \"ShortURL\" = @ShortURL", _connection);
            get.Parameters.AddWithValue("ShortURL", shortUrl);
            var result = await get.ExecuteScalarAsync();
            return result as string;
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
}