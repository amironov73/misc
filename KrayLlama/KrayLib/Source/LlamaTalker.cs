// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace

#region Using directives

using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using OpenAI.Chat;

using Polly;

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
        IConfiguration configuration,
        ResiliencePipeline resilience
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

        if (string.IsNullOrEmpty (input.GigaClientId))
        {
            input.GigaClientId = section["GigaClientId"];
        }

        if (string.IsNullOrEmpty (input.GigaClientSecret))
        {
            input.GigaClientSecret = section["GigaClientSecret"];
        }

        // включен ли обход сертификатов Минцифры
        var insecure = section["Insecure"] == "True";

        if (!string.IsNullOrEmpty (input.GigaClientId)
            && !string.IsNullOrEmpty (input.GigaClientSecret))
        {
            LogAcquiringGigaChatAccessToken (logger);

            var gigaClient = new GigaClient (input.GigaClientId, input.GigaClientSecret, insecure);
            apiKey = gigaClient.AcquireAccessToken().GetAwaiter().GetResult();
            LogGigaChatAccessTokenToken (logger, apiKey);
            if (string.IsNullOrEmpty (apiKey))
            {
                throw new ApplicationException ("Could not acquire GigaChat access token");
            }
        }

        var uri = new Uri (endpoint);
        var credential = new ApiKeyCredential (apiKey);
        var clientOptions = new OpenAI.OpenAIClientOptions
        {
            Endpoint = uri,
            // NetworkTimeout = TimeSpan.FromMinutes (10) // TODO: parametrize
        };

        if (insecure)
        {
            // для обхода сертификатов Минцифры просто отключаем проверку

            // Создаём HttpClientHandler с отключённой проверкой SSL
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            // Создаём HttpClient с кастомным обработчиком
            var httpClient = new HttpClient (handler);

            clientOptions.Transport = new HttpClientPipelineTransport (httpClient);
        }

        var imagePath = input.Images?[0];
        byte[]? imageBytes = null;
        if (!string.IsNullOrEmpty (imagePath))
        {
            imagePath = imagePath.PathToUnix();
            imageBytes = File.ReadAllBytes (imagePath);
        }

        var client = new OpenAI.OpenAIClient (credential, clientOptions);
        var chat = client.GetChatClient (modelId);

        var prompt = string
            .Join (Environment.NewLine, input.Prompt)
            .Trim();

        var parts = new List<ChatMessageContentPart>
        {
            ChatMessageContentPart.CreateTextPart (prompt)
        };

        if (imageBytes is not null)
        {
            var imagePart = ChatMessageContentPart.CreateImagePart
                (
                    new BinaryData (imageBytes),
                    System.Net.Mime.MediaTypeNames.Image.Jpeg,
                    input.DetailLevel
                );
            parts.Add (imagePart);
        }

        // var userMessage = ChatMessage.CreateUserMessage (parts);
        // var conversation = new ChatMessage[]
        // {
        //     userMessage
        // };

        // var completionOptions = new ChatCompletionOptions
        // {
        //     StoredOutputEnabled = false,
        //     TopP = input.TopP,
        //     Temperature = input.Temperature,
        //     MaxOutputTokenCount = input.MaxOutputTokens,
        // };

        LogCallingLlm (logger, input.Id);
        var moment = Stopwatch.GetTimestamp();

#if NOTDEF

        var state = (conversation, completionOptions);
        var response = resilience.Execute
            (
                the => chat.CompleteChat
                    (
                        the.conversation,
                        the.completionOptions
                    ),
                state
            );

        // var response = chat.CompleteChat
        //     (
        //         conversation,
        //         completionOptions
        //     );

        var elapsed = Stopwatch.GetElapsedTime (moment);
        var finishReason = response.Value.FinishReason;
        var usage = response.Value.Usage;
        LogElapsedTime (logger, elapsed);
        LogFinishReason (logger, finishReason);
        LogTokenUsage
            (
                logger,
                usage.InputTokenCount,
                usage.OutputTokenCount,
                usage.TotalTokenCount
            );

        var output = new OutputPackage
        {
            Message = response.Value.Content[0].Text,
            Refusal = response.Value.Refusal,
            FinishReason = finishReason,
            Usage = usage,
            Duration = elapsed,
        };

#else

        var slapdash = new SlapdashClient (endpoint);
        var request = new AiRequest
        {
            ApiKey = apiKey,
            Model = modelId
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

#endif

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
    static partial void LogFinishReason (ILogger logger, ChatFinishReason finishReason);

    [LoggerMessage (LogLevel.Debug, "Token usage: {Input}, {Output}, {Total}")]
    static partial void LogTokenUsage (ILogger logger, int input, int output, int total);

    [LoggerMessage (LogLevel.Debug, "Acquiring GigaChat access token")]
    static partial void LogAcquiringGigaChatAccessToken (ILogger logger);

    [LoggerMessage (LogLevel.Debug, "GigaClient access token={Token}")]
    static partial void LogGigaChatAccessTokenToken(ILogger logger, string? token);

    #endregion
}
