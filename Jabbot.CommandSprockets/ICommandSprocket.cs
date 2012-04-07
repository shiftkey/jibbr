using System.Collections.Generic;
using Jabbot.Core;

namespace Jabbot.CommandSprockets
{
    public interface ICommandSprocket : ISprocket
    {
        string[] Arguments { get; }
        IBot Bot { get; }
        string Command { get; }
        bool ExecuteCommand();
        bool HasArguments { get; }
        string Intitiator { get; }
        bool MayHandle(string initiator, string command);
        ChatMessage ChatMessage { get; }
        IEnumerable<string> SupportedCommands { get; }
        IEnumerable<string> SupportedInitiators { get; }
    }
}
