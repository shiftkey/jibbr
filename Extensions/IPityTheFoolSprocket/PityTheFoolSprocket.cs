using System.Text.RegularExpressions;
using Jabbot.Core;

namespace IPityTheFoolSprocket
{
	public class PityTheFoolSprocket : RegexSprocket
	{
		public override Regex Pattern
		{
			get { return new Regex(@".*(?:fool|pity)+.*", RegexOptions.IgnoreCase); }
		}

		protected override void ProcessMatch(Match match, ChatMessage chatMessage, IBot bot)
		{
			bot.Say("http://xamldev.dk/IPityTheFool.gif", chatMessage.User.Name);
		}
	}
}