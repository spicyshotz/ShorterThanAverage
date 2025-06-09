var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

//builder.Services.AddDbContext<YourDatabaseContextClassName>(options =>
//options.UseNpgsql(Configuration.GetConnectionString("YourDatabaseContextStringNameFromAppsettings")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// shortening endpoint
app.MapPost("/api/shorten/", (string request) =>
{
    URL UrlObject = new URL(request);
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

//record ShortenRequest(string URL);