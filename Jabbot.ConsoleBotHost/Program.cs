using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;
using JabbR.Client;
using Jabbot.Core;

namespace Jabbot.ConsoleBotHost
{
    class Program
    {
        private static readonly string _serverUrl = ConfigurationManager.AppSettings["Bot.Server"];
        private static readonly string _botName = ConfigurationManager.AppSettings["Bot.Name"];
        private static readonly string _botPassword = ConfigurationManager.AppSettings["Bot.Password"];
        private static readonly string _botRooms = ConfigurationManager.AppSettings["Bot.RoomList"];
        private static bool _appShouldExit = false;


        private const string ExtensionsFolder = "Sprockets";

        static void Main(string[] args)
        {
            Console.WriteLine("Jabbot Bot Runner Starting...");
            while (!_appShouldExit)
            {
                RunBot();
            }
        }

        private static void RunBot()
        {
            try
            {
                var scheduler = new Scheduler();

                var container = CreateCompositionContainer();
                // Add all the sprockets to the sprocket list
                var announcements = container.GetExportedValues<IAnnounce>();

                Console.WriteLine(String.Format("Connecting to {0}...", _serverUrl));

                var client = new JabbRClient(_serverUrl);
                client.Connect(_botName, _botPassword);
                
                //foreach (var s in container.GetExportedValues<ISprocket>())
                //    bot.AddSprocket(s);

                JoinRooms(client);

                var firstRoom = client.GetRooms().Result;

                client.Send("Hello World", firstRoom.FirstOrDefault().Name);

                //scheduler.Start(announcements, client);

                Console.Write("Press enter to quit...");
                Console.ReadLine();

                scheduler.Stop();
                client.Disconnect();

                _appShouldExit = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.GetBaseException().Message);
            }
        }

        private static void JoinRooms(JabbRClient client)
        {
            var pendingTasks = new List<Task>();

            foreach (var room in _botRooms.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()))
            {
                Console.Write("Joining {0}...", room);
                var task = client.JoinRoom(room);
                pendingTasks.Add(task);
                task.Start();
            }

            Task.WaitAll(pendingTasks.ToArray());
        }
        

        private static CompositionContainer CreateCompositionContainer()
        {
            ComposablePartCatalog catalog;

            var extensionsPath = GetExtensionsPath();

            // If the extensions folder exists then use them
            if (Directory.Exists(extensionsPath))
            {
                catalog = new AggregateCatalog(
                    new AssemblyCatalog(typeof(IBot).Assembly),
                    new AssemblyCatalog(typeof(Program).Assembly), 
                    new DirectoryCatalog(extensionsPath, "*.dll"));
            }
            else
            {
                catalog = new AssemblyCatalog(typeof(IBot).Assembly);
            }

            return new CompositionContainer(catalog);
        }

        private static string GetExtensionsPath()
        {
            var rootPath = HostingEnvironment.IsHosted
                ? HostingEnvironment.ApplicationPhysicalPath
                : Directory.GetCurrentDirectory();

            return Path.Combine(rootPath, ExtensionsFolder);
        }
    }
}
