// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

/* ConsoleProgress.cs -- прогресс скачивания, отображаемый в консоли
 */

#region Using directives

using System;

#endregion

#nullable enable

namespace NitroFlare
{
    /// <summary>
    /// Прогресс скачивания, отображаемый в консоли.
    /// </summary>
    public sealed class ConsoleProgress
        : IDownloadProgress
    {
        #region Construction

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public ConsoleProgress()
        {
            FileName = "unknown";
            
        } // constructor

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <param name="totalSize">Размер файла в байтах.</param>
        public ConsoleProgress
            (
                string fileName, 
                long totalSize
            )
        {
            FileName = fileName;
            TotalSize = totalSize;
            StartTime = DateTime.Now;
            _active = true;

        } // constructor

        #endregion

        #region Private members

        private bool _active;
        private TimeSpan _elapsed;

        #endregion

        #region IDownloadProgress members

        /// <inheritdoc cref="IDownloadProgress.StartTime"/>
        public DateTime StartTime { get; private set; }

        /// <inheritdoc cref="Elapsed"/>
        public TimeSpan Elapsed
        {
            get
            {
                if (!_active)
                {
                    return _elapsed;
                }

                return _elapsed = DateTime.Now - StartTime;
            }
            
        } // property Elapsed

        /// <inheritdoc cref="IDownloadProgress.FileName"/>
        public string FileName { get; private set; }
        
        /// <inheritdoc cref="IDownloadProgress.TotalSize"/>
        public long TotalSize { get; private set; }

        /// <inheritdoc cref="IDownloadProgress.Speed"/>
        public double Speed
        {
            get
            {
                var download = DownloadSize;
                if (download < 1024)
                {
                    return 0;
                }

                var elapsed = Elapsed.TotalSeconds;
                if (elapsed < 0.1)
                {
                    return 0;
                }

                return download / elapsed;
            }
            
        } // property Speed

        /// <inheritdoc cref="IDownloadProgress.DownloadSize"/>
        public long DownloadSize { get; private set; }

        /// <inheritdoc cref="IDownloadProgress.Interrupted"/>
        public bool Interrupted => false;

        /// <inheritdoc cref="IDownloadProgress.Init"/>
        public void Init
            (
                string fileName, 
                long totalSize
            )
        {
            StartTime = DateTime.Now;
            FileName = fileName;
            TotalSize = totalSize;
            DownloadSize = 0;
            _active = true;

        } // method Init

        /// <inheritdoc cref="IDownloadProgress.Report"/>
        public void Report
            (
                long download
            )
        {
            DownloadSize = download;
            var percent = (double)download / TotalSize;
            var elapsed = Elapsed;
            var elapsedText = $"{elapsed.Minutes:00}:{elapsed.Seconds:00}.{elapsed.Milliseconds}";
            Console.Write($"\r{FileName}: {download} of {TotalSize} ({percent:P}) {elapsedText} {(Speed / 1024.0):F0} Kb/s ");
            
        } // method Report

        /// <inheritdoc cref="IDownloadProgress.Done"/>
        public void Done
            (
                bool success
            )
        {
            _active = false;
            _elapsed = DateTime.Now - StartTime;
            Console.Write ("   ");
            Console.WriteLine(success ? "SUCCESS" : "FAILURE");
            
        } // method Done

        #endregion
        
    } // class ConsoleProgress
    
} // namespace NitroFlare
