namespace RaidFinder.Data
{
    public class Rootobject
    {
        public Data data { get; set; } = default!;
        public Matching_Rules[] matching_rules { get; set; } = default!;
    }

    public class Data
    {
        public string author_id { get; set; } = default!;
        public DateTime created_at { get; set; } = default!;
        public string id { get; set; } = default!;
        public string text { get; set; } = default!;
        public string message { get; set; } = default!;
        public string room { get; set; } = default!;
        public string enemy { get; set; } = default!;

    }

    public class Matching_Rules
    {
        public string id { get; set; } = default!;
        public string tag { get; set; } = default!;
    }

}
