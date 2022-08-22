namespace RaidFinder.Data
{
    using Cyotek.Collections.Generic;
    using System.Text.Json;

    internal class Deserializer
    {
        //public static List<Rootobject> tweetList { get; set; } = new();

        public Deserializer()
        {
        }

        public static bool DeserializeTweetAndSaveInList(string tweet, CircularBuffer<Rootobject> tweetList)
        {

            if (String.IsNullOrEmpty(tweet)) return false;

            try
            {
                var tweetObj = JsonSerializer.Deserialize<Rootobject>(tweet);
                if (tweetObj == null) return false;
                if (!Parser.Parse(tweetObj)) return false;
                tweetList.Put(tweetObj);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
                return false;
            }
        }
    }

}
