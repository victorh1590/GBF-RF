using System.Diagnostics;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using RaidFinder.Server.Hubs;

namespace RaidFinder.Data
{
    internal class StreamConsumerService
    {
        private readonly IConfiguration _config;
        private readonly IHubContext<FinderHub> _hub;

        public StreamConsumerService(IConfiguration configuration, IHubContext<FinderHub> hub)
        {
            _config = configuration;
            _hub = hub;
        }

        public async Task Watch()
        {
            StreamConnection connection = new(_config["token"]);
            WebResponse response = connection.ConnectToStream();
            StreamConsumer consumer = new(_hub);

            Console.OutputEncoding = Encoding.UTF8;
            using StreamReader reader = new(response.GetResponseStream(), Encoding.UTF8);

            Stopwatch clock = new();
            clock.Start();
            try
            {
                await consumer.Consume(clock, reader);
            }
            catch (IOException error)
            {
                Console.WriteLine(error.Message + error.StackTrace);
            }
            finally
            {
                reader.Close();
                Console.WriteLine("Reader Closed.");

                clock.Stop();
                Console.WriteLine("Clock Stopped.");
            }
        }
    }
}