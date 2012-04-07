using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JabbR.Client;
using JabbR.Client.Models;

namespace Jabbot.Core
{
    public interface IAnnounce
    {
        TimeSpan Interval { get; }
        void Execute(IBot bot);
    }

    public static class BotExtensions
    {
        public static Task SendToAllRooms(this IBot bot, string text)
        {
            return bot.GetRooms()
                .ContinueWith(c =>
                {
                    foreach (var room in c.Result)
                    {
                        bot.Send(text, room).Wait();
                    }
                });
        }
    }

    public class Bot : IBot
    {
        private readonly JabbRClient _client;
        private readonly IList<ISprocket> _sprockets = new List<ISprocket>();

        public Bot(JabbRClient client)
        {
            _client = client;
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public Task Send(string text, string room)
        {
            return _client.Send(text, room);
        }

        public Task PrivateReply(string userName, string message)
        {
            return _client.SendPrivateMessage(userName, message);
        }

        public Task<IEnumerable<string>> GetUsers(string room)
        {
            return _client.GetRoomInfo(room)
               .ContinueWith(c => c.Result.Users.Select(u => u.Name));
        }

        public Task<IEnumerable<string>> GetRooms()
        {
            return _client.GetRooms().ContinueWith(c => c.Result.Select(r => r.Name));
        }

        public void Connect(string botName, string botPassword)
        {
            _client.Connect(botName, botPassword);
        }

        public void AddSprocket(ISprocket sprocket)
        {
            _sprockets.Add(sprocket);
        }
    }

    public interface IBot
    {
        string Name { get; }
        Task Send(string text, string room);
        Task PrivateReply(string userName, string message);
        Task<IEnumerable<string>> GetUsers(string room);
        Task<IEnumerable<string>> GetRooms();
    }

    public interface IProxy {
        string Get(string requestUri);
    }

    public interface ISprocket { }

    public interface IUnhandledMessageSprocket : ISprocket { }

    public interface ISprocketInitializer { }

    public  abstract class RegexSprocket : ISprocket
    {
        public abstract Regex Pattern { get; }

        public abstract void ProcessMatch(Match match, ChatMessage chatMessage, IBot bot);
    }

    public interface ILogger
    {
        void WriteMessage(string p0);
        void Write(string format, params object[] args);
    }

    public interface ISettingsService
    {
        bool ContainsKey(string key);
        T Get<T>(string key);
        void Set<T>(string key, T value);
        void Save();
    }

    public class ChatMessage
    {
        public ChatMessage(string message, string user, string room)
        {
            Content = message;
            User = new User { Name = user };
            Room = room;
        }

        public string Id { get; set; }
        public string Content { get; set; }
        public DateTimeOffset When { get; set; }
        public User User { get; set; }
        public string Room { get; set; }
    }

    public class User
    {
        public string Name { get; set; }
        public string Hash { get; set; }
        public bool Active { get; set; }
        public UserStatus Status { get; set; }
        public string Note { get; set; }
        public string AfkNote { get; set; }
        public bool IsAfk { get; set; }
        public string Flag { get; set; }
        public string Country { get; set; }
        public DateTime LastActivity { get; set; }
    }

    public enum UserStatus
    {
        Active,
        Inactive,
        Offline
    }
}
