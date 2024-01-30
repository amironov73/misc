// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable LocalizableElement
// ReSharper disable StringLiteralTypo

/* MainWindow.cs -- главное окно приложения
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

#endregion

namespace FastInventoryBook;

/// <summary>
/// Главное окно приложения.
/// </summary>
public sealed class MainWindow
    : Window
{
    #region Window members

    /// <summary>
    /// Вызывается, когда окно проинициализировано фреймворком.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        Title = $"Инвентарные книги on runtime {Environment.Version}";
        Width = MinWidth = 800;
        Height = MinHeight = 600;
        // this.SetWindowIcon ("bookshelf.ico");

        if (!LoadBooks())
        {
            return;
        }

        var topPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Spacing = 10,

            Children =
            {
                new Label
                {
                    Content = "Введите инвентарный номер"
                },

                _inventoryBox,
                _searchButton,
                _openButton,
                _autoOpenBox
            }
        }
        .DockTop();

        _bookList.SelectionChanged += SelectionChanged;
        _searchButton.Click += DoSearch;
        _openButton.Click += DoOpen;
        _bookList.Items = _bookInfos;
        _bookList.DoubleTapped += (_, _) => DoOpen (_bookList, new RoutedEventArgs());

        Content = new DockPanel
            {
                Margin = new Thickness (10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Children =
                {
                    topPanel,
                    _bookList
                }
            };

        _inventoryBox.Focus();
    }

    #endregion

    #region Private members

    private readonly Button _searchButton = new ()
    {
        Content = "Поиск",
        IsDefault = true
    };

    private readonly Button _openButton = new ()
    {
        Content = "Открыть",
        IsEnabled = false
    };

    private readonly List<BookInfo> _bookInfos = new ();

    private readonly TextBox _inventoryBox = new ()
    {
        Width = 200
    };

    private readonly ListBox _bookList = new ()
    {
        HorizontalAlignment = HorizontalAlignment.Stretch,
        VerticalAlignment = VerticalAlignment.Stretch
    };

    private readonly CheckBox _autoOpenBox = new ()
    {
        Content = "Открывать автоматически",
        IsChecked = true
    };

    private void DoSearch
        (
            object? sender,
            RoutedEventArgs eventArgs
        )
    {
        _openButton.IsEnabled = false;
        _bookList.SelectedItem = null;

        var numberText = _inventoryBox.Text?.Trim();
        if (string.IsNullOrEmpty (numberText))
        {
            return;
        }

        var number = numberText.SafeToInt32();
        var found = _bookInfos.FirstOrDefault
            (
                info => info.From <= number && info.To >= number
            );
        if (found is not null)
        {
            _bookList.SelectedItem = found;
            _inventoryBox.Text = null;
            if (_autoOpenBox.IsChecked!.Value)
            {
                DoOpen (_openButton, new RoutedEventArgs());
            }
        }
    }

    private void DoOpen
        (
            object? sender,
            RoutedEventArgs eventArgs
        )
    {
        if (_bookList.SelectedItem is BookInfo selected)
        {
            var bookDirectory = Program.Configuration["book-directory"]
                .ThrowIfNull();
            var fullPath = Path.Combine
                (
                    bookDirectory,
                    selected.FileName
                );
            var startInfo = new ProcessStartInfo
            {
                FileName = fullPath,
                UseShellExecute = true
            };
            try
            {
                using var process = Process.Start (startInfo);
            }
            catch (Exception exception)
            {
                ShowError (exception.Message);
            }
        }
    }

    private bool LoadBooks()
    {
        try
        {
            var lines = File.ReadAllLines ("books.txt");
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty (trimmed))
                {
                    continue;
                }

                var parts = trimmed.Split ('\t');
                if (parts.Length != 3)
                {
                    continue;
                }

                var info = new BookInfo
                    (
                        From: parts[0].SafeToInt32(),
                        To: parts[1].SafeToInt32(),
                        FileName: parts[2]
                    );
                _bookInfos.Add (info);
            }

            _bookInfos.Sort
                (
                    (left, right) => left.From - right.From
                );

        }
        catch (Exception exception)
        {
            ShowError (exception.Message);
            return false;
        }

        return true;
    }

    private void SelectionChanged
        (
            object? sender,
            SelectionChangedEventArgs eventArgs
        )
    {
        _openButton.IsEnabled = _bookList.SelectedItem is BookInfo;
    }

    private void ShowError
        (
            string message
        )
    {
        var closeButton = new Button
        {
            Content = "Закрыть приложение",
            HorizontalAlignment = HorizontalAlignment.Center
        };
        closeButton.Click += (_, _) => Close();

        Content = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Spacing = 10,

            Children =
            {
                new Label
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Foreground = Brushes.Red,
                    Content = message
                },

                closeButton
            }
        };
    }

    #endregion
}
