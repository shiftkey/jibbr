using System.Collections.Generic;
using Jabbot.Core;
using Nancy;

namespace Jabbot.AspNetBotHost.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule(IEnumerable<IAnnounce> announcers, IEnumerable<ISprocket> sprockets, IBot bot)
        {
            Get["/"] = _ => View["Home/Index", new { Announcers = announcers, Sprockets = sprockets, Bot = bot }];
            Get["/Rooms"] = _ => View["Home/Rooms", bot.Rooms];
        }
    }
}