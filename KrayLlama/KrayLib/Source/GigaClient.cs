// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable CommentTypo

/* GigaClient.cs - клиент для GigaChat
 * Ars Magna project
 */

#region Using directives

using System.Text;
using System.Text.Json.Nodes;

using RestSharp;

#endregion

namespace KrayLib;

/// <summary>
/// Клиент для GigaChat.
/// </summary>
public sealed class GigaClient
{
    #region Properties

    /// <summary>
    /// Токен доступа.
    /// </summary>
    public string? AccessToken => _accessToken;

    /// <summary>
    /// Когда токен станет невалидным.
    /// </summary>
    public DateTime ExpiresAt => _expiresAt;

    #endregion

    #region Construction

    /// <summary>
    /// Конструктор.
    /// </summary>
    public GigaClient
        (
            string clientId,
            string clientSecret
        )
    {
        // _baseUrl = baseUrl ?? "https://gigachat.devices.sberbank.ru/api/v1";
        _clientId = clientId;
        _clientSecret = clientSecret;
    }

    #endregion

    #region Private members

    private readonly string _clientId;

    private readonly string _clientSecret;

    private string? _accessToken;

    private DateTime _expiresAt;

    private async Task GetAccessToken()
    {
        var options = new RestClientOptions
        {
            // Timeout = TimeSpan.MaxValue
        };
        var client = new RestClient (options);
        var request = new RestRequest ("https://ngw.devices.sberbank.ru:9443/api/v2/oauth", Method.Post);
        request.AddHeader (KnownHeaders.ContentType, "application/x-www-form-urlencoded");
        request.AddHeader (KnownHeaders.Accept, "application/json");
        request.AddHeader ("RqUID", Guid.NewGuid().ToString ("D"));

        var authorization = $"{_clientId}:{_clientSecret}";
        authorization = "Basic " + Convert.ToBase64String (Encoding.ASCII.GetBytes (authorization));
        request.AddHeader (KnownHeaders.Authorization, authorization);
        request.AddParameter ("scope", "GIGACHAT_API_PERS");

        var response = await client.ExecuteAsync (request);
        var content = response.Content;
        if (string.IsNullOrEmpty (content))
        {
            throw new Exception ("Empty response returned");
        }

        var json = JsonNode.Parse (content);
        _accessToken = json?["access_token"]?.ToString ();
        var expiresAt = json?["expires_at"]?.ToString ();
        if (!string.IsNullOrEmpty (expiresAt))
        {
            var milliseconds = long.Parse (expiresAt);
            var dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds (milliseconds);
            _expiresAt = dateTimeOffset.LocalDateTime;
        }
        else
        {
            _expiresAt = DateTime.Now.AddMinutes (30);
        }
    }

    // private bool IsAccessTokenExpired() => string.IsNullOrEmpty (_accessToken)
    //     || _expiresAt < DateTime.Now;

    // private async Task GetAccessTokenIfExpired()
    // {
    //     if (IsAccessTokenExpired())
    //     {
    //         await GetAccessToken();
    //     }
    //
    //     if (IsAccessTokenExpired())
    //     {
    //         throw new ApplicationException();
    //     }
    // }

    #endregion

    #region Public methods

    /// <summary>
    /// Затребовать токен доступа.
    /// </summary>
    public async Task<string?> AcquireAccessToken()
    {
        await GetAccessToken();

        return _accessToken;
    }

    #endregion
}
