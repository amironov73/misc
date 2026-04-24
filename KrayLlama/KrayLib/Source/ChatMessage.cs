// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable CommentTypo

/* ChatMessage.cs - абстрактное сообщение в чате с моделью
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

#endregion

namespace KrayLib;

/// <summary>
/// Сообщение в чате с моделью.
/// </summary>
public sealed class ChatMessage
{
    #region Properties

    /// <summary>
    /// Роль: "system", "user", "assistant".
    /// </summary>
    [JsonPropertyName ("role")]
    public string? Role { get; set; }

    [JsonPropertyName ("content")]
    public JsonNode? Content { get; set; }

    #endregion

    #region Public methods

    /// <summary>
    /// Построение сообщения, состоящего только из текста.
    /// </summary>
    public static ChatMessage BuildTextMessage
        (
            string role,
            string text
        )
    {
        var result = new ChatMessage
        {
            Role = role,
            Content = text
        };

        return result;
    }

    /// <summary>
    /// Построение сообщения, состоящего из текста и картинки.
    /// </summary>
    public static ChatMessage BuildTextAndImage
        (
            string role,
            string text,
            string imageUrl
        )
    {
        var parts = new JsonArray();
        var textPart = new JsonObject();
        textPart.Add ("type", "text");
        textPart.Add ("text", text);
        parts.Add ((JsonNode) textPart);
        var imagePart = new JsonObject();
        imagePart.Add ("type", "image_url");
        var urlObject = new JsonObject();
        urlObject.Add ("url", imageUrl);
        imagePart.Add ("image_url", urlObject);
        parts.Add ((JsonNode) imagePart);

        var result = new ChatMessage
        {
            Role = role,
            Content = parts
        };

        return result;
    }

    #endregion

    #region Object members

    /// <inheritdoc/>
    public override string ToString() => Content?.ToString() ?? "(none)";

    #endregion
}
