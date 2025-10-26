// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable CommentTypo

/* TokenInfo.cs -- информация о токене
 * Ars Magna project
 */

#region Using directives

using System.Text.Json.Serialization;

#endregion

namespace KrayLib;

/// <summary>
/// Информация о токене.
/// </summary>
public sealed class TokenInfo
{
    #region Properties

    /// <summary>
    /// Токен доступа.
    /// </summary>
    [JsonPropertyName ("access_token")]
    public string? AccessToken { get; set; }

    /// <summary>
    /// Дата истечения срока действия.
    /// </summary>
    [JsonPropertyName ("expires_at")]
    public long ExpiresAt { get; set; }

    #endregion
}
