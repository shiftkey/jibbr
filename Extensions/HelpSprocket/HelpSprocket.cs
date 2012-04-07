using System.Linq;
using Jabbot.Core;

namespace Help
{
	public class HelpSprocket : ISprocket
	{
		public bool Handle(ChatMessage chatMessage, IBot bot)
		{
			var acceptedCommands = new string[] { bot.Name + " help", "@" + bot.Name + " help" };

			if (acceptedCommands.Contains(chatMessage.Content.Trim()))
			{
				bot.PrivateReply(chatMessage.User.Name, "A list of commands this bot currently supports:\n\thelp");

				return true;
			}

			return false;
		}
	}
}
