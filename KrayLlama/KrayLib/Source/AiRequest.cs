// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable CommentTypo

/* AiRequest.cs - запрос к модели
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System.Text.Json.Serialization;

#endregion

namespace KrayLib;

/// <summary>
/// Запрос к модели.
/// </summary>
public sealed class AiRequest
{
    /// <summary>
    /// Ключ API.
    /// </summary>
    [JsonIgnore]
    public string? ApiKey { get; set; }

    [JsonPropertyName ("model")]
    public string? Model { get; set; }

    [JsonPropertyName ("messages")]
    public List<ChatMessage> Messages { get; set; } = new ();

    /// <summary>
    /// Максимальное количество токенов, которое модель может
    /// сгенерировать в ответе.
    /// </summary>
    [JsonPropertyName ("max_tokens")]
    [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxOutputTokens { get; set; }

    /// <summary>
    /// Температура. Степень случайности ответа (от 0 до 2).
    /// Чем выше значение, тем «творческим» и непредсказуемым
    /// будет ответ. При 0 модель становится максимально
    /// детерминированной.
    /// </summary>
    [JsonPropertyName ("temperature")]
    [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? Temperature { get; set; }

    /// <summary>
    /// льтернатива температуре (ядерное сэмплирование). Модель
    /// рассматривает только те токены, общая вероятность
    /// которых составляет p (или выше).
    /// Опционально.
    /// </summary>
    public float? TopP { get; set; }

    /// <summary>
    /// Если указано, система постарается генерировать детерминированные
    /// результаты (одинаковые ответы на одинаковые запросы).
    /// Опционально.
    /// </summary>
    public long? Seed { get; set; }

    [JsonPropertyName ("stream")]
    public bool Stream { get; set; }

    /// <summary>
    /// When true, returns separate thinking output in addition to content.
    /// Can be a boolean (true/false) or a string ("high", "medium", "low")
    /// for supported models.
    /// </summary>
    [JsonPropertyName ("think")]
    [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Think { get; set; }

    /// <summary>
    /// Model keep-alive duration (for example 5m or 0 to unload immediately).
    /// </summary>
    [JsonPropertyName ("keep_alive")]
    public string? KeepAlive { get; set; }

    /// <summary>
    /// Опции, специфичные для Ollama.
    /// </summary>
    [JsonPropertyName ("options")]
    public OllamaOptions? Options { get; set; }
}
