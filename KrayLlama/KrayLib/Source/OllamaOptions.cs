// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace

#region Using directives

using System.Text.Json.Serialization;

#endregion

namespace KrayLib;

/// <summary>
/// Опции Ollama.
/// </summary>
public sealed class OllamaOptions
{
    #region Properties

    /// <summary>
    /// Размер контекстного окна.
    /// </summary>
    [JsonPropertyName ("num_ctx")]
    [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ContextWindow { get; set; }

    /// <summary>
    /// Максимальное количество токенов для генерации
    /// (аналог max_tokens).
    /// </summary>
    [JsonPropertyName ("num_predict")]
    [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? NumPredictTokens { get; set; }

    /// <summary>
    /// Ограничивает выборку только k самыми вероятными токенами.
    /// </summary>
    [JsonPropertyName ("top_k")]
    [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? TopK { get; set; }

    /// <summary>
    /// Штраф за повторение слов (например, 1.1).
    /// </summary>
    [JsonPropertyName ("repeat_penalty")]
    [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? RepeatPenalty { get; set; }

    /// <summary>
    /// Список стоп-последовательностей (строк), при появлении
    /// которых модель прекратит генерацию..
    /// </summary>
    [JsonPropertyName ("stop")]
    [JsonIgnore (Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? Stop { get; set; }

    #endregion
}
