// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CommentTypo
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

/* FileInfo.cs -- информация о файле, размещенном на хостинге
 */

#region Using directives

using System;

#endregion

#nullable enable

namespace NitroFlare
{
    /// <summary>
    /// Информация о файле, размещенном на хостинге.
    /// </summary>
    public sealed class FileInfo
    {
        #region Properties

        /// <summary>
        /// Статус: офлайн/онлайн.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Имя файла.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Размер файла.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Дата загрузки на хостинг.
        /// </summary>
        public DateTime UploadDate { get; set; }

        /// <summary>
        /// URL.
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Доступен только с премиумом.
        /// </summary>
        public bool PremiumOnly { get; set; }

        #endregion
        
    } // class FileInfo
    
} // namespace NitroFlare
