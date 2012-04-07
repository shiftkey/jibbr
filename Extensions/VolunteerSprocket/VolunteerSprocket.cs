using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Jabbot.Core;

namespace VolunteerSprocket
{
    public class VolunteerSprocket : RegexSprocket
    {
        public override Regex Pattern
        {
            get { return new Regex(@"[-_./""\w\s]*volunteer some[-_./""\w\s]*"); }
        }

        protected override void ProcessMatch(Match match, ChatMessage chatMessage, IBot bot)
        {
            Debug.WriteLine("Volunteering!");
            if (chatMessage.Content.StartsWith(bot.Name) || chatMessage.Content.StartsWith("@" + bot.Name))
            {
                var users = bot.GetUsers(chatMessage.User.Name).ToList<dynamic>();

                users.RemoveAll(u => u.Name == bot.Name);

                if(users.Count == 0)
                {
                    // TODO: .Receiver is a room?
                    bot.Say("Bot, you can't tell yourself to do that", ""); // ChatMessage.Receiver);
                    return;
                }

                var random = new Random();

                var randomUser = random.Next(0, users.Count() - 1);

                bot.Say(string.Format("I volunteer {0} for that!", users[randomUser].Name), ""); // ChatMessage.Receiver);
            }
        }
    }
}
