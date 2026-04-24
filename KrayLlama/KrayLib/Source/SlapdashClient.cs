// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace
// ReSharper disable CommentTypo

/* SlapdashClient.cs - клиент для OpenAI
 * Ars Magna project
 */

#region Using directives

using RestSharp;
using RestSharp.Serializers.Json;

#endregion

namespace KrayLib;

/// <summary>
/// Клиент для OpenAI.
/// </summary>
public sealed class SlapdashClient
{
    #region Properties

    public string Endpoint { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Конструктор.
    /// </summary>
    public SlapdashClient
        (
            string endpoint
        )
    {
        Endpoint = endpoint;
    }

    #endregion

    #region Public methods

    public AiResponse? Execute
        (
            AiRequest prompt
        )
    {
        var options = new RestClientOptions
        {
            BaseUrl = new Uri (Endpoint)
        };
        var restClient = new RestClient
            (
                options,
                null,
                serializationConfig => serializationConfig
                    .UseSystemTextJson (KrayJsonSerializerContext.Default.Options)
            );
        var request = new RestRequest ("chat/completions", Method.Post);
        request.AddJsonBody (prompt);
        if (!string.IsNullOrEmpty (prompt.ApiKey))
        {
            request.AddHeader ("Authorization", "Bearer " + prompt.ApiKey);
        }

        var response = restClient.Execute<AiResponse> (request);
        var result = response.Data;

        return result;
    }

    #endregion
}
