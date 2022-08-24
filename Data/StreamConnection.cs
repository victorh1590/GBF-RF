using System.Net;

namespace RaidFinder.Data
{
    internal class StreamConnection
    {
        private readonly string _token;

        public StreamConnection(string token)
        {
            _token = token;
        }

        public WebResponse ConnectToStream()
        {
            string URL = "https://api.twitter.com/2/tweets/search/stream?tweet.fields=author_id,created_at,text";
#pragma warning disable SYSLIB0014 // Type or member is obsolete
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
#pragma warning restore SYSLIB0014 // Type or member is obsolete
            request.ReadWriteTimeout = 10000;

            while (true)
            {
                try
                {
                    request.Method = "GET";
                    request.Headers.Add("Authorization: Bearer " + _token);
                    return request.GetResponse();
                }
                catch (InvalidOperationException error)
                {
                    Console.WriteLine(error.Message + error.StackTrace);
                }
            }
        }
    }
}
