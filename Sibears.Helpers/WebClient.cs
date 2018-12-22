using System;
using System.IO;
using System.Net.Sockets;

namespace Sibears.Helpers
{
    public sealed class WebClient : IDisposable
    {
        private readonly TcpClient _client;

        public WebClient(string host, int port, int timeout = 30000)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentException("Value cannot be null or empty.", nameof(host));

            _client = new TcpClient(host, port);

            Reader = new StreamReader(_client.GetStream());
            Writer = new StreamWriter(_client.GetStream());

            _client.ReceiveTimeout = _client.SendTimeout = timeout;
        }

        public void Terminate() => _client.Close();

        public void Dispose()
        {
            Reader.Dispose();
            Writer.Dispose();
            _client.Dispose();
        }

        public StreamReader Reader { get; }

        public StreamWriter Writer { get; }

        public bool Connected => _client.Connected;
    }
}
