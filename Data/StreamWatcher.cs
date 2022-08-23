namespace RaidFinder.Data
{
    using System.Diagnostics;
    using System.Net;
    using System.Text;
    using Microsoft.AspNetCore.SignalR;
    using RaidFinder.Server.Hubs;

    internal class StreamWatcher
    {
        private readonly IConfiguration _config;
        private readonly IHubContext<FinderHub> _hub;

        public StreamWatcher(IConfiguration configuration, IHubContext<FinderHub> hub)
        {
            _config = configuration;
            _hub = hub;
        }

        private WebResponse ConnectToStream()
        {
            string URL = "https://api.twitter.com/2/tweets/search/stream?tweet.fields=author_id,created_at,text";
#pragma warning disable SYSLIB0014 // Type or member is obsolete
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
#pragma warning restore SYSLIB0014 // Type or member is obsolete
            request.ReadWriteTimeout = 10000;
            
            while(true)
            {
                try
                {
                    request.Method = "GET";
                    request.Headers.Add("Authorization: Bearer " + _config["token"]);
                    return request.GetResponse();
                }
                catch (InvalidOperationException error)
                {
                    Console.WriteLine(error.Message + error.StackTrace);
                }
            }
        }

        private async Task ReadStreamAndNotify(Stopwatch clock, StreamReader reader)
        {
            string? tweet;

            void DebugOutput(Rootobject tweetObj)
            {
                string dateFormat = "yyyy-MM-ddTHH:mm:ss";
                //Console.WriteLine($"Deserialization result: {tweetList.Size} elements in list.");
                Console.WriteLine($"author_id => {tweetObj.data.author_id}");
                Console.WriteLine($"created_at => {tweetObj.data.created_at.ToString(dateFormat)}");
                Console.WriteLine($"text => {tweetObj.data.text}");
                Console.WriteLine($"message => {tweetObj.data.message}");
                Console.WriteLine($"room => {tweetObj.data.room}");
                Console.WriteLine($"enemy => {tweetObj.data.enemy}");
            }
                        
            async Task NotifyClients(Rootobject tweetObj) => await _hub.Clients.All.SendAsync("ReceiveMessage", tweetObj);

            async Task TweetProcessment(Rootobject? tweetObj)
            {
                if (tweetObj != null)
                {
                    DebugOutput(tweetObj);
                    await NotifyClients(tweetObj);
                }
            }

            while (clock.IsRunning)
            {
                if ((tweet = await reader.ReadLineAsync()) != null)
                {
                    Console.WriteLine(tweet);
                    clock.Stop();

                    var tweetObj = Deserializer.DeserializeTweet(tweet);
                    await TweetProcessment(tweetObj);
 
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

        public async Task Watch()
        {
            WebResponse response = ConnectToStream();

            using StreamReader reader = new(response.GetResponseStream(), Encoding.ASCII);

            Stopwatch clock = new();

            clock.Start();
            try
            {
                await ReadStreamAndNotify(clock, reader);
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