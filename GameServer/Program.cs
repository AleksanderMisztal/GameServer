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
            Thread mainThread = new Thread(MainThread);
            mainThread.Start();
        }

        private static void MainThread()
        {
            Console.WriteLine($"Main thread started. Running at {Constants.TicksPerSec} ticks per second.");
            DateTime nextLoop = DateTime.Now;
            while (true)
            {
                if (nextLoop < DateTime.Now)
                {
                    GameCycle.Update();
                    nextLoop = nextLoop.AddMilliseconds(Constants.MsPerTick);
                }
                else
                {
                    int milliseconds = (int)(nextLoop - DateTime.Now).TotalMilliseconds;
                    Thread.Sleep(Math.Max(milliseconds, 0));
                }
            }
        }
    }
}
