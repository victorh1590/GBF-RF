using Microsoft.AspNetCore.SignalR;
using RaidFinder.Server.Hubs;
using System.Diagnostics;

namespace RaidFinder.Data
{
    internal class StreamConsumer
    {
        private readonly IHubContext<FinderHub> _hub;

        public StreamConsumer(IHubContext<FinderHub> hub) => _hub = hub;

        public async Task Consume(Stopwatch clock, StreamReader reader)
        {
            string? tweet;
            while (clock.IsRunning)
            {
                if ((tweet = await reader.ReadLineAsync()) != null)
                {
                    Console.WriteLine(tweet);
                    clock.Stop();

                    var tweetObj = Deserializer.DeserializeTweet(tweet);
                    if (tweetObj != null)
                    {
                        DebugMessages.DebugOutput(tweetObj);
                        await NotifyClients(tweetObj);
                    }

                    clock.Restart();
                    continue;
                }
                if (clock.ElapsedMilliseconds > 10000)
                {
                    clock.Stop();
                    reader.Close();
                    throw new IOException("Read timeout.");
                }
            }
        }

        private async Task NotifyClients(Tweet tweetObj) => await _hub.Clients.All.SendAsync("ReceiveMessage", tweetObj);
    }
}
