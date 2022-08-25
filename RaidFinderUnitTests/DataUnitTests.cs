using RaidFinder.Data;
using System.Text.Json;

namespace RaidFinderUnitTests;

public class DataUnitTests
{
    string jsonString = /*lang=json,strict*/ "{\"data\":{\"author_id\":\"1112860154\",\"created_at\":\"2020-05-11T23:58:26.000Z\",\"id\":\"123479141793669124\",\"source\":\"グランブルー ファンタジー\",\"text\":\"2 1A2B988D :Battle ID\\nI need backup!\\nLvl 200 Galleon\\nhttps://t.co/UYL3VfUba9\"},\"matching_rules\":[{\"id\":\"1111717526182010882\",\"tag\":\"raidwithimages\"}]}";
    string jsonString2 = /*lang=json,strict*/ "{\"data\":{\"author_id\":\"1112860154\",\"created_at\":\"2020-05-11T23:58:26.000Z\",\"id\":\"123479141793669124\",\"source\":\"グランブル\",\"text\":\"2 1A2B988D :Battle ID\\nI need backup!\\nLvl 200 Galleon\\nhttps://t.co/UYL3VfUba9\"},\"matching_rules\":[{\"id\":\"1111717526182010882\",\"tag\":\"raidwithimages\"}]}";

    [Fact]
    public void Author_id_and_text_are_correctly_deserialized()
    {
        var tweet = JsonSerializer.Deserialize<Tweet>(jsonString);
        Assert.NotNull(tweet);
        Assert.Equal("1112860154", tweet!.data.author_id);
        Assert.Equal("2 1A2B988D :Battle ID\nI need backup!\nLvl 200 Galleon\nhttps://t.co/UYL3VfUba9", tweet?.data.text);
    }

    [Fact]
    public void Created_at_time_is_correctly_deserialized()
    {
        string dateFormat = "yyyy-MM-ddTHH:mm:ss";
        var tweet = JsonSerializer.Deserialize<Tweet>(jsonString);
        Assert.NotNull(tweet);
        Assert.Equal("2020-05-11T23:58:26", tweet!.data.created_at.ToString(dateFormat));
    }

    [Fact]
    public void Parser_properties_are_not_empty_before_saving_in_list()
    {
        var tweet = JsonSerializer.Deserialize<Tweet>(jsonString);
        Assert.NotNull(tweet);
        Assert.True(String.IsNullOrEmpty(tweet!.data.message));
        Assert.True(String.IsNullOrEmpty(tweet!.data.room));
        Assert.True(String.IsNullOrEmpty(tweet!.data.enemy));
    }

    [Fact]
    public void Parser_properties_are_correctly_stored_after_parsing()
    {
        var tweet = JsonSerializer.Deserialize<Tweet>(jsonString);
        Assert.NotNull(tweet);
        Assert.True(Parser.Parse(tweet!));
        Assert.Equal("1A2B988D", tweet!.data.room);
        Assert.Equal("Lvl 200 Galleon", tweet!.data.enemy);
        Assert.Equal("2", tweet!.data.message);
    }

    [Fact]
    public void Refuse_tweet_from_different_source()
    {
        Assert.Null(Deserializer.DeserializeTweet(jsonString2));
    }

    [Fact]
    public void Source_encoding_is_correct()
    {
        var tweet = Deserializer.DeserializeTweet(jsonString);
        Assert.NotNull(tweet);
        Assert.Equal("グランブルー ファンタジー", tweet!.data.source);
    }
}