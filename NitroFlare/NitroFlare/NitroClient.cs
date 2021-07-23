// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CommentTypo
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

/* NitroClient.cs -- клиент для NitroFlare
 */

#region Using directives

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Http;

using Newtonsoft.Json.Linq;

using RestSharp;

#endregion

#nullable enable

namespace NitroFlare
{
    /// <summary>
    /// Клиент для NitroFlare.
    /// </summary>
    public sealed class NitroClient
    {
        #region Constants

        private const string NitroEndpoint = "https://nitroflare.com/api/v2/";        

        #endregion

        #region Properties

        /// <summary>
        /// Директория для сохранения файлов.
        /// </summary>
        public string OutputDirectory { get; set; }

        #endregion
        
        #region Construction

        /// <summary>
        /// Конструктор.
        /// </summary>
        public NitroClient
            (
                string username, 
                string password, 
                string? endpoint = null
            )
        {
            OutputDirectory = Path.GetFullPath("Downloads");
            
            _username = username;
            _password = password;
            _endpoint = endpoint ?? NitroEndpoint;
        }

        #endregion

        #region Private members

        private readonly string _username;
        private readonly string _password;
        private readonly string _endpoint;
        private HttpClient? _httpClient;

        private HttpClient GetHttpClient() => _httpClient ??= new HttpClient();
        private RestClient GetRestClient() => new (_endpoint);
        
        private bool DownloadFile
            (
                DownloadLink link,
                IDownloadProgress? progress
            )
        {
            var fileName = Path.Combine(OutputDirectory, link.Name!);
            if (!Directory.Exists(OutputDirectory))
            {
                Directory.CreateDirectory(OutputDirectory);
            }

            var client = GetHttpClient();
            using var output = File.Create(fileName);
            using var input = client.GetStreamAsync(link.Url!).Result;
            var buffer = new byte[10240];
            long downloaded = 0;
            while (true)
            {
                var read = input.Read(buffer, 0, buffer.Length);
                if (read < 0)
                {
                    progress?.Done(false);
                    return false;
                }
                
                if (read == 0)
                {
                    break;
                }

                output.Write(buffer, 0, read);
                downloaded += read;
                progress?.Report(downloaded);
            }
            
            progress?.Done(true);

            return true;
            
        } // method DownloadFile

        #endregion

        #region Public methods

        /// <summary>
        /// Получение информации о пользователе.
        /// </summary>
        /// <returns>Информация о пользователе либо <c>null</c>.</returns>
        public KeyInfo? GetKeyInfo()
        {
            var client = GetRestClient();
            var request = new RestRequest("getKeyInfo", Method.GET)
                .AddParameter("user", _username)
                .AddParameter("premiumKey", _password);
            var response = client.Execute(request);
            if (!response.IsSuccessful)
            {
                return null;
            }
            
            var content = response.Content;
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            var document = JObject.Parse(content);
            var result = document.GetValue("result");
            
            return result?.ToObject<KeyInfo>();
            
        } // method GetKeyInfo

        /// <summary>
        /// Получение ссылки для скачивания по идентификатору файла.
        /// </summary>
        /// <param name="fileId">Идентификатор файла, например, "B43BED95AF46D56".</param>
        /// <returns>Информация для скачивания либо <c>null</c>.</returns>
        public DownloadLink? GetDownloadLink
            (
                string fileId
            )
        {
            var client = GetRestClient();
            var request = new RestRequest("getDownloadLink", Method.GET)
                .AddParameter("user", _username)
                .AddParameter("premiumKey", _password)
                .AddParameter("file", fileId);
            var response = client.Execute(request);
            if (!response.IsSuccessful)
            {
                return null;
            }
            
            var content = response.Content;
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            var document = JObject.Parse(content);
            var result = document.GetValue("result");
            return result?.ToObject<DownloadLink>();
            
        } // method GetDownloadLink

