using System.Collections.Generic;
using Jabbot.Core;

namespace Jabbot.ConsoleBotHost
{
    public interface IScheduler
    {
        void Start(IEnumerable<IAnnounce> tasks, IBot bot);
        void Stop();
    }
}
