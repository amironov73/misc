using System.ClientModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

using Microsoft.Extensions.Configuration;

using Cocona;

using LanguageDetection;

using OpenAI;
using OpenAI.Chat;

using Polly;

using RestSharp;

using static OpenAI.Chat.ChatMessageContentPart;

var languageDetector = new LanguageDetector();
languageDetector.AddLanguages ("ru", "en");

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var builder = CoconaApp.CreateBuilder();
var app = builder.Build();
app.AddCommand
    (
        ([Argument (Description = "Папка с изображениями")] string folder,
         [Argument (Description = "Префикс, добавляемый к имени файла")] string prefix,
         [Argument (Description = "Имя результирующего файла")] string output) =>
        {
            var apiKey = configuration["apiKey"]!;
            var credentials = new ApiKeyCredential (apiKey);

            PrintBalance (apiKey);

            var endpoint = configuration["endpoint"]!;
            var clientOptions = new OpenAIClientOptions
            {
                Endpoint = new Uri (endpoint)
            };

            var api = new OpenAIClient (credentials, clientOptions);
            var model = configuration["model"]!;
            var chat = api.GetChatClient (model);

            var files = ScanFolder (folder);
            using var writer = File.CreateText (output);
            ProcessFiles (chat, writer, folder, prefix, files);
            Console.WriteLine();
            Console.WriteLine ("ALL DONE");
            PrintBalance (apiKey);
        }
    );
app.Run();

void PrintBalance
    (
        string apiKey
    )
{
    var url = configuration["balance"];
    if (!string.IsNullOrEmpty (url))
    {
        try
        {
            var client = new RestClient();
            var request = new RestRequest (url);
            request.AddHeader ("Authorization", "Bearer " + apiKey);
            var response = client.Get (request);
            var content = response.Content;
            if (response.IsSuccessful && !string.IsNullOrEmpty (content))
            {
                var json = JsonDocument.Parse (content);
                if (json.RootElement.TryGetProperty ("balance", out var balance))
                {
                    Console.WriteLine();
                    Console.WriteLine ($"Balance: {balance}");
                    Console.WriteLine();
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine ($"Can't get balance: {ex.Message}");
        }
    }
}

void ProcessFiles
    (
        ChatClient chat,
        TextWriter writer,
        string folder,
        string prefix,
        string[] files
    )
{
    var policy = Policy.Handle<Exception>()
        .WaitAndRetry (10, retryAttempt
                => TimeSpan.FromSeconds (Math.Pow (2, retryAttempt)),
                onRetry: (exception, timeSpan) =>
                    Console.WriteLine ($"Ошибка: {exception.Message}, ждем {timeSpan.Seconds} сек."));

    foreach (var file in files)
    {
        policy.Execute
            (
                () => ProcessPicture (chat, writer, folder, prefix, file)
            );
    }
}

string GetMimeType (string fileName)
{
    var extension = Path.GetExtension (fileName).ToLowerInvariant();
    return extension switch
    {
        ".bmp" => "image/bmp",
        ".gif" => "image/gif",
        ".jpg" or ".jpeg" or ".jfif" => "image/jpeg",
        ".png" => "image/png",
        ".tif" or ".tiff" => "image/tiff",
        ".webp" => "image/webp",
        _ => throw new InvalidDataException()
    };
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

            extension= extension.ToLowerInvariant();
            return extension is ".jpg" or ".jpeg" or ".jfif" or ".bmp"
                or ".gif" or ".png" or ".tif" or ".tiff" or ".webp";
        })
        .ToArray();

    return result;
}

void ProcessPicture
    (
        ChatClient chat,
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
    Console.WriteLine(shortName);

    // загружаем картинку
    var bytes = File.ReadAllBytes (file);
    var binaryData = BinaryData.FromBytes (bytes);
    var level = ChatImageDetailLevel.Low;
    var mimeType = GetMimeType (file);

    var prompt = configuration["prompt"]!;
    var userMessage = new UserChatMessage
        (
            CreateTextPart (prompt),
            CreateImagePart (binaryData, mimeType, level)
        );

    var messages = new List<ChatMessage> { userMessage };
    var invariant = CultureInfo.InvariantCulture;
    var temperature = float.Parse (configuration["temperature"]!, NumberStyles.Float, invariant);
    var maxTokens = int.Parse (configuration["maxTokens"]!, NumberStyles.Integer, invariant);
    var chatOptions = new ChatCompletionOptions
    {
        Temperature = temperature,
        MaxOutputTokenCount = maxTokens
    };
    var stopWatch = Stopwatch.StartNew();
    var response = chat.CompleteChat (messages, chatOptions);
    var elapsed = stopWatch.Elapsed;
    var completion = response.Value;
    var usage = completion.Usage;
    var log = $"finish={completion.FinishReason}, prompt={usage.InputTokenCount}, completion={usage.OutputTokenCount}, elapsed={elapsed}";
    if (!string.IsNullOrEmpty (completion.Refusal))
    {
        Console.WriteLine (log);
        Console.WriteLine (completion.Refusal);
        Console.WriteLine();
    }
    else
    {
        writer.WriteLine (shortName);
        writer.WriteLine (log);
        Console.WriteLine (log);

        var text = completion.Content[0].Text;
        Console.WriteLine (text);
        var parts = SplitText (text);
        if (parts.Length > 0)
        {
            writer.WriteLine (parts[0]);
            writer.WriteLine (parts[1]);
            writer.WriteLine();
        }
    }

    writer.Flush();
    Console.WriteLine();
}

string[] SplitText
    (
        string message
    )
{
    var lines = message.Split ('\n')
        .Select (l => l.Trim())
        .Where (l => l.Length > 0)
        .Where (l => l != "Sure!" && l != "Certainly!" && l != "Конечно!")
        .ToArray();

    var russianLines = new List<string>();
    var englishLines = new List<string>();

    foreach (var line in lines)
    {
        var language = languageDetector.Detect (line);
        switch (language)
        {
            case "ru":
                russianLines.Add (line);
                break;

            case "en":
                englishLines.Add (line);
                break;
        }
    }

    var russian = russianLines.MaxBy (one => one.Length) ?? string.Empty;
    var english = englishLines.MaxBy (one => one.Length) ?? string.Empty;

    return [russian, english];
}
