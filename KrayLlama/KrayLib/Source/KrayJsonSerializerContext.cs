// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable CommentTypo

/* KrayJsonSerializerContext.cs - для Native AOT
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

#endregion

namespace KrayLib;

/// <summary>
/// Для Native AOT.
/// </summary>
[JsonSerializable (typeof (AiRequest))]
[JsonSerializable (typeof (AiResponse))]
[JsonSerializable (typeof (ChatMessage))]
[JsonSerializable (typeof (Choice))]
[JsonSerializable (typeof (OllamaOptions))]
[JsonSerializable (typeof (Usage))]
public sealed partial /* нельзя убирать partial! */
    class KrayJsonSerializerContext
    : JsonSerializerContext
{
    // пустое тело класса
}