        /// <summary>
        /// Получение информации о файле по URL/идентификатору.
        /// </summary>
        /// <param name="input">URL или идентификатор файла.</param>
        /// <returns>Информация о файле или <c>null</c>.</returns>
        public FileInfo? GetFileInfo
            (
                string input
            )
        {
            var fileId = ExtractFileId (input);
            if (string.IsNullOrEmpty (fileId))
            {
                if (CheckFileId(input))
                {
                    fileId = input;
                }
                else
                {
                    // не удалось извлечь идентификатор файла
                    // переданная нам строка не является идентификатором файла
                    return null;
                }
            }
            
            var client = GetRestClient();
            var request = new RestRequest("getDownloadLink", Method.GET)
                .AddParameter("user", _username)
                .AddParameter("premiumKey", _password)
                .AddParameter("file", fileId);
            var response = client.Execute(request);
            if (!response.IsSuccessful)
            {
                return null;
            }
            
            var content = response.Content;
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            
            var document = JObject.Parse(content);
            var result = document.GetValue("result");
            
            return result?.ToObject<FileInfo>();
            
        } // method GetFileInfo

        /// <summary>
        /// Извлечение идентификатора файла из URL.
        /// </summary>
        /// <param name="url">URL для скачивания.</param>
        /// <returns>Идентиификатор либо <c>null</c>.</returns>
        public string? ExtractFileId
            (
                string url
            )
        {
            var viewRegex = new Regex("/view/(?<id>[^/]+)/");
            var match = viewRegex.Match(url);
            return match.Success 
                ? match.Groups["id"].Value 
                : null;
            
        } // method ExtractFileId

        /// <summary>
        /// Проверка, похожа ли данная строка на иденитификатор файла.
        /// </summary>
        public bool CheckFileId(string input) => new Regex("^[0-9A-F]+$").IsMatch(input);
        

        /// <summary>
        /// Скачивает файл по указанному URL либо идентификатору файла.
        /// </summary>
        /// <returns>Признак успешного скачивания.</returns>
        public bool DownloadFile
            (
                string input,
                IDownloadProgress? progress = null
            )
        {
            var id = ExtractFileId (input);
            if (string.IsNullOrEmpty (id))
            {
                if (CheckFileId(input))
                {
                    id = input;
                }
                else
                {
                    // не удалось извлечь идентификатор файла
                    // переданная нам строка не является идентификатором файла
                    return false;
                }
            }
            
            var link = GetDownloadLink (id);
            if (link is null)
            {
                // не удалось получить ссылку для скачивания
                return false;
            }
            
            progress?.Init(link.Name!, link.Size);
            
            return DownloadFile (link, progress);
            
        } // method DownloadFile

        /// <summary>
        /// Создание клиента по данным из JSON-файла.
        /// </summary>
        public static NitroClient FromJson
            (
                string? fileName = null
            )
        {
            fileName ??= Path.Combine(AppContext.BaseDirectory, "nitro.json");
            var content = File.ReadAllText(fileName);
            var data = JObject.Parse(content);
            var username = data["username"];
            var password = data["password"];
            if (username is null || password is null)
            {
                throw new ApplicationException();
            }
            
            var result = new NitroClient
                (
                    username.Value<string>()!, 
                    password.Value<string>()!
                );

            return result;
            
        } // method FromJson

        /// <summary>
        /// Создание клиента по строкам окружения.
        /// </summary>
        /// <returns></returns>
        public static NitroClient FromEnvironment()
        {
            var username = Environment.GetEnvironmentVariable("NITRO_USER");
            var password = Environment.GetEnvironmentVariable("NITRO_PASSWORD");
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ApplicationException();
            }

            return new NitroClient(username, password);
            
        }  // method FromEnvironment


        /// <summary>
        /// Создание клиента из командной строки.
        /// </summary>
        public static NitroClient FromCommandLine
            (
                string[] args
            )
        {
            if (args.Length < 2)
            {
                throw new ApplicationException();
            }

            var username = args[0];
            var password = args[0];
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ApplicationException();
            }

            return new NitroClient(username, password);
            
        } // method FromCommandLine

        #endregion

    } // class NitroClient
    
} // namespace NitroFlare
