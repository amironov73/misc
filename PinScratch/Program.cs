using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using VkNet.Utils;

using CM = System.Configuration.ConfigurationManager;

// ReSharper disable InconsistentNaming

namespace PinScratch
{
    class Program
    {
        private static VkApi _api;
        private static ulong _applicationId = _GetUInt64("appId");
        private static string _username = CM.AppSettings["username"];
        private static string _password = CM.AppSettings["password"];
        private static string _token = CM.AppSettings["token"];
        private static long _ownerId = _GetInt64("ownerId");
        private static string _albumId = CM.AppSettings["albumId"];
        private static ulong _offset;
        private static ulong _portion = _GetUInt64("portion", 500UL);
        private static string _output = CM.AppSettings["output"];
        private static int _parallel = _GetIn32("parallel", 5);
        private static SemaphoreSlim _semaphore;

        /// <summary>
        /// Get the signed 64-bit integer from the app configuration.
        /// </summary>
        private static long _GetInt64(string name, long defaultValue = 0)
        {
            var text = CM.AppSettings[name];
            return string.IsNullOrEmpty(text) ? defaultValue : long.Parse(text);
        }

        /// <summary>
        /// Get the unsigned 64-bit integer from the app configuration.
        /// </summary>
        private static ulong _GetUInt64(string name, ulong defaultValue = 0)
        {
            var text = CM.AppSettings[name];
            return string.IsNullOrEmpty(text) ? defaultValue: ulong.Parse(text);
        }

        /// <summary>
        /// Get the signed 32-bit integer from the app configuration.
        /// </summary>
        private static int _GetIn32(string name, int defaultValue = 0)
        {
            var text = CM.AppSettings[name];
            return string.IsNullOrEmpty(text) ? defaultValue : int.Parse(text);
        }

        /// <summary>
        /// Parse the command line.
        /// </summary>
        private static bool _ParseCommandLine(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i].ToLowerInvariant();

