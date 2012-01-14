﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Jabbot;
using Jabbot.Sprockets.Core;
using Newtonsoft.Json;

namespace DisqusAnnouncer
{
    public class DisqusAnnouncer : IAnnounce
    {
        public TimeSpan Interval
        {
            get { return TimeSpan.FromMinutes(5); }
        }

        public string Name
        {
            get { return "Disqus Bot"; }
        }

        public DateTime LastUpdate { get; set; }

        public const string APIKey = "txmSHGCXXRZt558E4pvT9akmwveiCd8Ny685WSDlUKlUeLECP8oxZQ3BtfFIIx0c";
        public const string CommentChannel = "code52";

        public static bool Lock { get; set; }

        public void Execute(Bot bot)
        {
            if (Lock == true) return;
            Lock = true;

            Debug.WriteLine(string.Format("Fetching from Disqus! - {0:HH.mm.ss}", DateTime.Now));

            var client = new WebClient();

            var threads = GetThreads(client).ToList();

            var posts = GetPosts(client).ToList();

            foreach(var post in posts.Where(p => !(p.isDeleted == true || p.isSpam == true) && DateTime.Parse(p.createdAt) > LastUpdate).OrderBy(p => DateTime.Parse(p.createdAt)))
            {
                    var thread = threads.SingleOrDefault(t => t.id == post.thread);

                    foreach (var room in bot.Rooms)
                    {
                        var msg = Regex.Replace(post.message, @"<br>", "\n");
                        msg = Regex.Replace(msg, @"\"" rel=\""nofollow\"">[-./""\w\s]*://[-./""\w\s]*", string.Empty);
                        msg = Regex.Replace(msg, @"<a href=\""", string.Empty);
                        msg = Regex.Replace(msg, @"<(.|\n)*?>", string.Empty);
                        bot.Say(
                            string.Format("{0} - {1} ({2}) - {3}", thread == null ? "Unknown" : thread.title,
                                          post.author.name, DateTime.Parse(post.createdAt), msg), room);
                    }

                LastUpdate = DateTime.Parse(post.createdAt);
            }

            Lock = false;
        }

        private static IEnumerable<dynamic> GetThreads(WebClient client)
        {
            var threadResponse = client.DownloadString(new Uri(string.Format("https://disqus.com/api/3.0/forums/listThreads.json?forum={0}&api_key={1}", CommentChannel, APIKey)));
            var threads = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(threadResponse);

            return threads;
        }

        private static IEnumerable<dynamic> GetPosts(WebClient client)
        {
            var postResponse =
                client.DownloadString(new Uri(string.Format("https://disqus.com/api/3.0/forums/listPosts.json?forum={0}&api_key={1}", CommentChannel, APIKey)));
            var posts = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(postResponse);

            return posts;
        }
    }
}
