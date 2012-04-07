using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Jabbot.Core
{
    public interface IAnnounce {
        TimeSpan Interval { get; }
        void Execute(IBot bot);
    }

    public interface IBot
    {
        IEnumerable<string> Rooms { get; }
        string Name { get; }
        void SayToAllRooms(string text);
        void Say(string text, string room);
        void PrivateReply(string userName, string message);
        IEnumerable<string> GetUsers(string name);
    }

    public interface ISprocket { }

    public interface IUnhandledMessageSprocket : ISprocket { }

    public interface ISprocketInitializer { }

    public class CommandSprocket : ISprocket { }

    public class RegexSprocket : ISprocket {
        public virtual Regex Pattern
        {
            get { throw new NotImplementedException(); }
        }

        protected virtual void ProcessMatch(Match match, ChatMessage chatMessage, IBot bot)
        {
            throw new NotImplementedException();
        }
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
            User = new User {Name = user};
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
