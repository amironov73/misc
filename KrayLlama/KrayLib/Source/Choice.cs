// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable CommentTypo

/* Choice.cs - вариант ответа
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System.Text.Json.Serialization;

#endregion

namespace KrayLib;

/// <summary>
/// Вариант ответа.
/// </summary>
public sealed class Choice
{
    #region Properties

    /// <summary>
    /// Порядковый номер варианта.
    /// </summary>
    [JsonPropertyName ("index")]
    public int? Index { get; set; }

    /// <summary>
    /// Сообщение.
    /// </summary>
    [JsonPropertyName ("message")]
    public ChatMessage? Message { get; set; }

    /// <summary>
    /// Причина завершения.
    /// </summary>
    [JsonPropertyName ("finish_reason")]
    public string? FinishReason { get; set; }

    #endregion
}
