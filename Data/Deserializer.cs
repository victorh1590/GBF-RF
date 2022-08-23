namespace RaidFinder.Data
{
    using System.Text.Json;

    internal class Deserializer
    {
        public Deserializer()
        {
        }

        public static Rootobject? DeserializeTweet(string tweet)
        {

            if (String.IsNullOrEmpty(tweet)) return null;
            try
            {
                var tweetObj = JsonSerializer.Deserialize<Rootobject>(tweet);
                if (tweetObj == null) return null;
                if (!Parser.Parse(tweetObj)) return null;
                return tweetObj;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
                return null;
            }
        }
    }

}
