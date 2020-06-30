using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using GameServer.Networking;

namespace GameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            StartServer();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void StartServer()
        {
            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Server.Start();
        }

        private static void MainThread()
        {
            Console.WriteLine($"Main thread started. Running at {Constants.TICKS_PER_SEC} ticks per second.");
            DateTime _nextLoop = DateTime.Now;
            while (true)
            {
                if (_nextLoop < DateTime.Now)
                {
                    GameCycle.Update();
                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);
                }
                else
                {
                    int milliseconds = (int)(_nextLoop - DateTime.Now).TotalMilliseconds;
                    Thread.Sleep(Math.Max(milliseconds, 0));
                }
            }
        }
    }
}
