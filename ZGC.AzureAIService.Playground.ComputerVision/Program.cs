using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Local.json");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var configuration = app.Services.GetRequiredService<IConfiguration>();
var key = configuration.GetValue<string>("AZURE_COMPUTER_VISION_KEY");
var endpoint = configuration.GetValue<string>("AZURE_COMPUTER_VISION_ENDPOINT");

var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
{
    Endpoint = endpoint
};

app.MapGet("/", () => "Hello World!");
app.MapPost("/describe", async (ImageRequest request) =>
{
    var result = await client.DescribeImageAsync(request.Url);
    return new
    {
        Description = result.Captions.FirstOrDefault()?.Text,
        Confidence = result.Captions.FirstOrDefault()?.Confidence,
        Tags = result.Tags
    };
});

app.Run();
public record ImageRequest(string Url);
