// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace

#region Using directives

using System.Diagnostics;
using System.Text.Json;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using RestSharp;

#endregion

namespace KrayLib;

/// <summary>
/// Умеет разговаривать с ИИ.
/// </summary>
/// <param name="logger">Логгер.</param>
/// <param name="configuration">Конфигурация.</param>
public sealed partial class LlamaTalker
    (
        ILogger logger,
        IConfiguration configuration
    )
{
    #region Public methods

    /// <summary>
    /// Вызов ИИ.
    /// </summary>
    /// <param name="input">Пакет входящих данных.</param>
    /// <returns></returns>
    public OutputPackage Call
        (
            InputPackage input
        )
    {
        var section = configuration.GetSection ("Llm");

        var endpoint = input.Endpoint;
        if (string.IsNullOrEmpty (endpoint))
        {
            endpoint = section["Endpoint"];
            if (string.IsNullOrEmpty (endpoint))
            {
                endpoint = "http://localhost:11434/v1/";
            }
        }

        var modelId = input.ModelId;
        if (string.IsNullOrEmpty (modelId))
        {
            modelId = section["Model"];
            if (string.IsNullOrEmpty (modelId))
            {
                modelId = "gemma3:4b";
            }
        }

        var apiKey = input.ApiKey;
        if (string.IsNullOrEmpty (apiKey))
        {
            apiKey = section["ApiKey"];
            if (string.IsNullOrEmpty (apiKey))
            {
                apiKey = "ollama";
            }
        }

        var imagePath = input.Images?[0];
        byte[]? imageBytes = null;
        if (!string.IsNullOrEmpty (imagePath))
        {
            imagePath = imagePath.PathToUnix();
            imageBytes = File.ReadAllBytes (imagePath);
        }

        var prompt = string
            .Join (Environment.NewLine, input.Prompt)
            .Trim();

        LogCallingLlm (logger, input.Id);
        var moment = Stopwatch.GetTimestamp();
        var slapdash = new SlapdashClient (endpoint);
        var options = new OllamaOptions
        {
            ContextWindow = input.ContextWindow,
            TopK = input.TopK
            // TODO: другие параметры
        };
        var request = new AiRequest
        {
            ApiKey = apiKey,
            Model = modelId,
            Temperature = input.Temperature,
            TopP = input.TopP,
            Seed = input.Seed,
            MaxOutputTokens =  input.MaxOutputTokens,
            Options = options
        };

        var message = imageBytes is null
            ? ChatMessage.BuildTextMessage ("user", prompt)
            : ChatMessage.BuildTextAndImage
                (
                    "user",
                    prompt,
                    "data:image/jpeg;base64," + Convert.ToBase64String (imageBytes)
                );

        request.Messages.Add (message);
        var response = slapdash.Execute (request);
        var elapsed = Stopwatch.GetElapsedTime (moment);

        var output = new OutputPackage
        {
            Message = response?.Choices?[0].Message?.Content?.ToString(),
            // Refusal = response.Value.Refusal,
            FinishReason = response?.Choices?[0].FinishReason,
            Usage = response?.Usage,
            Duration = elapsed
        };

        LogElapsedTime (logger, output.Duration);
        LogFinishReason (logger, output.FinishReason);
        LogTokenUsage (logger, output.Usage);

        return output;
    }

    public string[] ListOllamaModels
        (
            InputPackage input
        )
    {
        var section = configuration.GetSection ("Llm");

        var endpoint = input.Endpoint;
        if (string.IsNullOrEmpty (endpoint))
        {
            endpoint = section["Endpoint"];
            if (string.IsNullOrEmpty (endpoint))
            {
                endpoint = "http://localhost:11434/api/";
            }
        }

        var uri = new Uri (endpoint);
        uri = new Uri
            (
                new Uri (uri.GetLeftPart (UriPartial.Authority)),
                "api"
            );

        var options = new RestClientOptions (uri);
        var client = new RestClient (options);
        var request = new RestRequest ("tags");
        var response = client.Get (request);
        if (!response.IsSuccessful)
        {
            return [];
        }

        var result = new List<string> ();
        var json = JsonDocument.Parse (response.Content!);
        var models = json.RootElement.GetProperty ("models");
        var length = models.GetArrayLength();
        for (var i = 0; i < length; i++)
        {
            var name = models[i].GetProperty ("name").GetString();
            result.Add (name!);
        }

        return result.ToArray();
    }

    public void WriteOutput
        (
            TextWriter writer,
            OutputPackage output,
            InputPackage input
        )
    {
        if (!string.IsNullOrEmpty (input.Id))
        {
            writer.WriteLine (input.Id);
        }

        var message = output.Message!;
        if (input.OutMode == 1)
        {
            message = output.FlattenedMessage()!;
        }

        writer.WriteLine (message);

        if (!string.IsNullOrEmpty (input.Id))
        {
            writer.WriteLine ("*****");
        }

        writer.Flush();
    }

    #endregion

    #region Logging

    [LoggerMessage (LogLevel.Debug, "Calling LLM, ID={Id}")]
    static partial void LogCallingLlm (ILogger logger, string? id);

    [LoggerMessage (LogLevel.Debug, "LLM call completed: {Elapsed}")]
    static partial void LogElapsedTime (ILogger logger, TimeSpan elapsed);

    [LoggerMessage (LogLevel.Debug, "Finish reason: {FinishReason}")]
    static partial void LogFinishReason (ILogger logger, string? finishReason);

    [LoggerMessage (LogLevel.Debug, "Token usage: {Usage}")]
    static partial void LogTokenUsage (ILogger logger, Usage? usage);

    // [LoggerMessage (LogLevel.Debug, "Acquiring GigaChat access token")]
    // static partial void LogAcquiringGigaChatAccessToken (ILogger logger);

    // [LoggerMessage (LogLevel.Debug, "GigaClient access token={Token}")]
    // static partial void LogGigaChatAccessTokenToken(ILogger logger, string? token);

    #endregion
}
