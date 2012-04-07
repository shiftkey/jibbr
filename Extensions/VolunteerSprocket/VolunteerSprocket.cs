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

        public override void ProcessMatch(Match match, ChatMessage chatMessage, IBot bot)
        {
            Debug.WriteLine("Volunteering!");
            if (chatMessage.Content.StartsWith(bot.Name) || chatMessage.Content.StartsWith("@" + bot.Name))
            {
                var users = bot.GetUsers(chatMessage.Room).Result.Where(c => c != bot.Name).ToList();

                if(!users.Any())
                {
                    bot.Send("Bot, you can't tell yourself to do that", chatMessage.Room);
                    return;
                }

                var random = new Random();

                var randomUser = random.Next(0, users.Count() - 1);

                bot.Send(string.Format("I volunteer {0} for that!", users[randomUser]), chatMessage.Room);
            }
        }
    }
}
