using System.Text.RegularExpressions;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("PostgreDB");
builder.Services.AddScoped((provider) => new NpgsqlConnection(connectionString));
builder.Services.AddScoped<UrlDatabase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// shortening endpoint
app.MapPost("/api/shorten/", async (HttpContext context, string request, string? vanity, UrlDatabase db) =>
{
    if (!string.IsNullOrEmpty(vanity) && !Regex.IsMatch(vanity, @"^[a-zA-Z0-9]+$"))
    {
        return Results.BadRequest("Vanity URL must contain only a-z, A-Z, or 0-9 characters.");
    }
    
    if (!Uri.IsWellFormedUriString(request, UriKind.Absolute))
    {
        return Results.BadRequest("Invalid URL format. Please provide a valid URL.");
    }

    var host = context.Request.Host.Value;
    var scheme = context.Request.Scheme;
    var shortUrl = string.Empty;

    var dbResult = await db.CheckUrlExistanceAsync(request);
    if (dbResult.url is not null)
    {
        if (!string.IsNullOrWhiteSpace(vanity) && dbResult.shortUrl != vanity)
        {
            return Results.BadRequest("Vanity Failed: Shortened URL already exists for this full URL. Please try shortening a different URL.");
        }
        shortUrl = $"{scheme}://{host}/{dbResult.shortUrl}";
        return Results.Ok(shortUrl);
    }

    URL UrlObject = new URL(request, vanity); // vanity can be null, as i check it in URL class and generate a shorteneted URL if a vanity one is not provided.

    dbResult = await db.InsertUrlAsync(UrlObject.FullUrl, UrlObject.ShortenedUrl);
    while (dbResult.url != UrlObject.FullUrl)
    {
        if (!string.IsNullOrWhiteSpace(vanity))
        {
            return Results.BadRequest("Vanity URL already exists. Please choose a different one.");
        }

        UrlObject.RegenerateShortenedUrl();
        dbResult = await db.InsertUrlAsync(UrlObject.FullUrl, UrlObject.ShortenedUrl);
    }
    shortUrl = $"{scheme}://{host}/{UrlObject.ShortenedUrl}";
    return Results.Ok(shortUrl);
})
.WithName("ShortenURL");


// redirecting to full URL endpoint
app.MapGet("{short_code}", async (string short_code, UrlDatabase db) =>
{
    var originalURL = await db.GetFullUrlAsync(short_code);
    if (originalURL is null)
    {
        return Results.NotFound();

    }
    return Results.Redirect(originalURL);
})
.WithName("RedirectToFullURL");

app.Run();