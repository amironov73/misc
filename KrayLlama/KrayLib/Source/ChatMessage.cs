// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable CommentTypo

/* ChatMessage.cs - сообщение в чате с моделью
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
/// Сообщение в чате с моделью.
/// </summary>
[JsonDerivedType (typeof (PlainTextMessage))]
public abstract class ChatMessage
{
    // пустое тело класса
}
