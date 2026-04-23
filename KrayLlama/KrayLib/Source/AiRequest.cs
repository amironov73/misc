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
    [JsonIgnore]
    public string? ApiKey { get; set; }

    [JsonPropertyName ("model")]
    public string? Model { get; set; }

    [JsonPropertyName ("messages")]
    public List<ChatMessage> Messages { get; set; } = new ();

    [JsonPropertyName ("stream")]
    public bool Stream { get; set; }
}
