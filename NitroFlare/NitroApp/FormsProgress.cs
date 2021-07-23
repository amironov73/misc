// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable LocalizableElement
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

#region Using directives

using System;
using System.Windows.Forms;

using NitroFlare;

#endregion

#nullable enable

namespace NitroApp
{
    /// <summary>
    /// Индикация прогресса для WinForms
    /// </summary>
    public sealed class FormsProgress
        : IDownloadProgress
    {
        #region Properties

        /// <summary>
        /// Форма.
        /// </summary>
        public Form? Form { get; set; }

        /// <summary>
        /// Индикатор прогресса.
        /// </summary>
        public ProgressBar? ProgressBar { get; set; }

        /// <summary>
        /// Метка для вывода общей информации.
        /// </summary>
        public Label? InformationLabel { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public FormsProgress ()
        {
            FileName = "unknown";

        } // constructor

        #endregion

        #region Private members

        private bool _active;
        private TimeSpan _elapsed;

        private void _UpdateProgress()
        {
            if (ProgressBar is not null)
            {
                ProgressBar.Minimum = 0;
                ProgressBar.Maximum = 100;
                ProgressBar.Value = (int)(DownloadSize * 100.0 / TotalSize);
            }

            if (InformationLabel is not null)
            {
                var elapsed = Elapsed;
                var elapsedText = $"{elapsed.Minutes:00}:{elapsed.Seconds:00}.{elapsed.Milliseconds}";
                InformationLabel.Text = $"{DownloadSize} of {TotalSize}  {elapsedText} {(Speed / 1024.0):F0} Kb/s ";
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Обновление информации на контролах.
        /// </summary>
        public void UpdateProgress()
        {
            if (Form is not null)
            {
                Form.Invoke((MethodInvoker) _UpdateProgress);
            }
        } // method UpdateProgress

        #endregion

        #region IDownloadProgress members

        /// <inheritdoc cref="IDownloadProgress.StartTime"/>
        public DateTime StartTime { get; private set; }

        /// <inheritdoc cref="IDownloadProgress.Elapsed"/>
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

        /// <inheritdoc cref="IDownloadProgress.FileName"/>
        public string FileName { get; private set; }

        /// <inheritdoc cref="IDownloadProgress.TotalSize"/>
        public long TotalSize { get; private set; }

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
            _active = true;
            StartTime = DateTime.Now;
            FileName =fileName;
            TotalSize = totalSize;
            DownloadSize = 0;
            UpdateProgress();

        } // method Init

        /// <inheritdoc cref="IDownloadProgress.Report"/>
        public void Report
            (
                long download
            )
        {
            DownloadSize = download;
            UpdateProgress();

        } // method Report

        /// <inheritdoc cref="IDownloadProgress.Done"/>
        public void Done(bool success)
        {
            _active = false;
        }

        #endregion
    }
}
