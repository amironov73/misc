// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable CommentTypo
// ReSharper disable CoVariantArrayConversion
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable LocalizableElement
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable StringLiteralTypo

/* BookInfo.cs -- информация об инвентарной книге
 * Ars Magna project, http://arsmagna.ru
 */

namespace FastInventoryBook;

/// <summary>
/// Информация об инвентарной книге.
/// </summary>
internal sealed record BookInfo (int From, int To, string FileName)
{
    /// <inheritdoc cref="object.ToString"/>
    public override string ToString() => FileName;
}
