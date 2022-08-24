namespace RaidFinder.Data
{
    internal static class DebugMessages
    {
        public static void DebugOutput(Rootobject tweetObj)
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
    }
}
