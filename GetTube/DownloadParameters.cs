// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

/* DownloadParameters.cs -- параметры загрузки
 * Ars Magna project, http://arsmagna.ru
 */

#nullable enable

namespace GetTube;

/// <summary>
/// Параметры загрузки.
/// </summary>
public sealed class DownloadParameters
{
    #region Properties

    /// <summary>
    /// Ссылка на видео, которое нужно скачать.
    /// </summary>
    public string? Url { get; set; }
    
    /// <summary>
    /// Имя файла, в который следует записать видео.
    /// </summary>
    public string? Output { get; set; }

    #endregion
}
