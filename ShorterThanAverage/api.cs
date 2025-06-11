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
app.MapPost("/api/shorten/", async (string request, string? vanity, UrlDatabase db) =>
{
    if (!string.IsNullOrEmpty(vanity) && !Regex.IsMatch(vanity, @"^[a-zA-Z0-9]+$"))
    {
        return Results.BadRequest("Vanity URL must contain only a-z, A-Z, or 0-9 characters.");
    }
    // check if full url exists in DB, return the shortened version thats already in the DB
    // if not:
    URL UrlObject = new URL(request, vanity); // vanity can be null, as i check it in URL class and generate a shorteneted URL if a vanity one is not provided.
    var dbResult = await db.InsertUrlAsync(UrlObject.FullUrl, UrlObject.ShortenedUrl);
    while (dbResult.url != UrlObject.FullUrl)
    {
        UrlObject.RegenerateShortenedUrl(); // also randomizes vanity URL, this is expected behavior for now
        dbResult = await db.InsertUrlAsync(UrlObject.FullUrl, UrlObject.ShortenedUrl);
    }
    return Results.Ok(UrlObject.ShortenedUrl);
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