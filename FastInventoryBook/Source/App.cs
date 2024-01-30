// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable CommentTypo
// ReSharper disable CoVariantArrayConversion
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable LocalizableElement
// ReSharper disable StringLiteralTypo

/* App.cs -- класс приложения
 * Ars Magna project, http://arsmagna.ru
 */

#region Using directives

using System;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ThemeManager;
using Avalonia.Themes.Fluent;

#endregion

namespace FastInventoryBook;

/// <summary>
/// Класс приложения.
/// </summary>
public sealed class App
    : Application
{
    // public static IThemeManager? ThemeManager;

    /// <inheritdoc cref="Application.Initialize"/>
    public override void Initialize()
    {
        // ThemeManager = new FluentThemeManager();
        // ThemeManager.Initialize (this);
        var uri = new Uri ("avares://Avalonia.Themes.Fluent/FluentLight.xaml");
        var theme = new FluentTheme (uri);
        Styles.Add (theme);
    }

    /// <inheritdoc cref="Application.OnFrameworkInitializationCompleted"/>
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
