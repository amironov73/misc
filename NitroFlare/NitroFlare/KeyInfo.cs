// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CommentTypo
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

/* KeyInfo.cs -- информация о пользовательском доступе
 */

#region Using directives

using System;

#endregion

#nullable enable

namespace NitroFlare
{
    /// <summary>
    /// Информация о пользовательском доступе.
    /// </summary>
    public sealed class KeyInfo
    {
        #region Properties

        /// <summary>
        /// Статус "премиум"
        /// </summary>
        public string? Status { get; set; }
        
        /// <summary>
        /// Дата покупки премиум.
        /// </summary>
        public DateTime BuyDate { get; set; }
        
        /// <summary>
        /// Дата истечения премиум.
        /// </summary>
        public DateTime ExpiryDate { get; set; }
        
        /// <summary>
        /// Использованный объем.
        /// </summary>
        public long StorageUsed { get; set; }

        /// <summary>
        /// Оставшееся количество трафика (текущие сутки).
        /// </summary>
        public long TrafficLeft { get; set; }
        
        /// <summary>
        /// Максимальное количество трафика (текущие сутки).
        /// </summary>
        public long TrafficMax { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Активен ли статус "премиум".
        /// </summary>
        public bool IsActive() => string.Compare(Status, "active", StringComparison.OrdinalIgnoreCase) == 0;

        #endregion

    } // class KeyInfo
    
} // namespace NitroFlare
