#region Using directives

using System.Net.Mime;

using SkiaSharp;

#endregion

var builder = WebApplication.CreateBuilder (args);
builder.WebHost.UseIIS(); // нужно для хостинга на IIS

var app = builder.Build();

// здесь мы пишем "/", под IIS будет "/scale" (или что нам настроено)
app.MapGet ("/", (string name, float factor = 1.0f) =>
{
    var rootDirectory = app.Configuration["RootDirectory"]!;
    var fullPath = Path.Combine (rootDirectory, name);
    if (!File.Exists (fullPath))
    {
        return Results.NotFound();
    }

    if (Math.Abs (factor - 1.0f) < 0.01f)
    {
        return Results.File (fullPath, MediaTypeNames.Image.Jpeg);
    }

    using var fileStream = File.OpenRead (fullPath);
    using var original = SKBitmap.Decode (fileStream)!;
    var newHeight = (int)(original.Height * factor);
    var newWidth = (int)(original.Width * factor);

    var scaled = new SKBitmap (newWidth, newHeight);
    original.ScalePixels (scaled, SKFilterQuality.High);

    var memoryStream = new MemoryStream();
    scaled.Encode (memoryStream, SKEncodedImageFormat.Jpeg, 95);
    memoryStream.Position = 0;

    return Results.Stream (memoryStream, MediaTypeNames.Image.Jpeg);
});

app.Run();
