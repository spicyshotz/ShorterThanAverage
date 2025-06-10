var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

//builder.Services.AddDbContext<YourDatabaseContextClassName>(options =>
//options.UseNpgsql(Configuration.GetConnectionString("YourDatabaseContextStringNameFromAppsettings")));
// https://stackoverflow.com/questions/70473009/how-to-make-database-connectivity-in-asp-net-core-with-postgresql

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// shortening endpoint
app.MapPost("/api/shorten/", (string request, string? vanity) =>
{
    // check if full url exists in DB, return the shortened version thats already in the DB
    // if not:
    URL UrlObject = new URL(request, vanity); // vanity can be null, as i check it in URL class and generate a shorteneted URL if a vanity one is not provided.
    // check if generated URL is unique (keep checking until it is unique) :^)
    //if not:
    // UrlObject.RegenerateShortenedUrl(); // will use if i have time.
    return Results.Ok(UrlObject.ShortenedUrl);
})
.WithName("ShortenURL");

// redirecting to full URL endpoint
app.MapGet("{short_code}", (string short_code) =>
{
    var originalURL = "https://learn.microsoft.com/en-us/aspnet/web-api/overview/older-versions/build-restful-apis-with-aspnet-web-api";
    return Results.Redirect(originalURL);
})
.WithName("RedirectToFullURL");

app.Run();