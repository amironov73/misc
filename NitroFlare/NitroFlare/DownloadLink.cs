// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

/* DonwloadLink.cs -- информация о скачиваемом файле
 */

#nullable enable

namespace NitroFlare
{
    /// <summary>
    /// Информация о скачиваемом файле.
    /// </summary>
    public sealed class DownloadLink
    {
        #region Properties

        /// <summary>
        /// URL, по которому возможно прямое скачивание.
        /// </summary>
        public string? Url { get; set; }
        
        /// <summary>
        /// Имя файла. 
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// Размер файла.
        /// </summary>
        public long Size { get; set; }
        
        /// <summary>
        /// Тип ссылки, как правило, "premium".
        /// </summary>
        public string? LinkType { get; set; }

        #endregion
        
    } // class DownloadLink
    
} // namespace NitroFlare
