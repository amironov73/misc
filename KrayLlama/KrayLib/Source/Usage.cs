// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable CommentTypo

/* Usage.cs - использование токенов
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System.Text.Json.Serialization;

#endregion

namespace KrayLib;

/// <summary>
/// Использование токенов.
/// </summary>
public sealed class Usage
{
    #region Properties

    /// <summary>
    /// Количество токенов в промпте.
    /// </summary>
    [JsonPropertyName ("prompt_tokens")]
    public int? PromptTokens { get; set; }

    /// <summary>
    /// Количество токенов в ответе.
    /// </summary>
    [JsonPropertyName ("completion_tokens")]
    public int? CompletionTokens { get; set; }

    /// <summary>
    /// Всего токенов.
    /// </summary>
    [JsonPropertyName ("total_tokens")]
    public int? TotalTokens { get; set; }

    #endregion

    #region Object members

    /// <inheritdoc/>
    public override string ToString () =>
     $"prompt={PromptTokens}, completion={CompletionTokens}, total={TotalTokens}";

    #endregion
}
