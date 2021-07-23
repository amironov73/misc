// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CommentTypo
// ReSharper disable LocalizableElement
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NitroFlare;

#endregion

#nullable enable

namespace NitroApp
{
    public partial class MainForm 
        : Form
    {
        public MainForm()
        {
            InitializeComponent();

            _client = NitroClient.FromJson();
        }

        private readonly NitroClient _client;

        private void _Download(string url)
        {
            var progress = new FormsProgress()
            {
                Form = this,
                InformationLabel = _infoLabel,
                ProgressBar = _progressBar
            };

            _client.DownloadFile(url, progress);
        }

        public void WriteLine(string line)
        {
            _listBox.Items.Insert(0, line);
        }

        public void ShowAccountInformation()
        {
            try
            {
                var key = _client.GetKeyInfo();
                if (key is null)
                {
                    WriteLine("Can't get account info");
                }
                else if (!key.IsActive())
                {
                    WriteLine("Premium not active");
                }
                else
                {
                    _remainingLabel.Text = $"{(key.TrafficLeft / 1024.0 / 1024.0):F} Mb";
                    WriteLine($"Expiry date: {key.ExpiryDate:d}");
                }
            }
            catch (Exception exception)
            {
                WriteLine($"ERROR: {exception.Message}");
            }
        }

        public void Download
            (
                string url
            )
        {
            Task.Run(() => _Download(url));
        }

        private void _startupTimer_Tick
            (
                object sender, 
                EventArgs e
            )
        {
            _startupTimer.Enabled = false;
            ShowAccountInformation();
        }

        private void MainForm_DragEnter
            (
                object sender, 
                DragEventArgs e
            )
        {
            e.Effect = e.Data.GetDataPresent(typeof(string)) 
                ? DragDropEffects.Link 
                : DragDropEffects.None;
        }

        private void MainForm_DragDrop
            (
                object sender, 
                DragEventArgs e
            )
        {
            var url = (string?) e.Data.GetData(typeof(string));
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            WriteLine(url);
            var info = _client.GetFileInfo(url);
            if (info is not null)
            {
                Download(url);
            }
        }
    }
}
