using Npgsql;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("PostgreDB");
builder.Services.AddScoped((provider) => new NpgsqlConnection(connectionString));
builder.Services.AddScoped<UrlShortenerService>();
builder.Services.AddScoped<IUrlDatabase, UrlDatabase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

// shortening endpoint
app.MapPost("/api/shorten/", async (HttpContext context, ShortenRequest request, UrlShortenerService shortenerService) =>
{
    try
    {
        var shortCode = await shortenerService.ShortenUrlAsync(request.Url, request.Vanity);
        var host = context.Request.Host.Value;
        var scheme = context.Request.Scheme;
        var shortUrl = $"{scheme}://{host}/{shortCode}";
        return Results.Text(shortUrl, "text/plain");
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.WithName("ShortenURL");

// redirecting to full URL endpoint
app.MapGet("{short_code}", async (string short_code, UrlShortenerService shortenerService) =>
{
    var originalURL = await shortenerService.GetOriginalUrlAsync(short_code);
    if (originalURL is null)
        return Results.NotFound();
    return Results.Redirect(originalURL);
})
.WithName("RedirectToFullURL");

app.Run();