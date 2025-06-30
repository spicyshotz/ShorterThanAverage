using Xunit;
using Moq;
using System.Threading.Tasks;
using System;
using System.Net.Http.Json;

public class UrlShortenerServiceTests
{
    [Fact]
    public async Task ShortenUrlAsync_ReturnsShortCode_ForValidUrl()
    {
        // Arrange
        var mockDb = new Mock<IUrlDatabase>();
        mockDb.Setup(db => db.CheckUrlExistanceAsync(It.IsAny<string>()))
              .ReturnsAsync((null, null));
        mockDb.Setup(db => db.InsertUrlAsync(It.IsAny<string>(), It.IsAny<string>()))
              .ReturnsAsync((string url, string shortUrl) => (url, shortUrl));

        var service = new UrlShortenerService(mockDb.Object);

        // Act
        var result = await service.ShortenUrlAsync("https://wikipedia.org", null);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(result));
        Assert.True(result.Length == 6);
    }

    [Fact]
    public async Task ShortenUrlAsync_ThrowsArgumentException_ForInvalidUrl()
    {
        var mockDb = new Mock<IUrlDatabase>();
        var service = new UrlShortenerService(mockDb.Object);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.ShortenUrlAsync("not-a-url", null));
    }

    [Fact]
    public async Task ShortenUrlAsync_ThrowsArgumentException_ForInvalidVanity()
    {
        var mockDb = new Mock<IUrlDatabase>();
        var service = new UrlShortenerService(mockDb.Object);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.ShortenUrlAsync("https://wikipedia.org", "bad*vanity"));
    }

    [Fact]
    public async Task ShortenUrlAsync_ThrowsInvalidOperationException_WhenVanityExistsForDifferentUrl()
    {
        var mockDb = new Mock<IUrlDatabase>();
        mockDb.Setup(db => db.CheckUrlExistanceAsync("https://wikipedia.org"))
              .ReturnsAsync((null, null));
        mockDb.Setup(db => db.CheckUrlExistanceAsync("vanity"))
              .ReturnsAsync(("https://wikihow.com", "vanity"));

        var service = new UrlShortenerService(mockDb.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ShortenUrlAsync("https://wikipedia.org", "vanity"));
    }

    [Fact]
    public async Task GetOriginalUrlAsync_ReturnsUrl_WhenExists()
    {
        var mockDb = new Mock<IUrlDatabase>();
        mockDb.Setup(db => db.GetFullUrlAsync("iTAy05"))
              .ReturnsAsync("https://wikipedia.org");

        var service = new UrlShortenerService(mockDb.Object);

        var result = await service.GetOriginalUrlAsync("iTAy05");

        Assert.Equal("https://wikipedia.org", result);
    }

    [Fact]
    public async Task GetOriginalUrlAsync_ReturnsNull_WhenNotExists()
    {
        var mockDb = new Mock<IUrlDatabase>();
        mockDb.Setup(db => db.GetFullUrlAsync("notfound"))
              .ReturnsAsync((string?)null);

        var service = new UrlShortenerService(mockDb.Object);

        var result = await service.GetOriginalUrlAsync("notfound");

        Assert.Null(result);
    }

    [Fact]
    public async Task ShortenUrl_Handles_Multiple_Concurrent_Requests() // this one needs the API and DB to be running, comment out if not relevant i guess.
    {
        using var client = new HttpClient { BaseAddress = new Uri("http://localhost:5196") };
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => client.PostAsJsonAsync("/api/shorten/", new { url = "https://www.morfix.co.il/en/concurrent" }));
        var responses = await Task.WhenAll(tasks);
        foreach (var response in responses)
            response.EnsureSuccessStatusCode();
    }
}
