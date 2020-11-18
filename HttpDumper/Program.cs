/*
 * Данная утилита просто записывает в файл заголовки всех запросов,
 * пришедших на указанный порт. Записываются первые 2 килобайта
 * запроса "как есть", назад всего отправляется пустой ответ "200 OK".
 */

#region Using directives

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using CM=System.Configuration.ConfigurationManager;

#endregion

namespace HttpDumper
{
    internal class PseudoServer
    {
        private object _lock = new object();
        private volatile int _counter = 0;
        private TcpListener _listener;

        public void HandleClient(TcpClient client)
        {
            int number;
            lock (_lock)
            {
                number = ++_counter;
            }

            Console.WriteLine(number);
            var input = client.GetStream();
            var data = new byte[2048];
            var now = DateTime.Now.ToString("R", CultureInfo.InvariantCulture);
            var responseText = $@"HTTP/1.1 200 OK
Date: {now}
Server: Apache/2.2.14 (Win32)
Last-Modified: {now}
Connection: Closed";
            var responseBytes = Encoding.UTF8.GetBytes(responseText);
            var responseSend = false;
            using (var output = File.Create($"{number:000000}"))
            {
                var read = input.Read(data, 0, data.Length);
                if (!responseSend)
                {
                    input.Write(responseBytes, 0, responseBytes.Length);
                    responseSend = true;
                }

                output.Write(data, 0, read);
            }

            client.Close();
        }
        
        public void Start()
        {
            var portNumber = int.Parse(CM.AppSettings["port"]);
            _listener = new TcpListener(IPAddress.Any, portNumber);
            _listener.Start();

            Console.WriteLine("Waiting for connection...");
            while (true)
            {
                var client = _listener.AcceptTcpClient();

                Task.Factory.StartNew(() => HandleClient(client));
            }
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var server = new PseudoServer();
            server.Start();
        }
    }
}