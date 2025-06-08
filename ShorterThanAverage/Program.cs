var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi(); // it was needed, stoopid

// why is this not included with dotnet 9 :sad:
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// shortening endpoint
app.MapPost("/api/shorten/", (string URL) =>
{
    var shortened = new
    {
        URL,
        shortURL = "Hellooooooo"
    };
    return Results.Ok(shortened);
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
