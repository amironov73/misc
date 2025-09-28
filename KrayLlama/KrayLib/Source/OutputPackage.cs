// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace

#region Using directives

using OpenAI.Chat;

#endregion

namespace KrayLib;

/// <summary>
/// Сведения о полученном ответе.
/// </summary>
public sealed class OutputPackage
{
    #region Properties

    public string? Message { get; set; }

    public string? Refusal { get; set; }

    public TimeSpan Duration { get; set; }

    public ChatFinishReason FinishReason { get; set; }

    public ChatTokenUsage? Usage { get; set; }

    #endregion

    #region Public methods

    /// <summary>
    /// Ответ нейросети в одну строчку.
    /// </summary>
    public string? FlattenedMessage() => Message?.ReplaceLineEndings (" ")
        .Replace ("  ", " ");

    /// <summary>
    /// Краткое изложение ответа нейросети.
    /// </summary>
    public string? GetExceprpt() => FlattenedMessage()?.Substring (0, 70);

    #endregion
}