                switch (arg)
                {
                    case "-app":
                    case "/app":
                        _applicationId = ulong.Parse(args[++i]);
                        break;

                    case "-user":
                    case "/user":
                        _username = args[++i];
                        break;

                    case "-password":
                    case "/password":
                        _password = args[++i];
                        break;

                    case "-token": 
                    case "/token":
                        _token = args[++i];
                        break;

                    case "-owner":
                    case "/owner":
                        _ownerId = long.Parse(args[++i]);
                        break;

                    case "-album":
                    case "/album":
                        _albumId = args[++i];
                        break;

                    case "-offset":
                    case "/offset":
                        _offset = ulong.Parse(args[++i]);
                        break;

                    case "-portion":
                    case "/portion":
                        _portion = ulong.Parse(args[++i]);
                        break;

                    case "-parallel":
                    case "/parallel":
                        _parallel = int.Parse(args[++i]);
                        break;

                    case "-output":
                    case "-out":
                    case "/output":
                    case "/out":
                        _output = args[++i];
                        break;

                    default:
                        if (char.IsDigit(arg, 0) || arg[0] == '-')
                        {
                            if (_ownerId == 0)
                            {
                                _ownerId = long.Parse(arg);
                            }
                            else
                            {
                                _albumId = arg;
                            }
                        }
                        else if (arg == "wall" || arg == "profile" || arg == "saved")
                        {
                            _albumId = arg;
                        }
                        else
                        {
                            _output = arg;
                        }

                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Get the two-factor authorization code.
        /// </summary>
        private static string _GetCode()
        {
            Console.Write("Enter code: ");
            return Console.ReadLine();
        }

        /// <summary>
        /// Authorize the application.
        /// </summary>
        private static bool _Authorize()
        {
            var authParams = new ApiAuthParams
            {
                ApplicationId = _applicationId,
                Settings = Settings.All,
                TwoFactorSupported = true,
                TwoFactorAuthorization = _GetCode
            };

            if (string.IsNullOrEmpty(_token))
            {
                if (string.IsNullOrEmpty(_username))
                {
                    Console.Write("Enter user name: ");
                    _username = Console.ReadLine();
                }

                if (string.IsNullOrEmpty(_password))
                {
                    Console.Write("Enter password: ");
                    _password = Console.ReadLine();
                }

                try
                {
                    authParams.Login = _username;
                    authParams.Password = _password;
                    _api.Authorize(authParams);
                    _token = _api.Token;
                    Console.WriteLine($"Token: {_token}");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return false;
                }
            }
            else
            {
                authParams.AccessToken = _token;
                try
                {
                    _api.Authorize(authParams);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Download the file.
        /// </summary>
        private static void _DownloadFile
            (
                ulong offset,
                Uri url,
                string fileName
            )
        {
            Console.WriteLine($"{offset}:  {url} {fileName}");
            _semaphore.Wait();
            var client = new WebClient();
            client.DownloadFileCompleted += _DownloadFileCompleted;
            client.DownloadFileAsync(url, fileName);
        }

        /// <summary>
        /// Called when the file is downloaded.
        /// </summary>
        private static void _DownloadFileCompleted
            (
                object sender, AsyncCompletedEventArgs e
            )
        {
            var client = (WebClient) sender;
            client.DownloadFileCompleted -= _DownloadFileCompleted;
            _semaphore.Release();
            client.Dispose();
        }

        /// <summary>
        /// Download the biggest photo image.
        /// </summary>
        private static void _DownloadPhoto
            (
                Photo photo
            )
        {
            Uri url = null;
            ulong biggest = 0;
            foreach (var one in photo.Sizes)
            {
                if (one.Height > biggest)
                {
                    biggest = one.Height;
                    url = one.Url;
                }
            }

            if (ReferenceEquals(url, null))
            {
                return;
            }

            var folder = Path.Combine
                (
                    _output,
                    (_offset / 1000).ToString(CultureInfo.InvariantCulture)
                );
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var fileName = url.LocalPath.Split('/').Last();
            fileName = $"{_offset:D6}_{fileName}";
            fileName = Path.Combine(folder, fileName);
            _DownloadFile(_offset, url, fileName);
        }

        /// <summary>
        /// Fetch info about album portion.
        /// </summary>
        private static VkCollection<Photo> _FetchInfo()
        {
            var photoParams = new PhotoGetParams()
            {
                OwnerId = _ownerId,
                Offset = _offset,
                Count = _portion
            };

            switch (_albumId.ToLowerInvariant())
            {
                case "wall":
                    photoParams.AlbumId = PhotoAlbumType.Wall;
                    break;

                case "profile":
                    photoParams.AlbumId = PhotoAlbumType.Profile;
                    break;

                case "saved":
                    photoParams.AlbumId = PhotoAlbumType.Saved;
                    break;

                default:
                    if (long.TryParse(_albumId, out long number))
                    {
                        photoParams.AlbumId = PhotoAlbumType.Id(number);
                    }
                    break;
            }

            var result = _api.Photo.Get(photoParams);
            if (ReferenceEquals(result, null) || result.Count == 0)
            {
                Console.WriteLine("Photo is over");
                return null;
            }

            Console.WriteLine($"Portion size: {result.Count}");

            return result;
        }

        /// <summary>
        /// Download the album.
        /// </summary>
        static void _DownloadAlbum()
        {
            try
            {
                while (true)
                {
                    var portion = _FetchInfo();
                    if (ReferenceEquals(portion, null))
                    {
                        break;
                    }

                    foreach (var photo in portion)
                    {
                        _offset++;
                        _DownloadPhoto(photo);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        static void Main(string[] args)
        {
            if (!_ParseCommandLine(args))
            {
                return;
            }

            _semaphore = new SemaphoreSlim(_parallel, _parallel);

            try
            {
                if (string.IsNullOrEmpty(_output))
                {
                    _output = Directory.GetCurrentDirectory();
                }

                if (!Directory.Exists(_output))
                {
                    Directory.CreateDirectory(_output);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return;
            }

            _api = new VkApi();

            if (!_Authorize())
            {
                Console.WriteLine("Authorization failed, exiting");
                return;
            }

            _DownloadAlbum();

            Console.WriteLine();
            Console.Write("Waiting for downloads...");
            SpinWait.SpinUntil(() => _semaphore.CurrentCount == _parallel);
            Console.WriteLine("done");

            Console.WriteLine();
            Console.WriteLine("ALL DONE");
            Console.WriteLine();
        }
    }
}
