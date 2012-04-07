using System;
using System.Collections.Generic;
using System.Linq;
using Jabbot.Core;

namespace Jabbot.CommandSprockets
{
	public abstract class UnhandledCommandSprocket : IUnhandledMessageSprocket
	{
		public abstract IEnumerable<string> SupportedInitiators { get; }
		public abstract bool ExecuteCommand();
		public string Intitiator { get; protected set; }
		public string Command { get; protected set; }
		public string[] Arguments { get; protected set; }
		public ChatMessage ChatMessage { get; protected set; }
		public IBot Bot { get; protected set; }
		public bool HasArguments { get { return Arguments.Length > 0; } }


		public virtual bool MayHandle(string initiator, string command)
		{
			return SupportedInitiators.Any(i => i.Equals(initiator, StringComparison.OrdinalIgnoreCase));
		}

		public virtual bool Handle(ChatMessage chatMessage, IBot bot)
		{
			try
			{

				string[] args = chatMessage.Content
					   .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				Intitiator = args.Length > 0 ? args[0] : string.Empty;
				Command = args.Length > 1 ? args[1] : string.Empty;
				ChatMessage = chatMessage;
				Bot = bot;

				if (MayHandle(Intitiator, Command))
				{
					Arguments = args.Skip(2).ToArray();
					return ExecuteCommand();
				}
			}
			catch (InvalidOperationException e)
			{
				Bot.PrivateReply(ChatMessage.User.Name, e.GetBaseException().Message);
			}
			return false;
		}
	}
}
