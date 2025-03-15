#region Using directives

using System.Text.Json;

using Cocona;

using Microsoft.Extensions.Configuration;

using RestSharp;

#endregion

var configuration = new ConfigurationBuilder()
    .AddJsonFile ("appsettings.json", optional: false)
    .Build();

var client = new RestClient();

var endpoint = configuration["endpoint"]!;
var model = configuration["model"]!;
var russian = configuration["russian"]!;
var english = configuration["english"]!;

var builder = CoconaApp.CreateBuilder();
var app = builder.Build();
app.AddCommand
    (
        ([Argument (Description = "Папка с изображениями")] string folder,
            [Argument (Description = "Префикс, добавляемый к имени файла")]
            string prefix,
            [Argument (Description = "Имя результирующего файла")]
            string output) =>
        {
            var files = ScanFolder (folder);
            using var writer = File.CreateText (output);
            ProcessFiles (writer, folder, prefix, files);
            Console.WriteLine();
            Console.WriteLine ("ALL DONE");
        }
    );

app.Run();

void ProcessFiles
    (
        TextWriter writer,
        string folder,
        string prefix,
        string[] files
    )
{
    foreach (var file in files)
    {
        ProcessPicture (writer, folder, prefix, file);
    }
}

string[] ScanFolder (string folder)
{
    var result = Directory.GetFiles
            (
                folder,
                "*.*",
                SearchOption.AllDirectories
            )
        .Where (fileName =>
        {
            var extension = Path.GetExtension (fileName);
            if (string.IsNullOrEmpty (extension))
            {
                return false;
            }

            extension = extension.ToLowerInvariant();
            return extension is ".jpg" or ".jpeg" or ".jfif" or ".bmp"
                or ".gif" or ".png" or ".tif" or ".tiff" or ".webp";
        })
        .ToArray();

    return result;
}

void ProcessPicture
    (
        TextWriter writer,
        string folder,
        string prefix,
        string file
    )
{
    var shortName = prefix + file[folder.Length..].ToLowerInvariant();
    if (shortName.StartsWith ('\\') || shortName.StartsWith ('/'))
    {
        shortName = shortName[1..];
    }

    var russianDescription = QueryByImage
        (
            russian,
            file
        );
    var englishDescription = QueryByImage
        (
            english,
            file
        );
    writer.WriteLine (shortName);
    Console.WriteLine (shortName);
    writer.WriteLine (russianDescription);
    Console.WriteLine (russianDescription);
    writer.WriteLine (englishDescription);
    Console.WriteLine (englishDescription);
    writer.WriteLine ();
    Console.WriteLine();
    writer.Flush();
}

string QueryByImage
    (
        string prompt,
        string imagePath
    )
{
    var bytes = File.ReadAllBytes (imagePath);
    var base64 = Convert.ToBase64String (bytes);

    var message = new
    {
        role = "user",
        content = prompt,
        images = new[] { base64 }
    };

    var payload = new
    {
        model,
        stream = false,
        messages = new object[] { message }
    };

    var request = new RestRequest (endpoint, Method.Post);
    request.AddJsonBody (payload);
    var response = client.Execute (request);
    if (!response.IsSuccessful)
    {
        throw new Exception (response.ErrorMessage);
    }

    var doc = JsonDocument.Parse (response.Content!);
    var result = doc.RootElement
                     .GetProperty ("message")
                     .GetProperty ("content")
                     .GetString()
                 ?? string.Empty;

    result = result
        .Replace ('\n', ' ')
        .Replace ('\r', ' ')
        .Replace ('\t', ' ');

    result = result.Replace ("  ", " ");

    return result;
}
