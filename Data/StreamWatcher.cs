namespace BlazorServerSignalRApp.Data
{
    using Cyotek.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Text;
    using Microsoft.AspNetCore.SignalR;
    using BlazorServerSignalRApp.Server.Hubs;

    internal class StreamWatcher
    {
        private readonly IConfiguration _config;
        private readonly IHubContext<ChatHub> _hub;

        public StreamWatcher(IConfiguration configuration, IHubContext<ChatHub> hub)
        {
            _config = configuration;
            _hub = hub;
        }

        public async Task Watch(HttpWebRequest request)
        {
            try
            {
                request.Method = "GET";
                request.Headers.Add("Authorization: Bearer " + _config["token"]);
            }
            catch (InvalidOperationException error)
            {
                Console.WriteLine(error.Message + error.StackTrace);
            }

            CircularBuffer<Rootobject> tweetList = new(10);

            WebResponse response = request.GetResponse();

            using (StreamReader reader = new(response.GetResponseStream(), Encoding.ASCII))
            {

                Stopwatch clock = new();
                clock.Start();
                try
                {
                    string? tweet;

                    while (clock.IsRunning)
                    {
                        if ((tweet = await reader.ReadLineAsync()) != null)
                        {
                            Console.WriteLine(tweet);
                            clock.Stop();

                            if (Deserializer.DeserializeTweetAndSaveInList(tweet, tweetList))
                            {
                                string dateFormat = "yyyy-MM-ddTHH:mm:ss";
                                var tweetObj = tweetList.Last();
                                Console.WriteLine($"Deserialization result: {tweetList.Size} elements in list.");
                                Console.WriteLine($"author_id => {tweetObj.data.author_id}");
                                Console.WriteLine($"created_at => {tweetObj.data.created_at.ToString(dateFormat)}");
                                Console.WriteLine($"text => {tweetObj.data.text}");
                                Console.WriteLine($"message => {tweetObj.data.message}");
                                Console.WriteLine($"room => {tweetObj.data.room}");
                                Console.WriteLine($"enemy => {tweetObj.data.enemy}");
                            }

                            await _hub.Clients.All.SendAsync("ReceiveMessage", tweetList.Get());
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
}