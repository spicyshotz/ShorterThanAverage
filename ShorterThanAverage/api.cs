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
    // check if full url exists in DB, return the shortened version thats already in the DB
    // if not:
    URL UrlObject = new URL(request, vanity); // vanity can be null, as i check it in URL class and generate a shorteneted URL if a vanity one is not provided.
    // check if generated URL is unique (keep checking until it is unique) :^)
    //if not:
    // UrlObject.RegenerateShortenedUrl(); // will use if i have time.
    await db.InsertUrlAsync(UrlObject.FullUrl, UrlObject.ShortenedUrl);
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