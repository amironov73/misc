// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

/* IDownloadProgress.cs -- интерфейс прогресса скачивания
 */

using System;

#nullable enable

namespace NitroFlare
{
    /// <summary>
    /// Интерфейс прогресса скачивания.
    /// </summary>
    public interface IDownloadProgress
    {
        /// <summary>
        /// Момент начала скачивания.
        /// </summary>
        DateTime StartTime { get; }
        
        /// <summary>
        /// Время, затраченное на скачивание.
        /// </summary>
        TimeSpan Elapsed { get; }
        
        /// <summary>
        /// Скорость скачивания, байты в секунду.
        /// </summary>
        double Speed { get; }
        
        /// <summary>
        /// Имя файла.
        /// </summary>
        string FileName { get; }
        
        /// <summary>
        /// Размер файла.
        /// </summary>
        long TotalSize { get; }
        
        /// <summary>
        /// Размер скачанного.
        /// </summary>
        long DownloadSize { get; }
        
        /// <summary>
        /// Пользователь прервал скачивание?
        /// </summary>
        bool Interrupted { get; }

        /// <summary>
        /// Вызывается перед началом скачивания.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <param name="totalSize">Размер файла.</param>
        void Init (string fileName, long totalSize);
        
        /// <summary>
        /// Обновление прогресса.
        /// </summary>
        /// <param name="download">Новый размер скачанного.</param>
        void Report (long download);

        /// <summary>
        /// Вызывается по окончании скачивания.
        /// </summary>
        /// <param name="success">Признак успешного завершения.</param>
        void Done (bool success);

    } // interface IDownloadProgress
    
} // namespace NitroFlare
