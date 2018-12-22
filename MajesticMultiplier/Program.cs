using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using WebClient = Sibears.Helpers.WebClient;

namespace MajesticMultiplier
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var logger = new Serilog.LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();


            const string host = "school.sibears.ru";
            const int port = 4040;

            const string RIGHT = "right :)";
            const string WRONG = "wrong :(";
            const string NOT_FAST = "you are not so fast :(";
            const string FLAG = "This is flag:";


            try
            {
                using (var client = new WebClient(host, port, 8000))
                {
                    var watch = Stopwatch.StartNew();

                    while (client.Connected)
                    {
                        var data = await client.Reader.ReadLineAsync();

                        logger.Information($"Received: {data}");


                        if (data.Contains(WRONG, StringComparison.OrdinalIgnoreCase)
                            || data.Contains(NOT_FAST, StringComparison.OrdinalIgnoreCase))
                        {
                            client.Terminate();

                            break;
                        }

                        if (data.Contains(RIGHT, StringComparison.OrdinalIgnoreCase))
                        {
                            client.Reader.DiscardBufferedData();
                            continue;
                        }

                        if (data.StartsWith(FLAG, StringComparison.InvariantCultureIgnoreCase))
                        {
                            Console.Write($"\n{data.Substring(data.IndexOf(':') + 2).Trim()}\n\n");
                            break;
                            
                        }


                        // Process data.
                        var vector = data.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        BigInteger a, b;

                        try
                        {
                            a = BigInteger.Parse(vector[0]);
                            b = BigInteger.Parse(vector[2]);
                        }
                        catch(Exception ex)
                        {
                            logger.Error(ex, ex.Message);
                        }

                        // Send back some data.
                        var response = BigInteger.Multiply(a, b).ToString();

                        logger.Information($"Send: {response}");

                        await client.Writer.WriteLineAsync(response);
                        await client.Writer.FlushAsync();

                    }

                    watch.Stop();

                    logger.Information($"Time: {watch.Elapsed.TotalSeconds} ms");
                }
            }
            catch (SocketException se)
            {
                logger.Error(se.Message);
            }
        }
    }
}
