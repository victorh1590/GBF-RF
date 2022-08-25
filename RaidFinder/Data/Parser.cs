namespace RaidFinder.Data
{
	using System.Text.RegularExpressions;

	internal class Parser
	{
		public Parser()
		{
		}

		public static bool Parse(Tweet tweetObj)
		{
			Regex rx = new Regex(@"(.*?)\s*(\w{8}(?=\s*\:Battle ID))\s*(\:Battle ID)\n(I need backup!)\n(.*?(?=\nhttps))",
				RegexOptions.Compiled | RegexOptions.Singleline);
			MatchCollection matches = rx.Matches(tweetObj.data.text);

			foreach (Match match in matches)
			{
				GroupCollection groups = match.Groups;

				if (!groups[0].Success) break;

				var parsedText = groups.Cast<Group>()
										.Skip(1)
										.Where(group => group.Name != "4" && group.Name != "3")
										.ToList();

				tweetObj.data.message = parsedText[0].Captures[0].Value;
				tweetObj.data.room = parsedText[1].Captures[0].Value;
				tweetObj.data.enemy = parsedText[2].Captures[0].Value;
				return true;
			}
			return false;
		}
	}

}
