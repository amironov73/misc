// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable CommentTypo

/* PlainTextMessage.cs - чисто текстовое сообщение в чате с моделью
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System.Text.Json.Serialization;

#endregion

namespace KrayLib;

/// <summary>
/// Чисто текстовое сообщение в чате с моделью.
/// </summary>
public sealed class PlainTextMessage
    : ChatMessage
{
    [JsonPropertyName ("role")]
    public string? Role { get; set; }

    [JsonPropertyName ("content")]
    public string? Content { get; set; }
}
