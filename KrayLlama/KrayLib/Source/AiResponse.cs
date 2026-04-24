// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable CommentTypo

/* AiResponse.cs - ответ модели
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System.Text.Json.Serialization;

#endregion

namespace KrayLib;

/// <summary>
/// Ответ модели.
/// </summary>
public sealed class AiResponse
{
    /// <summary>
    /// Модель.
    /// </summary>
    [JsonPropertyName ("model")]
    public string? Model { get; set; }

    /// <summary>
    /// Варианты ответов.
    /// </summary>
    [JsonPropertyName ("choices")]
    public Choice[]? Choices { get; set; }

    /// <summary>
    /// Использование токенов.
    /// </summary>
    [JsonPropertyName ("usage")]
    public Usage? Usage { get; set; }
}
